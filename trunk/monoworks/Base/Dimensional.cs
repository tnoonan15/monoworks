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
	public class Dimensional : ICloneable, IStringParsable
	{
		
		/// <summary>
		/// Default Dimensional constructor.
		/// </summary>
		public Dimensional() 
		{
			_val = 0;
		}
		
		/// <summary>
		/// Constructor overload to initialize value.  
		/// </summary>
		/// <param name="val">Value in default units.</param>
		public Dimensional(double val) : this()
		{
			this._val = val;
		}
		
		/// <summary>
		/// Prints the value of the dimensional.
		/// </summary>
		public override string ToString()
		{
			return _val.ToString();
		}

		/// <summary>
		/// Clones the dimensional.
		/// </summary>
		/// <returns></returns>
		public object Clone()
		{
			Dimensional other = (Dimensional)System.Activator.CreateInstance(this.GetType());
			other._val = _val;
			return other;
		}

		/// <summary>
		/// Parses the dimensional from a string.
		/// </summary>
		public void Parse(string valString)
		{
			
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
				return DimensionManager.Global.GetUnits(this.ClassName);
			}
		}
		
		
		/// <summary>
		/// The name of the default units.  
		/// </summary>
		public string DefaultUnits
		{
			get	{return DimensionManager.Global.GetDefaultUnits(ClassName);}
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
			get {return DimensionManager.Global.GetDisplayUnits(this.ClassName);}
		}
		
#endregion
		
		
#region Accessors
		
		/// <summary>
		/// The value of the dimensional.
		/// </summary>
		protected double _val;
		
		/// <value>
		/// The raw value of the Dimensional (in default units).
		/// </value>
		public double Value
		{
			get {return _val;}
			set {_val = value;}
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
				double unitFactor;
				if (!UnitFactors.TryGetValue(units, out unitFactor))
					throw new UnitException(units);
				return _val / unitFactor;
			}
			set
			{
				double unitFactor;
				if (!UnitFactors.TryGetValue(units, out unitFactor))
					throw new UnitException(units);
				_val = value * unitFactor; 
			}
		}
				
		
		
#endregion
		

#region Convenience Conversions

		/// <summary>
		/// Converts a display value to default value.
		/// </summary>
		public static double DisplayToDefault<T>(double val) where T : Dimensional
		{
			string[] nameComps = typeof(T).ToString().Split('.');
			string name = nameComps[nameComps.Length-1];
			double unitFactor;
			string units = DimensionManager.Global.GetDefaultUnits(name);
			if (!DimensionManager.Global.GetUnits(name).TryGetValue(units, out unitFactor))
				throw new UnitException(units);
			return val * unitFactor;
		}

		/// <summary>
		/// Converts a default value to display value.
		/// </summary>
		public static double DefaultToDisplay<T>(double val) where T : Dimensional
		{
			string[] nameComps = typeof(T).ToString().Split('.');
			string name = nameComps[nameComps.Length - 1];
			double unitFactor;
			string units = DimensionManager.Global.GetDefaultUnits(name);
			if (!DimensionManager.Global.GetUnits(name).TryGetValue(units, out unitFactor))
				throw new UnitException(units);
			return val / unitFactor;
		}

#endregion

		
	}
		
}
