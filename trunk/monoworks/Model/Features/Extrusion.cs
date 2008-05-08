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

namespace MonoWorks.Model
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
		}
		

		/// <value>
		/// Name of the type.
		/// </value>
		public override string TypeName
		{
			get {return "extrusion";}
		}
	

#region Momentos
				
		/// <summary>
		/// Appends a momento to the momento list.
		/// </summary>
		protected override void AddMomento()
		{
			base.AddMomento();
			Momento momento = momentos[momentos.Count-1];
			momento["path"] = new RefLine();
			momento["spin"] = new Angle();
			momento["scale"] = 1.0;
			momento["travel"] = new Length();
		}
		
#endregion
		

		
#region Attributes
		

		/// <value>
		/// Extrusion path.
		/// </value>
		public RefLine Path
		{
			get {return (RefLine)CurrentMomento["path"];}
			set
			{
				CurrentMomento["path"] = value;
				MakeDirty();
			}
		}		
		

		/// <value>
		/// Spin angle.
		/// </value>
		public Angle Spin
		{
			get {return (Angle)CurrentMomento["spin"];}
			set
			{
				CurrentMomento["spin"] = value;
				MakeDirty();
			}
		}
		
		
		/// <value>
		/// Scaling factor.
		/// </value>
		public double Scale
		{
			get {return (double)CurrentMomento["scale"];}
			set
			{
				CurrentMomento["scale"] = value;
				MakeDirty();
			}
		}
		
		
		/// <value>
		/// Travel distance.
		/// </value>
		public Length Travel
		{
			get {return (Length)CurrentMomento["travel"];}
			set
			{
				CurrentMomento["travel"] = value;
				MakeDirty();
			}
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
			Vector direction = Path.Direction;
			
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
			Vector direction = Path.Direction;
			
			// cycle through sketch children
			foreach (Sketchable sketchable in this.Sketch.Sketchables)
			{
				sketchable.ComputeGeometry();
				Vector[] verts = sketchable.SolidPoints;
				Vector[] directions = sketchable.Directions;
				for (int n=0; n<N; n++)
				{
					gl.glBegin(gl.GL_QUAD_STRIP);
					for (int i=0; i<verts.Length; i++)
					{
						Vector vert = verts[i];
						
						// add the normal
						Vector normal = directions[i].Cross(direction).Normalize();
						gl.glNormal3d(normal[0], normal[1], normal[2]);
						
						// add the vertex
						gl.glVertex3d(vert[0], vert[1], vert[2]);	
						gl.glVertex3d(vert[0]+direction[0]*dTravel, vert[1]+direction[1]*dTravel, vert[2]+direction[2]*dTravel); 
						bounds.Resize(vert);
						bounds.Resize(vert + direction*dTravel);
//						gl.glTranslated(direction[0]*dTravel, direction[1]*dTravel, direction[2]*dTravel);
//						gl.glVertex3d(vert[0], vert[1], vert[2]);		
//						gl.glTranslated(-direction[0]*dTravel, -direction[1]*dTravel, -direction[2]*dTravel); 
					}
					gl.glEnd();
//					gl.glTranslated(direction[0]*dTravel, direction[1]*dTravel, direction[2]*dTravel);
				}
//				gl.glTranslated(-direction[0]*Travel.Value, -direction[1]*Travel.Value, -direction[2]*Travel.Value);
			}
			
			gl.glEndList();
		}
		
#endregion
		
	}
}
