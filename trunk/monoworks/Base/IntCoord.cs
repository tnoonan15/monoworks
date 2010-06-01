// 
//  IntCoord.cs - MonoWorks Project
//  
//  Author:
//       Andy Selvig <ajselvig@gmail.com>
// 
//  Copyright (c) 2010 Andy Selvig
// 
//  This library is free software; you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as
//  published by the Free Software Foundation; either version 2.1 of the
//  License, or (at your option) any later version.
// 
//  This library is distributed in the hope that it will be useful, but
//  WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
//  Lesser General Public License for more details.
// 
//  You should have received a copy of the GNU Lesser General Public
//  License along with this library; if not, write to the Free Software
//  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA

using System;

namespace MonoWorks.Base
{

	/// <summary>
	/// Same as coord but uses integers instead of doubles.
	/// </summary>
	public class IntCoord : IStringParsable
	{
		public IntCoord() : this(0,0)
		{
			
		}
		
		/// <summary>
		/// Initialization constructor.
		/// </summary>
		public IntCoord(int x, int y)
		{
			X = x;
			Y = y;
		}
		
		/// <summary>
		/// Creates a copy of the coordinate. 
		/// </summary>
		/// <remarks>It is generally a good idea to use this for assignments 
		/// as it's usually what is meant by the operation.</remarks>
		public IntCoord Copy()
		{
			return new IntCoord(X, Y);
		}

		/// <summary>
		/// The X coordinate.
		/// </summary>
		public int X;

		/// <summary>
		/// The y coordinate.
		/// </summary>
		public int Y;

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
			X = int.Parse(comps[0]);
			Y = int.Parse(comps[1]);
		}
		
		/// <summary>
		/// An always accessible coord that is all zeros. 
		/// </summary>
		/// <remarks>Use this to reduce allocations if you 
		/// need this sort of thing a lot.</remarks>
		public static readonly IntCoord Zeros = new IntCoord();
		
		/// <summary>
		/// Addition operator.
		/// </summary>
		public static IntCoord operator +(IntCoord lhs, IntCoord rhs)
		{
			return new IntCoord(lhs.X + rhs.X, lhs.Y + rhs.Y);
		}
		
		/// <summary>
		/// Addition operator with a scalar.
		/// </summary>
		public static IntCoord operator +(IntCoord lhs, int rhs)
		{
			return new IntCoord(lhs.X + rhs, lhs.Y + rhs);
		}
		
		/// <summary>
		/// Subtraction operator.
		/// </summary>
		public static IntCoord operator -(IntCoord lhs, IntCoord rhs)
		{
			return new IntCoord(lhs.X - rhs.X, lhs.Y - rhs.Y);
		}
		
		/// <summary>
		/// Subtraction operator with a scalar.
		/// </summary>
		public static IntCoord operator -(IntCoord lhs, int rhs)
		{
			return new IntCoord(lhs.X - rhs, lhs.Y - rhs);
		}
		
		/// <summary>
		/// Multiplication operator.
		/// </summary>
		public static IntCoord operator *(IntCoord lhs, int rhs)
		{
			return new IntCoord(lhs.X * rhs, lhs.Y * rhs);
		}
		
		/// <summary>
		/// Division operator.
		/// </summary>
		public static IntCoord operator/(IntCoord lhs, int rhs)
		{
			return new IntCoord(lhs.X / rhs, lhs.Y / rhs);
		}
		
		
		/// <summary>
		/// Keeps the minimum of each dimension.
		/// </summary>
		public void Min(IntCoord other)
		{
			if (other.X < X)
				X = other.X;
			if (other.Y < Y)
				Y = other.Y;
		}

		/// <summary>
		/// Keeps the maximum of each dimension.
		/// </summary>
		public void Max(IntCoord other)
		{
			if (other.X > X)
				X = other.X;
			if (other.Y > Y)
				Y = other.Y;
		}
		
	}
}
