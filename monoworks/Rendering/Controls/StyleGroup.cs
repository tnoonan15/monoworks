// StyleGroup.cs - MonoWorks Project
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

namespace MonoWorks.Rendering.Controls
{
	
	/// <summary>
	/// Contains rendering style information for controls. 
	/// </summary>
	/// <remarks>It's basically a collection of StyleClasses.
	/// The caller asks for the class for their type. This object 
	/// either gives them the corresponding class or the default one.
	/// </remarks>
	public class StyleGroup
	{
		
		public StyleGroup()
		{
			DefaultClass = new StyleClass();
		}

		/// <summary>
		/// Maps control type to style class.
		/// </summary>
		protected Dictionary<Type, StyleClass> classes = new Dictionary<Type, StyleClass>();

		/// <summary>
		/// The default class for this style group.
		/// </summary>
		public StyleClass DefaultClass
		{
			get { return classes[typeof(Control)]; }
			set { classes[typeof(Control)] = value; }
		}

		/// <summary>
		/// Adds a style class for the given type.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="styleClass"></param>
		public void AddClass(Type type, StyleClass styleClass)
		{
			if (!type.IsSubclassOf(typeof(Control)))
				throw new InvalidCastException("Type must be a subclass of Control");
			classes[type] = styleClass;
		}


		/// <summary>
		/// Gets the style class associated with the given control type.
		/// </summary>
		/// <remarks>Returns the default class if there isn't one associated with this type.</remarks>
		public StyleClass GetClass(Type type)
		{
			if (classes.ContainsKey(type))
				return classes[type];
			else
				return DefaultClass;
		}

		/// <summary>
		/// Gets the style class associated with the given control.
		/// </summary>
		public StyleClass GetClass(Control control)
		{
			return GetClass(control.GetType());
		}

	}
}
