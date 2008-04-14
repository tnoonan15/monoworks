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

namespace MonoWorks.Model
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
		}
		

#region Momentos
				
		/// <summary>
		/// Appends a momento to the momento list.
		/// </summary>
		protected override void AddMomento()
		{
			base.AddMomento();
			Momento momento = momentos[momentos.Count-1];
			momento["axis"] = new RefLine();
			momento["travel"] = new Angle();
		}
		
#endregion
		

		
#region Attributes
		
		/// <value>
		/// Revolution axis.
		/// </value>
		public RefLine Axis
		{
			get {return (RefLine)CurrentMomento["axis"];}
			set
			{
				CurrentMomento["axis"] = value;
				MakeDirty();
			}
		}	

		
		/// <value>
		/// Revolution travel.
		/// </value>
		public Angle Travel
		{
			get {return (Angle)CurrentMomento["travel"];}
			set
			{
				CurrentMomento["travel"] = value;
				MakeDirty();
			}
		}			
				
#endregion
		
		
		
#region Rendering

		/// <summary>
		/// Computes the display lists. 
		/// </summary>
		public override void ComputeGeometry()
		{
			base.ComputeGeometry();
			
			// determine sweep and scaling factors
			int N = 64;
			Angle dTravel = Travel / (double)N;

			gl.glNewList(displayLists, gl.GL_COMPILE);
			
			// cycle through sketch children
			gl.glColor3f(0.5f, 0.5f, 0.5f);
			foreach (Sketchable sketchable in this.Sketch.Sketchables)
			{
				sketchable.ComputeGeometry();
				Vector[] verts = sketchable.RawPoints;
				for (int n=0; n<N; n++)
				{
					gl.glBegin(gl.GL_QUAD_STRIP);
					foreach (Vector vert in verts)
					{
						Vector relPos = vert - Axis.Center.ToVector();
						Vector pos1 = relPos.Rotate(Axis.Direction, dTravel * n);
						Vector pos2 = relPos.Rotate(Axis.Direction, dTravel * (n+1));
						gl.glVertex3d(pos1[0], pos1[1], pos1[2]);	 
						gl.glVertex3d(pos2[0], pos2[1], pos2[2]);	 
					}
					gl.glEnd();
				}
			}
			
			gl.glEndList();
		}
		
		/// <summary>
		/// Renders the extrusion to the given viewport.
		/// </summary>
		/// <param name="viewport"> A <see cref="Viewport"/> to render to. </param>
//		public override void Render(Viewport viewport)
//		{
//		}

		
		
#endregion
	}
}
