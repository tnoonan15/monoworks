// HandleBox.cs - Slate Mono Application Framework
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

namespace MonoWorks.GuiGtk.Framework.Tools
{
	/// <summary>
	/// Handlebox that holds only tools.
	/// </summary>
	public class HandleBox : Gtk.Table
	{

		/// <summary>
		/// Default constructor.
		/// </summary>
		/// <param name="tool"> Tool to handle. </param>
		public HandleBox(ITool tool) : base(1, 1, false)
		{
			// initialize handle icons
			InitializeIcons();
			
			// create the handle
			handle = new Gtk.EventBox();
			
			// attach the handle events
			handle.ButtonPressEvent += OnButtonPress;
			handle.ButtonReleaseEvent += OnButtonRelease;
			handle.MotionNotifyEvent += OnMotionNotify;
			
			// create the float window
			floatWindow = new FloatWindow("tool");
			
			this.tool = tool;
			Orientation = tool.Orientation;
		}
		


#region Handle Icons
		
		protected static Dictionary<Gtk.Orientation, Gdk.Pixbuf> handlePixbufs = new Dictionary<Gtk.Orientation,Gdk.Pixbuf>();
		
		/// <summary>
		/// Initialize the icons.
		/// </summary>
		protected static void InitializeIcons()
		{			
			if (!iconsInitialized)
			{
				iconsInitialized = true;
								
				handlePixbufs[Gtk.Orientation.Vertical] = new Gdk.Pixbuf(ResourceHelper.GetStream("VHandle.png"));
				handlePixbufs[Gtk.Orientation.Horizontal] = new Gdk.Pixbuf(ResourceHelper.GetStream("HHandle.png"));
			}
		}
		
		protected static bool iconsInitialized = false;
		
#endregion
		

		protected ITool tool;
		/// <summary>
		/// The tool.
		/// </summary>
		public ITool Tool
		{
			get { return tool; }
		}

		/// <summary>
		/// Removes the tool from the handle box's control.
		/// </summary>
		public void RemoveTool()
		{
			Remove(tool as Gtk.Widget);
			tool = null;
		}
		
		/// <value>
		/// The float window.
		/// </value>
		protected FloatWindow floatWindow;

		/// <value>
		/// The handle.
		/// </value>
		protected Gtk.EventBox handle;
		
		protected Gtk.Orientation orientation;
		/// <value>
		/// The orientation of the handle box and its tool.
		/// </value>
		public Gtk.Orientation Orientation
		{
			get {return orientation;}
			set
			{
				orientation = value;
				tool.Orientation = orientation;
				
				// remove everything
				foreach (Gtk.Widget child in Children)
					Remove(child);
				
				// resize appropriately
				if (orientation == Gtk.Orientation.Horizontal)
					Resize(1, 2);
				else // vertical
					Resize(2, 1);
				
				// change the handle icon
				if (handle.Child != null)
					handle.Remove(handle.Child);
				handle.Child = new Gtk.Image(handlePixbufs[orientation]);
				
				// attach the handle
				Attach(handle, 0, 1, 0, 1);
				
				// attach the tool
				if (tool != null)
				{
					if (orientation == Gtk.Orientation.Horizontal)
						Attach(tool as Gtk.Widget, 1, 2, 0, 1); 
					else // vertical
						Attach(tool as Gtk.Widget, 0, 1, 1, 2);
				}
				
				ShowAll();
			}
		}

		protected override void OnSizeRequested(ref Gtk.Requisition requisition)
		{
			base.OnSizeRequested(ref requisition);			
			
			Gtk.Requisition handleReq = handle.SizeRequest();
			Gtk.Requisition toolReq = (tool as Gtk.Widget).SizeRequest();
			if (orientation == Gtk.Orientation.Horizontal)
			{
				requisition.Width = handleReq.Width + toolReq.Width;
				requisition.Height = toolReq.Height;
			}
			else // vertical
			{				
				requisition.Height = handleReq.Height + toolReq.Height;
				requisition.Width = toolReq.Width;
			}
		}


		
		
#region Mouse Interaction

		protected int grabX, grabY;
		
		protected bool isGrabbed = false;
		/// <summary>
		/// Whether or not the 
		/// </summary>
		public bool IsGrabbed
		{
			get { return isGrabbed; }
			set { isGrabbed = value; }
		}
		
		/// <summary>
		/// Grabs the Gdk pointer to intercept all mouse interaction.
		/// </summary>
		protected void GrabPointer()
		{
			Gdk.Pointer.Grab(handle.GdkWindow, false, Gdk.EventMask.ButtonPressMask | 
			                 Gdk.EventMask.ButtonReleaseMask | Gdk.EventMask.PointerMotionMask, 
			                 null, new Gdk.Cursor(Gdk.CursorType.Hand2), 0); 
		}
		
		protected void OnButtonPress(object sender, Gtk.ButtonPressEventArgs args)
		{
			isGrabbed = true;
			
			GrabPointer();
			GetPointer(out grabX, out grabY);
		}


		protected void OnButtonRelease(object sender, Gtk.ButtonReleaseEventArgs args)
		{
			isGrabbed = false;
			Gdk.Pointer.Ungrab(0);
		}

		protected void OnMotionNotify(object sender, Gtk.MotionNotifyEventArgs args)
		{
			if (isGrabbed)
			{
				if (Parent is ToolGutter) // the tool is in a gutter
				{
					int x, y;
					(Parent as ToolGutter).GetPointer(out x, out y);
					(Parent as ToolGutter).OnMoveRequest(this, x - grabX, y - grabY);				
				}
				else // the tool is floating
				{
					// move the floating window
					FloatMoveToCursor();
					
					tool.ToolArea.OnHover(this);
				}
			}
			
		}


		/// <summary>
		/// Moves the floating window to the current cursor position.
		/// </summary>
		protected void FloatMoveToCursor()
		{
			int x, y, winX, winY;
			floatWindow.GetPointer(out x, out y);
			floatWindow.GetPosition(out winX, out winY);
			floatWindow.GdkWindow.Move(x + winX - grabX, y + winY - grabY);
		}
		

#endregion
		
		
		
#region Floating
		
		/// <summary>
		/// Floats the handle box (detaches it from the gutter).
		/// </summary>
		public virtual void Float()
		{
			if (!(Parent is ToolGutter)) // doesn't belong to a gutter
				throw new Exception("Handle box can't be floated if it doesn't belong to a gutter.");
						
			// get the current size
			Gdk.Rectangle alloc = Allocation;
			
			
			// reparent the handle box
			(Parent as ToolGutter).RemoveHandleBox(this);
			floatWindow.Child = this;
			floatWindow.ShowAll();
			FloatMoveToCursor();
			
			// regrab the pointer
			Gdk.Pointer.Ungrab(0);
			GrabPointer();
			
			// make the floating window the correct size
			floatWindow.Resize(alloc.Width, alloc.Height);
		}
		
		/// <summary>
		/// Docks the handle box to the given gutter.
		/// </summary>
		/// <param name="gutter"> A <see cref="ToolGutter"/> to dock to. </param>
		/// <remarks> Tries to put it at the cursor position.</remarks>
		public virtual void Dock(ToolGutter gutter)
		{
			if (Parent != floatWindow)
				throw new Exception("HandleBox must be floating to be docked.");
			
			// remove this from the float window
			floatWindow.Remove(this);
			floatWindow.Hide();
			
			// add this to the gutter
			gutter.AddHandleBox(this);
			int x, y;
			gutter.GetPointer(out x, out y);
			gutter.OnMoveRequest(this, x, y);
			gutter.ShowAll();
			
			// regrab the pointer
			Gdk.Pointer.Ungrab(0);
			GrabPointer();
		}
		
#endregion
		

	}
}
