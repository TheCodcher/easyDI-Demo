using System;
using System.Collections.Generic;
using System.Text;

namespace IntegerExemple
{
    static partial class Extentions
    {
        //структуры использую, чтобы все обращения происходили на стаке, а не в куче, так быстрее и это отличает метод от стандартного linq
        //все find останавливают поиск на первом найденом экземпляре
        public static int FindIndx<T>(this T[] source, Predicate<T> predicate) where T : struct
        {
            for (int i = 0; i < source.Length; i++)
            {
                if (predicate(source[i])) return i;
            }
            return -1;
        }
        public static T Find<T>(this T[] source, Predicate<T> predicate) where T : struct
        {
            var res = FindIndx(source, predicate);
            if (res == -1)
            {
                throw new Exception($"{nameof(T)} not found");
            }
            else
            {
                return source[res];
            }
        }
        public static bool TryFind<T>(this T[] source, Predicate<T> predicate, out T founded) where T : struct
        {
            founded = default;
            try
            {
                founded = Find(source, predicate);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static string ToStringNewLine<T>(this T[] source) => ToString(source, Environment.NewLine);
        public static string ToStringSpace<T>(this T[] source) => ToString(source, " ");
        public static string ToStringDotAndSpace<T>(this T[] source) => ToString(source, ", ");
        public static string ToString<T>(this T[] source, string separator)
        {
            var lngth = source.Length - 1;
            var result = "";
            for (int i = 0; i < lngth; i++)
            {
                result += $"{i}: {source[i]}" + separator;
            }
            result += $"{lngth}: {source[lngth]}";
            return result;
        }
    }
}
