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
	public class MenuBox : Control2D, IStringParsable
	{

		public MenuBox()
		{
			_menu = new Menu {ParentControl = this};
			_overlay = new ModalControlOverlay {Control = _menu};
		}
		
		/// <summary>
		/// The root menu containing the items.
		/// </summary>
		private readonly Menu _menu;
		
		/// <summary>
		/// AddChild an item to the menu.
		/// </summary>
		public void Add(MenuItem item)
		{
			_menu.AddChild(item);
		}

		/// <summary>
		/// Remove an item to the menu.
		/// </summary>
		public void Remove(MenuItem item)
		{
			_menu.RemoveChild(item);
		}
		
		/// <summary>
		/// All of the items in the menu.
		/// </summary>
		public IEnumerable<MenuItem> Items
		{
			get { return _menu; }
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
				if (!_menu.ContainsChild(value))
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
				return _menu.IndexOfChild(_current);
			}
			set
			{
				if (value < 0 || value >= _menu.NumChildren)
					throw new Exception("Index " + value + " is out of bounds");
			}
		}
		
		/// <summary>
		/// The text box to show the current item.
		/// </summary>
		private readonly TextBox _textBox = new TextBox();
		
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
			
			_menu.ComputeGeometry();

			RenderSize.X = _menu.RenderSize.X;
			
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


		#region Interaction

		/// <summary>
		/// If true, the user can manually edit the current value.
		/// </summary>
		[MwxProperty]
		public bool IsCurrentEditable { get; set; }

		private readonly ModalControlOverlay _overlay;

		public override void OnButtonPress(MouseButtonEvent evt)
		{
			base.OnButtonPress(evt);

			if (!HitTest(evt.Pos))
				return;

			if (IsCurrentEditable)
			{
				_textBox.OnButtonPress(evt);
			}
			else
			{
				evt.Scene.ShowModal(_overlay);
			}
		}

		public override void OnButtonRelease(MouseButtonEvent evt)
		{
			base.OnButtonRelease(evt);
		}

		public override void OnMouseMotion(MouseEvent evt)
		{
			base.OnMouseMotion(evt);
		}

		#endregion

	}
	
	
}

