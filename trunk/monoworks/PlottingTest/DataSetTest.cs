using System;
using System.Collections.Generic;

using NUnit.Framework;

using MonoWorks.Plotting;

namespace MonoWorks.PlottingTest
{
    [TestFixture]
    public class DataSetTest
    {
        [Test]
        public void TestArrayData()
        {
            ArrayDataSet data = new ArrayDataSet();
            data.FromFile("SampleArrayData.txt", '\t');
            Assert.AreEqual(5, data.NumColumns);
            Assert.AreEqual(3, data.NumRows);
        }

    }
}
