using System;
using System.Collections.Generic;

using NUnit.Framework;

using MonoWorks.Plotting;

namespace MonoWorks.PlottingTest
{
	/// <summary>
	/// This fixture contains tests for data set handling in the plotting library.
	/// </summary>
    [TestFixture]
    public class DataSetTest
    {
		/// <summary>
		/// Tests reading a simple array data set from a file.
		/// </summary>
        [Test]
        public void TestArrayData()
        {
            ArrayDataSet data = new ArrayDataSet();
            data.FromFile("Test/array-data.txt", '\t');
            Assert.AreEqual(5, data.NumColumns);
            Assert.AreEqual(3, data.NumRows);
        }


		/// <summary>
		/// Tests reading HMS time from a file.
		/// </summary>
		[Test]
		public void TestHmsTime()
		{
			ArrayDataSet data = new ArrayDataSet();
			data.FromFile("Test/array-data-hms.txt", '\t');
			Assert.AreEqual(5, data.NumColumns);
			Assert.AreEqual(338, data.NumRows);
		}

    }
}
