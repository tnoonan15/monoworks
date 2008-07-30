using System;
using System.Collections.Generic;


namespace MonoWorks.Plotting
{
	/// <summary>
	/// The axes used in the plotting demos.
	/// </summary>
	public class TestAxes : AxesBox
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public TestAxes()
			: base()
		{

			// make the array data set
			arrayData = new ArrayDataSet(1024, 4);
			Random rand = new Random();
			for (int r = 0; r < arrayData.NumRows; r++)
			{
				arrayData[r, 0] = rand.NextDouble() * 2 * Math.PI;
				arrayData[r, 1] = rand.NextDouble() * Math.PI;
				arrayData[r, 2] = Math.Sin(arrayData[r, 0]) * Math.Cos(arrayData[r, 1]);
			}

			// add an axes box and plot
			PointPlot plot1 = new PointPlot(this);
			plot1.DataSet = arrayData;

		}


		protected ArrayDataSet arrayData;

	}
}
