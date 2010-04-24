// 
//  PropertyPanel.cs - MonoWorks Project
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
	/// A panel that holds all of the property controls for an Mwx object.
	/// </summary>
	public class PropertyPanel : GenericStack<PropertyControl>
	{
		public PropertyPanel()
		{
			Orientation = Orientation.Vertical;
		}
		
		public PropertyPanel(IMwxObject obj) : this()
		{
			MwxObject = obj;
		}

		private IMwxObject _mwxObject;
		/// <summary>
		/// The object whose properties are being shown.
		/// </summary>
		public IMwxObject MwxObject
		{
			get { return _mwxObject; }
			set {
				_mwxObject = value;
				Clear();
				foreach (var prop in _mwxObject.GetMwxProperties())
				{
					AddChild(PropertyControl.Create(MwxObject, prop));
				}
			}
		}
	}
}

