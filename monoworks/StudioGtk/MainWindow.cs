// MainWindow.cs - MonoWorks Project
//
//  Copyright (C) 2008 Andy Selvig
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA

using System;

using MonoWorks.GuiGtk.Framework;

namespace MonoWorks.StudioGtk
{	
	/// <summary>
	/// Main application window for the Studio.
	/// </summary>
	public class MainWindow : SlateWindow
	{

		/// <summary>
		/// Application entry point.
		/// </summary>
		public static void Main()
		{
			Gtk.Application.Init();
			MainWindow window = new MainWindow();
			window.Resize(900, 800);
			window.ShowAll();
			Gtk.Application.Run();
		}
		
		public MainWindow() : base("MonoWorks Studio")
		{

			new MainController(this);

		}

	}
}
