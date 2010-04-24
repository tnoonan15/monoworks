// 
//  ReflectionExtensions.cs
//  
//  Author:
//       Andy Selvig <ajselvig@gmail.com>
// 
//  Copyright (c) 2009 Andy Selvig
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
using System.Collections.Generic;
using System.Reflection;

namespace MonoWorks.Base
{

	/// <summary>
	/// Extensions methods for reflection.
	/// </summary>
	public static class ReflectionExtensions
	{
		/// <summary>
		/// Generic form of GetCustomAttributes().
		/// </summary>
		public static T[] GetCustomAttributes<T>(this ICustomAttributeProvider provider) where T : Attribute
		{
			return GetCustomAttributes<T>(provider, true);
		}
		
		/// <summary>
		/// Generic form of GetCustomAttributes().
		/// </summary>
		public static T[] GetCustomAttributes<T>(this ICustomAttributeProvider provider, bool inherit) where T : Attribute
		{
			if (provider == null)
				throw new ArgumentNullException("provider");
			T[] attributes = provider.GetCustomAttributes(typeof(T), inherit) as T[];
			if (attributes == null)
				return new T[0];
			return attributes;
		}
		
		/// <summary>
		/// Gets the Mwx properties from an Mwx object.
		/// </summary>
		public static IEnumerable<MwxPropertyAttribute> GetMwxProperties(this IMwxObject obj)
		{
			var mwxProps = new List<MwxPropertyAttribute>();
			foreach (var prop in obj.GetType().GetProperties()) {
				var mwxProps_ = Attribute.GetCustomAttributes(prop, typeof(MwxPropertyAttribute), true);
				if (mwxProps_.Length > 0) {
					var mwxProp = mwxProps_[0] as MwxPropertyAttribute;
					mwxProp.PropertyInfo = prop;
					if (mwxProp.Name == null || mwxProp.Name == "")
						mwxProp.Name = prop.Name;
					mwxProps.Add(mwxProp);
				}
			}
			return mwxProps;
		}
		
		
		/// <summary>
		/// Sets the property on the target using a string.
		/// </summary>
		/// <param name="prop"> A <see cref="PropertyInfo"/> defining the property.</param>
		/// <param name="target"> The target of the assignment. </param>
		/// <param name="valString"> The property value expressed as a string. </param>
		/// <remarks>This method will try to parse the string into the appropriate value.</remarks>
		/// <exception cref="NotImplementedException">Gets thrown when there is no known way to 
		/// parse the string to the correct type.</exception>
		public static void SetFromString(this PropertyInfo prop, object target, string valString)
		{
			object val = null;
			if (prop.PropertyType == typeof(string))
			{
				val = valString;
			}
			else if (prop.PropertyType == typeof(double))
			{
				val = double.Parse(valString);
			}
			else if (prop.PropertyType == typeof(int))
			{
				val = int.Parse(valString);
			}
			else if (prop.PropertyType == typeof(bool))
			{
				val = bool.Parse(valString);
			}
			else if (prop.PropertyType.IsEnum)
			{
				val = Enum.Parse(prop.PropertyType, valString);
			}
			else if (prop.PropertyType.Implements(typeof(IStringParsable)))
			{
				val = Activator.CreateInstance(prop.PropertyType);
				(val as IStringParsable).Parse(valString);
			}
			else
				throw new NotImplementedException(String.Format("Don't know what to do with properties of type {0}", prop.PropertyType));
			
			prop.SetValue(target, val, null);
		}
		
		
		/// <summary>
		/// Returns true if type implements iface.
		/// </summary>
		public static bool Implements(this Type type, Type iface)
		{
			return type.GetInterface(iface.Name) != null;
		}
		
	}
}
