// DockOverlay.cs - Slate Mono Application Framework
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
using System.IO;

using MonoWorks.Framework;

namespace MonoWorks.GuiGtk.Framework.Dock
{
	
	/// <summary>
	/// Docking position overlay.
	/// </summary>
	public class DockOverlay : Gtk.Window
	{
		public const int Width = 32;
		
		public DockOverlay(Position position) : base(Gtk.WindowType.Popup)
		{
			this.position = position;
			
			SetSizeRequest(Width, Width);
			Decorated = false;
			
			// get the image
			Gdk.Pixbuf pixBuf = new Gdk.Pixbuf(ResourceHelper.GetStream(position.ToString() + "Overlay.png"));
			Gdk.Pixbuf hoveringPixbuf = new Gdk.Pixbuf(pixBuf, 0, 0, Width, Width);
			normalImage = new Gtk.Image(pixBuf);
			pixBuf.SaturateAndPixelate(hoveringPixbuf, 3.0F, false);
			hoveringImage = new Gtk.Image(hoveringPixbuf);
			
			this.Add(normalImage);

			ShowAll();

			Visible = false;
		}
		
		protected Position position;
		/// <value>
		/// The position represented by the overlay.
		/// </value>
		public Position Position
		{
			get {return position;}
		}
		
		protected Gtk.Image normalImage;
		
		protected Gtk.Image hoveringImage;
		
		
		protected bool hovering;
		/// <value>
		/// Whether the cursor is hovering over the overlay.
		/// </value>
		public bool Hovering
		{
			get {return hovering;}
			set
			{
				if (hovering != value)
				{
					this.Remove(Child);
					hovering = value;
					if (hovering)
						Add(hoveringImage);
					else
						Add(normalImage);
					Child.Show();
				}
			}
		}
		
		/// <summary>
		/// Returns true if the cursor is over the overlay.
		/// </summary>
		public bool HitTest()
		{
			int x, y;
			GetPointer(out x, out y);
			return x >= 0 && x <= Width && y >= 0 && y <= Width;
		}

	}
}
