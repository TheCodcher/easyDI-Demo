using System;
using System.Collections.Generic;
using System.Text;

namespace IntegerExemple
{
    interface ICommunicable<T>
    {
        void Write(T message);
        T Read();
    }
    [DIClass]
    class ConsoleUI : ICommunicable<string>
    {
        public string Read()
        {
            return Console.ReadLine();
        }

        public void Write(string message)
        {
            Console.WriteLine(message);
        }
    }
    //[DIClass]
    //class CalculatorUI : ICommunicable<int>
    //{
    //    public CalculatorUI()
    //    {

    //    }
    //    [DICtor]
    //    public CalculatorUI(ConsoleUI df)
    //    {

    //    }
    //    public int Read()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public void Write(int message)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
}
