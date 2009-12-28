//   Line.cs - MonoWorks Project
//
//    Copyright Andy Selvig 2008
//
//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU Lesser General Public License as published 
//    by the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU Lesser General Public License for more details.
//
//    You should have received a copy of the GNU Lesser General Public 
//    License along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using MonoWorks.Base;
using MonoWorks.Rendering;
using MonoWorks.Rendering.Events;

using gl = Tao.OpenGl.Gl;

namespace MonoWorks.Modeling.Sketching
{
	
	/// <summary>
	/// The Line class represents a line in 3D space connected by two point.
	/// </summary>
	public class Line : Sketchable
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public Line(Sketch sketch) : base(sketch)
		{
			IsClosed = false;
		}
		
		
		/// <summary>
		/// Initialization constructor.
		/// Sets the values of the first two points.
		/// </summary>
		/// <param name="sketch">The sketch.</param>
		/// <param name="p1"> The first <see cref="Point"/>. </param>
		/// <param name="p2"> The second <see cref="Point"/>. </param>
		public Line(Sketch sketch, Point p1, Point p2) : this(sketch)
		{
			Points.Add(p1);
			Points.Add(p2);
		}
		
		
#region Attributes

		/// <value>
		/// The list of points.
		/// </value>
		public List<Point> Points
		{
			get { return this["points"] as List<Point>; }
		}

		/// <summary>
		/// Call this method when the points list is possibly updated.
		/// </summary>
		public void PointsUpdated()
		{
			RaiseAttributeUpdated("points");
		}

		/// <summary>
		/// Whether or not the line is closed on itself.
		/// </summary>
		public bool IsClosed
		{
			get { return (bool)this["isClosed"]; }
			set { this["isClosed"] = value; }
		}
			
#endregion
		
		
#region Geometry
		
		/// <summary>
		/// Computes the raw points needed to draw the sketch.
		/// </summary>
		public override void ComputeGeometry()
		{
			base.ComputeGeometry();

			if (IsClosed)
			{
				solidPoints = new Vector[Points.Count+1];
				directions = new Vector[Points.Count+1];
			}
			else // open
			{
				solidPoints = new Vector[Points.Count];
				directions = new Vector[Points.Count];
			}
			for (int i=0; i<Points.Count; i++)
			{
				solidPoints[i] = Points[i].ToVector();
				
				// compute the direction
				if (i < Points.Count - 1)
					directions[i] = (solidPoints[i] - Points[i + 1].ToVector()).Normalize();
				else if (Points.Count > 1) // only compute direction if there's more than one point
					directions[i] = (solidPoints[i] - solidPoints[i - 1]).Normalize();
				else
					directions[i] = new Vector();
				
				bounds.Resize(solidPoints[i]);
			}
			
			// if closed, the first point needs to be copied to the end
			if (IsClosed)
			{
				solidPoints[Points.Count] = solidPoints[0];
				directions[Points.Count] = directions[0];
			}

			// for lines, the solid and wireframe points are the same
			wireframePoints = solidPoints;
		}
		
		
		/// <summary>
		/// Draws the vertices to the current GL context.
		/// Updates the geometry if the number of points has changed.
		/// </summary>
		public override void DrawVertices()
		{
			if ((!IsClosed && Points.Count != solidPoints.Length) || 
				(IsClosed && Points.Count+1 != solidPoints.Length))
				ComputeGeometry();
			base.DrawVertices();
		}
		
#endregion


#region Hit Testing

		public override bool HitTest(HitLine hit)
		{
			if (Points.Count == 0)
				return false;

			for (int i = 0; i < Points.Count - 1; i++)
			{
				HitLine line = new HitLine() {
					Front = Points[i].ToVector(),
					Back = Points[i+1].ToVector(),
					Camera = hit.Camera
				};
				if (line.ShortestDistance(hit) < HitTol * hit.Camera.ViewportToWorldScaling)
				{
					lastHit = hit.GetIntersection((ParentEntity as Sketch).Plane);
					return true;
				}
			}
			return false;
		}

#endregion

	}
	
}
