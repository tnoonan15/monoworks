// 
//  Menu.cs - MonoWorks Project
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
using MonoWorks.Rendering;

namespace MonoWorks.Controls
{
	/// <summary>
	/// Menu that displays a list of menu items for the user to select.
	/// </summary>
	public class Menu : GenericStack<MenuItem>
	{
		public Menu()
		{
			Orientation = Orientation.Vertical;
			FontSize = 12;
		}

		/// <summary>
		/// Add an item to the menu.
		/// </summary>
		public override void AddChild(MenuItem item)
		{
			base.AddChild(item);
			if (CurrentItem == null)
				CurrentItem = item;
		}

		/// <summary>
		/// Remove an item to the menu.
		/// </summary>
		public override void RemoveChild(MenuItem item)
		{
			base.RemoveChild(item);
			if (CurrentItem == item)
				CurrentItem = null;
		}

		private MenuItem _current;
		/// <summary>
		/// The current menu item.
		/// </summary>
		public MenuItem CurrentItem
		{
			get { return _current; }
			set
			{
				if (!ContainsChild(value))
					throw new Exception("The menu doesn't contain the item " + value);
				_current = value;
				MakeDirty();
			}
		}

		/// <summary>
		/// The index of the current item.
		/// </summary>
		public int CurrentIndex
		{
			get
			{
				if (_current == null)
					return -1;
				return IndexOfChild(_current);
			}
			set
			{
				if (value < 0 || value >= NumChildren)
					throw new Exception("Index " + value + " is out of bounds");
			}
		}


		#region Rendering
        
		private double _fontSize;
		/// <summary>
		/// The font size (in pixels).
		/// </summary>
		[MwxProperty]
		public double FontSize
		{
			get { return _fontSize; }
			set
			{
				_fontSize = value;
				MakeDirty();
			}
		}

		protected override void Render(RenderContext context)
		{
			context.Cairo.Save();
			context.Cairo.SetFontSize(FontSize);
			base.Render(context);
			context.Cairo.Restore();
		}

		#endregion

	}
}

