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
            data.FromFile("Test/array-data.txt");
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
			data.FromFile("Test/array-data-hms.txt");
			Assert.AreEqual(5, data.NumColumns);
			Assert.AreEqual(338, data.NumRows);
			Assert.AreEqual(34283.1, data[1,0]);
		}

		/// <summary>
		/// Tests parsing a byte into bits.
		/// </summary>
		[Test]
		public void TestParseBits()
		{
			ArrayDataSet data = new ArrayDataSet();
			data.FromFile("Test/array-data-byte.csv");
			data.ParseBits("one byte", 8);
			Assert.AreEqual(10, data.NumColumns);
			Assert.AreEqual(256, data.NumRows);
			Assert.AreEqual(1, data[7, 3]);
		}

    }
}
