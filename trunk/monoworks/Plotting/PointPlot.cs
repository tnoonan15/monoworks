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
using System.Text;
using System.Collections.Generic;

using gl = Tao.OpenGl.Gl;

using MonoWorks.Base;
using MonoWorks.Framework;
using MonoWorks.Rendering;
using MonoWorks.Rendering.Events;

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
	/// The line style.
	/// </summary>
	public enum LineStyle { Solid, Dashed, Dotted };
	
	/// <summary>
	/// Plots a set of points from an ArrayDataSet.
	/// </summary>
	public class PointPlot : AbstractPlot
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		/// <param name="parent"> The parent axes box.</param>
		public PointPlot(AxesBox parent)
			: base(parent)
		{
			color = ColorManager.Global["Blue"];
			DefaultColumns();
		}


		protected ArrayDataSet dataSet = null;
		/// <summary>
		/// The data set to use.
		/// </summary>
		public ArrayDataSet DataSet
		{
			get { return dataSet; }
			set
			{
				dataSet = value;
				selectedIndex = new PlotIndex(dataSet.NumRows);
				selectedIndex.SetAll(false);
				DefaultColumns();
				MakeDirty();
			}
		}


#region The Column Indices

		public void DefaultColumns()
		{
			columns = new int[] { 0, 1, 2, -1, -1, -1 };
		}

		protected int[] columns;
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
				MakeDirty();
			}
		}

		/// <summary>
		/// Access the indices by column index.
		/// </summary>
		public int this[int index]
		{
			get	{return columns[index];}
			set
			{
				columns[index] = value;
				MakeDirty();
			}
		}

		/// <summary>
		/// Access the indices by column name.
		/// </summary>
		public int this[ColumnIndex column]
		{
			get	{return columns[(int)column];}
			set
			{
				columns[(int)column] = value;
				MakeDirty();
			}
		}
		
		/// <summary>
		/// Gets the column name for the given index.
		/// </summary>
		/// <param name="column"> A <see cref="ColumnIndex"/>. </param>
		/// <returns> The column's name. </returns>
		public string GetColumnName(ColumnIndex column)
		{
			return dataSet.GetColumnName(this[column]);
		}
		
		/// <summary>
		/// Gets the name of the column with the given index.
		/// </summary>
		/// <param name="index"> </param>
		/// <returns> The column name. </returns>
		public string GetColumnName(int index)
		{
			return dataSet.GetColumnName(this[index]);
		}

#endregion
		
		
#region Attributes
		
		
		protected PlotShape shape;
		/// <value>
		/// The plot shape.
		/// </value>
		/// <remarks> This is only used if the shape column is -1.</remarks>
		public PlotShape Shape
		{
			get {return shape;}
			set
			{
				shape = value;
				MakeDirty();
			}
		}
		
		protected ColorMap colorMap = new ColorMap();
		/// <value>
		/// The color map used for automatically generating colors.
		/// </value>
		public ColorMap ColorMap
		{
			get
			{
				return colorMap;
			}
		}
		
		protected float markerSize = 4f;
		/// <value>
		/// The marker size;
		/// </value>
		public float MarkerSize
		{
			get {return markerSize;}
			set
			{
				markerSize = value;
				MakeDirty();
			}
		}

		protected bool markersVisible = true;
		/// <summary>
		/// Whether or not to draw the markers.
		/// </summary>
		public bool MarkersVisible
		{
			get { return markersVisible; }
			set
			{
				markersVisible = value;
				MakeDirty();
			}
		}

		
		protected static float[] possibleMarkerSizes = new float[]{2, 4, 6};
		/// <value>
		/// The possible marker sizes.
		/// </value>
		public static float[] PossibleMarkerSizes
		{
			get {return possibleMarkerSizes;}
		}


		protected float lineWidth = 2;
		/// <summary>
		/// The width of the line, in pixels.
		/// </summary>
		public float LineWidth
		{
			get { return lineWidth; }
			set
			{
				lineWidth = value;
				MakeDirty();
			}
		}

		protected LineStyle lineStyle;
		/// <summary>
		/// The line style.
		/// </summary>
		public LineStyle LineStyle
		{
			get { return lineStyle; }
			set
			{
				lineStyle = value;
				MakeDirty();
			}
		}


		protected bool lineVisible = false;
		/// <summary>
		/// Whether or not to draw a line between the points.
		/// </summary>
		public bool LineVisible
		{
			get { return lineVisible; }
			set
			{
				lineVisible = value;
				MakeDirty();
			}
		}

		
#endregion


#region Geometry

		/// <summary>
		/// Update the bounds to fit everything in.
		/// </summary>
		public override void UpdateBounds()
		{
			base.UpdateBounds();

			if (dataSet == null)
			{
				plotBounds.Minima = new Vector(-1, -1, -1);
				plotBounds.Maxima = new Vector(1, 1, 1);
			}
			else // there's some data there
			{

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
		}


		/// <summary>
		/// Sets up the rendering of a shape.
		/// </summary>
		public static void SetupShape(PlotShape shape)
		{
			if (shape == PlotShape.Circle)
				gl.glEnable(gl.GL_POINT_SMOOTH);
			else
				gl.glDisable(gl.GL_POINT_SMOOTH);
		}


		/// <summary>
		/// Compute the plot geometry.
		/// </summary>
		public override void ComputeGeometry()
		{
			base.ComputeGeometry();

			bounds.Reset();
			
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
			double[] shapeRange; // the range of values that the shapes correspond to
			PlotShape[] shapes; // the shapes
			PlotIndex shapeIndex = null; // the index of points for the current shape 
			if (this[ColumnIndex.Shape] < 0) // use the predefined shape
			{
				shapeRange = new double[]{0, 0};
				shapes = new PlotShape[]{shape};
				shapeIndex = new PlotIndex(dataSet.NumRows);
			}
			else // use predefined shapes
			{
				double min, max; // the min/max of the shape column
				dataSet.ColumnMinMax(this[ColumnIndex.Shape], out min, out max);
				double[] shapeRange_ = Bounds.NiceRange(min, max);
				min = shapeRange_[0];
				max = shapeRange_[shapeRange_.Length-1];
				shapeRange = new double[3]{min, (max+min)/2, max}; 
				shapes = new PlotShape[]{PlotShape.Circle, PlotShape.Square};
			}
			

			// generate sizes
			PlotIndex sizeIndex = null;
			float[] sizes; // the sizes
			double[] sizeRange; // the range of values that the size correspond to
			if (this[ColumnIndex.Size] < 0) // use the predefined sizes
			{
				sizeRange = new double[]{0,0};
				sizes = new float[]{markerSize};
				sizeIndex = new PlotIndex(dataSet.NumRows);
			}
			else // generate sizes from a range
			{
				double min, max; // the min/max of the size column
				dataSet.ColumnMinMax(this[ColumnIndex.Size], out min, out max);
				sizeRange = Bounds.NiceRange(min, max, 2.5);
				sizes = new float[sizeRange.Length-1];
				for (int i=0; i<sizes.Length; i++)
					sizes[i] = (float)2*i + 1f;
			}
			
			
			
			displayList = gl.glGenLists(1);
			
			// generate the display list
			gl.glNewList(displayList, gl.GL_COMPILE);

				
			for (int shapeI=0; shapeI<shapes.Length; shapeI++) // cycle through shapes
			{
				// compute the index for this shape
				if (this[ColumnIndex.Shape] >= 0)
					shapeIndex = dataSet.GetColumnIndex(this[ColumnIndex.Shape], shapeRange[shapeI], shapeRange[shapeI+1]);

				// set the marker shape
				SetupShape(shapes[shapeI]);
				
				for (int sizeI=0; sizeI<sizes.Length; sizeI++) // cycle through sizes
				{
					// compute the index for this size
					if (this[ColumnIndex.Size] >= 0)
						sizeIndex = dataSet.GetColumnIndex(this[ColumnIndex.Size], sizeRange[sizeI], sizeRange[sizeI+1]);
					
					// begin the rendering
					gl.glPointSize(sizes[sizeI]); // set the size
			
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
						if (markersVisible)
							gl.glBegin(gl.GL_POINTS);
						foreach (int r in thisIndex)
						{
							double x, y, z;
							x = dataSet[r, columns[0]];
							y = dataSet[r, columns[1]];
							z = dataSet[r, columns[2]];
							Parent.PlotToWorldSpace.Apply(ref x, ref y, ref z);
							if (markersVisible)
								gl.glVertex3d(x, y, z);
							bounds.Resize(x, y, z);
						}
						if (markersVisible)
							gl.glEnd();


						// plot the line for this color/shape/size
						if (lineVisible)
						{
							gl.glLineWidth(lineWidth);
							gl.glBegin(gl.GL_LINE_STRIP);
							foreach (int r in thisIndex)
							{
								double x, y, z;
								x = dataSet[r, columns[0]];
								y = dataSet[r, columns[1]];
								z = dataSet[r, columns[2]];
								Parent.PlotToWorldSpace.Apply(ref x, ref y, ref z);
								gl.glVertex3d(x, y, z);
							}
							gl.glEnd();
							gl.glLineWidth(1);
						}


					} // colorI
					
						
				} // sizeI
				
			} // shapeI
			
			gl.glEndList();

			// add legend items
			if (Parent.Legend != null)
			{
				// color legend entries
				if (colors.Length > 1)
				{
					string colorColumn = dataSet.GetColumnName(this[ColumnIndex.Color]);
					for (int i = 0; i < colors.Length; i++ )
					{
						LegendItem item = new LegendItem();
						item.Icon.LineWidth = lineWidth;
						item.Icon.ShowLine = LineVisible;
						item.Icon.ShowMarker = MarkersVisible;
						item.Icon.Color = colors[i];
						item.Text = String.Format("{0}: {1} to {2}", colorColumn, 
							colorRange[i], colorRange[i+1]);
						if (shapes.Length == 1) // only one shape
							item.Icon.MarkerShape = shapes[0];
						if (sizes.Length == 1) // only one size
							item.Icon.MarkerSize = markerSize;
						Parent.Legend.Add(item);
					}
				}

				// shape legend entries
				if (shapes.Length > 1)
				{
					string shapeColumn = dataSet.GetColumnName(this[ColumnIndex.Shape]);
					for (int i = 0; i < shapes.Length; i++)
					{
						LegendItem item = new LegendItem();
						item.Icon.LineWidth = lineWidth;
						item.Icon.ShowLine = LineVisible;
						item.Icon.ShowMarker = MarkersVisible;
						item.Text = String.Format("{0}: {1} to {2}", shapeColumn,
							shapeRange[i], shapeRange[i + 1]);
						item.Icon.MarkerShape = shapes[i];
						if (colors.Length == 1) // only one color
							item.Icon.Color = colors[0];
						if (sizes.Length == 1) // only one size
							item.Icon.MarkerSize = markerSize;
						Parent.Legend.Add(item);
					}
				}

				// size legend entries
				if (sizes.Length > 1)
				{
					string sizeColumn = dataSet.GetColumnName(this[ColumnIndex.Size]);
					for (int i = 0; i < sizes.Length; i++)
					{
						LegendItem item = new LegendItem();
						item.Icon.LineWidth = lineWidth;
						item.Icon.ShowLine = LineVisible;
						item.Icon.ShowMarker = MarkersVisible;
						item.Text = String.Format("{0}: {1} to {2}", sizeColumn,
							sizeRange[i], sizeRange[i + 1]);
						item.Icon.MarkerSize = sizes[i];
						if (colors.Length == 1) // only one color
							item.Icon.Color = colors[0];
						if (shapes.Length == 1) // only one shape
							item.Icon.MarkerShape = shapes[0];
						Parent.Legend.Add(item);
					}
				}

				// single legend entry, if nothing varies
				if (colors.Length == 1 && shapes.Length == 1 && sizes.Length == 1)
				{
					LegendItem item = new LegendItem();
					item.Icon.LineWidth = lineWidth;
					item.Icon.ShowLine = LineVisible;
					item.Icon.ShowMarker = MarkersVisible;
					item.Text = dataSet.GetColumnName(Columns[2]);
					item.Icon.Color = colors[0];
					item.Icon.MarkerShape = shapes[0];
					item.Icon.MarkerSize = sizes[0];
					Parent.Legend.Add(item);
				}

			}

		}


		public override void RenderOpaque(MonoWorks.Rendering.Viewport viewport)
		{
			base.RenderOpaque(viewport);

			if (!IsVisible)
				return;

			viewport.RenderManager.Lighting.Disable();

			CallDisplayList();

			// render the selection highlight
			gl.glPointSize(12);
			color.Inverse.Setup();
			gl.glBegin(gl.GL_POINTS);
			foreach (int r in selectedIndex)
			{

				double x, y, z;
				x = dataSet[r, columns[0]];
				y = dataSet[r, columns[1]];
				z = dataSet[r, columns[2]];
				Parent.PlotToWorldSpace.Apply(ref x, ref y, ref z);
				gl.glVertex3d(x, y, z);
			}
			gl.glEnd();

			viewport.RenderManager.Lighting.Enable();
		}


		public override void RenderOverlay(Viewport viewport)
		{
			base.RenderOverlay(viewport);
		}

#endregion


#region Hitting and Selection


		protected PlotIndex selectedIndex;
		/// <summary>
		/// The indices of the selected points.
		/// </summary>
		public PlotIndex SelectedIndex
		{
			get { return selectedIndex; }
		}

		public override void Deselect()
		{
			base.Deselect();

			SelectedIndex.AllOff();
		}

		public override bool HitTest(HitLine hitLine)
		{
			if (!base.HitTest(hitLine))
				return false;

			// project each point on to the screen and find the shortest distance
			int closestIndex = -1;
			double shortestDistance = -1;
			foreach (int r in dataSet.DisplayIndex)
			{
				double x = dataSet[r, columns[0]];
				double y = dataSet[r, columns[1]];
				double z = dataSet[r, columns[2]];
				Parent.PlotToWorldSpace.Apply(ref x, ref y, ref z);
				Coord coord = hitLine.Camera.WorldToScreen(x, y, z);

				// compute the distance
				double dist = (coord - hitLine.Screen).MagnitudeSquared;
				if (closestIndex < 0 || dist < shortestDistance)
				{
					closestIndex = r;
					shortestDistance = dist;
				}
			}

			// determine if one was selected
			double tolSquared = 64;
			//Console.WriteLine("shorted distance {0} at index {1}", Math.Sqrt(shortestDistance), closestIndex);
			if (shortestDistance < tolSquared)
			{
				selectedIndex[closestIndex] = true;
				IsSelected = true;
				return true;
			}

			return false;
		}


		public override void OnButtonRelease(MouseButtonEvent evt)
		{
			if (evt.Handled)
			{
				Deselect();
				return;
			}

			base.OnButtonRelease(evt);

			if (evt.Modifier != InteractionModifier.Shift)
				Deselect();
			if (HitTest(evt.HitLine))
			{
				evt.Handle();
			}
		}

		public override string SelectionDescription
		{
			get
			{
				// determine which index is selected
				int r = selectedIndex.First;
				if (r < 0)
					return "";

				StringBuilder desc = new StringBuilder();
				for (int c=0; c<dataSet.NumColumns; c++)
				{
					desc.Append(dataSet.GetColumnName(c));
					desc.Append(": " );
					desc.Append(dataSet[r, c]);
					desc.AppendLine("");
				}
				return desc.ToString();
			}
		}

#endregion


	}
}
