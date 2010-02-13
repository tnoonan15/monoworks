// ActionAttribute.cs - MonoWorks Project
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
using System.Reflection;

namespace MonoWorks.Controls
{
	/// <summary>
	/// This attributes mark methods that can be executed as actions in a Slate application.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public class ActionAttribute : Attribute
	{

		public ActionAttribute()
			: this("")
		{
		}

		public ActionAttribute(string name)
			: base()
		{
			this.name = name;
			IsTogglable = false;
		}

		protected string name = "";
		/// <summary>
		/// The name of the action.
		/// </summary>
		public string Name
		{
			get { return name; }
			set {name = value;}
		}

		protected MethodInfo methodInfo;
		/// <summary>
		/// The method information for the action.
		/// </summary>
		public MethodInfo MethodInfo
		{
			get { return methodInfo; }
			set { methodInfo = value; }
		}

		protected string iconName = null;
		/// <summary>
		/// The name of the icon.
		/// </summary>
		public string IconName
		{
			get { return iconName; }
			set { iconName = value; }
		}

		protected string tooltip = null;
		/// <summary>
		/// The tooltip associated with this action.
		/// </summary>
		public string Tooltip
		{
			get { return tooltip; }
			set { tooltip = value; }
		}
		
		protected string shortcut = null;
		/// <value>
		/// String describing shortcut key.
		/// </value>
		public string Shortcut
		{
			get {return shortcut;}
			set {shortcut = value;}
		}
		
		/// <value>
		/// Whether the action is a togglable (will result in toggle buttons).
		/// </value>
		public bool IsTogglable {get; set;}
		
		/// <value>
		/// The shortcut in Gtk accelerator format.
		/// </value>
		/// <remarks> Will be null if Shortcut is null.</remarks>
		public string GtkAccelerator
		{
			get
			{
				if (shortcut == null)
					return null;
				string[] comps = shortcut.Split('+');
				if (comps.Length > 1)
					return String.Format("<{0}>{1}", comps[0], comps[1]);
				else
					return comps[0];
			}
		}
				


	}
}
