// ToolShelf.cs - Slate Mono Application Framework
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

namespace MonoWorks.GuiGtk.Framework.Tools
{
	/// <summary>
	/// Delegate to transmit the tool shelf visibility change.
	/// </summary>
	/// <param name="shelf"> A <see cref="ToolShelf"/> that has changed its visibility.
	/// </param>
	public delegate void ToolShelfVisibilityHandler(ToolShelf shelf);
	
	/// <summary>
	/// A toolshelf belongs to a toolbox and contains large, labeled icon controls.
	/// </summary>
	public class ToolShelf : DynamicBox
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		/// <param name="toolBox"> The parent <see cref="ToolBox"/>. </param>
		/// <param name="name"> The name of the shelf. </param>
		public ToolShelf(ToolBox toolBox, string name) : base()
		{
			this.toolBox = toolBox;
			Name = name;
			
			this.Shadow = Gtk.ShadowType.None;
			
			button = new Gtk.Button(buttonLabel);
			button.Clicked += OnButtonPress;
			PackStart(button, false, true, 0);
			
			toolbar.IconSize = Gtk.IconSize.Dnd;
			PackStart(toolbar, true, true, 0);
		}
		
		/// <summary>
		/// The parent toolbox.
		/// </summary>
		protected ToolBox toolBox;
		
		protected string name;		
		/// <summary>
		/// The name of the shelf.
		/// </summary>		
		public new string Name
		{
			get {return buttonLabel.Text;}
			set {buttonLabel.Text = value;}
		}
		
		private bool shelfVisible = true;
		/// <value>
		/// Whether the tools are shown.
		/// </value>
		public bool ShelfVisible
		{
			get {return shelfVisible;}
			set
			{
				if (value != shelfVisible)
				{
					shelfVisible = value;
					if (shelfVisible)
						PackStart(toolbar, true, true, 0);
					else
						Remove(toolbar);
					UpdateToolbarSize();
					ShowAll();
				}
			}
		}
		
		/// <summary>
		/// Gets called when the visibility gets changed by the user.
		/// </summary>
		public ToolShelfVisibilityHandler VisibilityChanged;
		
		/// <summary>
		/// The button used to switch between tool shelves.
		/// </summary>
		protected Gtk.Button button;
		
		/// <summary>
		/// Callback for the button press.
		/// </summary>
		protected void OnButtonPress(object sender, EventArgs args)
		{
			ShelfVisible = true;
			VisibilityChanged(this);
		}
		
		/// <summary>
		/// The button's label.
		/// </summary>
		protected Gtk.Label buttonLabel = new Gtk.Label();
		
		/// <summary>
		/// The toolbar containing all the items.
		/// </summary>
		protected Gtk.Toolbar toolbar = new Gtk.Toolbar();
		
		/// <summary>
		/// Add an item.
		/// </summary>
		/// <param name="button"> A <see cref="Gtk.ToolButton"/>. </param>
		public void AddButton(Gtk.ToolButton button)
		{
			toolbar.Add(button);
			UpdateToolbarSize();
		}
		
		public override Gtk.Orientation Orientation
		{
			set
			{
				base.Orientation = value;
				toolbar.Orientation = value;				
				
				if (orientation == Gtk.Orientation.Horizontal)
					buttonLabel.Angle = 90;
				else
					buttonLabel.Angle = 0;
				
				UpdateToolbarSize();
			}
		}
		
		
		protected void UpdateToolbarSize()
		{			
			int width, height;
			
			if (Orientation == Gtk.Orientation.Horizontal)
			{
				width = 4;
				height = 0;
				foreach (Gtk.Widget child in toolbar.Children)
				{
					Gtk.Requisition req = child.SizeRequest();
					width += req.Width;
					height = Math.Max(height, req.Height);
				}
				height += 4;
			}
			else // vertical
			{			
				width = 0;
				height = 4;	
				foreach (Gtk.Widget child in toolbar.Children)
				{
					Gtk.Requisition req = child.SizeRequest();
					height += req.Height;
					width = Math.Max(width, req.Width);
				}
				width += 4;
			}			
			toolbar.SetSizeRequest(width, height);
		}

		
	}
}
