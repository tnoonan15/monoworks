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
		

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		
		/// <summary>
		/// Compares two bounds to see if they are the same.
		/// </summary>
		/// <param name="other"> Something to compare to.</param>
		/// <returns> True if they're the same (have the same minima and maxima. </returns>
		public override bool Equals(object other)
		{
			if (!(other is Bounds))
				throw new Exception("Only compare Bounds to other Bounds.");
			return minima==(other as Bounds).minima && maxima==(other as Bounds).maxima; 
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
		/// <remarks> This is generated on demand so you can do whatever you want with the instance.</remarks>
		public Vector Center
		{
			get
			{
				if (isSet)
					return (minima + maxima) / 2;
				else
					return null;
			}
		}

		/// <summary>
		/// The size of the bounds in each dimension.
		/// </summary>
		/// <remarks> This is generated on demand so you can do whatever you want with the instance.</remarks>
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
		
		/// <value>
		/// The maximum width in any dimension.
		/// </value>
		public double MaxWidth
		{
			get
			{
				if (isSet)
				{
					double max = 0;
					for (int i=0; i<3; i++)
						max = Math.Max(max, maxima[i]-minima[i]);
					return max;
				}
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
		/// Resizes the bounds to fit x, y, and z.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="z"></param>
		public void Resize(double x, double y, double z)
		{
			if (isSet) // the bounds have already been set
			{
				minima.KeepMinima(x, y, z);
				maxima.KeepMaxima(x, y, z);
			}
			else // the bounds haven't been set
			{
				isSet = true;
				minima = new Vector(x, y, z);
				maxima = new Vector(x, y, z);
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
		
		/// <summary>
		/// Expands the bounds in all dimensions by the given factor around the center.
		/// </summary>
		/// <param name="factor"> </param>
		/// <remarks> factors less than one make it contract.</remarks>
		public void Expand(double factor)
		{
			Vector center = Center;
			minima = center + (minima - center) * factor;
			maxima = center + (maxima - center) * factor;
		}
		
#endregion

		
#region Translation
		
		/// <summary>
		/// Translates the bounds by the given difference vector.
		/// </summary>
		/// <param name="diff"> A <see cref="Vector"/> to translate by. </param>
		public void Translate(Vector diff)
		{
			minima += diff;
			maxima += diff;
		}
		
#endregion


#region Outside Edges

		/// <summary>
		/// Gets the most outside edges from the viewpoint of a camera.
		/// </summary>
		/// <param name="viewport"> The viewport containing the camera.</param>
		/// <returns> The low and high end of each edge (6 elements).</returns>
		public Vector[] GetOutsideEdges(Viewport viewport)
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

		
#region Outside Corners
		
		/// <value>
		/// An array of vectors representing the 8 corners.
		/// </value>
		public Vector[] Corners
		{
			get
			{
				Vector[] corners = new Vector[8];
				corners[0] = new Vector(minima[0], minima[1], minima[2]);
				corners[1] = new Vector(maxima[0], minima[1], minima[2]);
				corners[2] = new Vector(minima[0], maxima[1], minima[2]);
				corners[3] = new Vector(minima[0], minima[1], maxima[2]);
				corners[4] = new Vector(maxima[0], maxima[1], maxima[2]);
				corners[5] = new Vector(minima[0], maxima[1], maxima[2]);
				corners[6] = new Vector(maxima[0], minima[1], maxima[2]);
				corners[7] = new Vector(maxima[0], maxima[1], minima[2]);
				return corners;
			}
		}
		
		/// <summary>
		/// Finds the furthest corner from the given point.
		/// </summary>
		/// <param name="point"> A <see cref="Vector"/> to compute the distance from. </param>
		/// <returns> A <see cref="Vector"/> representing the furthest corner. </returns>
		public Vector FurthestCorner(Vector point)
		{
			double dist = 0;
			Vector furthest = null;
			foreach (Vector corner in Corners)
			{
				double dist_ = (point - corner).Magnitude;
				if (dist_ > dist)
				{
					dist = dist_;
					furthest = corner;
				}
			}
			return furthest;
		}
		
#endregion


#region Rendering

		/// <summary>
		/// Renders the bounding box to the given viewport.
		/// </summary>
		/// <param name="viewport"> A <see cref="IViewport"/> to render to. </param>
		public virtual void Render(Viewport viewport)
		{
			if (isSet && minima != null)
			{
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
		
		/// <summary>
		/// Returns true if the hit is inside a specific axis.
		/// </summary>
		/// <param name="Hit"> </param>
		/// <param name="minima"> </param>
		/// <param name="maxima"> </param>
		/// <param name="Axis"> </param>
		/// <returns> True if there's a hit. </returns>
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
		/// <param name="hitLine"> A <see cref="HitLine"/> defining the hit. </param>
		/// <returns> True if the entity was hit. </returns>
		public virtual bool HitTest(HitLine hitLine)
		{
			if (!isSet)
				return false;

			Vector Hit;
			
			// check if the line lies entirely outside the box
			if (hitLine.Back[0] < minima[0] && hitLine.Front[0] < minima[0])
				return false;
			if (hitLine.Back[0] > maxima[0] && hitLine.Front[0] > maxima[0])
				return false;
			if (hitLine.Back[1] < minima[1] && hitLine.Front[1] < minima[1])
				return false;
			if (hitLine.Back[1] > maxima[1] && hitLine.Front[1] > maxima[1])
				return false;
			if (hitLine.Back[2] < minima[2] && hitLine.Front[2] < minima[2])
				return false;
			if (hitLine.Back[2] > maxima[2] && hitLine.Front[2] > maxima[2])
				return false;
			
			// check if the line lies entirely inside the box
			if (hitLine.Front[0] > minima[0] && hitLine.Front[0] < maxima[0] &&
			    hitLine.Front[1] > minima[1] && hitLine.Front[1] < maxima[1] &&
			    hitLine.Front[2] > minima[2] && hitLine.Front[2] < maxima[2]) 
			{
				Hit = hitLine.Front; 
			    return true;
			}
			
			// check if the line intersects any of the individual sides
			if ( (GetIntersection( hitLine.Front[0]-minima[0], hitLine.Back[0]-minima[0], hitLine.Front, hitLine.Back, out Hit) && InBox( Hit, minima, maxima, 1 ))
			  || (GetIntersection( hitLine.Front[1]-minima[1], hitLine.Back[1]-minima[1], hitLine.Front, hitLine.Back, out Hit) && InBox( Hit, minima, maxima, 2 )) 
			  || (GetIntersection( hitLine.Front[2]-minima[2], hitLine.Back[2]-minima[2], hitLine.Front, hitLine.Back, out Hit) && InBox( Hit, minima, maxima, 3 )) 
			  || (GetIntersection( hitLine.Front[0]-maxima[0], hitLine.Back[0]-maxima[0], hitLine.Front, hitLine.Back, out Hit) && InBox( Hit, minima, maxima, 1 )) 
			  || (GetIntersection( hitLine.Front[1]-maxima[1], hitLine.Back[1]-maxima[1], hitLine.Front, hitLine.Back, out Hit) && InBox( Hit, minima, maxima, 2 )) 
			  || (GetIntersection( hitLine.Front[2]-maxima[2], hitLine.Back[2]-maxima[2], hitLine.Front, hitLine.Back, out Hit) && InBox( Hit, minima, maxima, 3 )))
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
		/// <remarks> The default desired number of steps is 7.</remarks>
		public static double NiceStep(double min, double max)
		{
			return NiceStep(min, max, 7);
		}

		/// <summary>
		/// Returns a nice step that is between 1/5 to 1/8 of the range.
		/// </summary>
		/// <param name="min"> The minimum value of the range.</param>
		/// <param name="max"> the maximum value of the range.</param>
		/// <param name="desiredNumSteps"> The desired number of steps between min and max (will get within +/- 1).</param>
		/// <returns> The nice step size.</returns>
		public static double NiceStep(double min, double max, double desiredNumSteps)
		{
			double range = max - min;

			// get the order of magnitude of the range
			// which is a good first guess for the step
			double step = Math.Pow(10, Math.Floor(Math.Log10(max - min)));

			// adjust step up if it's too low
			while (range / step > desiredNumSteps+1)
				step *= 2.0;

			// adjust step down if it's too high
			while (range / step < desiredNumSteps-1)
				step /= 2.0;

			return step;
		}
		
		
		/// <summary>
		/// Generates a range of points that include min and max with
		/// a step defined by NiceStep(min,max).
		/// </summary>
		/// <param name="min"> </param>
		/// <param name="max"> </param>
		/// <returns> </returns>
		public static double[] NiceRange(double min, double max)
		{
			return NiceRange(min, max, 7, false);
		}
		
		/// <summary>
		/// Generates a range of points that include min and max with
		/// a step defined by NiceStep(min,max).
		/// </summary>
		/// <param name="min"> </param>
		/// <param name="max"> </param>
		/// <param name="desiredNumSteps"> The desired number of steps in the range (will get within +/- 1).</param>
		/// <returns> </returns>
		public static double[] NiceRange(double min, double max, double desiredNumSteps)
		{
			return NiceRange(min, max, desiredNumSteps, false);
		}		
		
		/// <summary>
		/// Generates a range of points that include min and max with
		/// a step defined by NiceStep(min,max).
		/// </summary>
		/// <param name="min"> </param>
		/// <param name="max"> </param>
		/// <param name="truncate"> Set true to ensure that the range doesn't exceed the min/max.</param>
		/// <returns> </returns>
		public static double[] NiceRange(double min, double max, bool truncate)
		{
			return NiceRange(min, max, 7, truncate);
		}		
		
		/// <summary>
		/// Generates a range of points that include min and max with
		/// a step defined by NiceStep(min,max).
		/// </summary>
		/// <param name="min"> </param>
		/// <param name="max"> </param>
		/// <param name="desiredNumSteps"> The desired number of steps in the range (will get within +/- 1).</param>
		/// <param name="truncate"> Set true to ensure that the range doesn't exceed the min/max.</param>
		/// <returns> </returns>
		public static double[] NiceRange(double min, double max, double desiredNumSteps, bool truncate)
		{
			if (min == max)
				return new double[] { min, max };

			// determine the step and number of points
			double step = NiceStep(min, max, desiredNumSteps);
			int numVals;
			if (truncate)
				numVals = (int)Math.Floor((max-min) / step) + 1; // number of points
			else
				numVals = (int)Math.Ceiling((max-min) / step) + 1; // number of points
			double[] range = new double[numVals];
			
			// populate the range
			if (truncate)
				range[0] = Math.Ceiling(min / step) * step;
			else
				range[0] = Math.Floor(min / step) * step;
			for (int i = 1; i < numVals; i++)
				range[i] = range[i - 1] + step;
			
			if (truncate && range[numVals-1] > max) // there were too many points generated
			{
				double[] trimmedRange = new double[numVals-1];
				Array.Copy(range, 0, trimmedRange, 0, numVals-1);
				return trimmedRange;
			}
			else // the correct number of points were generated
				return range;
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
				if (step > 0) // avoid divide by zero
				{
					minima[i] = Math.Floor(minima[i] / step) * step;
					maxima[i] = Math.Ceiling(maxima[i] / step) * step;
				}
			}

		}

		/// <summary>
		/// Ensures that the max-min of all ranges is greater than zero.
		/// </summary>
		/// <remarks> Anything equal to zero will be offset by 1 in each direction.</remarks>
		public void EnsureNonZeroRanges()
		{
			for (int i = 0; i < 3; i++)
			{
				if (minima[i] == maxima[i])
				{
					minima[i]--;
					maxima[i]++;
				}
			}
		}

#endregion


	}
}
