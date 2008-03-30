// 
// Point.cs - MonoWorks Project
//
// Copyright (C) 2008 Andy Selvig
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
//

using System;

namespace MonoWorks.Base
{
	/// <summary>
	/// The Point class represents a point real in 3D space.
	/// The point is defined by 3 Length dimensionals.
	/// </summary>
	public class Point
	{
		/// <summary>
		/// Default constructor.
		/// Sets the coordinates to the origin.
		/// </summary>
		public Point()
		{
			val = new Length[]{new Length(), new Length(), new Length()};
		}
		
		/// <summary>
		/// Initialization constructor.
		/// Uses the default units.
		/// </summary>
		/// <param name="x"> The x coordinate. </param>
		/// <param name="y"> The y coordinate. </param>
		/// <param name="z"> The z coordinate.  </param>
		public Point(double x, double y, double z)
		{
			val = new Length[]{new Length(x), new Length(y), new Length(z)};
		}			
		
		/// <summary>
		/// Initialization constructor.
		/// </summary>
		/// <param name="x"> The x coordinate. </param>
		/// <param name="y"> The y coordinate. </param>
		/// <param name="z"> The z coordinate.  </param>
		public Point(Length x, Length y, Length z)
		{
			val = new Length[]{x, y, z};
		}
			
			
#region Position

		/// <summary>
		/// The position of the point.
		/// </summary>
		protected Length[] val;
		
		/// <summary>
		/// Accesses individual dimensions.
		/// </summary>
		public Length this[int index]
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

		
		
#region Operator Overloading
		
		/// <summary>
		/// Adds the elements of each point.
		/// </summary>
		/// <param name="lhs"> The left hand operand. </param>
		/// <param name="rhs"> The right hand operand. </param>
		/// <returns> The resulting <see cref="Point"/>. </returns>
		public static Point operator+(Point lhs, Point rhs)
		{
			return new Point(lhs[0]+rhs[0], lhs[1]+rhs[1], lhs[2]+rhs[2]);
		}
		
		/// <summary>
		/// Subtracts the elements of each point.
		/// </summary>
		/// <param name="lhs"> The left hand operand. </param>
		/// <param name="rhs"> The right hand operand. </param>
		/// <returns> The resulting <see cref="Point"/>. </returns>
		public static Point operator-(Point lhs, Point rhs)
		{
			return new Point(lhs[0]-rhs[0], lhs[1]-rhs[1], lhs[2]-rhs[2]);
		}
		
		/// <summary>
		/// Multiplies each element in point by factor and returns the result.
		/// </summary>
		/// <param name="vector"> The left hand operand. </param>
		/// <param name="factor"> The right hand operand. </param>
		/// <returns> The resulting <see cref="Point"/>. </returns>
		public static Point operator*(Point vector, double factor)
		{
			return new Point(vector[0]*factor, vector[1]*factor, vector[2]*factor);
		}
		
		/// <summary>
		/// Divides each element in point by factor and returns the result.
		/// </summary>
		/// <param name="vector"> The left hand operand. </param>
		/// <param name="factor"> The right hand operand. </param>
		/// <returns> The resulting <see cref="Point"/>. </returns>
		public static Point operator/(Point vector, double factor)
		{
			return new Point(vector[0]/factor, vector[1]/factor, vector[2]/factor);
		}
		
#endregion
		
		/// <summary>
		/// Converts the point to a dimensionless vector.
		/// </summary>
		public Vector ToVector()
		{
			return new Vector(val[0].Value, val[1].Value, val[2].Value);
		}
		
	}
}
