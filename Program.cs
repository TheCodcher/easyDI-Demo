using System;

namespace IntegerExemple
{
    class Program
    {
        static void Main(string[] args)
        {
            //есть зависимости, ест и di, мы же программисты
            new DICargo().Run(typeof(ShellFinderTest), "RunTest");
            Console.ReadKey();
        }
    }
}
