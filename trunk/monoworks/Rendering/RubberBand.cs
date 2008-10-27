//   RubberBand.cs - MonoWorks Project
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
using glu = Tao.OpenGl.Glu;

using MonoWorks.Base;

namespace MonoWorks.Rendering
{
		
	/// <summary>
	/// The RubberBand class represents a draggable rectangle in the viewport.
	/// </summary>
	public class RubberBand
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public RubberBand()
		{
			// initialize the position
			start.X = 0.0;
			start.Y = 0.0;
			stop.X = 0.0;
			stop.Y = 0.0;
		}

		
#region Rendering

		protected bool m_enabled;
		/// <value>
		/// The rubber band willonly be shown if its enabled.
		/// </value>
		public bool Enabled
		{
			get {return m_enabled;}
			set {m_enabled = value;}
		}
		
		/// <summary>
		/// Renders the rubber band to the viewport.
		/// </summary>
		public virtual void Render(IViewport viewport)
		{
			if (m_enabled) // only render if it's enabled
			{
				viewport.Camera.PlaceOverlay();
									
				gl.glBegin(gl.GL_LINE_STRIP);
					gl.glColor3f(1.0f, 0.0f, 1.0f);
					gl.glLineWidth(1.5f);
					gl.glVertex3d(start.X, start.Y, 0);
					gl.glVertex3d(start.X, stop.Y, 0);
					gl.glVertex3d(stop.X, stop.Y, 0);
					gl.glVertex3d(stop.X, start.Y, 0);		
					gl.glVertex3d(start.X, start.Y, 0);			
				gl.glEnd();
				
			}
		}
		
		
#endregion
		
		
#region Position

		protected Coord start;
		/// <summary>
		/// The starting position.
		/// </summary>
		public Coord Start
		{
			get { return start; }
			set { start = value; }
		}

		protected Coord stop;
		/// <summary>
		/// The stop position.
		/// </summary>
		public Coord Stop
		{
			get { return stop; }
			set { stop = value; }
		}

		/// <summary>
		/// The point closest to the lower left.
		/// </summary>
		public Coord Min
		{
			get { return new Coord(Math.Min(start.X, stop.X), Math.Min(start.Y, stop.Y)); }
		}

		/// <summary>
		/// The point closest to the upper right.
		/// </summary>
		public Coord Max
		{
			get { return new Coord(Math.Max(start.X, stop.X), Math.Max(start.Y, stop.Y)); }
		}
		
#endregion
		
		
	}
	
}
