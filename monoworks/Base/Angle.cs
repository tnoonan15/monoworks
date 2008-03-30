//    Angle.cs - MonoWorks Project
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
	/// The Angle class represents length quantities. 
	/// </summary>
	public class Angle : Dimensional
	{
		/// <summary>
		/// The radian value of pi.
		/// Use Pi() to get the Angle object.
		/// </summary>
		public const double PI = 3.14159;
		
		
		/// <summary>
		/// Generates an angle whos value is pi.
		/// </summary>
		/// <returns>
		/// A new <see cref="Angle"/> with value pi.
		/// </returns>
		public static Angle Pi()
		{
			return new Angle(Angle.PI);
		}
		
		/// <summary>
		/// Default angle constructor.
		/// </summary>
		public Angle()  : base()
		{
			m_unitFactors["rad"] = 1;
//				m_unitFactors["deg"] = 180.0/PI;
			m_unitFactors["deg"] = PI/180.0;
			
		}
		
		/// <summary>
		/// Inititalizaton angle constructor.
		/// </summary>
		/// <param name="val">
		/// A <see cref="System.Double"/> representing
		/// the default angle value (in radians).
		/// </param>
		public Angle(double val) : this()
		{
			m_val = val;
		}
		
		
#region Conversions
		
		/// <value>
		/// Radian conversion convenience property.
		/// </value>
		public double Radians
		{
			get {return this["rad"];}
			set {this["rad"] = value;}
		}
		
		/// <value>
		/// Degree conversion convenience property.
		/// </value>
		public double Degrees
		{
			get {return this["deg"];}
			set {this["deg"] = value;}
		}
		
		
#endregion
		
		
		
		
#region Arithmatic
		
		/// <summary>
		/// Addition operator overloading.  
		/// </summary>
		public static Angle operator +(Angle lhs, Angle rhs)
		{
			return new Angle(lhs.m_val+rhs.m_val);
		}
		
		/// <summary>
		/// Subtraction operator overloading.  
		/// </summary>
		public static Angle operator -(Angle lhs, Angle rhs)
		{
			return new Angle(lhs.m_val-rhs.m_val);
		}
		
		/// <summary>
		/// Multiplcation operator overloading.  
		/// </summary>
		public static Angle operator *(Angle lhs, double rhs)
		{
			return new Angle(lhs.m_val*rhs);
		}
		
		/// <summary>
		/// Division operator overloading.  
		/// </summary>
		public static Angle operator /(Angle lhs, double rhs)
		{
			return new Angle(lhs.m_val/rhs);
		}
		
#endregion
		
		
#region Trigonometry
		
		/// <summary>
		/// Computes the sine of the angle.
		/// </summary>
		/// <returns>
		/// The sine of the angle.
		/// </returns>
		public double Sin()
		{
			return Math.Sin(m_val);
		}
		
		
		/// <summary>
		/// Computes the cosine of the angle.
		/// </summary>
		/// <returns>
		/// The cosine of the angle.
		/// </returns>
		public double Cos()
		{
			return Math.Cos(m_val);
		}
		
		
		/// <summary>
		/// Computes the tangent of the angle.
		/// </summary>
		/// <returns>
		/// The tangent of the angle.
		/// </returns>
		public double Tan()
		{
			return Math.Tan(m_val);
		}
		
		
		/// <summary>
		/// Generates an angle from the arcsine of a double.
		/// </summary>
		/// <returns>
		/// The new angle.
		/// </returns>
		public static Angle ArcSin(double val)
		{
			return new Angle(Math.Asin(val));
		}
		
		
		/// <summary>
		/// Generates an angle from the arccosine of a double.
		/// </summary>
		/// <returns>
		/// The new angle.
		/// </returns>
		public static Angle ArcCos(double val)
		{
			return new Angle(Math.Acos(val));
		}
		
		
		/// <summary>
		/// Generates an angle from the arctangent of a double.
		/// </summary>
		/// <returns>
		/// The new angle.
		/// </returns>
		public static Angle ArcTan(double val)
		{
			return new Angle(Math.Atan(val));
		}
		
		
		/// <summary>
		/// Generates an angle from the arctangent of an x and y value.
		/// </summary>
		/// <returns>
		/// The new angle.
		/// </returns>
		public static Angle ArcTan(double x, double y)
		{
			return new Angle(Math.Atan2(x, y));
		}
		
		
#endregion
		
		
	}

}
