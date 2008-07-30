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

namespace MonoWorks.Rendering
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


		protected bool isSet;
		/// <value>
		/// Is true if the values have been set.
		/// </value>
		public bool IsSet
		{
			get { return isSet; }
			set { isSet = value; }
		}
		
		
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
				isSet = maxima != null;
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
				isSet = minima != null;
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
		
		/// <summary>
		/// Resets the bounds so that the next resize will initialize it.
		/// </summary>
		public void Reset()
		{
			isSet = false;
		}
		
#endregion
		
		
#region Geometry

		/// <value>
		/// The center of the box.
		/// </value>
		public Vector Center
		{
			get {return (minima + maxima) / 2;}
		}

		/// <summary>
		/// The size of the bounds in each dimension.
		/// </summary>
		public Vector Size
		{
			get { return maxima - minima; }
		}
		
		/// <value>
		/// The "radius" of the box.
		/// </value>
		/// <remarks>Basically the average of the width in each dimension.</remarks>
		public double Radius
		{
			get
			{
				if (isSet)
					return (maxima[0] + maxima[1] + maxima[2] - minima[0] - minima[1] - minima[2]) / 3.0;
				else
					return 0;
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



#region Outside Edges

		/// <summary>
		/// Gets the most outside edges from the viewpoint of a camera.
		/// </summary>
		/// <param name="viewport"> The viewport containing the camera.</param>
		/// <returns> The low and high end of each edge (6 elements).</returns>
		public Vector[] GetOutsideEdges(IViewport viewport)
		{
			Vector[] edges = new Vector[6];
			Vector camPos = viewport.Camera.Position;
			Vector camCenter = viewport.Camera.Center;

			double zVal = minima[2]; // z value of the x and y axes
			if (camPos[2] < camCenter[2]) // z position of the camera is negative
				zVal = maxima[2]; // draw the z axis on top

			// big, fugly if statement
			if (camPos[0] >= camCenter[0] && camPos[1] >= camCenter[1])
			{
				edges[0] = new Vector(minima[0], maxima[1], zVal); // x axis
				edges[1] = new Vector(maxima[0], maxima[1], zVal);
				edges[2] = new Vector(maxima[0], minima[1], zVal); // y axis
				edges[3] = new Vector(maxima[0], maxima[1], zVal);
				edges[4] = new Vector(maxima[0], minima[1], minima[2]); // z axis
				edges[5] = new Vector(maxima[0], minima[1], maxima[2]);
			}
			else if (camPos[0] < camCenter[0] && camPos[1] >= camCenter[1])
			{
				edges[0] = new Vector(minima[0], maxima[1], zVal); // x axis
				edges[1] = new Vector(maxima[0], maxima[1], zVal);
				edges[2] = new Vector(minima[0], minima[1], zVal); // y axis
				edges[3] = new Vector(minima[0], maxima[1], zVal);
				edges[4] = new Vector(maxima[0], maxima[1], minima[2]); // z axis
				edges[5] = new Vector(maxima[0], maxima[1], maxima[2]);
			}
			else if (camPos[0] < camCenter[0] && camPos[1] < camCenter[1])
			{
				edges[0] = new Vector(minima[0], minima[1], zVal); // x axis
				edges[1] = new Vector(maxima[0], minima[1], zVal);
				edges[2] = new Vector(minima[0], minima[1], zVal); // y axis
				edges[3] = new Vector(minima[0], maxima[1], zVal);
				edges[4] = new Vector(minima[0], maxima[1], minima[2]); // z axis
				edges[5] = new Vector(minima[0], maxima[1], maxima[2]);
			}
			else if (camPos[0] >= camCenter[0] && camPos[1] < camCenter[1])
			{
				edges[0] = new Vector(minima[0], minima[1], minima[2]); // x axis
				edges[1] = new Vector(maxima[0], minima[1], minima[2]);
				edges[2] = new Vector(maxima[0], minima[1], minima[2]); // y axis
				edges[3] = new Vector(maxima[0], maxima[1], minima[2]);
				edges[4] = new Vector(minima[0], minima[1], minima[2]); // z axis
				edges[5] = new Vector(minima[0], minima[1], maxima[2]);
			}


			return edges;
		}


#endregion



#region Rendering

		/// <summary>
		/// Renders the bounding box to the given viewport.
		/// </summary>
		/// <param name="viewport"> A <see cref="IViewport"/> to render to. </param>
		public virtual void Render(IViewport viewport)
		{
			if (isSet && minima != null)
			{
				gl.glLineWidth( 1.5f);
				gl.glColor3f(0.0f, 0.0f, 1.0f);
				gl.glBegin(gl.GL_LINES);
				
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



#region Prettifying

		/// <summary>
		/// Returns a nice step that is between 1/5 to 1/8 of the range.
		/// </summary>
		/// <param name="min"> The minimum value of the range.</param>
		/// <param name="max"> the maximum value of the range.</param>
		/// <returns> The nice step size.</returns>
		public static double NiceStep(double min, double max)
		{
			double range = max - min;

			// get the order of magnitude of the range
			// which is a good first guess for the step
			double step = Math.Pow(10, Math.Floor(Math.Log10(max - min)));

			// adjust step up if it's too low
			while (range / step > 8)
				step *= 2.0;

			// adjust step down if it's too high
			while (range / step < 5)
				step /= 2.0;

			return step;
		}

		/// <summary>
		/// Makes each dimension an even multiple of a "nice" number.
		/// </summary>
		/// <remarks> "nice" numbers are determined using multiples of NiceStep(minima, maxima).</remarks>
		public void Prettify()
		{
			for (int i = 0; i < 3; i++)
			{
				double step = NiceStep(minima[i], maxima[i]);
				minima[i] = Math.Floor(minima[i] / step) * step;
				maxima[i] = Math.Ceiling(maxima[i] / step) * step;
			}

		}


#endregion


	}
}
