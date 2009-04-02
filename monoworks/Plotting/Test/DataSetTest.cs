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
			data.ParseBits("one byte");
			Assert.AreEqual(11, data.NumColumns);
			Assert.AreEqual(256, data.NumRows);

			// test the column naming
			Assert.AreEqual("one byte bit0", data.GetColumnName(3));
			Assert.AreEqual("one byte bit1", data.GetColumnName(4));
			Assert.AreEqual("one byte bit2", data.GetColumnName(5));

			// check the range of the computed values
			double min, max;
			data.ColumnMinMax(6, out min, out max);
			Assert.AreEqual(0, min);
			Assert.AreEqual(1, max);

			// test some actual values
			Assert.AreEqual(1, data[7, 4]);
		}

    }
}
