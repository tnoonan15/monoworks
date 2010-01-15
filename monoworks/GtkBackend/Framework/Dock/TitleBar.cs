// TitleBar.cs - Slate Mono Application Framework
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
using System.IO;

using MonoWorks.Framework;

namespace MonoWorks.GtkBackend.Framework.Dock
{

	/// <summary>
	/// Rendering and interaction mode for a title bar.
	/// </summary>
	public enum TitleBarMode {Normal, Tabbed};
	
	/// <summary>
	/// Title bar for a main dockable.
	/// </summary>
	public class TitleBar : Gtk.HBox
	{
		Dockable dockable;
		
		/// <summary>
		/// Default constructor.
		/// </summary>
		/// <param name="title"> The title. </param>
		public TitleBar(Dockable dockable, string title) : base()
		{
			this.dockable = dockable;

			// add the icon, if there is one
			if (dockable.IconName != null)
			{
				Gtk.Image icon = new Gtk.Image(RenderIcon(dockable.IconName, Gtk.IconSize.SmallToolbar, ""));
				PackStart(icon, false, true, 4);
			}
			
			titleLabel = new Gtk.Label(title);
			titleLabel.SetAlignment(0f, 0.5f);
			Title = title;
			
			// add the handle and associated event handling
			handle = new Gtk.EventBox();
			handle.ButtonPressEvent += OnButtonPress;
			handle.ButtonReleaseEvent += OnButtonRelease;
			handle.MotionNotifyEvent += OnMotionNotify;
			handle.EnterNotifyEvent += OnEnterHandle;
			handle.LeaveNotifyEvent += OnLeaveHandle;
			handle.CanFocus = false;
			PackStart(handle, true, true, 2);
			handle.Child = titleLabel;

			CanFocus = false;
						
			// add the buttons
			CreateButtons();	
			
			Mode = TitleBarMode.Normal;
		}
		

		protected TitleBarMode mode;
		/// <value>
		/// The titlebar mode.
		/// </value>
		public TitleBarMode Mode
		{
			get {return mode;}
			set
			{
				mode = value;
				
				// remove the buttons
				foreach (Gtk.Widget child in Children)
				{
					if (child is Gtk.Button)
						Remove(child);
				}
				
				if (mode == TitleBarMode.Normal)
				{						
					// add all of the buttons
					foreach (Gtk.Button button in buttons.Values)
						PackEnd(button, false, false, 1);
				}
				else // tabbed
				{										
					// add only the close button
					PackEnd(buttons["close"], false, false, 0);
				}
				ShowAll();
				
			}
		} 
		
		
#region Handle
		
		/// <value>
		/// The handle (user interaction) area.
		/// </value>
		protected Gtk.EventBox handle;
		
		/// <value>
		/// The title on the decoration.
		/// </value>
		public string Title
		{
			get {return titleLabel.Text;}
			set
			{
				titleLabel.Text = value;
			}
		}
		
		/// <summary>
		/// The label used to draw the title.
		/// </summary>
		protected Gtk.Label titleLabel;
		
#endregion
		
		
#region Buttons

		protected Dictionary<string, Gtk.Button> buttons;
		
		/// <summary>
		/// Creates the buttons.
		/// </summary>
		public void CreateButtons()
		{
			buttons = new Dictionary<string,Gtk.Button>();
						
			Gtk.Button minimizeButton = new Gtk.Button();
			minimizeButton.Image = new Gtk.Image(new Gdk.Pixbuf(ResourceHelper.GetStream("minimize.png")));
			minimizeButton.Clicked += delegate(object sender, EventArgs e) {
				dockable.Minimize();
			};
			buttons.Add("minimize", minimizeButton);
			
			Gtk.Button dockButton = new Gtk.Button();
			dockButton.Image =  new Gtk.Image(new Gdk.Pixbuf(ResourceHelper.GetStream("float.png")));
			dockButton.Clicked += delegate(object sender, EventArgs e) {
				if (dockable.DockFloating)
					dockable.Dock();
				else
					dockable.Float(false);
			};
			buttons.Add("dock", dockButton);
			
			Gtk.Button closeButton = new Gtk.Button();
			closeButton.Image =  new Gtk.Image(new Gdk.Pixbuf(ResourceHelper.GetStream("close.png")));
			closeButton.Clicked += delegate(object sender, EventArgs e) {
				dockable.Close();
			};
			buttons.Add("close", closeButton);
			
			foreach (Gtk.Button button in buttons.Values)
			{
				button.SetSizeRequest(22,22);
				button.BorderWidth = 0;
				button.Relief = Gtk.ReliefStyle.None;
				button.CanFocus = false;
			}
		}
		
#endregion
		
		
#region Mouse Interaction
		
		/// <summary>
		/// Handles button presses on the title area.
		/// </summary>
		/// <param name="sender"> The <see cref="System.Object"/> sending the event. </param>
		/// <param name="args"> A <see cref="Gtk.ButtonPressEventArgs"/>. </param>
		protected void OnButtonPress(object sender, Gtk.ButtonPressEventArgs args)
		{
			GetPointer(out offsetX, out offsetY);
			Grab();
		}

		/// <summary>
		/// Handles button releases on the title area.
		/// </summary>
		/// <param name="sender"> The <see cref="System.Object"/> sending the event. </param>
		/// <param name="args"> A <see cref="Gtk.ButtonReleaseEventArgs"/>. </param>
		protected void OnButtonRelease(object sender, Gtk.ButtonReleaseEventArgs args)
		{
			Release();
		}
		
		/// <summary>
		/// Handles mouse motion over the title area.
		/// </summary>
		/// <param name="sender"> The <see cref="System.Object"/> sending the event. </param>
		/// <param name="args"> A <see cref="Gtk.MotionNotifyEventArgs"/>. </param>
		protected void OnMotionNotify(object sender, Gtk.MotionNotifyEventArgs args)
		{
			
			if (grabbed)
			{	
				if (mode == TitleBarMode.Normal) // docked normally
				{	
					if (!dockable.DockFloating) // not floating
					{					
						dockable.Float();	
					}
					else // already floating
					{
						MoveToCursor();
					}
				}
				else // tabbed
				{
					// let the dock book handle the motion event
					(Parent as DockBook).OnChildMotion(dockable);
				}
			}
		}
		
		/// <summary>
		/// Handles enter notifies on the title are.
		/// </summary>
		/// <param name="sender"> The <see cref="System.Object"/> sending the event. </param>
		/// <param name="args"> A <see cref="Gtk.EnterNotifyEventArgs"/>. </param>
		protected void OnEnterHandle(object sender, Gtk.EnterNotifyEventArgs args)
		{
//			GdkWindow.Cursor = new Gdk.Cursor(Gdk.CursorType.Hand2);			
		}
		
		/// <summary>
		/// Handles leave notifies on the title area.
		/// </summary>
		/// <param name="sender"> The <see cref="System.Object"/> sending the event. </param>
		/// <param name="args"> A <see cref="Gtk.LeaveNotifyEventArgs"/>. </param>
		protected void OnLeaveHandle(object sender, Gtk.LeaveNotifyEventArgs args)
		{
//			if (grabbed)
//				MoveTo(args.Event.X, args.Event.Y);
//			else
//				GdkWindow.Cursor = new Gdk.Cursor(Gdk.CursorType.LastCursor);		
		}
		
#endregion
		

#region Floating

		
		protected int offsetX;
		protected int offsetY;
		
		/// <summary>
		/// Whether the handle has been grabbed.
		/// </summary>
		protected bool grabbed = false;
		
		/// <summary>
		/// Grabs the title bar at the given position.
		/// </summary>
		public void Grab()
		{			
			grabbed = true;
			Gdk.Pointer.Grab(handle.GdkWindow, false, Gdk.EventMask.ButtonPressMask | 
			                 Gdk.EventMask.ButtonReleaseMask | Gdk.EventMask.PointerMotionMask, 
			                 null, new Gdk.Cursor(Gdk.CursorType.Hand2), 0); 
		}
		
		/// <summary>
		/// Releases the title bar.
		/// </summary>
		public void Release()
		{
			grabbed = false;
			dockable.Manager.OnRelease(dockable);
			Gdk.Pointer.Ungrab(0);
		}
				
		/// <summary>
		/// Move the dockable to the cursor position.
		/// </summary>
		public void MoveToCursor()
		{		
			// get the window position
			int dockX, dockY, cursorX, cursorY;
			dockable.GdkWindow.GetPosition(out dockX, out dockY);
			GetPointer(out cursorX, out cursorY);
			
			dockable.GdkWindow.Move(dockX + cursorX - offsetX, dockY + cursorY - offsetY);
			dockable.Manager.OnHover(dockable, dockX + cursorX, dockY + cursorY);
		}
		
#endregion
		
	}
}
