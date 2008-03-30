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


namespace MonoWorks.Base
{
		
	/// <summary>
	/// The Length class represents length quantities. 
	/// </summary>
	public class Length : Dimensional
	{
		
		public Length()  : base()
		{
			m_unitFactors["m"] = 1;
			m_unitFactors["cm"] = 0.01;
			m_unitFactors["mm"] = 0.001;
			m_unitFactors["inch"] = 0.0254;
			m_unitFactors["foot"] = 0.3048;
			
		}
		
		/// <summary>
		/// Initialization constructor.
		/// </summary>
		/// <param name="val">
		/// The initial value (in meters).
		/// </param>
		public Length(double val) : this()
		{
			m_val = val;
		}
		
		
#region Arithmatic
		
		/// <summary>
		/// Addition operator overloading.  
		/// </summary>
		public static Length operator +(Length lhs, Length rhs)
		{
			return new Length(lhs.m_val+rhs.m_val);
		}
		
		/// <summary>
		/// Subtraction operator overloading.  
		/// </summary>
		public static Length operator -(Length lhs, Length rhs)
		{
			return new Length(lhs.m_val-rhs.m_val);
		}
		
		/// <summary>
		/// Multiplcation operator overloading.  
		/// </summary>
		public static Length operator *(Length lhs, double rhs)
		{
			return new Length(lhs.m_val*rhs);
		}
		
		/// <summary>
		/// Division operator overloading.  
		/// </summary>
		public static Length operator /(Length lhs, double rhs)
		{
			return new Length(lhs.m_val/rhs);
		}
		
#endregion
		
	}
}

