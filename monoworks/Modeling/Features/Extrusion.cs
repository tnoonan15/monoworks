// Extrusion.cs - MonoWorks Project
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

using gl = Tao.OpenGl.Gl;

using MonoWorks.Base;
using MonoWorks.Rendering;
using MonoWorks.Modeling.Sketching;

namespace MonoWorks.Modeling
{
		
	
	/// <summary>
	/// An Extrusion is a feature that extrudes a 2D sketch into 3D along an axis.
	/// </summary>
	public class Extrusion : Feature
	{		
		
		/// <summary>
		/// Constructor that initializes the sketch.
		/// </summary>
		/// <param name="sketch"> A <see cref="Sketch"/> to extrude. </param>
		public Extrusion(Sketch sketch) : base(sketch)
		{

			Scale = 1;
			Travel = new Length(1);
		}
			
				
		#region Attributes		

		/// <value>
		/// Extrusion path.
		/// </value>
		[MwxProperty(MwxPropertyType.Reference)]
		public RefLine Path
		{
			get {return (RefLine)this["Path"];}
			set {this["Path"] = value;}
		}				

		/// <value>
		/// Spin angle.
		/// </value>
		[MwxProperty]
		public Angle Spin
		{
			get {return (Angle)this["Spin"];}
			set {this["Spin"] = value;}
		}		
		
		/// <value>
		/// Scaling factor.
		/// </value>
		[MwxProperty]
		public double Scale
		{
			get {return (double)this["Scale"];}
			set {this["Scale"] = value;}
		}		
		
		/// <value>
		/// Travel distance.
		/// </value>
		[MwxProperty]
		public Length Travel
		{
			get {return (Length)this["Travel"];}
			set {this["Travel"] = value;}
		}
				
		#endregion
				
				
		#region Rendering
		
		/// <summary>
		/// Computes the wireframe geometry.
		/// </summary>
		public override void ComputeWireframeGeometry()
		{
			base.ComputeWireframeGeometry();
			
			gl.glNewList(displayLists + WireframeListOffset, gl.GL_COMPILE);
			
			
			int N = 1;
			double dTravel = Travel.Value / (double)N;
			Vector direction = null;
			if (Path != null)
				direction = Path.Direction;
			else
				direction = Sketch.Plane.Normal;
			
			// cycle through sketch children
			foreach (Sketchable sketchable in this.Sketch.Sketchables)
			{
				// add the wireframe points
				sketchable.ComputeGeometry();
				Vector[] verts = sketchable.WireframePoints;
				foreach (Vector vert in verts)
				{
					gl.glBegin(gl.GL_LINES);
					for (int n=0; n<=N; n++)
					{
						gl.glVertex3d(vert[0]+direction[0]*dTravel*((double)n), 
						              vert[1]+direction[1]*dTravel*((double)n), 
						              vert[2]+direction[2]*dTravel*((double)n));  
					}
					gl.glEnd();
				}
				
				// add the solid points at the ends
				verts = sketchable.SolidPoints;
				gl.glBegin(gl.GL_LINE_STRIP);
				foreach (Vector vert in verts)
				{
					gl.glVertex3d(vert[0], vert[1], vert[2]);	 
				}
				gl.glEnd();
				gl.glBegin(gl.GL_LINE_STRIP);
				foreach (Vector vert in verts)
				{
					gl.glVertex3d(vert[0]+direction[0]*dTravel, vert[1]+direction[1]*dTravel, vert[2]+direction[2]*dTravel);  
				}
				gl.glEnd();
			}
			
			gl.glEndList();
		}
		
		
		/// <summary>
		/// Computes the solid geometry.
		/// </summary>
		public override void ComputeSolidGeometry()
		{
			base.ComputeSolidGeometry();
			
			gl.glNewList(displayLists + SolidListOffset, gl.GL_COMPILE);

			// determine spin and scaling factors
			int N = 1;
//			Angle dSpin;
//			double dScale;
//			bool isSpined = false;
//			bool isScaled = false;
//			if (Spin.Value != 0.0 || Scale != 1)
//			{
//				N = 24; // number of divisions
//				
//				if (Spin.Value != 0.0)
//				{
//					isSpined = true;
//					dSpin = Spin / (double)N;
//				}
//				if (Scale != 1.0)
//				{
//					isScaled= true;
//					dScale = Scale / (double)N;
//				}
//			}
			double dTravel = Travel.Value / (double)N;
			Vector direction = null;
			if (Path != null)
				direction = Path.Direction;
			else
				direction = Sketch.Plane.Normal;
			
			// cycle through sketch children
			foreach (Sketchable sketchable in this.Sketch.Sketchables)
			{
//				List<Vector> poses = new List<Vector>();
//				List<Vector> normals = new List<Vector>();
				
				sketchable.ComputeGeometry();
				Vector[] verts = sketchable.SolidPoints;
				Vector[] directions = sketchable.Directions;
				for (int n=0; n<N; n++)
				{
					gl.glBegin(gl.GL_QUAD_STRIP);
					for (int i=0; i<verts.Length; i++)
					{
						Vector vert = verts[i];
						
						// compute the normal
						Vector normal = directions[i].Cross(direction).Normalize();
						
						// add the first vertex
						bounds.Resize(vert);
//						poses.AddChild(vert);
						normal.glNormal();
//						normals.AddChild(normal);
						vert.glVertex();
						
						Vector otherVert = vert + direction * dTravel;
						bounds.Resize(otherVert);
//						poses.AddChild(otherVert);
						normal.glNormal();
//						normals.AddChild(normal);
						otherVert.glVertex();
						
					}
					gl.glEnd();
				}
									
//				gl.glLineWidth(1f);
//				gl.glBegin(gl.GL_LINES);
//				ColorManager.Global["Black"].Setup();
//				for (int n=0; n<poses.Count; n++)
//				{
//					poses[n].glVertex();
//					(poses[n] + normals[n]*0.2).glVertex();
//				}
//				gl.glEnd();
//				this.CartoonColor.Setup();
			}
			
			gl.glEndList();
		}
		
		#endregion
		
	}
}
