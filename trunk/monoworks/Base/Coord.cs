// ScreenCoord.cs - MonoWorks Project
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
	/// Stores a pair of integer coordinates (representing a position on the screen).
	/// </summary>
	public struct Coord
	{
		/// <summary>
		/// Initialization constructor.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		public Coord(double x, double y)
		{
			X = x;
			Y = y;
		}

		private double x;
		/// <summary>
		/// The x coordinate.
		/// </summary>
		public double X
		{
			get { return x; }
			set { x = value; }
		}		

		private double y;
		/// <summary>
		/// The y coordinate.
		/// </summary>
		public double Y
		{
			get { return y; }
			set { y = value; }
		}


		public override string ToString()
		{
			return String.Format("[{0}, {1}]", x, y);
		}
		
		
		public static Coord operator+(Coord lhs, Coord rhs)
		{
			return new Coord(lhs.X + rhs.X, lhs.Y + rhs.Y);
		}
		
		public static Coord operator-(Coord lhs, Coord rhs)
		{
			return new Coord(lhs.X - rhs.X, lhs.Y - rhs.Y);
		}
		
		public static Coord operator*(Coord lhs, double rhs)
		{
			return new Coord(lhs.X * rhs, lhs.Y * rhs);
		}
		
		public static Coord operator/(Coord lhs, double rhs)
		{
			return new Coord(lhs.X / rhs, lhs.Y / rhs);
		}

	}
}
