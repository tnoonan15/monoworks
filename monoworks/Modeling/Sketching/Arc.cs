// Arc.cs - MonoWorks Project
//
// Copyright (C) 2008 Andy Selvig
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
using MonoWorks.Rendering.Events;

using gl = Tao.OpenGl.Gl;

namespace MonoWorks.Modeling.Sketching
{
	
	
	/// <summary>
	/// The Arc class represents a circular arc with a 
	/// defined center, start point, and sweep angle.
	/// </summary>
	public class Arc : Sketchable
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public Arc(Sketch sketch) : this(sketch, null, null, new Angle())
		{
		}
		
		/// <summary>
		/// Initialization constructor.
		/// </summary>
		/// <param name="sketch">The sketch.</param>
		/// <param name="center"> The center. </param>
		/// <param name="start"> The starting point. </param>
		/// <param name="sweep"> The sweep angle. </param>
		public Arc(Sketch sketch, Point center, Point start, Angle sweep) : base(sketch)
		{
			Center = center;
			Start = start;
			Sweep = sweep;
		}
		
	
		
#region Attributes

		/// <value>
		/// The center.
		/// </value>
		public Point Center
		{
			get {return (Point)this["center"];}
			set
			{
				this["center"] = value;
			}
		}

		/// <value>
		/// The starting point.
		/// </value>
		public Point Start
		{
			get {return (Point)this["start"];}
			set
			{
				this["start"] = value;
			}
		}

		/// <summary>
		/// The radius of the arc.
		/// </summary>
		public Length Radius
		{
			get { return (Start - Center).Magnitude; }
		}

		/// <value>
		/// The sweep angle.
		/// Rotations are applied using the right hand rule around the normal vector.
		/// </value>
		public Angle Sweep
		{
			get {return (Angle)this["sweep"];}
			set
			{
				this["sweep"] = value;
			}
		}
			
#endregion
		
		
#region Rendering

		/// <summary>
		/// Points used to draw the complete circle.
		/// </summary>
		private Vector[] circlePoints;

		/// <summary>
		/// Computes the raw points needed to draw the sketch.
		/// </summary>
		public override void ComputeGeometry()
		{
			base.ComputeGeometry();

			if (Center == null || Start == null)
				return;

			int N = ModelingOptions.Global.CircleDivs; // number of circle divisions

			// compute the arc points
			Vector centerVec = Center.ToVector();
			Vector radius = (Start-Center).ToVector();
			Angle dSweep = Sweep / (double)N;
			Vector normal = Sketch.Plane.Plane.Normal;
			solidPoints = new Vector[N+1];
			directions = new Vector[N+1];
			for (int i=0; i<=N; i++)
			{
				Vector thisPos = centerVec + radius.Rotate(normal, dSweep*i);
				solidPoints[i] = thisPos;
				bounds.Resize(solidPoints[i]);
				
				// compute the direction
				directions[i] = (centerVec - thisPos).Cross(normal).Normalize();
			}

			// compute the circle points
			dSweep = Angle.TwoPi / (double)N;
			circlePoints = new Vector[N+1];
			for (int i = 0; i <= N; i++)
			{
				Vector thisPos = centerVec + radius.Rotate(normal, dSweep * i);
				circlePoints[i] = thisPos;
			}
			
			// make the wireframe points the first, middle, and last solid points
			wireframePoints = new Vector[3];
			wireframePoints[0] = solidPoints[0];
			wireframePoints[1] = solidPoints[solidPoints.Length/2];
			wireframePoints[2] = solidPoints[solidPoints.Length-1];
		}

		/// <summary>
		/// Draw the arc-specific decorations.
		/// </summary>
		public override void RenderOpaque(Viewport viewport)
		{
			if ((IsHovering || IsSelected) && Center != null)
			{
				Color highlightColor = ModelingOptions.Global.GetColor("sketchable", HitState.Hovering);

				if (Start != null)
				{
					// draw the circle
					gl.glLineWidth(1);
					highlightColor.Setup();
					gl.glLineStipple(2, 0xFF00);
					gl.glBegin(gl.GL_LINE_LOOP);
					foreach (var point in circlePoints)
						point.glVertex();
					gl.glEnd();
					gl.glLineStipple(1, 0xffff);

					// highlight the radius lines
					gl.glBegin(gl.GL_LINE_STRIP);
					Center.glVertex();
					Start.glVertex();
					gl.glEnd();

					gl.glBegin(gl.GL_LINE_STRIP);
					Center.glVertex();
					solidPoints[solidPoints.Length-1].glVertex();
					gl.glEnd();
				}
			}

			base.RenderOpaque(viewport);
		}


		public override void DrawVertices()
		{
			if (IsDirty)
				ComputeGeometry();
			if (solidPoints.Length > 0)
			{
				solidPoints[0].glVertex();
				solidPoints[solidPoints.Length - 1].glVertex();
			}
		}
		
#endregion


#region Hit Testing
		
		public override bool HitTest(HitLine hit)
		{
			if (Center == null)
				return false;

			// set the last hit, even if we didn't hit anything
			lastHit = hit.GetIntersection((Parent as Sketch).Plane.Plane);

			// test for hitting the center
			Coord center = hit.Camera.WorldToScreen(Center.ToVector());
			double distance = (hit.Screen - center).Magnitude;
			if (distance < Arc.HitTol)
				return true;

			if (Start == null)
				return false;

			// test for hitting the circle
			Coord start = hit.Camera.WorldToScreen(Start.ToVector());
			double radius = (start - center).Magnitude;
			if (Math.Abs(distance - radius) < Arc.HitTol)
				return true;

			return false;
		}

#endregion

	}
}
