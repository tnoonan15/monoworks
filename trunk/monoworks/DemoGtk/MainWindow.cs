// MainWindow.cs - MonoWorks Project
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

using MonoWorks.Rendering;
using MonoWorks.Plotting;
using MonoWorks.GuiGtk;
using MonoWorks.GuiGtk.Framework;
using MonoWorks.Modeling;

namespace MonoWorks.DemoGtk
{
	
	/// <summary>
	/// Main window for the Gtk port of the plotting demo.
	/// </summary>
	public class MainWindow : Gtk.Window
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public MainWindow() : base(Gtk.WindowType.Toplevel)
		{
			Title = "MonoWorks Demo";
			
			DeleteEvent += OnDeleteEvent;
			
			// initialize the Gtk Resource Manager
			ResourceManager.Initialize();
			
			// create the notebook
			book = new Gtk.Notebook();
			Add(book);
			book.ChangeCurrentPage += OnPageChanged;
			
			// create model page		
			DrawingFrame drawingFrame = new DrawingFrame();
			book.AppendPage(drawingFrame, new Gtk.Label("Model"));
			drawingFrame.Drawing = new TestPart();	
			drawingFrame.Viewport.Camera.SetViewDirection(ViewDirection.Standard);
			
//			// create the 2D page
//			Pane2D pane2D = new Pane2D();
//			book.AppendPage(pane2D, new Gtk.Label("Basic 2D"));
//			
//			// create the 3D page
//			Pane3D pane3D = new Pane3D();
//			book.AppendPage(pane3D, new Gtk.Label("Basic 3D"));
			
			// create the controls page
			PaneControls paneControls = new PaneControls();
			book.AppendPage(paneControls, new Gtk.Label("Controls"));
			
			ShowAll();
		}
		
		Gtk.Notebook book;
		
		protected void OnDeleteEvent(object sender, Gtk.DeleteEventArgs args)
		{
			args.RetVal = true;
			Gtk.Application.Quit();
		}
		
		protected void OnPageChanged(object sender, Gtk.ChangeCurrentPageArgs args)
		{
			book.CurrentPageWidget.QueueDraw();
		}
		
	}
}
