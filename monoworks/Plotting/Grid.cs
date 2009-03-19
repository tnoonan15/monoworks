// Grid.cs - Slate Mono Application Framework
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
	/// Represents the grid displayed at the edges of the axes box.
	/// </summary>
	public class Grid : Plottable
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public Grid(AxesBox parent) : base(parent)
		{
			Color = ColorManager.Global["Gray"];
		}
		
		

		protected Axis[] axes = new Axis[2];
		/// <value>
		/// The two axes that this grid draws between.
		/// </value>
		public Axis[] Axes
		{
			get {return axes;}
			set
			{
				if (axes.Length != 2)
					throw new Exception("Grid.Axes should always only have 2 elements.");
				axes = value;
			}
		}
		
		protected Vector corner;
		/// <value>
		/// The corner that anchors the grid.
		/// </value>
		/// <remarks> The axes may be on the other side of the bounds from the grid.
		/// This says which part of the bounds to draw the grid on.</remarks>
		public Vector Corner
		{
			get {return corner;}
			set
			{
				corner = value;
				MakeDirty();
			}
		}
		

		
		public override void RenderOpaque(Viewport viewport)
		{
			base.RenderOpaque(viewport);

			if (!IsVisible)
				return;

			if (axes[0] == null && corner == null)
				return;

			gl.glLineWidth(1f);
			gl.glBegin(gl.GL_LINES);
			for (int n = 0; n < 2; n++) // axes number
			{
				int nOther = 1 - n; // number of the other axes
				int dim = axes[n].Dimension; // dimension of this axis
				int dimOther = axes[nOther].Dimension; // dimension of the other axis
				double firstTickStep = axes[n].FirstWorldTickStep; // first tick step in world coords
				double tickStep = axes[n].WorldTickStep; // all other tick steps in world coords

				// figure out the corner to actually start in
				Vector corner_ = Corner.Copy();
				if (corner_[dim] == axes[n].Stop[dim])
					corner_[dim] = axes[n].Start[dim];

				// get the two points that transverse the axis
				Vector currentPoint1 = corner_; // one of the points moving along the axis
				Vector currentPoint2 = corner_.Copy(); // one of the points moving along the axis 
				if (corner_[dimOther] == axes[nOther].Start[dimOther])
					currentPoint2[dimOther] = axes[nOther].Stop[dimOther];
				else
				    currentPoint2[dimOther] = axes[nOther].Start[dimOther];

				// initialize the points to the first tick
				currentPoint1[dim] += firstTickStep;
				currentPoint2[dim] += firstTickStep;

				// cycle through tick marks for this dimension
				for (int t = 0; t < axes[n].TickVals.Length; t++)
				{
					currentPoint1.glVertex();
					currentPoint1[dim] += tickStep;
					currentPoint2.glVertex();
					currentPoint2[dim] += tickStep;
				}
			}
			gl.glEnd();
		}


		public override bool HitTest(HitLine hitLine)
		{
			return false;
		}
		
	}
}
 