// 
//  EnumControl.cs - MonoWorks Project
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
	/// Property control for an enum property.
	/// </summary>
	public class EnumControl : GenericPropertyControl<Enum>
	{
		public EnumControl(IMwxObject obj, MwxPropertyAttribute property) : base(obj, property)
		{
			_menuBox = new MenuBox();
			AddChild(_menuBox);
			foreach (var val in Enum.GetValues(Value.GetType()))
			{
				var item = new MenuItem() {
					Text = val.ToString()
				};
				_menuBox.Add(item);
				if (Value == val)
					_menuBox.CurrentItem = item;
			}
			
			_menuBox.Changed += delegate(MenuBox sender, MenuItemChangedEvent evt) {
				Property.PropertyInfo.SetFromString(MwxObject, _menuBox.CurrentItem.Text);
			};
		}
		
		private MenuBox _menuBox;
		
	}
}

