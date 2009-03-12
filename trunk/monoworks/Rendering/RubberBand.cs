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
			Enabled = false;

			// initialize the position
			Start = new Coord();
			Stop = new Coord();
		}

		
#region Rendering

		/// <value>
		/// The rubber band willonly be shown if its enabled.
		/// </value>
		public bool Enabled {get; set;}

		/// <summary>
		/// Enables the rubber band.
		/// </summary>
		public void Enable()
		{
			Enabled = true;
		}

		/// <summary>
		/// Disables the rubber band.
		/// </summary>
		public void disable()
		{
			Enabled = false;
		}
		
		/// <summary>
		/// Renders the rubber band to the viewport.
		/// </summary>
		public virtual void Render(Viewport viewport)
		{
			if (Enabled) // only render if it's enabled
			{									
				gl.glBegin(gl.GL_LINE_STRIP);
				gl.glColor3f(0f, 0.5f, 0.8f);
				gl.glLineWidth(1.5f);
				gl.glVertex3d(Start.X, Start.Y, 0);
				gl.glVertex3d(Start.X, Stop.Y, 0);
				gl.glVertex3d(Stop.X, Stop.Y, 0);
				gl.glVertex3d(Stop.X, Start.Y, 0);		
				gl.glVertex3d(Start.X, Start.Y, 0);			
				gl.glEnd();
				
			}
		}
		
		
#endregion
		
		
#region Position

		/// <summary>
		/// Sets both Start and Stop to the given coord.
		/// </summary>
		public void Reset(Coord coord)
		{
			Start = coord;
			Stop = coord;
		}

		/// <summary>
		/// The starting position.
		/// </summary>
		public Coord Start { get; set; }

		/// <summary>
		/// The stop position.
		/// </summary>
		public Coord Stop {get; set;}

		/// <summary>
		/// The point closest to the lower left.
		/// </summary>
		public Coord Min
		{
			get { return new Coord(Math.Min(Start.X, Stop.X), Math.Min(Start.Y, Stop.Y)); }
		}

		/// <summary>
		/// The point closest to the upper right.
		/// </summary>
		public Coord Max
		{
			get { return new Coord(Math.Max(Start.X, Stop.X), Math.Max(Start.Y, Stop.Y)); }
		}
		
#endregion
		
		
	}
	
}
