using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;
using System.Diagnostics.CodeAnalysis;

namespace IntegerExemple
{
    //так как по дефолту сравниваются ссылки у Type, а они всегда разные
    class TypeComparer : IEqualityComparer<Type>
    {
        public static TypeComparer Comparer = new TypeComparer();
        private TypeComparer() { }
        public bool Equals([AllowNull] Type x, [AllowNull] Type y)
        {
            return x.FullName.Equals(y.FullName);
        }
        public int GetHashCode([DisallowNull] Type obj)
        {
            return obj.FullName.GetHashCode();
        }
    }
    class DICargo : IDisposable, IEnumerable<Type>
    {
        Dictionary<Type, Cargo> dict;
        //в принципе можно сделать локальным в конструкторе класса
        List<Type> noDeterministicInterfaces = new List<Type>();
        public DICargo()
        {
            var diTypes = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.GetCustomAttribute(typeof(DIClassAttribute)) != null);
            dict = new Dictionary<Type, Cargo>(TypeComparer.Comparer);

            foreach(var di in diTypes)
            {
                var carg = new Cargo(this, ((DIClassAttribute)di.GetCustomAttribute(typeof(DIClassAttribute))).CreateSetting);
                dict.TryAdd(di, carg);
                foreach(var impl in di.GetInterfaces())
                {
                    if(!dict.TryAdd(impl, carg))
                    {
                        noDeterministicInterfaces.Add(impl);
                    }
                }
            }

            foreach(var intr in noDeterministicInterfaces)
            {
                dict.Remove(intr);
            }

            foreach(var c in dict)
            {
                if (!c.Value.isInicialized)
                    c.Value.Inicialize(c.Key);
            }
        }

        public void Dispose()
        {
            foreach(var c in dict)
            {
                c.Value.Dispose();
            }
            dict = null;
        }

        public IEnumerator<Type> GetEnumerator()
        {
            var tempEn = dict.GetEnumerator();
            while (tempEn.MoveNext())
            {
                yield return tempEn.Current.Key;
            }
        }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Run(Type runType, string startMethodName)
        {
            var method = runType.GetMethod(startMethodName);
            if (method == null) throw new ArgumentException($"Method {startMethodName} don't exist");
            if (method.GetParameters().Length != 0) throw new ArgumentException($" Method {startMethodName} shouldn't have parameters");
            method.Invoke(GetDependency(runType), null);
        }
        private object GetDependency(Type type)
        {
            Cargo inst;
            if (!dict.TryGetValue(type, out inst)) throw new ArgumentNullException($"dependency type {type} not found ");
            return inst.GetInstance();
        }
        private object[] GetDependency(Type[] types)
        {
            IEnumerable<object> GetDepndcis()
            {
                foreach(var t in types)
                {
                    yield return GetDependency(t);
                }
            }
            return GetDepndcis().ToArray();
        }
        private bool IncludeType(IEnumerable<Type> types)
        {
            foreach(var t in types)
            {
                Cargo some;
                if (!dict.TryGetValue(t, out some)) return false;
            }
            return true;
        }
        //все интерфейсы также ссылаются на один контейнер
        class Cargo : IDisposable
        {
            DICargo parent;
            object singletonInst;
            bool isSingleton;
            ConstructorInfo constr;
            Type[] constrParams;
            public bool isInicialized { get; private set; }
            public Cargo(DICargo owner, Pattern pattern)
            {
                parent = owner;
                singletonInst = null;
                isSingleton = pattern == Pattern.Singleton;
            }
            //inicialize не является публичным api, поэтому не нужно делать проверки на то, был ли он вызван.
            public void Inicialize(Type type)
            {
                List<(ConstructorInfo ctor, Type[] param)> suitableConstructors = new List<(ConstructorInfo ctor, Type[] param)>();
                foreach (var ctor in type.GetConstructors())
                {
                    var tempParams = ctor.GetParameters().Select(p => p.ParameterType);
                    if (parent.IncludeType(tempParams))
                    {
                        suitableConstructors.Add((ctor, tempParams.ToArray()));
                    }
                }
                if (suitableConstructors.Count == 0)
                    throw new ArgumentException($"construct for dependency type {type} not found");
                if (suitableConstructors.Count > 1)
                {
                    suitableConstructors = new List<(ConstructorInfo ctor, Type[] param)>(suitableConstructors.Where(c => c.ctor.GetCustomAttribute(typeof(DICtorAttribute)) != null).ToArray());
                }
                if (suitableConstructors.Count > 1 || suitableConstructors.Count == 0)
                    throw new Exception($"construct for dependency type {type} no deterministic");
                var select = suitableConstructors.First();
                constr = select.ctor;
                constrParams = select.param;
                isInicialized = true;
            }
            public object GetInstance()
            {
                if (isSingleton)
                {
                    if (singletonInst == null)
                    {
                        singletonInst = constr.Invoke(parent.GetDependency(constrParams));
                    }
                    return singletonInst;
                }
                return constr.Invoke(parent.GetDependency(constrParams));
            }

            public void Dispose()
            {
                (singletonInst as IDisposable)?.Dispose();
                singletonInst = null;
            }
        }

    }
    enum Pattern
    {
        Singleton,
        Prototype
    }
    class DICtorAttribute : Attribute { }
    class DIClassAttribute : Attribute
    {
        public readonly Pattern CreateSetting;
        public DIClassAttribute(Pattern createSett = Pattern.Singleton)
        {
            CreateSetting = createSett;
        }
    }
}
