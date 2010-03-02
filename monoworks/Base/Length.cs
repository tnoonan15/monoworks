//   Length.cs - MonoWorks Project
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
using System.Collections.Generic;

namespace MonoWorks.Base
{
		
	/// <summary>
	/// The Length class represents length quantities. 
	/// </summary>
	public class Length : Dimensional
	{
		/// <summary>
		/// Default constructor (value = zero).
		/// </summary>
		public Length()  : base()
		{
			
		}
		
		/// <summary>
		/// Initialization constructor.
		/// </summary>
		/// <param name="val">
		/// The initial value (in meters).
		/// </param>
		public Length(double val) : this()
		{
			this._val = val;
		}
		
		
#region Arithmatic
		
		/// <summary>
		/// Addition operator overloading.  
		/// </summary>
		public static Length operator +(Length lhs, Length rhs)
		{
			return new Length(lhs._val+rhs._val);
		}
		
		/// <summary>
		/// Subtraction operator overloading.  
		/// </summary>
		public static Length operator -(Length lhs, Length rhs)
		{
			return new Length(lhs._val-rhs._val);
		}
		
		/// <summary>
		/// Multiplcation operator overloading.  
		/// </summary>
		public static Length operator *(Length lhs, double rhs)
		{
			return new Length(lhs._val*rhs);
		}
		
		/// <summary>
		/// Division operator overloading.  
		/// </summary>
		public static Length operator /(Length lhs, double rhs)
		{
			return new Length(lhs._val/rhs);
		}
		
#endregion
		
	}
}

