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

using MonoWorks.GtkBackend;
using MonoWorks.GtkBackend.Framework;
using MonoWorks.Modeling;

namespace MonoWorks.GtkStudio
{	
	/// <summary>
	/// Main application window for the Studio.
	/// </summary>
	public class MainWindow : Gtk.Window
	{

		/// <summary>
		/// Application entry point.
		/// </summary>
		public static void Main()
		{
			Gtk.Application.Init();
			MainWindow window = new MainWindow();
			window.Resize(1100, 1000);
			window.ShowAll();
			Gtk.Application.Run();
		}
		
		public MainWindow() : base(Gtk.WindowType.Toplevel)
		{
			Title = "MonoWorks Studio";
			
			_adapter = new ViewportAdapter();
			_scene = new StudioScene(_adapter.Viewport);
			
			Add(_adapter);
			_adapter.Viewport.RootScene = _scene;
			
			DeleteEvent += delegate {
				Gtk.Application.Quit();
			};
			
			
			_scene.AddDrawing(new Part() {Name="Part1"});
			_scene.AddDrawing(new Part() {Name="Part2"});
		}
		
		private readonly ViewportAdapter _adapter;
		
		private readonly StudioScene _scene;

	}
}
