using System;
using System.Collections.Generic;
using System.Text;

namespace IntegerExemple
{
    interface IFabric<T>
    {
        T Create();
    }
    interface ISampleFabric<T>
    {
        T[] CreateSample();
        T GetStandart();
    }
    abstract class RandomFactory<T>
    {
        int bottomRand;
        int upperRand;

        public RandomFactory(int bottom = int.MinValue, int upper = int.MaxValue)
        {
            if (upper <= bottom)
                throw new ArgumentException("Границы равны или инвертированы");
            bottomRand = bottom;
            upperRand = upper;
        }


        protected abstract T Create(int choosen);
        protected abstract int GetSingleSeed(T choosen);


        public T[] CreateRandom(int count)
        {
            return CreateRandom(count, Create);
        }
        public T[] CreateSingleRandom(int count, T single)
        {
            bool isExist = false;
            int check = GetSingleSeed(single);
            return CreateRandom(count, s =>
            {
                if (s == check)
                {
                    if (isExist)
                    {
                        s = CollisionSolution(s);
                    }
                    else
                    {
                        isExist = true;
                    }
                }
                return Create(s);
            });
        }


        private T[] CreateRandom(int count, Func<int, T> choosenToInst)
        {
            int seed = InicializeSeed();
            var result = new T[count];
            for (int i = 0; i < count; i++)
            {
                int choose = GenRand(seed);
                seed = GenSeed(seed);
                result[i] = choosenToInst(choose);
            }
            return result;
        }
        private int GenRand(int seed)
        {
            return new Random(seed).Next(bottomRand, upperRand);
        }


        protected virtual int InicializeSeed()
        {
            return (int)DateTime.Now.Ticks;
        }
        protected virtual int GenSeed(int lastSeed)
        {
            return (lastSeed + 1) * 2;
        }
        protected virtual int CollisionSolution(int collisionValue)
        {
            return collisionValue == bottomRand ? upperRand - 1 : collisionValue--;
        }
    }
}
