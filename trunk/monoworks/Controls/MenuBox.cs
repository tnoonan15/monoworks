// 
//  MenuBox.cs - MonoWorks Project
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
using System.Collections.Generic;

using MonoWorks.Base;
using MonoWorks.Rendering;
using MonoWorks.Rendering.Events;

namespace MonoWorks.Controls
{

	/// <summary>
	/// Displays a list of items with one selected at a time.
	/// </summary>
	public class MenuBox : Control2D, IMenu, IStringParsable
	{

		public MenuBox()
		{
			FontSize = 12;
			
			_itemView = new MenuItemView(this);
		}
		
		
		private MenuItemView _itemView;
		
		/// <summary>
		/// The items to display in the menu.
		/// </summary>
		private List<MenuItem> _items = new List<MenuItem>();
		
		/// <summary>
		/// Add an item to the menu.
		/// </summary>
		public void Add(MenuItem item)
		{
			_items.Add(item);
			if (CurrentItem == null)
				CurrentItem = item;
			MakeDirty();
		}

		/// <summary>
		/// Remove an item to the menu.
		/// </summary>
		public void Remove(MenuItem item)
		{
			_items.Remove(item);
			if (CurrentItem == item)
				CurrentItem = null;
			MakeDirty();
		}
		
		/// <summary>
		/// All of the items in the menu.
		/// </summary>
		public IEnumerable<MenuItem> Items
		{
			get { return _items; }
		}
		
		
		private MenuItem _current;
		/// <summary>
		/// The current menu item.
		/// </summary>
		public MenuItem CurrentItem
		{
			get {return _current;}
			set
			{
				if (!_items.Contains(value))
					throw new Exception("The menu doesn't contain the item " + value.ToString());
				_current = value;
				MakeDirty();
			}
		}
		
		/// <summary>
		/// The index of the current item.
		/// </summary>
		public int CurrentIndex
		{
			get {
				if (_current == null)
					return -1;
				return _items.IndexOf(_current);
			}
			set {
				if (value < 0 || value >= _items.Count)
					throw new Exception("Index " + value.ToString() + " is out of bounds");
			}
		}

		private double _fontSize;
		/// <summary>
		/// The font size (in pixels).
		/// </summary>
		[MwxProperty]
		public double FontSize
		{
			get { return _fontSize; }
			set {
				_fontSize = value;
				MakeDirty();
			}
		}
		
		/// <summary>
		/// The text box to show the current item.
		/// </summary>
		private TextBox _textBox = new TextBox();
		
		/// <summary>
		/// Parses the menu items from a comma-delimited string.
		/// </summary>
		public void Parse(string stringVal)
		{
			foreach (var itemVal in stringVal.Split(',')) 
			{
				var item = new MenuItem();
				item.Parse(itemVal);
				Add(item);
			}
		}
		
		#region Rendering
		
		public override void ComputeGeometry()
		{
			base.ComputeGeometry();
			
			_itemView.ComputeGeometry();
			
			RenderSize.X = _itemView.RenderSize.X;
			
			if (CurrentItem != null)
				_textBox.Body = CurrentItem.Text;
			_textBox.UserSize = RenderSize;
			_textBox.ComputeGeometry();
			
			RenderSize.Y = _textBox.RenderSize.Y;
		}

		protected override void Render(RenderContext context)
		{
			base.Render(context);
			
			_textBox.RenderCairo(context);
		}
		
		#endregion
		
		
		
	}
	
	
	/// <summary>
	/// Displays the items in a menu.
	/// </summary>
	internal class MenuItemView : Control2D
	{
		
		internal MenuItemView(IMenu menu)
		{
			_menu = menu;
		}
		
		/// <summary>
		/// The menu that this view renders items from.
		/// </summary>
		private IMenu _menu;
		
		
		#region Rendering

		/// <summary>
		/// A dummy surface used by Cairo to compute the text extents.
		/// </summary>
		private static Cairo.ImageSurface dummySurface = new Cairo.ImageSurface(Cairo.Format.ARGB32, 128, 128);
		
		public override void ComputeGeometry()
		{
			base.ComputeGeometry();
			
			double width = 0;
			double height = 0;
			using (var cr = new Cairo.Context(dummySurface))
			{
				foreach (var item in _menu.Items) 
				{
					var extents = cr.TextExtents(item.Text);
					width = Math.Max(width, extents.Width);
					height += _menu.FontSize + Padding;
				}
			}
			RenderSize.X = width + 2 * Padding;
			RenderSize.Y = height + Padding;
		}

		protected override void Render(RenderContext context)
		{
			base.Render(context);
			
		}
		
		#endregion
	}
	
}

