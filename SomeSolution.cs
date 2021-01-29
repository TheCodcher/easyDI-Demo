using System;
using System.Collections.Generic;
using System.Text;

namespace IntegerExemple
{
    //чет типа интеграционного тестирования
    class StructFinderTest<T> where T : struct
    {
        ISampleFabric<T> sampleFabric;
        ICommunicable<string> resultUI;
        public StructFinderTest(ISampleFabric<T> sampleFabric, ICommunicable<string> resultUI)
        {
            this.sampleFabric = sampleFabric;
            this.resultUI = resultUI;
        }
        public void RunTest()
        {
            var exmpl = sampleFabric.CreateSample();
            var testExmpl = sampleFabric.GetStandart();
            resultUI.Write(exmpl.ToStringDotAndSpace());
            resultUI.Write($"Find {testExmpl} in {exmpl.FindIndx(sh => sh.Equals(testExmpl))}");
        }
    }
    //не любит рефлексия генерики
    [DIClass]
    class ShellFinderTest : StructFinderTest<Shell>
    {
        public ShellFinderTest(ISampleFabric<Shell> sampleFabric, ICommunicable<string> resultUI) : base(sampleFabric, resultUI) { }
    }
}
