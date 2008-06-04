////   Vertex.cs - MonoWorks Project
////
////    Copyright Andy Selvig 2008
////
////    This program is free software: you can redistribute it and/or modify
////    it under the terms of the GNU Lesser General Public License as published 
////    by the Free Software Foundation, either version 3 of the License, or
////    (at your option) any later version.
////
////    This program is distributed in the hope that it will be useful,
////    but WITHOUT ANY WARRANTY; without even the implied warranty of
////    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
////    GNU Lesser General Public License for more details.
////
////    You should have received a copy of the GNU Lesser General Public 
////    License along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using MonoWorks.Base;

using gl = Tao.OpenGl.Gl;
using glu = Tao.OpenGl.Glu;

namespace MonoWorks.Model
{
	
	/// <summary>
	/// The Vertex class represents a vertex in 3D space.
	/// Its position is defined by a Base.Point.
	/// </summary>
	public class Vertex : Entity
	{
		
		public Vertex() : base()
		{
		}
		
			
#region Position
			
		protected Point m_pos;
		
		/// <value>
		/// Access the position of the point.
		/// </value>
		public Point Position
		{
			get {return m_pos;}
			set {m_pos = value;}
		}
			
#endregion
			
		
		
#region Rendering
		
		/// <summary>
		/// Renders the sketch to the given viewport.
		/// </summary>
		/// <param name="viewport"> A <see cref="IViewport"/> to render to. </param>
		protected override void Render(IViewport viewport)
		{
			gl.glBegin(gl.GL_POINTS);
			gl.glVertex3d(m_pos[0].Value, m_pos[1].Value, m_pos[2].Value); 
			gl.glEnd();
		}
		
		
#endregion
		
	}
}
