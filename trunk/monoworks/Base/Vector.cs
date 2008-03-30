////   Vector.cs - MonoWorks Project
////
////    Copyright Andy Selvig 2008
////
////    This program is free software: you can redistribute it and/or modify
////    it under the terms of the GNU Lesser General Public License as published 
////    by the Free Software Foundation, either version 3 of the License, or
////    (at your option) any later version.
////
////    This program is distributed in the hope that it will be useful,
////    but WITHOUT ANaxis[1] WARRANTaxis[1]; without even the implied warranty of
////    MERCHANTABILITaxis[1] or FITNESS FOR A PARTICULAR PURPOSE.  See the
////    GNU Lesser General Public License for more details.
////
////    axis[1]ou should have received a copy of the GNU Lesser General Public 
////    License along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;


namespace MonoWorks.Base
{
	
	
	/// <summary>
	/// The Vector class represents a vector in 3D space. 
	/// </summary>
	public class Vector
	{
		/// <summary>
		/// Default initializer.
		/// All coordinates are set to zero.
		/// </summary>
		public Vector()
		{
			val = new double[]{0.0, 0.0, 0.0};
		}
		
		
		/// <summary>
		/// Initialization constructor.
		/// </summary>
		/// <param name="x"> A <see cref="System.Double"/> representing x. </param>
		/// <param name="y"> A <see cref="System.Double"/> representing y. </param>
		/// <param name="z"> A <see cref="System.Double"/> representing z. </param>
		public Vector(double x, double y, double z)
		{
			val = new double[]{x, y, z};
		}
		
		
#region Coordinates
		
		/// <summary>
		/// Actual value of the coordinates.
		/// </summary>
		protected double [] val;
		
		/// <summary>
		/// Access the coordinate (x,y,z) values by index.
		/// </summary>
		public double this[int index]
		{
			get
			{
				// ensure the index has appropriate range
				if (index<0 || index>2)
					throw new Exception("index is out of bounds!");	
				return val[index];
			}
			set
			{
				// ensure the index has appropriate range
				if (index<0 || index>2)
					throw new Exception("index is out of bounds!");	
				val[index] = value;
			}
		}
		
#endregion
		

#region Vector Math
		
		/// <value>
		/// The magnitude of the vector.
		/// </value>
		public double Magnitude
		{
			get {return Math.Sqrt(Math.Pow(val[0], 2) + Math.Pow(val[1], 2) + Math.Pow(val[2], 2));}
		}
		
		
		/// <summary>
		/// Normalizes the vector so the magnitude is 1.0 and return the result.
		/// </summary>
		public Vector Normalize()
		{
			double mag = Magnitude;
			return new Vector(val[0] / mag, val[1] / mag, val[2] / mag);
		}
				
		
		/// <summary>
		/// Performs a cross product on two vectors.
		/// </summary>
		/// <param name="rhs"> The second <see cref="Vector"/>. </param>
		/// <returns> The resulting <see cref="Vector"/>. </returns>
		public Vector Cross(Vector rhs)
		{
			Vector res = new Vector();
			res[0] = val[1]*rhs[2] - val[2]*rhs[1];
			res[1] = val[2]*rhs[0] - val[0]*rhs[2];
			res[2] = val[0]*rhs[1] - val[1]*rhs[0];
			return res;
		}
		
		/// <summary>
		/// Performs a dot product on two vectors.
		/// </summary>
		/// <param name="rhs"> The second <see cref="Vector"/>. </param>
		/// <returns> The resulting value. </returns>
		public double Dot(Vector rhs)
		{
			return val[0]*rhs[0] + val[1]*rhs[1] + val[2]*rhs[2];
		}
		
#endregion
		
		
		
#region Operator Overloading
		
		/// <summary>
		/// Adds the elements of each vector.
		/// </summary>
		/// <param name="lhs"> The left hand operand. </param>
		/// <param name="rhs"> The right hand operand. </param>
		/// <returns> The resulting <see cref="Vector"/>. </returns>
		public static Vector operator+(Vector lhs, Vector rhs)
		{
			return new Vector(lhs[0]+rhs[0], lhs[1]+rhs[1], lhs[2]+rhs[2]);
		}
		
		/// <summary>
		/// Subtracts the elements of each vector.
		/// </summary>
		/// <param name="lhs"> The left hand operand. </param>
		/// <param name="rhs"> The right hand operand. </param>
		/// <returns> The resulting <see cref="Vector"/>. </returns>
		public static Vector operator-(Vector lhs, Vector rhs)
		{
			return new Vector(lhs[0]-rhs[0], lhs[1]-rhs[1], lhs[2]-rhs[2]);
		}
		
		/// <summary>
		/// Multiplies each element in vector by factor and returns the result.
		/// </summary>
		/// <param name="vector"> The left hand operand. </param>
		/// <param name="factor"> The right hand operand. </param>
		/// <returns> The resulting <see cref="Vector"/>. </returns>
		public static Vector operator*(Vector vector, double factor)
		{
			return new Vector(vector[0]*factor, vector[1]*factor, vector[2]*factor);
		}
		
		/// <summary>
		/// Divides each element in vector by factor and returns the result.
		/// </summary>
		/// <param name="vector"> The left hand operand. </param>
		/// <param name="factor"> The right hand operand. </param>
		/// <returns> The resulting <see cref="Vector"/>. </returns>
		public static Vector operator/(Vector vector, double factor)
		{
			return new Vector(vector[0]/factor, vector[1]/factor, vector[2]/factor);
		}
		
#endregion
		
		
#region Rotation
		
		/// <summary>
		/// Rotates the vector about the given axis with the given angle.
		/// </summary>
		/// <param name="axis"> Axis of rotation. </param>
		/// <param name="angle"> Angle of rotation. </param>
		public Vector Rotate(Vector axis, Angle angle)
		{
			// matrix method
//			double s = angle.Sin();
//			double c = angle.Cos();
//			double t = 1.0 - c;
//			Matrix R = new Matrix(t*Math.Pow(axis[0],2) + c, t*axis[0]*axis[1] + s*axis[2], t*axis[0]*axis[2] - s*axis[1], 
//			                      t*axis[0]*axis[1] - s*axis[2], t*Math.Pow(axis[1],2) + c, t*axis[1]*axis[2] + s*axis[0],
//			                      t*axis[0]*axis[2] + s*axis[1], t*axis[1]*axis[2] - s*axis[0], t*Math.Pow(axis[2],2) + c);
//			return R * this;			
			
			// quaternion method
			Quaternion R = new Quaternion();
			R.Scalar = (angle/2.0).Cos();
			R.Vector = axis.Normalize() * (angle/2.0).Sin();
			Quaternion v = new Quaternion(0.0, this);
			Quaternion res = R * v * R.T();
			return res.Vector;
		}
		
#endregion
		
		
		/// <summary>
		/// Returns a string displaying the coordinate values.
		/// </summary>
		public override string ToString()
		{
			return String.Format("[{0}, {1}, {2}]", val[0], val[1], val[2]);
		}
	}
}
