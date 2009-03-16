// TestAxes2D.cs - MonoWorks Project
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

using MonoWorks.Rendering;

namespace MonoWorks.Plotting
{
	/// <summary>
	/// The axes used in the 2D plotting demos.
	/// </summary>
	public class TestAxes2D : AxesBox
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public TestAxes2D()
			: base()
		{

			// make the array data set
			arrayData = new ArrayDataSet(1024*1, 4);
			double dt = 0.005; // time step
			for (int r = 0; r < arrayData.NumRows; r++)
			{
				double t = r * dt;
				arrayData[r, 0] = t;
				arrayData[r, 1] = Math.Sin(t);
				arrayData[r, 2] = Math.Cos(t*2);
				arrayData[r, 3] = 0;
			}
			arrayData.SetColumnName(0, "time");
			arrayData.SetColumnName(1, "sin(t)");
			arrayData.SetColumnName(2, "cos(2t)");
			arrayData.SetColumnName(3, "zero");
						

			// add a plot
			pointPlot1 = new PointPlot(this);
			pointPlot1.DataSet = arrayData;
			pointPlot1.Columns[2] = 1;
			pointPlot1.Shape = PlotShape.Square;
			pointPlot1.LineVisible = true;
			
			// add a plot
			pointPlot2 = new PointPlot(this);
			pointPlot2.DataSet = arrayData;
			pointPlot2.Shape = PlotShape.Circle;
			pointPlot2.Color = new Color(0, 1f, 0);
			pointPlot2.LineVisible = true;
			pointPlot2.LineWidth = 1;
		}


		protected ArrayDataSet arrayData;
		/// <value>
		/// The array data set.
		/// </value>
		public ArrayDataSet ArrayData
		{
			get {return arrayData;}
		}
		

		protected PointPlot pointPlot1;
		/// <value>
		/// The first point plot.
		/// </value>
		public PointPlot PointPlot1
		{
			get {return pointPlot1;}
		}

		protected PointPlot pointPlot2;
		/// <value>
		/// The second point plot.
		/// </value>
		public PointPlot PointPlot2
		{
			get {return pointPlot2;}
		}
	}
}
