// 
//  PropertyControl.cs - MonoWorks Project
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

using MonoWorks.Base;
using MonoWorks.Controls;

namespace MonoWorks.Controls.Properties
{
	/// <summary>
	/// Base class for Mwx property controls.
	/// </summary>
	/// <remarks>This can be used as a factory to create new property controls with PropertyControl.Create().</remarks>
	public abstract class PropertyControl : Stack
	{
		public PropertyControl(IMwxObject obj, MwxPropertyAttribute property) : base()
		{
			MwxObject = obj;
			Property = property;
			Orientation = Orientation.Vertical;
			
			_label = new Label(property.Name);
			AddChild(_label);
		}
		
		/// <summary>
		/// The Mwx object controlled by this control.
		/// </summary>
		public IMwxObject MwxObject {
			get;
			private set;
		}
		
		/// <summary>
		/// The property managed by this control.
		/// </summary>
		public MwxPropertyAttribute Property { get; set; }
		
		
		private Label _label;
		
		/// <summary>
		/// Factory method to create a new control based on the property.
		/// </summary>
		public static PropertyControl Create(IMwxObject obj, MwxPropertyAttribute property)
		{
			var type = property.PropertyInfo.PropertyType;
			if (typeof(string).IsAssignableFrom(type))
				return new StringControl(obj, property);
			if (typeof(Enum).IsAssignableFrom(type))
			{
				var genericType = typeof(EnumControl<>).MakeGenericType(new Type[] { type });
				return Activator.CreateInstance(genericType, new object[] { obj, property }) as PropertyControl;
			}
			
			throw new Exception("Don't know how to create a property control for type " + type);
		}
	}
	
	
	/// <summary>
	/// A generic property control that contains the type of the property.
	/// </summary>
	public abstract class GenericPropertyControl<T> : PropertyControl
	{
		public GenericPropertyControl(IMwxObject obj, MwxPropertyAttribute property) : base(obj, property)
		{
			if (!typeof(T).IsAssignableFrom(property.PropertyInfo.PropertyType))
				throw new Exception(typeof(T).Name + " is not of type " + property.PropertyInfo.PropertyType);
		}
		
		public T Value
		{
			get {
				return (T)Property.PropertyInfo.GetValue(MwxObject, new object[]{});
			}
		}
	}
}

