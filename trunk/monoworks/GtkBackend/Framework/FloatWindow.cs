// FloatWindow.cs - Slate Mono Application Framework
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

namespace MonoWorks.GtkBackend.Framework
{
	
	/// <summary>
	/// Window used for floating dockables.
	/// </summary>
	public class FloatWindow : Gtk.Window
	{
		
		public FloatWindow(string name) : base(Gtk.WindowType.Popup)
		{
			Title = name;
			
			Decorated = false;
		}
		

//		protected override void OnSizeRequested (ref Gtk.Requisition requisition)
//		{
//			base.OnSizeRequested(ref requisition);
//			
//			requisition = Child.SizeRequest();
//			
//			Console.WriteLine("float window size request: {0}, {1}", requisition.Width, requisition.Height);
//		}

		
		
	}
}
