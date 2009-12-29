// LegendIcon.cs - MonoWorks Project
//
//  Copyright (C) 2009 Andy Selvig
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

using MonoWorks.Base;
using MonoWorks.Rendering;
using MonoWorks.Controls;

namespace MonoWorks.Plotting
{
	public class LegendIcon : Control2D
	{

		public LegendIcon()
			: base()
		{
			//size = new Coord(12, 12);

			ShowLine = true;
			ShowMarker = true;

			Color = ColorManager.Global["Black"];
			LineWidth = 2f;
			MarkerSize = 4f;
			MarkerShape = PlotShape.Square;
		}



#region Attributes

		/// <summary>
		/// The color of the marker.
		/// </summary>
		public Color Color { get; set; }

		/// <summary>
		/// The shape of the marker.
		/// </summary>
		public PlotShape MarkerShape { get; set; }

		/// <summary>
		/// The size of the marker.
		/// </summary>
		public float MarkerSize { get; set; }

		/// <summary>
		/// The line width.
		/// </summary>
		public float LineWidth { get; set; }

		/// <summary>
		/// The line style.
		/// </summary>
		public LineStyle LineStyle { get; set; }

		/// <summary>
		/// Whether or not to show the line.
		/// </summary>
		public bool ShowLine { get; set; }

		/// <summary>
		/// Whether or not to show the marker.
		/// </summary>
		public bool ShowMarker { get; set; }

#endregion


#region Rendering
		
		public override void ComputeGeometry()
		{
			base.ComputeGeometry();
			
			MinSize = new Coord(14, 14);
			RenderSize = MinSize;
		}


		protected override void Render(RenderContext context)
		{
			base.Render(context);

			Color.Setup();

//			if (ShowMarker)
//			{
//				PointPlot.SetupShape(MarkerShape);
//				gl.glPointSize(MarkerSize * 2);
//				gl.glBegin(gl.GL_POINTS);
//				(size / 2).glVertex();
//				gl.glEnd();
//			}
//
//			if (ShowLine)
//			{
//				gl.glLineWidth(LineWidth);
//				gl.glBegin(gl.GL_LINES);
//				double y = Height / 2;
//				gl.glVertex2d(0 , y);
//				gl.glVertex2d(Width , y);
//				gl.glEnd();
//			}
		}

#endregion


	}
}
