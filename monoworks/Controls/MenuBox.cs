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
using MonoWorks.Framework;
using MonoWorks.Rendering;
using MonoWorks.Rendering.Events;
using System.IO;

namespace MonoWorks.Controls
{

	/// <summary>
	/// Displays a list of items with one selected at a time.
	/// </summary>
	public class MenuBox : Control2D, IStringParsable
	{

		public MenuBox()
		{
			_menu = new Menu();
			_overlay = new ModalControlOverlay { Control = _menu };
			_menu.ItemActivated += delegate(object sender, MenuItem item) {
				_overlay.Close();
				CurrentItem = item;
			};
			
			ButtonOrigin = new Coord();
			ButtonSize = new Coord();
			
			IsHoverable = true;
		}
		
		static MenuBox()
		{
			// read the data
			var stream = ResourceHelper.GetStream("expand.png");
			_iconSurface = CairoHelper.ImageSurfaceFromStream(stream);
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
				
		/// <value>
		/// The surface containing the expand icon.
		/// </value>
		private static Cairo.ImageSurface _iconSurface;
		
		/// <summary>
		/// The relative origin of the expand button.
		/// </summary>
		public Coord ButtonOrigin { get; private set; }

		/// <summary>
		/// The size of the expand button.
		/// </summary>
		public Coord ButtonSize { get; private set; }
		
		private double _buttonPadding;
		
		public override void ComputeGeometry()
		{
			base.ComputeGeometry();
			
			_menu.ComputeGeometry();

			
			if (CurrentItem != null)
				_textBox.Body = CurrentItem.Text;
			_textBox.UserSize.X = _menu.RenderSize.X;
			_textBox.ComputeGeometry();
			
			// place and size the button
			ButtonOrigin.X = _textBox.RenderSize.X - 1;
			ButtonSize.X = _textBox.RenderSize.Y;
			ButtonSize.Y = _textBox.RenderSize.Y;
			_buttonPadding = (ButtonSize.Y - 12) / 2;
			
			RenderSize.X = ButtonOrigin.X + ButtonSize.X;			
			RenderSize.Y = _textBox.RenderSize.Y;
		}

		protected override void Render(RenderContext context)
		{
			base.Render(context);
			
			_textBox.RenderCairo(context);
			
			// render the expand icon
			context.Cairo.Save();
			context.Cairo.SetSourceSurface(_iconSurface, 
				(int)(LastPosition.X + ButtonOrigin.X + _buttonPadding), (int)(LastPosition.Y + _buttonPadding));
			context.Cairo.Paint();
			context.Cairo.Restore();
		}
		
		#endregion


		#region Interaction

		/// <summary>
		/// If true, the user can manually edit the current value.
		/// </summary>
		[MwxProperty]
		public bool IsCurrentEditable { get; set; }

		private readonly ModalControlOverlay _overlay;
		
		private bool _justHit;

		public override void OnButtonPress(MouseButtonEvent evt)
		{
			base.OnButtonPress(evt);

			if (!HitTest(evt.Pos))
				return;

			_justHit = true;
			
			if (IsCurrentEditable)
			{
				_textBox.OnButtonPress(evt);
				// TODO: Implement MenuBox.IsCurrentEditable=true functionality
			}
			else
			{
				ShowMenu(evt.Scene);
			}
		}

		public override void OnButtonRelease(MouseButtonEvent evt)
		{
			base.OnButtonRelease(evt);
			
			if (_justHit)
			{
				_justHit = false;
				evt.Handle();
			}
		}

		public override void OnMouseMotion(MouseEvent evt)
		{
			base.OnMouseMotion(evt);
		}
		
		/// <summary>
		/// Shows the list of menu items on the given scene.
		/// </summary>
		public void ShowMenu(Scene scene)
		{
			scene.ShowModal(_overlay);
			
			// get the offset at the selected item
			double dy = 0;
			if (CurrentItem != null)
				dy = CurrentItem.Origin.Y;
			
			// get the overall scene location of the menu box
			if (Pane == null || LastPosition == null)
				return;
			if (Pane is OverlayPane)
			{
				var pane = Pane as OverlayPane;
				var pos = pane.Origin + new Coord(LastPosition.X, 
				          -LastPosition.Y + pane.RenderHeight + dy - _menu.RenderHeight);
				_overlay.Origin = pos;
				
				// TODO: improve menu placement at edges of the scene
			}
		}

		#endregion

	}
	
	
}

