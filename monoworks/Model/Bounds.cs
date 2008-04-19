//   Bounds.cs - MonoWorks Project
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

using gl=Tao.OpenGl.Gl;

using MonoWorks.Base;

namespace MonoWorks.Model
{
	
	/// <summary>
	/// The Bounds class stores the bounding box of an entity.
	/// </summary>
	public class Bounds
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public Bounds()
		{
			isSet = false;
			minima = null;
			maxima = null;
		}

		
		/// <summary>
		/// Initialization constructor.
		/// </summary>
		/// <param name="minima"> The minima. </param>
		/// <param name="maxima"> The maxima. </param>
		public Bounds(Vector minima, Vector maxima)
		{
			isSet = true;
			this.minima = minima;
			this.maxima = maxima;
		}
		
		
		/// <value>
		/// Is true if the values have been set.
		/// </value>
		protected bool isSet;
		
		
#region Minima and Maxima
		
		protected Vector minima;
		/// <value>
		/// The minima.
		/// </value>
		public Vector Minima
		{
			get {return minima;}
			set
			{
				minima = value;
			}
		}
		
		
		protected Vector maxima;
		/// <value>
		/// The maxima.
		/// </value>
		public Vector Maxima
		{
			get {return maxima;}
			set
			{
				maxima = value;
			}
		}		
		
#endregion
		
		
#region Geometry

		/// <value>
		/// The center of the box.
		/// </value>
		public Vector Center
		{
			get
			{
				return (minima + maxima) / 2;
			}
		}
		
#endregion
		
		
#region Resizing
		
		/// <summary>
		/// Resizes the bounds to fit vector.
		/// </summary>
		/// <param name="vector"> A <see cref="Vector"/> that needs to fit in the bounds. </param>
		public void Resize(Vector vector)
		{
			if (isSet) // the bounds have already been set
			{
				minima.KeepMinima(vector);
				maxima.KeepMaxima(vector);
			}
			else // the bounds haven't been set
			{
				isSet = true;
				minima = vector.Copy();
				maxima = vector.Copy();
			}		
		}
		
		
		/// <summary>
		/// Resizes the bounds to fit another bounds.
		/// </summary>
		/// <param name="other"> Another <see cref="Bounds"/> that needs to fit in this one. </param>
		public void Resize(Bounds other)
		{
			if (other.Minima != null)
			{
				if (isSet) // the bounds have already been set
				{
					minima.KeepMinima(other.Minima);
					maxima.KeepMaxima(other.Maxima);
				}
				else // the bounds haven't been set
				{
					isSet = true;
					minima = other.Minima.Copy();
					maxima = other.Maxima.Copy();
				}	
			}
		}
		
#endregion
		
		
		
#region Rendering
		
		/// <summary>
		/// Renders the bounding box to the given viewport.
		/// </summary>
		/// <param name="viewport"> A <see cref="IViewport"/> to render to. </param>
		public virtual void Render(IViewport viewport)
		{
			if (minima != null)
			{
	//			Console.WriteLine("bounding box minima: {0}, maxima: {1}", minima, maxima);
				gl.glColor3d(0.0, 0.0, 1.0);
				gl.glLineWidth( 2.0f);
				gl.glBegin(gl.GL_LINE);
				
				// minimum z box
				gl.glVertex3d(minima[0], minima[1], minima[2]);
				gl.glVertex3d(maxima[0], minima[1], minima[2]);
				
				gl.glVertex3d(maxima[0], minima[1], minima[2]);
				gl.glVertex3d(maxima[0], maxima[1], minima[2]);
				
				gl.glVertex3d(maxima[0], maxima[1], minima[2]);
				gl.glVertex3d(minima[0], maxima[1], minima[2]);
				
				gl.glVertex3d(minima[0], maxima[1], minima[2]);
				gl.glVertex3d(minima[0], minima[1], minima[2]);
				
				// maximum z box
				gl.glVertex3d(minima[0], minima[1], maxima[2]);
				gl.glVertex3d(maxima[0], minima[1], maxima[2]);
				
				gl.glVertex3d(maxima[0], minima[1], maxima[2]);
				gl.glVertex3d(maxima[0], maxima[1], maxima[2]);
				
				gl.glVertex3d(maxima[0], maxima[1], maxima[2]);
				gl.glVertex3d(minima[0], maxima[1], maxima[2]);
				
				gl.glVertex3d(minima[0], maxima[1], maxima[2]);
				gl.glVertex3d(minima[0], minima[1], maxima[2]);
				
				// connecting minimum to maximum z
				gl.glVertex3d(minima[0], minima[1], minima[2]);
				gl.glVertex3d(minima[0], minima[1], maxima[2]);
				
				gl.glVertex3d(minima[0], maxima[1], minima[2]);
				gl.glVertex3d(minima[0], maxima[1], maxima[2]);
				
				gl.glVertex3d(maxima[0], maxima[1], minima[2]);
				gl.glVertex3d(maxima[0], maxima[1], maxima[2]);
				
				gl.glVertex3d(maxima[0], minima[1], minima[2]);
				gl.glVertex3d(maxima[0], minima[1], maxima[2]);
				              
				gl.glEnd();
			}
		}
		
#endregion
		
		
	}
}
