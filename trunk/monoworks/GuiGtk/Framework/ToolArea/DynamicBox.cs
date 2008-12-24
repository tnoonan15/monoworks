// DynamicBox.cs - Slate Mono Application Framework
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
	
	public class BoxContent
	{
		public BoxContent(Gtk.Widget widget, bool expand, bool fill, uint padding)
		{
			this.widget = widget;
			this.expand = expand;
			this.fill = fill;
			this.padding = padding;
		}
		
		protected Gtk.Widget widget;
		/// <value>
		/// The widget.
		/// </value>
		public Gtk.Widget Widget
		{
			get {return widget;}
		}
		
		protected bool expand;
		/// <value>
		/// Whether or not to expand.
		/// </value>
		public bool Expand
		{
			get {return expand;}
		}
		
		protected bool fill;
		/// <value>
		/// Whether or not to fill the allocated area.
		/// </value>
		public bool Fill
		{
			get {return fill;}
		}
		
		protected uint padding;
		/// <value>
		/// The padding around the widget.
		/// </value>
		public uint Padding
		{
			get {return padding;}
		}
	}
	
	/// <summary>
	/// Box that can dynamically change orientation.
	/// </summary>
	public class DynamicBox : Gtk.Frame
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public DynamicBox() : base()
		{
			Orientation = Gtk.Orientation.Horizontal;
		}
		
		/// <value>
		/// The box used for layout (either a HBox or VBox).
		/// </value>
		protected Gtk.Box box = null;

		/// <value>
		/// The contents of the box.
		/// </value>
		protected List<BoxContent> contents = new List<BoxContent>();
		
		
		/// <summary>
		/// Packs a widget into the box.
		/// </summary>
		/// <param name="widget"> A <see cref="Gtk.Widget"/>. </param>
		/// <param name="expand"> True if the widget should expand outside of its size request. </param>
		/// <param name="fill"> True if the widget should fill its allocation. </param>
		/// <param name="padding"> The padding around the widget. </param>
		public void PackStart(Gtk.Widget widget, bool expand, bool fill, uint padding)
		{
			BoxContent content = new BoxContent(widget, expand, fill, padding);
			contents.Add(content);
			box.PackStart(widget, expand, fill, padding);
		}
				
		/// <summary>
		/// Removes the given widget from the box.
		/// </summary>
		/// <param name="widget"> A <see cref="Gtk.Widget"/> to remove. </param>
		public new void Remove(Gtk.Widget widget)
		{
			BoxContent contentToRemove = null;
			foreach (BoxContent content in contents)
			{
				if (content.Widget == widget)
				{
					box.Remove(widget);
					contentToRemove = content;
					break;
				}
			}
			
			if (contentToRemove != null)
				contents.Remove(contentToRemove);
		}
		
		protected Gtk.Orientation orientation;
		/// <value>
		/// The orientation.
		/// </value>
		public virtual Gtk.Orientation Orientation
		{
			get {return orientation;}
			set
			{
				orientation = value;
				
				if (box != null)
				{			
					// remove the contents
					foreach (BoxContent content in contents)
						box.Remove(content.Widget);
					
					// regenerate the box
					base.Remove(box);
				}
				
				if (orientation == Gtk.Orientation.Horizontal)
					box = new Gtk.HBox();
				else // vertical
					box = new Gtk.VBox();
				Add(box);
				
				// pack the contents
				foreach (BoxContent content in contents)
					box.PackStart(content.Widget, content.Expand, content.Fill, content.Padding);
				
				ShowAll();
				
			}
		}
		
	}
}
