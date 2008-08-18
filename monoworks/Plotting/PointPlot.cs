// PointPlot.cs - MonoWorks Project
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

using gl = Tao.OpenGl.Gl;

using MonoWorks.Base;
using MonoWorks.Rendering;

namespace MonoWorks.Plotting
{

	/// <summary>
	/// Indicates a valid 
	/// </summary>
	public enum ColumnIndex {X = 0, Y, Z, Color, Shape, Size};
	
	/// <summary>
	/// Possible shapes for plots.
	/// </summary>
	public enum PlotShape {Circle, Square};
	
	/// <summary>
	/// Plots a set of points from an ArrayDataSet.
	/// </summary>
	public class PointPlot : Plottable
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		/// <param name="parent"> The parent axes box.</param>
		public PointPlot(AxesBox parent)
			: base(parent)
		{
			color = new MonoWorks.Rendering.Color(0F, 0F, 1F);
		}


		protected ArrayDataSet dataSet = null;
		/// <summary>
		/// The data set to use.
		/// </summary>
		public ArrayDataSet DataSet
		{
			get { return dataSet; }
			set { dataSet = value; }
		}


#region The Column Indices

		protected int[] columns = new int[]{0, 1, 2, -1, -1, -1};
		/// <summary>
		/// The indices of the columns to plot.
		/// </summary>
		public int[] Columns
		{
			get { return columns; }
			set
			{
				if (value.Length != 6)
					throw new Exception("Point plot column arrays should have 6 elements.");
				columns = value; 
			}
		}

		/// <summary>
		/// Access the indices by column index.
		/// </summary>
		public int this[int index]
		{
			get	{return columns[index];}
			set	{columns[index] = value;}
		}

		/// <summary>
		/// Access the indices by column name.
		/// </summary>
		public int this[ColumnIndex column]
		{
			get	{return columns[(int)column];}
			set	{columns[(int)column] = value;}
		}
		

#endregion
		
		
#region Attributes
		
		
		protected PlotShape shape;
		/// <value>
		/// The plot shape.
		/// </value>
		/// <remarks> This is only used if the shape column is -1.</remarks>
		public PlotShape PlotShape
		{
			get {return shape;}
			set {shape = value;}
		}
		
		protected ColorMap colorMap = new ColorMap();
		/// <value>
		/// The color map used for automatically generating colors.
		/// </value>
		public ColorMap ColorMap
		{
			get {return colorMap;}
		}
		
#endregion


#region Geometry

		/// <summary>
		/// Update the bounds to fit everything in.
		/// </summary>
		public override void UpdateBounds()
		{
			base.UpdateBounds();

			Vector minima = new Vector();
			Vector maxima = new Vector();
			for (int i = 0; i < 3; i++)
			{
				if (columns[i] < 0 || columns[i] >= dataSet.NumColumns)
					throw new Exception(String.Format("Index {0} is out of range", columns[i]));

				double min, max;
				dataSet.ColumnMinMax(columns[i], out min, out max);
				minima[i] = min;
				maxima[i] = max;
			}
			plotBounds.Minima = minima;
			plotBounds.Maxima = maxima;

		}


		/// <summary>
		/// Compute the plot geometry.
		/// </summary>
		public override void ComputeGeometry()
		{
			base.ComputeGeometry();
			
			// generate the colors			
			double[] colorRange; // the range of values that the colors correspond to
			Color[] colors; // the colors
			PlotIndex colorIndex = null; // the index of points for the current color 
			if (this[ColumnIndex.Color] < 0) // use the predefined color
			{
				colorRange = new double[]{0, 0};
				colors = new Color[]{color};
				colorIndex = new PlotIndex(dataSet.NumRows);
			}
			else // generate the values from a color map
			{
				double min, max; // the min/max of the color column
				dataSet.ColumnMinMax(this[ColumnIndex.Color], out min, out max);
				colorRange = Bounds.NiceRange(min, max);
				colors = colorMap.GetColors(colorRange.Length-1);
			}
			

			// generate shapes
			PlotIndex shapeIndex = new PlotIndex(dataSet.NumRows);
			

			// generate sizes
			PlotIndex sizeIndex = new PlotIndex(dataSet.NumRows);
			
			
			
			displayList = gl.glGenLists(1);
			
			// generate the display list
			gl.glNewList(displayList, gl.GL_COMPILE);

				
			for (int shapeI=0; shapeI<1; shapeI++) // cycle through shapes
			{
				
				for (int sizeI=0; sizeI<1; sizeI++) // cycle through sizes
				{
					gl.glPointSize(3F);
					gl.glBegin(gl.GL_POINTS);
			
					for (int colorI=0; colorI<colors.Length; colorI++) // cycle through colors
					{				
						// compute the index for this color
						if (this[ColumnIndex.Color] >= 0)
							colorIndex = dataSet.GetColumnIndex(this[ColumnIndex.Color], colorRange[colorI], colorRange[colorI+1]);

						colors[colorI].Setup();
						
						PlotIndex thisIndex = dataSet.DisplayIndex.Copy();
						thisIndex.Intersect(colorIndex); // apply the color index
						thisIndex.Intersect(shapeIndex); // apply the shape index
						thisIndex.Intersect(sizeIndex); // apply the size index
						
						// plot the points for this color/shape/size
						for (int r = 0; r < dataSet.NumRows; r++ )
						{
							if (thisIndex[r])
							{
								double x, y, z;
								x = dataSet[r, columns[0]];
								y = dataSet[r, columns[1]];
								z = dataSet[r, columns[2]];
								parent.PlotToWorldSpace.Apply(ref x, ref y, ref z);
								gl.glVertex3d(x, y, z);
							}
						}
					} // colorI
						
				} // sizeI
				
			} // shapeI
				
						
			gl.glEnd();

			gl.glEndList();
		}


		public override void RenderOpaque(MonoWorks.Rendering.IViewport viewport)
		{
			base.RenderOpaque(viewport);

			CallDisplayList();
		}

#endregion

	}
}
