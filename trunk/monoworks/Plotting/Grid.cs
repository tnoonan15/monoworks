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
		
			
		/// <summary>
		/// Generates a display list for the grid.
		/// </summary>
		public override void ComputeGeometry()
		{
			base.ComputeGeometry();
			
			if (axes[0] != null && corner != null)
			{			
				displayList = gl.glGenLists(1);
				
				
				// generate the display list
				gl.glNewList(displayList, gl.GL_COMPILE);
				
//				gl.glEnable(gl.GL_LINE_SMOOTH);
				
				gl.glBegin(gl.GL_LINES);
				for (int n=0; n<2; n++) // axes number
				{
					int nOther = 1 - n; // number of the other axes
					int dim = axes[n].Dimension; // dimension of this axis
					int dimOther = axes[nOther].Dimension; // dimension of the other axis
					double firstTickStep = axes[n].FirstWorldTickStep; // first tick step in world coords
					double tickStep = axes[n].WorldTickStep; // all other tick steps in world coords

					// get the two points that transverse the axis
					Vector currentPoint1 = Corner.Copy(); // one of the points moving along the axis
					Vector currentPoint2 = Corner.Copy(); // one of the points moving along the axis 
					if (corner[dimOther] == axes[nOther].Start[dimOther])
						currentPoint2[dimOther] = axes[nOther].Stop[dimOther];
					else
						currentPoint2[dimOther] = axes[nOther].Start[dimOther];
					
					// get the sign of the travel across the axis
					double travelSign;
					if (corner[dim] == axes[n].Start[dim])
						travelSign = 1;
					else // travel backwards
						travelSign = -1;
					
					currentPoint1[dim] += travelSign*firstTickStep;
					currentPoint2[dim] += travelSign*firstTickStep;
					
					// cycle through tick marks for this dimension
					for (int t=0; t<axes[n].TickVals.Length; t++)
					{
						gl.glVertex3d(currentPoint1[0], currentPoint1[1], currentPoint1[2]);
						currentPoint1[dim] += travelSign*tickStep;
						gl.glVertex3d(currentPoint2[0], currentPoint2[1], currentPoint2[2]);
						currentPoint2[dim] += travelSign*tickStep;
					}
				}
//				gl.glDisable(gl.GL_LINE_SMOOTH);
				gl.glEnd();
				
				
				gl.glEndList();
			}
		}

		
		public override void RenderOpaque(IViewport viewport)
		{
			base.RenderOpaque(viewport);

			if (!visible)
				return;

			CallDisplayList();
		}


		public override bool HitTest(HitLine hitLine)
		{
			return false;
		}
		
	}
}
 