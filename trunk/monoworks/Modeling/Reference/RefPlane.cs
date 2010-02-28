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

namespace MonoWorks.Modeling
{
	
	/// <summary>
	/// The RefPlane entity is a reference element that represents
	/// a plane in 3D space. It is used as the base for 2D sketches.
	/// </summary>
	public class RefPlane : Reference, IPlane
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
		/// The origin of the plane.
		/// </value>
		[MwxProperty]
		public Point Position
		{
			get {return (Point)this["Position"];}
			set { this["Position"] = value;}
		}
		
		public Vector Origin
		{
			get {return Position.ToVector();}
			set {Position = new Point(value);}
		}
		
		/// <summary>
		/// The normal of the plane.
		/// </summary>
		[MwxProperty]
		public Vector Normal
		{
			get {return (Vector)this["Normal"];}
			set { this["Normal"] = value;}
		}
		
		/// <summary>
		/// The x axis of the plane.
		/// </summary>
		[MwxProperty]
		public Vector XAxis
		{
			get {return (Vector)this["XAxis"];}
			set { this["XAxis"] = value;}
		}
				
		#endregion
		
				
		#region Geometry

		/// <summary>
		/// The corners of the quadrilateral that represents the plane.
		/// </summary>
		protected Vector[] quadCorners;

		/// <summary>
		/// The local x axis.
		/// </summary>
		public Vector LocalX;

		/// <summary>
		/// The local y axis.
		/// </summary>
		public Vector LocalY;

		/// <summary>
		/// The center of the plane as it's being rendered.
		/// </summary>
		public Vector RenderCenter
		{
			get
			{
				if (quadCorners == null || quadCorners.Length != 4)
					return new Vector();
				else
					return (quadCorners[0] + quadCorners[1] + quadCorners[2] + quadCorners[3]) / 4.0;
			}
		}

		/// <summary>
		/// The up vector of the plane as it's rendered.
		/// </summary>
		public Vector RenderUpVector
		{
			get
			{
				if (quadCorners == null || quadCorners.Length != 4)
					return new Vector(0, 0, 1);
				else
					return (quadCorners[0] + quadCorners[3] - RenderCenter*2).Normalize();
			}
		}

		/// <summary>
		/// The width of the plane as it's rendered.
		/// </summary>
		public double RenderWidth
		{
			get
			{
				if (quadCorners == null || quadCorners.Length != 4)
					return 1.0;
				else
					return Math.Abs((quadCorners[0] - quadCorners[1]).Magnitude);
			}
		}
				
		#endregion		
		
		
#region Rendering

		/// <summary>
		/// Computes the plane's geometry.
		/// </summary>
		public override void ComputeGeometry()
		{
			base.ComputeGeometry();

			// get the radius of the drawing
			double radius = ParentEntity.Bounds.MaxWidth;
			// radius of the bounds
			if (radius == 0)
				// when there's nothing in the drawing
				radius = 1;
		

			// find one corner of the plane to draw
			Vector direction = Normal;
			Vector corner;
			if (direction[1] == 0 && direction[2] == 0)
				// the plane is in the Y-Z axis
				corner = new Vector(0.0, 1.0, 0.0);
			else
				// the plane is not in the Y-Z axis
				corner = new Vector(1.0, 0.0, 0.0);
			corner = direction.Cross(corner).Normalize();
			corner = corner.Rotate(direction, new Angle(Angle.PI / 4.0)) * (1.0 * radius);
			
			// find the center of the plane to draw
			Vector boundsCenter = ParentEntity.Bounds.Center;
			if (boundsCenter == null)
				boundsCenter = new Vector();
			Vector planeCenter = Origin;
			Vector planeToBounds = planeCenter - boundsCenter;
			double dist = 0;
			if (planeToBounds.Magnitude > 0)
				dist = planeToBounds.Dot(Normal) 
					 / Normal.Magnitude;
			Vector center = boundsCenter + Normal * dist;
			
			// generate the corner points
			quadCorners = new Vector[4];
			bounds.Reset();
			for (int i=0; i<4; i++)
			{
				quadCorners[i] = center + corner;
				bounds.Resize(quadCorners[i]);
				corner = corner.Rotate(direction, new Angle(Angle.PI/2.0));
			}

			// compute the x and y axes of the local coordinate system
			LocalX = ((quadCorners[0] + quadCorners[1]) * 0.5 - center).Normalize();
			LocalY = ((quadCorners[1] + quadCorners[2]) * 0.5 - center).Normalize();
			
		}				

		public override void RenderFill(Scene scene)
		{			
			gl.glBegin(gl.GL_POLYGON);
			foreach (Vector corner in quadCorners)
				corner.glVertex();
			gl.glEnd();
			
		}

		public override void RenderEdge(Scene scene)
		{
			gl.glBegin(gl.GL_LINE_LOOP);
			foreach (Vector corner in quadCorners)
				corner.glVertex();
			gl.glEnd();

		}
				
#endregion


#region The Grid

		/// <summary>
		/// The grid.
		/// </summary>
		private GridDef Grid = new GridDef();

		/// <summary>
		/// Renders the grid on the plane.
		/// </summary>
		public void RenderGrid(Scene scene)
		{
			// project the edges of the view frustum to the plane
			HitLine[] hits = scene.Camera.FrustumEdges;
			double xMin = 0, xMax = 0, yMin = 0, yMax = 0;
			for (int i = 0; i < hits.Length; i++)
			{
				Vector vec = hits[i].GetIntersection(this);
				Coord coord = WorldToLocal(vec);
				if (i == 0)
				{
					xMin = xMax = coord.X;
					yMin = yMax = coord.Y;
				}
				else // not the first one
				{
					xMin = Math.Min(xMin, coord.X);
					xMax = Math.Max(xMax, coord.X);
					yMin = Math.Min(yMin, coord.Y);
					yMax = Math.Max(yMax, coord.Y);
				}
			}

			// compute the grid step
			double displayStep = Bounds.NiceStep(Dimensional.DefaultToDisplay<Length>(xMin), 
				Dimensional.DefaultToDisplay<Length>(xMax), 40);
			Grid.Step = Dimensional.DisplayToDefault<Length>(displayStep);

			// round the limits to the outside step
			xMin = Math.Floor(xMin / Grid.Step) * Grid.Step;
			xMax = Math.Ceiling(xMax / Grid.Step) * Grid.Step;
			yMin = Math.Floor(yMin / Grid.Step) * Grid.Step;
			yMax = Math.Ceiling(yMax / Grid.Step) * Grid.Step;

			// draw the grid
			scene.RenderManager.Lighting.Disable();
			gl.glColor4f(0.5f, 0.5f, 0.5f, 0.5f);
			gl.glLineWidth(1f);
			gl.glBegin(gl.GL_LINES);
			for (double x = xMin; x < xMax; x += Grid.Step)
			{
				LocalToWorld(new Coord(x, yMin)).glVertex();
				LocalToWorld(new Coord(x, yMax)).glVertex();
			}
			for (double y = yMin; y < yMax; y += Grid.Step)
			{
				LocalToWorld(new Coord(xMin, y)).glVertex();
				LocalToWorld(new Coord(xMax, y)).glVertex();
			}
			gl.glEnd();
		}

		/// <summary>
		/// Snaps a vector to the grid.
		/// </summary>
		public Vector SnapToGrid(Vector vec)
		{
			Coord local = WorldToLocal(vec);
			Coord snapped = new Coord(Math.Round(local.X / Grid.Step) * Grid.Step, 
				Math.Round(local.Y / Grid.Step) * Grid.Step);
			return LocalToWorld(snapped);
		}

		/// <summary>
		/// Transforms a point on the plane into the local coordinate system.
		/// </summary>
		public Coord WorldToLocal(Vector point)
		{
			Coord coord = new Coord();
			Vector renderCenter = RenderCenter;
			Vector dCenter = point - renderCenter;
			coord.X = dCenter.Dot(LocalX);
			coord.Y = dCenter.Dot(LocalY);
			return coord;
		}

		/// <summary>
		/// Transforms a point from the local coordinate system to the world system.
		/// </summary>
		public Vector LocalToWorld(Coord coord)
		{
			return Origin + LocalX * coord.X + LocalY * coord.Y;
		}


		/// <summary>
		/// Finds the intersection of the hit line and the plane.
		/// </summary>
		/// <remarks>The vector will be snapped if ModelingOptions.Global.SnapToGrid.</remarks>
		public Vector GetIntersection(HitLine hit)
		{
			Vector vec = hit.GetIntersection(this);
			if (ModelingOptions.Global.SnapToGrid)
				vec = SnapToGrid(vec);
			return vec;
		}

#endregion


	}


	/// <summary>
	/// Definition of the grid on a plane.
	/// </summary>
	internal class GridDef
	{

		/// <summary>
		/// The step size of the grid.
		/// </summary>
		internal double Step;

		///// <summary>
		///// The x grid intervals.
		///// </summary>
		//internal double[] X;

		///// <summary>
		///// The y grid intervals.
		///// </summary>
		//internal double[] Y;

	}


}
