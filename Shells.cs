using System;
using System.Collections.Generic;
using System.Text;

namespace IntegerExemple
{
    enum Color : int
    {
        Black,
        White
    }
    struct Shell
    {
        public Color ShellColor { get; set; }
        public override string ToString()
        {
            return ShellColor.ToString();
        }
    }

    class ShellFactory : RandomFactory<Shell>
    {
        public ShellFactory() : base(0, Enum.GetValues(typeof(Color)).Length) { }
        protected override Shell Create(int choosen)
        {
            return new Shell { ShellColor = (Color)choosen };
        }

        protected override int GetSingleSeed(Shell choosen)
        {
            return (int)choosen.ShellColor;
        }
    }

    //по сути, это лишь конфигурация класса-родителя
    [DIClass]
    class ConcrateTestShellFactory : ShellFactory, ISampleFabric<Shell>
    {
        public Shell[] CreateSample()
        {
            return CreateSingleRandom(10, GetStandart());
        }

        public Shell GetStandart()
        {
            return new Shell { ShellColor = Color.Black };
        }
    }
}
