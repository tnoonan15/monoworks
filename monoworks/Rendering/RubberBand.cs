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
			m_startX = 0.0;
			m_startY = 0.0;
			m_stopX = 0.0;
			m_stopY = 0.0;
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
					gl.glVertex3d(m_startX, m_startY, 0);
					gl.glVertex3d(m_startX, m_stopY, 0);
					gl.glVertex3d(m_stopX, m_stopY, 0);
					gl.glVertex3d(m_stopX, m_startY, 0);		
					gl.glVertex3d(m_startX, m_startY, 0);			
				gl.glEnd();
				
			}
		}
		
		
#endregion
		
		
#region Position
	
		protected double m_startX;
		/// <summary>
		/// The starting x position.
		/// </summary>
		public double StartX
		{
			get {return m_startX;}
			set {m_startX = value;}
		}
	
		protected double m_stopX;
		/// <summary>
		/// The end x position.
		/// </summary>
		public double StopX
		{
			get {return m_stopX;}
			set {m_stopX = value;}
		}
	
		protected double m_startY;
		/// <summary>
		/// The starting y position.
		/// </summary>
		public double StartY
		{
			get {return m_startY;}
			set {m_startY = value;}
		}
	
		protected double m_stopY;
		/// <summary>
		/// The end y position.
		/// </summary>
		public double StopY
		{
			get {return m_stopY;}
			set {m_stopY = value;}
		}
		
#endregion
		
		
	}
	
}
