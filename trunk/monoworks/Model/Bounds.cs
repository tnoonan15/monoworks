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
		
		/// <summary>
		/// Returns a string representing the bounds.
		/// </summary>
		/// <returns> A string saying the minima and maxima. </returns>
		public override string ToString ()
		{
			return String.Format("from {0} to {1}", minima, maxima);
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
				Console.WriteLine("bounding box minima: {0}, maxima: {1}", minima, maxima);
				gl.glLineWidth( 1.5f);
				gl.glColor3f(0.0f, 0.0f, 1.0f);
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
		
		
#region Hit Test		
		
		protected bool GetIntersection( double fDst1, double fDst2, Vector P1, Vector P2, out Vector Hit)
		{
			Hit = null;
			if ( (fDst1 * fDst2) >= 0.0f)
				return false;
			if ( fDst1 == fDst2)
				return false; 
			Hit = P1 + (P2-P1) * ( -fDst1/(fDst2-fDst1) );
			return true;
		}
		
		protected bool InBox( Vector Hit, Vector minima, Vector maxima, int Axis)
		{
			if ( Axis==1 && Hit[2] > minima[2] && Hit[2] < maxima[2] && Hit[1] > minima[1] && Hit[1] < maxima[1])
				return true;
			if ( Axis==2 && Hit[2] > minima[2] && Hit[2] < maxima[2] && Hit[0] > minima[0] && Hit[0] < maxima[0])
				return true;
			if ( Axis==3 && Hit[0] > minima[0] && Hit[0] < maxima[0] && Hit[1] > minima[1] && Hit[1] < maxima[1])
				return true;
			return false;
		}
		
		/// <summary>
		/// Performs a hit test with two vectors lying on a 3D line.
		/// </summary>
		/// <param name="v1"> A <see cref="Vector"/> on the hit line. </param>
		/// <param name="v2"> A <see cref="Vector"/> on the hit line. </param>
		/// <returns> True if the entity was hit. </returns>
		public virtual bool HitTest(Vector v1, Vector v2)
		{
			Vector Hit;

			// returns true if line (v1, v2) intersects with the box (minima, maxima)
			// returns intersection point in Hit
//			int CheckLineBox( Vector minima, Vector maxima, Vector v1, Vector v2, Vector &Hit)
//			{
			
			// check if the line lies entirely outside the box
			if (v2[0] < minima[0] && v1[0] < minima[0])
				return false;
			if (v2[0] > maxima[0] && v1[0] > maxima[0])
				return false;
			if (v2[1] < minima[1] && v1[1] < minima[1])
				return false;
			if (v2[1] > maxima[1] && v1[1] > maxima[1])
				return false;
			if (v2[2] < minima[2] && v1[2] < minima[2])
				return false;
			if (v2[2] > maxima[2] && v1[2] > maxima[2])
				return false;
			
			// check if the line lies entirely inside the box
			if (v1[0] > minima[0] && v1[0] < maxima[0] &&
			    v1[1] > minima[1] && v1[1] < maxima[1] &&
			    v1[2] > minima[2] && v1[2] < maxima[2]) 
			    {Hit = v1; 
			    return true;}
			
			// check if the line intersects any of the individual sides
			if ( (GetIntersection( v1[0]-minima[0], v2[0]-minima[0], v1, v2, out Hit) && InBox( Hit, minima, maxima, 1 ))
			  || (GetIntersection( v1[1]-minima[1], v2[1]-minima[1], v1, v2, out Hit) && InBox( Hit, minima, maxima, 2 )) 
			  || (GetIntersection( v1[2]-minima[2], v2[2]-minima[2], v1, v2, out Hit) && InBox( Hit, minima, maxima, 3 )) 
			  || (GetIntersection( v1[0]-maxima[0], v2[0]-maxima[0], v1, v2, out Hit) && InBox( Hit, minima, maxima, 1 )) 
			  || (GetIntersection( v1[1]-maxima[1], v2[1]-maxima[1], v1, v2, out Hit) && InBox( Hit, minima, maxima, 2 )) 
			  || (GetIntersection( v1[2]-maxima[2], v2[2]-maxima[2], v1, v2, out Hit) && InBox( Hit, minima, maxima, 3 )))
				return true;

			return false;

		}
		
#endregion
		
	}
}
