// 
//  MwxPropertyAttribute.cs
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
using System.Reflection;

namespace MonoWorks.Base
{

	/// <summary>
	/// Place this attribute on properties or Renderables that can be stored in mwx files.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public class MwxPropertyAttribute : Attribute
	{

		public MwxPropertyAttribute()
		{
		}
		
		/// <summary>
		/// Constructor allowing the user to specify a name different than the property.
		/// </summary>
		public MwxPropertyAttribute(string name) : this()
		{
			Name = name;
		}
		
		/// <summary>
		/// The name of the property.
		/// </summary>
		public string Name { get; set; }
		
		/// <summary>
		/// Info for the property associated with this attribute.
		/// </summary>
		public PropertyInfo PropertyInfo { get; set; }
		
		/// <summary>
		/// Instantiates the attribute as an object.
		/// </summary>
		public object Instantiate()
		{
			return Activator.CreateInstance(PropertyInfo.PropertyType);
		}
		
	}
}
