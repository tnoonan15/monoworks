// MainWindow.cs - Slate Mono Application Framework
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

using MonoWorks.Framework;
using MonoWorks.GtkBackend.Framework.Dock;

namespace MonoWorks.GtkBackend.Framework
{
	
	/// <summary>
	/// A main window for a Slate application, containing a tool area and dock area.
	/// </summary>
	/// <remarks> The first SlateWindow instantiated will be marked as the application window. 
	/// The application will only quit when the application window is closed.</remarks>
	public class SlateWindow : Gtk.Window
	{
		/// <summary>
		/// The window associated with the application.
		/// </summary>
		protected static SlateWindow ApplicationWindow;
		
		/// <summary>
		/// Default constructor.
		/// </summary>
		public SlateWindow(string title) : base(Gtk.WindowType.Toplevel)
		{
			if (ApplicationWindow == null)
				ApplicationWindow = this;
			
			Title = title;
			
			DeleteEvent += OnDelete;
			
			vbox = new Gtk.VBox();
			Add(vbox);			
			
			toolArea = new Tools.ToolArea();
			vbox.PackEnd(toolArea, true, true, 0);
			
			dockArea = new Dock.DockArea();
			dockManager = new Dock.DockManager(dockArea);
			toolArea.Content = dockArea;
		}
		
		/// <summary>
		/// The vbox that holds the menu bar and tool area.
		/// </summary>
		private Gtk.VBox vbox;
		
		/// <summary>
		/// Adds a menubar to the window.
		/// </summary>
		/// <param name="menuBar"> A <see cref="Gtk.MenuBar"/>. </param>
		public void AddMenuBar(Gtk.MenuBar menuBar)
		{
			vbox.PackStart(menuBar, false, true, 0);
			menuBar.ShowAll();
		}
		
		
		protected Tools.ToolArea toolArea;
		/// <value>
		/// The tool area.
		/// </value>
		public Tools.ToolArea ToolArea
		{
			get {return toolArea;}
		}
		
		/// <summary>
		/// Creates a toolbar at the given position.
		/// </summary>
		/// <param name="position"> A <see cref="ToolPosition"/>. </param>
		/// <returns> A <see cref="Tools.Toolbar"/>. </returns>
		public Tools.Toolbar CreateToolbar(ToolPosition position)
		{
			Tools.Toolbar toolbar = new Tools.Toolbar();
			toolArea.AddTool(toolbar, position);
			return toolbar;
		}
		
		/// <summary>
		/// Creates a toolbox at the given position.
		/// </summary>
		/// <param name="position"> A <see cref="ToolPosition"/>. </param>
		/// <returns> A <see cref="Tools.ToolBox"/>. </returns>
		public Tools.ToolBox CreateToolBox(ToolPosition position, string name)
		{
			Tools.ToolBox toolBox = new Tools.ToolBox(name);
			toolArea.AddTool(toolBox, position);
			return toolBox;
		}
		
		
		protected Dock.DockArea dockArea;
		/// <value>
		/// The main dock area.
		/// </value>
		public Dock.DockArea MainDockArea
		{
			get {return dockArea;}
		}
		
		protected Dock.DockManager dockManager;
		/// <value>
		/// The dock manager.
		/// </value>
		public Dock.DockManager DockManager
		{
			get {return dockManager;}
		}
				
		/// <summary>
		/// Handles the window being deleted.
		/// </summary>
		/// <param name="arg"> A <see cref="Gtk.DeleteEventArgs"/>. </param>
		protected virtual void OnDelete(object sender, Gtk.DeleteEventArgs arg)
		{
			if (this == ApplicationWindow)
			{
				arg.RetVal = true;
				Gtk.Application.Quit();
			}
		}
				
		
	}
}
