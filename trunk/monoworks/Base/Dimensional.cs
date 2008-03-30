//    Dimensional.cs - MonoWorks Project
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
	using UnitsHash = Dictionary<String, double>;
	
	/// <summary>
	/// The UnitException is thrown whenever an invalid unit is used in a Dimensional."
	/// </summary>
	public class UnitException : Exception
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		/// <param name="unit">
		/// The unit string that was used.
		/// </param>
		public UnitException(string unit) : base(unit + " is not a vlid unit for this Dimensional!")
		{
		}
	}
	
	
	/// <summary>
	/// The Dimensional class represents dimensional quantities. 
	/// </summary>
	public class Dimensional
	{
		
		/// <summary>
		/// A hash containing the multiplication factors for each unit.
		/// </summary>
		protected UnitsHash m_unitFactors;
		
		/// <summary>
		/// Default Dimensional constructor.
		/// </summary>
		public Dimensional() 
		{
			m_val = 0;
			m_unitFactors = new UnitsHash();
		}
		
		/// <summary>
		/// Constructor overload to initialize value.  
		/// </summary>
		/// <param name="val">Value in default units.</param>
		public Dimensional(double val) : this()
		{
			m_val = val;
		}
		
		
#region Units
		
		/// <summary>
		/// Returns the default unit string.  
		/// </summary>
		public string DefaultUnits()
		{
			return Units[0];
		}
		
		/// <summary>
		/// Returns a list of unit strings.  
		/// </summary>
		public string[] Units
		{
			get
			{
				string[] units = new string[m_unitFactors.Count];
				m_unitFactors.Keys.CopyTo(units, 0);
				return units;
			}
		}
		
		
#endregion
		
		
#region Accessors
		
		/// <summary>
		/// The value of the dimensional.
		/// </summary>
		protected double m_val;
		
		/// <value>
		/// The raw value of the Dimensional (in default units).
		/// </value>
		public double Value
		{
			get {return m_val;}
			set {m_val = value;}
		}
			
		/// <summary>
		/// Gets/sets the value in the given units.
		/// Throws a UnitException if units is invalid.
		/// </summary>
		public double this [string units]
		{
			get 
			{
				if (!m_unitFactors.ContainsKey(units))
					throw new UnitException(units);
				return m_val / m_unitFactors[units];
			}
			set 
			{
				if (!m_unitFactors.ContainsKey(units))
					throw new UnitException(units);
				m_val = value * m_unitFactors[units]; 
			}
		}
				
		
		
#endregion
		
		
		
	}
		
}
