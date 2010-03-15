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

namespace MonoWorks.Base
{

	/// <summary>
	/// Stores a pair of coordinates (representing a position on the screen).
	/// </summary>
	public class Coord : IStringParsable
	{
		public Coord() : this(0,0)
		{
			
		}
		
		/// <summary>
		/// Initialization constructor.
		/// </summary>
		public Coord(double x, double y)
		{
			X = x;
			Y = y;
		}
		
		/// <summary>
		/// Creates a copy of the coordinate. 
		/// </summary>
		/// <remarks>It is generally a good idea to use this for assignments 
		/// as it's usually what is meant by the operation.</remarks>
		public Coord Copy()
		{
			return new Coord(X, Y);
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
		/// Parses the coord from a string with format "x,y".
		/// </summary>
		public void Parse(string valString)
		{
			var comps = valString.Split(',');
			if (comps.Length != 2)
				throw new Exception("Value string for coord must have form x,y, unlike: " + valString);
			X = double.Parse(comps[0]);
			Y = double.Parse(comps[1]);
		}
		
		/// <summary>
		/// An always accessible coord that is all zeros. 
		/// </summary>
		/// <remarks>Use this to reduce allocations if you 
		/// need this sort of thing a lot.</remarks>
		public static readonly Coord Zeros = new Coord();
		
		/// <summary>
		/// Addition operator.
		/// </summary>
		public static Coord operator+(Coord lhs, Coord rhs)
		{
			return new Coord(lhs.X + rhs.X, lhs.Y + rhs.Y);
		}
		
		/// <summary>
		/// Addition operator with a scalar.
		/// </summary>
		public static Coord operator+(Coord lhs, double rhs)
		{
			return new Coord(lhs.X + rhs, lhs.Y + rhs);
		}
		
		/// <summary>
		/// Subtraction operator.
		/// </summary>
		public static Coord operator-(Coord lhs, Coord rhs)
		{
			return new Coord(lhs.X - rhs.X, lhs.Y - rhs.Y);
		}
		
		/// <summary>
		/// Subtraction operator with a scalar.
		/// </summary>
		public static Coord operator-(Coord lhs, double rhs)
		{
			return new Coord(lhs.X - rhs, lhs.Y - rhs);
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
		
		
		#region Rounding
		
		/// <summary>
		/// Returns a new coord with each dimension rounded. 
		/// </summary>
		public Coord Round
		{
			get {
				return new Coord(Math.Round(X), Math.Round(Y));
			}
		}
		
		/// <summary>
		/// Returns a new coord that is the round of each dimension minus 0.5. 
		/// </summary>
		/// <remarks>Useful for Cairo rendering on exact pixels.</remarks>
		public Coord HalfFloor
		{
			get {
				return new Coord(Math.Round(X) - 0.5, Math.Round(Y) - 0.5);
			}
		}
		
		/// <summary>
		/// Returns a new coord that is the round of each dimension plus 0.5. 
		/// </summary>
		/// <remarks>Useful for Cairo rendering on exact pixels.</remarks>
		public Coord HalfCeiling
		{
			get {
				return new Coord(Math.Round(X) + 0.5, Math.Round(Y) + 0.5);
			}
		}
		
		/// <summary>
		/// Returns a new coord that is the floor of each dimension minus. 
		/// </summary>
		public Coord Floor
		{
			get {
				return new Coord(Math.Floor(X), Math.Floor(Y));
			}
		}
		
		/// <summary>
		/// Returns a new coord that is the ceiling of each dimension. 
		/// </summary>
		public Coord Ceiling
		{
			get {
				return new Coord(Math.Ceiling(X), Math.Ceiling(Y));
			}
		}
		
		#endregion


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
