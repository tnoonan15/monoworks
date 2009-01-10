//   RefPlane.cs - MonoWorks Project
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

using gl = Tao.OpenGl.Gl;

using MonoWorks.Base;
using MonoWorks.Rendering;

namespace MonoWorks.Model
{
	
	/// <summary>
	/// The RefPlane entity is a reference element that represents
	/// a plane in 3D space. It is used as the base for 2D sketches.
	/// </summary>
	public class RefPlane : Reference
	{
		
		/// <summary>
		/// Default constructor.
		/// </summary>
		public RefPlane() : base()
		{
			quadCorners = null;
		}
		
		
		
#region Attributes
		
		/// <value>
		/// The plane geometry.
		/// </value>
		public Plane Plane
		{
			get {return (Plane)this["plane"];}
			set { this["plane"] = value;}
		}
		
#endregion
		
		
#region Geometry

		/// <summary>
		/// The corners of the quadrilateral that represents the plane.
		/// </summary>
		protected Vector[] quadCorners;
		
#endregion		
		
		
#region Rendering

		/// <summary>
		/// Computes the plane's geometry.
		/// </summary>
		public override void ComputeGeometry()
		{
			base.ComputeGeometry();
			
			double radius = GetDrawing().Bounds.Radius; // radius of the bounds
//			Console.WriteLine("bounds radius {0}", radius);

			// find one corner of the plane to draw
			Vector direction = Plane.Normal;
			Vector corner;
			if (direction[1]==0 && direction[2]==0) // the plane is in the Y-Z axis
				corner = new Vector(0.0, 1.0, 0.0);
			else // the plane is not in the Y-Z axis
				corner = new Vector(1.0, 0.0, 0.0);			
			corner = direction.Cross(corner).Normalize();
			corner = corner.Rotate(direction, new Angle(Angle.PI/4.0)) * (1.0 * radius);
			
			// find the center of the plane to draw
			Vector boundsCenter = GetDrawing().Bounds.Center;
			if (boundsCenter == null)
			{
				Console.WriteLine("bounds center null");
				boundsCenter = new Vector();
			}
			else
				Console.WriteLine("bounds center not null");
			Vector planeCenter = Plane.Center.ToVector();
			Vector planeToBounds = planeCenter - boundsCenter;
			double dist = planeToBounds.Magnitude * planeToBounds.Dot(Plane.Normal) 
				/ planeToBounds.Magnitude / Plane.Normal.Magnitude;
			Vector center = boundsCenter + Plane.Normal * dist;
			Console.WriteLine("plane center {0}, distance {1}", center, dist);
			
			// generate the corner points
			quadCorners = new Vector[4];
			bounds.Reset();
			for (int i=0; i<4; i++)
			{
//				Console.WriteLine("quad corner at {0}", Plane.Center.ToVector() + corner);
				quadCorners[i] = center + corner;
				bounds.Resize(quadCorners[i]);
				corner = corner.Rotate(direction, new Angle(Angle.PI/2.0));
			}
			
		}
		
		
		/// <summary>
		/// Renders the plane to the viewport.
		/// </summary>
		/// <param name="viewport"> A <see cref="IViewport"/> to render to. </param>
		public override void RenderTransparent(IViewport viewport)
		{
			base.RenderTransparent(viewport);
			
			viewport.RenderManager.ReferenceColor.Setup();
			
			gl.glBegin(gl.GL_POLYGON);
			foreach (Vector corner in quadCorners)
				gl.glVertex3d(corner[0], corner[1], corner[2]);
			gl.glEnd();
			
		}

		
#endregion
		
	}
}
