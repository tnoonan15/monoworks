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
using MonoWorks.Rendering;
using MonoWorks.Rendering.Events;

namespace MonoWorks.Modeling.Sketching
{
	
	/// <summary>
	/// The sketchable is the bae class for all entities that can belong to a sketch.
	/// </summary>
	public abstract class Sketchable : Entity
	{
		
		public Sketchable(Sketch sketch) : base()
		{
			Sketch = sketch;
			sketch.AddChild(this);
			solidPoints = new Vector[0];
			wireframePoints = new Vector[0];
			directions = new Vector[0];
		}


		/// <summary>
		/// Tolerance for hit testing.
		/// </summary>
		public const double HitTol = 6;

		/// <summary>
		/// The sketch this sketchable belongs to.
		/// </summary>
		[MwxProperty(MwxPropertyType.Reference)]
		public Sketch Sketch
		{
			get { return GetAttribute("Sketch") as Sketch; }
			set { SetAttribute("Sketch", value); }
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

		public override void RenderOpaque(Scene scene)
		{
			base.RenderOpaque(scene);

			scene.RenderManager.Lighting.Disable();

			// edges
			ModelingOptions.Global.GetColor("sketchable", hitState).Setup();
			gl.glLineWidth(2);
			gl.glBegin(gl.GL_LINE_STRIP);
			DrawEdges();
			gl.glEnd();

			// vertices
			if (IsSelected)
			{
				gl.glPointSize(5);
				gl.glBegin(gl.GL_POINTS);
				DrawVertices();
				gl.glEnd();
			}
		}		
		
		/// <summary>
		/// Draws the vertices to the OpenGL context.
		/// </summary>
		public virtual void DrawVertices()
		{
			foreach (Vector point in solidPoints)
				point.glVertex();
		}

		/// <summary>
		/// Draws the edges to the OpenGL context.
		/// </summary>
		public virtual void DrawEdges()
		{
			foreach (Vector point in solidPoints)
				point.glVertex();
		}
		
#endregion
	}
}
