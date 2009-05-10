// Dockable.cs - Slate Mono Application Framework
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

using MonoWorks.GuiGtk.Framework;

namespace MonoWorks.GuiGtk.Framework.Dock
{
	
	public enum Position {Top, Left, Right, Bottom, Tab};
	
	/// <summary>
	/// Dockable widget inside the main window area.
	/// </summary>
	public class Dockable : Gtk.Frame
	{
		
		public Dockable(DockManager manager, DockableBase widget, string name) : base()
		{
			this.manager = manager;
			this.widget = widget;
			widget.Dockable = this;
			
			this.AddEvents((int)Gdk.EventMask.AllEventsMask);
						
			// create the floating window
			floatWindow = new FloatWindow(name);
			
			titleBar = new TitleBar(this, name);
			
			Name = name;
			
			vbox = new Gtk.VBox();
		
			AddTitleBar();
			
			vbox.PackEnd(widget, true, true, 0);
			internalSizeReq = widget.SizeRequest();
			SetSizeRequest(internalSizeReq.Width, internalSizeReq.Height);
			
			Add(vbox);
			
			NewParent();
		}
		
		
		protected DockManager manager;
		/// <value>
		/// The dock manager.
		/// </value>
		public DockManager Manager
		{
			get {return manager;}
		}
		
		protected DockableBase widget;

		//// <value>
		/// True if the dockable contains a document.
		/// </value>
		public bool ContainsADocument
		{
			get {return widget is DocumentBase;}
		}

		
		protected Gtk.VBox vbox;

		protected TitleBar titleBar;
		/// <value>
		/// The dockable's title bar.
		/// </value>
		public TitleBar TitleBar
		{
			get {return titleBar;}
		}
		
		/// <summary>
		/// Removes the titlebar from the dockable (to be put somewhere else, like a DockBook).
		/// </summary>
		public void RemoveTitleBar()
		{
			vbox.Remove(titleBar);
		}
		
		/// <summary>
		/// Adds the title bar to the dockable.
		/// </summary>
		public void AddTitleBar()
		{
			vbox.PackStart(titleBar, false, true, 0);
		}
		
		
		protected string name;
		/// <value>
		/// The name of the dockable.
		/// </value>
		public new string Name
		{
			get {return name;}
			set
			{
				name = value;
				floatWindow.Name = name;
				titleBar.Title = name;
			}
		}

		/// <value>
		/// The name of the icon associated with this dockable's widget.
		/// </value>
		public string IconName
		{
			get {return widget.IconName;}
		}

		/// <summary>
		/// Called when the dockable has a new parent.
		/// </summary>
		public void NewParent()
		{
			if (DockTabbed)
			{
				titleBar.Mode = TitleBarMode.Tabbed;
			}
			else // not in a book
			{
				titleBar.Mode = TitleBarMode.Normal;
				
				// add the title bar back
				if (titleBar.Parent != vbox)
					vbox.PackStart(titleBar, false, true, 0);
			}
		}


		/// <value>
		/// Closes the dockable after verification from the dockable widget.
		/// </value>
		public void Close()
		{
			if (widget.VerifyClose())
			{
				manager.Detach(this);
			}
		}


		/// <summary>
		/// Minimizes the dockable to a side of the window.
		/// </summary>
		public void Minimize()
		{
			// TODO: implement minimizing
		}
		
#region Size Request
		
		protected Gtk.Requisition internalSizeReq;
		/// <value>
		/// The size requested by the widget.
		/// </value>
		public Gtk.Requisition InternalSizeReq
		{
			get {return internalSizeReq;}
		}
		
		
#endregion
		
		
#region Docking
		
		/// <value>
		/// True if docked in a DockBook.
		/// </value>
		public bool DockTabbed
		{
			get { return Parent != null && Parent is DockBook;}
		}
					
		/// <summary>
		/// Docks the dockable next to another one.
		/// </summary>
		/// <param name="other"> The other <see cref="Dockable"/>. </param>
		/// <param name="position"> The relative position to other. </param>
		public void Dock(Dockable other, Position position)
		{
			UnFloat();
			manager.Dock(this, other, position);			
		}

		/// <summary>
		/// Docks the dockable to a main docking area.
		/// </summary>
		public void Dock()
		{
			Dock(Position.Top);
		}
		
		/// <summary>
		/// Docks the dockable to a main docking area at the given position.
		/// </summary>
		/// <param name="position"> The <see cref="Position"/> to dock to. </param>
		public void Dock(Position position)
		{
			UnFloat();
			manager.Dock(this, position);
		}
		
#endregion
				
		
#region Floating

		protected FloatWindow floatWindow;
		
		/// <value>
		/// Whether the dock is floating.
		/// </value>
		public bool DockFloating
		{
			get {return Parent == floatWindow;}
		}	
		
		/// <summary>
		/// Floats the dockable.
		/// </summary>
		/// <remarks>The cursor is automatically grabbed.</remarks>
		public void Float()
		{
			this.Float(true);
		}

		/// <summary>
		/// Floats the dockable and optionally grabs the cursor.
		/// </summary>
		/// <param name="grab"> </param>
		public void Float(bool grab)
		{
			Gdk.Rectangle alloc = Allocation;
			
			manager.Detach(this);
			
			titleBar.Mode = TitleBarMode.Normal;
			
			floatWindow.Add(this);
			floatWindow.ShowAll();
			floatWindow.Resize(alloc.Width, alloc.Height);

			if (grab)
				titleBar.Grab();
		}	
		
		
		/// <summary>
		/// Unfloats the dockable if it's floating.
		/// </summary>
		public void UnFloat()
		{			
			if (Parent == floatWindow)
			{
				floatWindow.Remove(this);
				floatWindow.Hide();
				Gdk.Pointer.Ungrab(0);
				SetSizeRequest(internalSizeReq.Width, internalSizeReq.Height);
			}
		}
		
#endregion

		
	}
}
