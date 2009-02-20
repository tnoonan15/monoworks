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
	public class Dimensional : ICloneable
	{
		
		/// <summary>
		/// Default Dimensional constructor.
		/// </summary>
		public Dimensional() 
		{
			val = 0;
		}
		
		/// <summary>
		/// Constructor overload to initialize value.  
		/// </summary>
		/// <param name="val">Value in default units.</param>
		public Dimensional(double val) : this()
		{
			this.val = val;
		}
		
		/// <summary>
		/// Prints the value of the dimensional.
		/// </summary>
		public override string ToString()
		{
			return val.ToString();
		}

		/// <summary>
		/// Clones the dimensional.
		/// </summary>
		/// <returns></returns>
		public object Clone()
		{
			Dimensional other = (Dimensional)System.Activator.CreateInstance(this.GetType());
			other.val = val;
			return other;
		}

		
		
#region Units
		
		/// <value>
		/// The unqualified name of the class.
		/// </value>
		public string ClassName
		{
			get
			{
				string[] nameComps = this.GetType().ToString().Split('.');
				return nameComps[nameComps.Length-1];
			}
		}
		

		/// <value>
		/// The units factors for this object.
		/// </value>
		protected UnitsHash UnitFactors
		{
			get
			{
				return DimensionManager.CurrentInstance.GetUnits(this.ClassName);
			}
		}
		
		
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
				string[] units = new string[UnitFactors.Count];
				UnitFactors.Keys.CopyTo(units, 0);
				return units;
			}
		}
		
		/// <value>
		/// The name of the display units for this object.
		/// </value>
		public string DisplayUnits
		{
			get {return DimensionManager.CurrentInstance.GetDisplayUnits(this.ClassName);}
		}
		
#endregion
		
		
#region Accessors
		
		/// <summary>
		/// The value of the dimensional.
		/// </summary>
		protected double val;
		
		/// <value>
		/// The raw value of the Dimensional (in default units).
		/// </value>
		public double Value
		{
			get {return val;}
			set {val = value;}
		}
		
		/// <value>
		/// The value in display units.
		/// </value>
		public double DisplayValue
		{
			get {return this[DisplayUnits];}
			set {this[this.DisplayUnits] = value;}
		}
			
		/// <summary>
		/// Gets/sets the value in the given units.
		/// Throws a UnitException if units is invalid.
		/// </summary>
		public double this [string units]
		{
			get 
			{
				if (!UnitFactors.ContainsKey(units))
					throw new UnitException(units);
				return val / UnitFactors[units];
			}
			set 
			{
				if (!UnitFactors.ContainsKey(units))
					throw new UnitException(units);
				val = value * UnitFactors[units]; 
			}
		}
				
		
		
#endregion


		
	}
		
}
