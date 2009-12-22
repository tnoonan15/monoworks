// Coord.cs - MonoWorks Project
//
//  Copyright (C) 2008 Andy Selvig
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
//
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA 

using System;
using System.Collections.Generic;


namespace MonoWorks.Base
{
	/// <summary>
	/// Your standard orientation enumeration.
	/// </summary>
	public enum Orientation {
		/// <summary>
		/// Horizontal.
		/// </summary>
		Horizontal, 
		/// <summary>
		/// Vertical.
		/// </summary>
		Vertical
	};

	/// <summary>
	/// Stores a pair of coordinates (representing a position on the screen).
	/// </summary>
	public class Coord
	{
		public Coord() : this(0,0)
		{
			
		}
		
		/// <summary>
		/// Initialization constructor.
		/// </summary>
		/// <param name="X"></param>
		/// <param name="Y"></param>
		public Coord(double X, double Y)
		{
			this.X = X;
			this.Y = Y;
		}

		/// <summary>
		/// The X coordinate.
		/// </summary>
		public double X;

		/// <summary>
		/// The y coordinate.
		/// </summary>
		public double Y;

		/// <summary>
		/// Prints the coordinate.
		/// </summary>
		public override string ToString()
		{
			return String.Format("[{0}, {1}]", X, Y);
		}
		
		/// <summary>
		/// Addition operator.
		/// </summary>
		public static Coord operator+(Coord lhs, Coord rhs)
		{
			return new Coord(lhs.X + rhs.X, lhs.Y + rhs.Y);
		}
		
		/// <summary>
		/// Subtraction operator.
		/// </summary>
		public static Coord operator-(Coord lhs, Coord rhs)
		{
			return new Coord(lhs.X - rhs.X, lhs.Y - rhs.Y);
		}
		
		/// <summary>
		/// Multiplication operator.
		/// </summary>
		public static Coord operator*(Coord lhs, double rhs)
		{
			return new Coord(lhs.X * rhs, lhs.Y * rhs);
		}
		
		/// <summary>
		/// Division operator.
		/// </summary>
		public static Coord operator/(Coord lhs, double rhs)
		{
			return new Coord(lhs.X / rhs, lhs.Y / rhs);
		}


		/// <summary>
		/// The square of the magnitude of the coord.
		/// </summary>
		public double MagnitudeSquared
		{
			get { return X*X + Y*Y; }
		}

		/// <summary>
		/// The magnitude of the coord.
		/// </summary>
		public double Magnitude
		{
			get { return Math.Sqrt(MagnitudeSquared); }
		}

		/// <summary>
		/// Returns a normalized copy of the coord.
		/// </summary>
		public Coord Normalize()
		{
			return this / Magnitude;
		}

		/// <summary>
		/// Returns whether the coord is more horizontal of vertical.
		/// </summary>
		public Orientation Orientation
		{
			get
			{
				if (Math.Abs(X) > Math.Abs(Y))
					return Orientation.Horizontal;
				return Orientation.Vertical;
			}
		}
		
		/// <summary>
		/// Converts the coord to a vector in the X-Y dimensions. 
		/// </summary>
		public Vector ToVector()
		{
			return new Vector(X, Y, 0);
		}


		#region Comparison Operators

		/// <summary>
		/// Return true if this coord is smaller than other in both dimensions.
		/// </summary>
		/// <remarks>Note that the Coord comparison operators are not reversible 
		/// like they are for numbers (i.e. if c1.LessThanOrEqual(2), then c2.GreaterThanOrEqual(c1) is not necessarily true).
		/// Also, the comparison operators do not form a trichotomy. So it is possible that 
		/// c1.LessThanOrEqual(c2), c1.GreaterThanOrEqual(c2), and c1.Equals(c2) all return false.
		/// That being said, they are extremely useful for UI code like hit testing.</remarks>
		public bool LessThanOrEqual(Coord other)
		{
			return X <= other.X && Y <= other.Y;
		}

		/// <summary>
		/// Return true if this coord is larger than other in both dimensions.
		/// </summary>
		/// <remarks>Note that the Coord comparison operators are not reversible 
		/// like they are for numbers (i.e. if c1.LessThanOrEqual(2), then c2.GreaterThanOrEqual(c1) is not necessarily true).
		/// Also, the comparison operators do not form a trichotomy. So it is possible that 
		/// c1.LessThanOrEqual(c2), c1.GreaterThanOrEqual(c2), and c1.Equals(c2) all return false.
		/// That being said, they are extremely useful for UI code like hit testing.</remarks>
		public bool GreaterThanOrEqual(Coord other)
		{
			return X >= other.X && Y >= other.Y;
		}

		/// <summary>
		/// Equivilant to c1.LessThanOrEqual(c2).	
		/// </summary>
		public static bool operator <=(Coord c1, Coord c2)
		{
			return c1.LessThanOrEqual(c2);
		}

		/// <summary>
		/// Equivilant to c1.GreaterThanOrEqual(c2).	
		/// </summary>
		public static bool operator >=(Coord c1, Coord c2)
		{
			return c1.GreaterThanOrEqual(c2);
		}
		
		/// <summary>
		/// Returns a new coord with the largest of each dimension from c1 and c2. 
		/// </summary>
		public static Coord Max(Coord c1, Coord c2)
		{
			return new Coord(Math.Max(c1.X, c2.X), Math.Max(c1.Y, c2.Y));
		}

		#endregion


		#region Trigonometry and Vector Math

		/// <summary>
		/// Performs the 2D dot product.
		/// </summary>
		public double Dot(Coord other)
		{
			return X * other.X + Y * other.Y;
		}

		/// <summary>
		/// Computes the angle between one vector and another.
		/// </summary>
		/// <remarks>The angle is positive if the shortest way to go from this to other is counter clockwise.</remarks>
		public Angle AngleTo(Coord other)
		{
			Angle angle = Angle.ArcTan(other.Y, other.X) - Angle.ArcTan(Y, X);
			return angle;
		}

		#endregion

	}
}
