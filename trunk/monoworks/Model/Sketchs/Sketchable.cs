//   Sketchable.cs - MonoWorks Project
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

namespace MonoWorks.Model
{
	
	/// <summary>
	/// The sketchable is the bae class for all entities that can belong to a sketch.
	/// </summary>
	public class Sketchable : Entity
	{
		
		public Sketchable() : base()
		{
			solidPoints = new Vector[0];
			wireframePoints = new Vector[0];
			directions = new Vector[0];
		}
		

		/// <value>
		/// Name of the type.
		/// </value>
		public override string TypeName
		{
			get {return "sketchable";}
		}

		
#region Points
		
		protected Vector[] solidPoints;
		/// <summary>
		/// The points needed to render the solid.
		/// These should be generated with ComputeGeometry().
		/// </summary>
		public Vector[] SolidPoints
		{
			get {return solidPoints;}
		}
		
		protected Vector[] wireframePoints;
		/// <summary>
		/// The points needed to render the wireframe.
		/// These should be generated with ComputeGeometry().
		/// </summary>
		public Vector[] WireframePoints
		{
			get {return wireframePoints;}
		}
		
		protected Vector[] directions;
		/// <summary>
		/// The direction vectors corresponding to each solid point.
		/// These should be generated with ComputeGeometry().
		/// </summary>
		public Vector[] Directions
		{
			get {return directions;}
		}
		
#endregion
		
#region Rendering

		public override void RenderOpaque(IViewport viewport)
		{
			base.RenderOpaque(viewport);
			Render(viewport);
		}
		
		/// <summary>
		/// Renders the sketchable to the viewport.
		/// </summary>
		/// <param name="viewport"> A <see cref="IViewport"/> to render to. </param>
		protected override void Render(IViewport viewport)
		{
			gl.glColor3f(1.0f, 1.0f, 1.0f);
			gl.glBegin(gl.GL_LINE_STRIP);
			DrawVertices();
			gl.glEnd();
			
		}

		
		/// <summary>
		/// Draws the vertices to the OpenGL context.
		/// </summary>
		public virtual void DrawVertices()
		{
			foreach (Vector vector in solidPoints)
			{
				gl.glVertex3d(vector[0], vector[1], vector[2]);
			}
		}
		
#endregion
	}
}
