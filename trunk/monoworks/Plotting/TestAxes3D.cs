// TestAxes3D.cs - MonoWorks Project
//
//  Copyright (C) 2008 Andy Selvig
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
//
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA 

using System;
using System.Collections.Generic;


namespace MonoWorks.Plotting
{
	/// <summary>
	/// The axes used in the 3D plotting demos.
	/// </summary>
	public class TestAxes3D : AxesBox
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public TestAxes3D()
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
				arrayData[r, 3] = arrayData[r, 0] + arrayData[r, 1];
			}
			arrayData[512, 2] = Double.NaN; // just to test out the handling of NaN
			arrayData.SetColumnName(0, "x");
			arrayData.SetColumnName(1, "y");
			arrayData.SetColumnName(2, "sin(x)*cos(y)");
			arrayData.SetColumnName(3, "x + y");
						

			// add an axes box and plot
			pointPlot = new PointPlot(this);
			pointPlot.DataSet = arrayData;
			pointPlot.Shape = PlotShape.Square;
		}


		protected ArrayDataSet arrayData;
		/// <value>
		/// The array data set.
		/// </value>
		public ArrayDataSet ArrayData
		{
			get {return arrayData;}
		}
		

		protected PointPlot pointPlot;
		/// <value>
		/// The point plot.
		/// </value>
		public PointPlot PointPlot
		{
			get {return pointPlot;}
		}
		
	}
}
