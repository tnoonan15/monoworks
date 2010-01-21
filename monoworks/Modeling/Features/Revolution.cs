// Revolution.cs - MonoWorks Project
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
	/// The Revolution is a features that sweeps a sketch about an axis.
	/// </summary>
	public class Revolution : Feature
	{		
		
		/// <summary>
		/// Constructor that initializes the sketch.
		/// </summary>
		/// <param name="sketch"> A <see cref="Sketch"/> to extrude. </param>
		public Revolution(Sketch sketch) : base(sketch)
		{
			Travel = Angle.TwoPi;
		}
		
			
#region Attributes
		
		/// <value>
		/// Revolution axis.
		/// </value>
		public RefLine Axis
		{
			get {return (RefLine)this["axis"];}
			set {this["axis"] = value;}
		}	

		
		/// <value>
		/// Revolution travel.
		/// </value>
		public Angle Travel
		{
			get {return (Angle)this["travel"];}
			set {this["travel"] = value;}
		}			
				
#endregion
		
		
		
#region Rendering

		/// <summary>
		/// Computes the wireframe geometry. 
		/// </summary>
		public override void ComputeWireframeGeometry()
		{
			base.ComputeWireframeGeometry();

			if (Axis == null)
				return;

			// determine sweep and scaling factors
			int N = 64;
			Angle dTravel = Travel / (double)N;
			Vector axisCenter = Axis.Center.ToVector();

			gl.glNewList(displayLists+WireframeListOffset, gl.GL_COMPILE);
			
			// cycle through sketch children
			foreach (Sketchable sketchable in this.Sketch.Sketchables)
			{
				// add the wireframe points
				sketchable.ComputeGeometry();
				Vector[] verts = sketchable.WireframePoints;
				for (int i=0; i<verts.Length; i++)
				{
					Vector vert = verts[i];
					
					gl.glBegin(gl.GL_LINE_STRIP);
					for (int n=0; n<=N; n++)
					{
						Vector relPos = vert - Axis.Center.ToVector();
						Vector pos1 = relPos.Rotate(Axis.Direction, dTravel * n) + axisCenter;
						
						// add the vertex
						pos1.glVertex();
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
					Vector relPos = vert - Axis.Center.ToVector();
					Vector pos = relPos.Rotate(Axis.Direction, Travel ) + axisCenter;
					gl.glVertex3d(pos[0], pos[1], pos[2]);	 
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

			if (Axis == null)
				return;
			
			// determine sweep and scaling factors
			int N = 32;
			Angle dTravel = Travel / (double)(N-1);
			Vector axisCenter = Axis.Center.ToVector();
			
			gl.glNewList(displayLists+SolidListOffset, gl.GL_COMPILE);
			
			// cycle through sketch children
			foreach (Sketchable sketchable in this.Sketch.Sketchables)
			{
				sketchable.ComputeGeometry();
				Vector[] verts = sketchable.SolidPoints;
				Vector[] directions = sketchable.Directions;
				for (int i=0; i<verts.Length-1; i++) // vertices in this sketch
				{
//					List<Vector> poses = new List<Vector>();
//					List<Vector> normals = new List<Vector>();
					gl.glBegin(gl.GL_QUAD_STRIP);
					for (int n=0; n<N; n++) // divisions of the revolution
					{
						Vector relPos = verts[i] - axisCenter;
						Vector pos = relPos.Rotate(Axis.Direction, dTravel * n) + axisCenter;
						bounds.Resize(pos);
//						poses.AddChild(pos);
						
						// add the first normal
						Vector direction_ = directions[i].Rotate(Axis.Direction, dTravel * n);
						Vector travel = (pos-axisCenter).Cross(Axis.Direction);
						Vector normal = direction_.Cross(travel).Normalize();
						normal.glNormal();
//						normals.AddChild(normal);
						pos.glVertex();
						
						relPos = verts[i+1] - axisCenter;
						pos = relPos.Rotate(Axis.Direction, dTravel * n) + axisCenter;			
						bounds.Resize(pos);
//						poses.AddChild(pos);
						
						// add the second normal
//						direction_ = directions[i].Rotate(Axis.Direction, dTravel * n);
//						travel = (pos-axisCenter).Cross(Axis.Direction);
//						normal = direction_.Cross(travel).Normalize();
						normal.glNormal();
//						normals.AddChild(normal);			
						pos.glVertex();
					}
					gl.glEnd();
					
//					gl.glLineWidth(1f);
//					gl.glBegin(gl.GL_LINES);
//					ColorManager.Global["Black"].Setup();
//					for (int n=0; n<poses.Count; n++)
//					{
//						poses[n].glVertex();
//						(poses[n] + normals[n]*0.2).glVertex();
//					}
//					gl.glEnd();
//					this.CartoonColor.Setup();
				}
			}
			
			gl.glEndList();
		}
		

		
#endregion
	}
	
	
	
	
	
}
