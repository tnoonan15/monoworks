// Viewport.cs - MonoWorks Project
//
//    Copyright Andy Selvig 2008
//
//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU Lesser General Public License as published 
//    by the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU Lesser General Public License for more details.
//
//    You should have received a copy of the GNU Lesser General Public 
//    License along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections;
using System.Collections.Generic;

using gl=Tao.OpenGl.Gl;
using glu=Tao.OpenGl.Glu;

using GtkGL;

using MonoWorks.Base;
using MonoWorks.Rendering;
using MonoWorks.Rendering.Events;
using MonoWorks.Rendering.Interaction;

namespace MonoWorks.GtkBackend
{
	
	/// <summary>
	/// Gtk viewport.
	/// </summary>
	public class ViewportAdapter : GLArea, IViewportAdapter
	{
		/// <summary>
		/// The attributes list.
		/// </summary>
		static System.Int32[] attrlist = {
		    (int)GtkGL._GDK_GL_CONFIGS.Rgba,
		    (int)GtkGL._GDK_GL_CONFIGS.RedSize,1,
		    (int)GtkGL._GDK_GL_CONFIGS.GreenSize,1,
		    (int)GtkGL._GDK_GL_CONFIGS.BlueSize,1,
		    (int)GtkGL._GDK_GL_CONFIGS.DepthSize,1,
		    (int)GtkGL._GDK_GL_CONFIGS.Doublebuffer,
		    (int)GtkGL._GDK_GL_CONFIGS.None,
		};
		
		/// <summary>
		/// Default constructor.
		/// </summary>
		public ViewportAdapter() : this(attrlist)
		{
		}
		
		/// <summary>
		/// Constructor that takes an attribute list.
		/// </summary>
		/// <param name="attrList"> A <see cref="System.Int32"/>. </param>
		public ViewportAdapter(System.Int32[] attrList) : base(attrList)
		{
			Viewport = new Viewport(this);
			
			// make sure the viewport catches all events
			AddEvents((int)Gdk.EventMask.AllEventsMask);
					
			
			// mouse interaction signals
			ButtonPressEvent += OnButtonPress;
			ButtonReleaseEvent += OnButtonRelease;
			MotionNotifyEvent += OnMotionNotify;
			ScrollEvent += OnScroll;
			KeyPressEvent += OnKeyPress;
			KeyReleaseEvent += OnKeyRelease;
			
			// Connect some other signals		
			DeleteEvent += OnWindowDeleteEvent;
			ExposeEvent += OnExposed;
			Realized += OnRealized;
			SizeAllocated += OnSizeAllocated;
			ConfigureEvent += OnConfigure;	
			
			CanFocus = true;
			GrabFocus();
		}
		
		/// <value>
		/// The viewport used by this adapter.
		/// </value>
		public Viewport Viewport { get; private set; }
		
		
		private void OnWindowDeleteEvent(object sender, Gtk.DeleteEventArgs a) 
		{
			Gtk.Application.Quit ();
			a.RetVal = true;
		}
		
				
		#region Mouse and Keyboard Interaction
	
		/// <summary>
		/// Converts a Gdk modifier into an InteractionModifier.
		/// </summary>
		private static InteractionModifier GetModifier(Gdk.ModifierType modifier)
		{
			if ((modifier & Gdk.ModifierType.ControlMask) == Gdk.ModifierType.ControlMask)
				return InteractionModifier.Control;
			if ((modifier & Gdk.ModifierType.ShiftMask) == Gdk.ModifierType.ShiftMask)
				return InteractionModifier.Shift;
			if ((modifier & Gdk.ModifierType.HyperMask) == Gdk.ModifierType.HyperMask)
				return InteractionModifier.Alt;
			return InteractionModifier.None;
		}
		
		protected void OnButtonPress(object sender, Gtk.ButtonPressEventArgs args)
		{
			GrabFocus();
			
			// get the number of clicks
			ClickMultiplicity multiplicity = ClickMultiplicity.Single;
			if (args.Event.Type == Gdk.EventType.TwoButtonPress)
				multiplicity = ClickMultiplicity.Double;
			
			MouseButtonEvent evt = new MouseButtonEvent(Viewport.RootScene, new Coord(args.Event.X, HeightGL - args.Event.Y), 
			                                            (int)args.Event.Button, GetModifier(args.Event.State), multiplicity);
			Viewport.OnButtonPress(evt);
			
			PaintGL();

		}
		
		protected void OnButtonRelease(object sender, Gtk.ButtonReleaseEventArgs args)
		{
			MouseButtonEvent evt = new MouseButtonEvent(Viewport.RootScene, new Coord(args.Event.X, HeightGL - args.Event.Y), 
			                                      (int)args.Event.Button, GetModifier(args.Event.State));
			Viewport.OnButtonRelease(evt);
			
			PaintGL();
		}
		
		protected virtual void OnMotionNotify(object sender, Gtk.MotionNotifyEventArgs args)
		{
			MouseEvent evt = new MouseEvent(Viewport.RootScene, new Coord(args.Event.X, HeightGL - args.Event.Y), GetModifier(args.Event.State));			
			Viewport.OnMouseMotion(evt);
			
			PaintGL();
		}
		
		protected virtual void OnScroll(object sender, Gtk.ScrollEventArgs args)
		{
			WheelDirection direction = WheelDirection.Up;
			if (args.Event.Direction == Gdk.ScrollDirection.Down)
				direction = WheelDirection.Down;
			
			MouseWheelEvent evt = new MouseWheelEvent(Viewport.RootScene, direction, GetModifier(args.Event.State));
			Viewport.OnMouseWheel(evt);
			
			PaintGL();
		}

		
		/// <value>
		/// Sets the tooltip on the viewport.
		/// </value>
		public string ToolTip
		{
			set
			{
//				this.ToolTip = value;
			}
		}
		
		public void ClearToolTip()
		{
		}
				
		/// <summary>
		/// Set the current cursor to the given type.
		/// </summary>
		public void SetCursor(CursorType type)
		{
			switch (type)
			{
			case CursorType.Normal:
				GdkWindow.Cursor = new Gdk.Cursor(Gdk.CursorType.Arrow);
				break;
			case CursorType.Beam:
				GdkWindow.Cursor = new Gdk.Cursor(Gdk.CursorType.Xterm);
				break;
			case CursorType.Hand:
				GdkWindow.Cursor = new Gdk.Cursor(Gdk.CursorType.Hand2);
				break;
			}
		}
		
		protected KeyEvent GetKeyEvent(Gdk.EventKey evt)
		{
			var modifier = GetModifier(evt.State);
			
			// look for special keys
			var val = (int)evt.KeyValue;
			switch (evt.Key)
			{
			case Gdk.Key.Right:
				val = (int)SpecialKey.Right;
				break;
			case Gdk.Key.Left:
				val = (int)SpecialKey.Left;
				break;
			case Gdk.Key.Up:
				val = (int)SpecialKey.Up;
				break;
			case Gdk.Key.Down:
				val = (int)SpecialKey.Down;
				break;
			case Gdk.Key.Delete:
				val = (int)SpecialKey.Delete;
				break;
			case Gdk.Key.BackSpace:
				val = (int)SpecialKey.Backspace;
				break;
			case Gdk.Key.Return:
				val = (int)SpecialKey.Enter;
				break;
			case Gdk.Key.Home:
				val = (int)SpecialKey.Home;
				break;
			case Gdk.Key.End:
				val = (int)SpecialKey.End;
				break;
			case Gdk.Key.Tab:
				val = (int)SpecialKey.Tab;
				break;
			case Gdk.Key.ISO_Left_Tab:
				val = (int)SpecialKey.Tab;
				modifier = InteractionModifier.Shift;
				break;
			}
			
			return new KeyEvent(null, val, modifier);
		}
		
		protected virtual void OnKeyPress(object sender, Gtk.KeyPressEventArgs args)
		{
			var evt = GetKeyEvent(args.Event);
			Viewport.OnKeyPress(evt);
			args.RetVal = false;
			
			PaintGL();
		}

		protected virtual void OnKeyRelease(object sender, Gtk.KeyReleaseEventArgs args)
		{
			var evt = GetKeyEvent(args.Event);
			Viewport.OnKeyRelease(evt);
			args.RetVal = false;
			
			PaintGL();
		}

		
		#endregion
		
				
		#region OpenGL Methods
		
		/// <summary>
		/// Initialize the OpenGL rendering.
		/// </summary>
		public void InitializeGL()
		{			
			Viewport.Initialize();
						
			PaintGL();
		}

		/// <summary>
		/// Called when the widget is realized.
		/// </summary>
		void OnRealized(object o, EventArgs e)
		{
			if (base.MakeCurrent() == 0)
				return;
			
			// Run the state setup routine
			InitializeGL();
		}

		/// <summary>
		/// Called when the viewport is configured.
		/// </summary>
		void OnConfigure(object o, EventArgs e)
		{	
			if(base.MakeCurrent() == 0)
				return;
				
			Viewport.Resize();
		}
		
		/// <summary>
		/// Called when the viewport is resized.
		/// </summary>
		void OnSizeAllocated(object o, Gtk.SizeAllocatedArgs e)
		{
			ResizeGL();
		}
		
		public void ResizeGL()
		{
			if( !IsRealized || base.MakeCurrent() == 0)
				return;	
			
			Viewport.Resize();
			PaintGL();
		}
		
		public new void MakeCurrent()
		{
			base.MakeCurrent();
		}
		
		/// <summary>
		/// Called when the viewport is exposed.
		/// </summary>
		protected void OnExposed(object o, EventArgs e)
		{
			Render();
		}
		

		/// <summary>
		/// Queues the scene for rendering.
		/// </summary>
		public void PaintGL()
		{
			QueueDraw();
		}
		
		/// <summary>
		/// Queues the scene for rendering from another thread.
		/// </summary>
		public void RemotePaintGL()
		{
			Gtk.Application.Invoke(delegate {QueueDraw();});
		}		
		
		/// <summary>
		/// Actually performs the rendering.
		/// </summary>
		protected void Render()
		{
			if (!IsRealized)
				return;
			
			if (base.MakeCurrent() == 0)
				return;
			
			Viewport.Render();
													
			SwapBuffers();
		}
		
		/// <summary>
		/// The width of the viewport.
		/// </summary>
		public int WidthGL
		{
			get {return Allocation.Width;}
		}
		
		/// <summary>
		/// The height of the viewport.
		/// </summary>
		public int HeightGL
		{
			get {return Allocation.Height;}
		}
		
		#endregion

		
		#region File Dialog Stuff
		
		/// <summary>
		/// Exports the contents of the viewport to the given file.
		/// </summary>
		public void Export(string fileName)
		{
			throw new System.NotImplementedException();
		}
		
		/// <summary>
		/// Opens a file dialog with the given settings.
		/// </summary>
		public bool FileDialog(FileDialogDef def)
		{
			// decide which action to use
			Gtk.FileChooserAction action = Gtk.FileChooserAction.Save;
			string accept = "";
			if (def.Type == FileDialogType.Open)
			{
				action = Gtk.FileChooserAction.Open;
				accept = "Open";
			}
			else if (def.Type == FileDialogType.SaveAs)
			{
				action = Gtk.FileChooserAction.Save;
				accept = "Save";
			}
			
			var dialog = new Gtk.FileChooserDialog(def.Title, 
							this.Toplevel as Gtk.Window, action,
							"Cancel", Gtk.ResponseType.Cancel,
                            accept, Gtk.ResponseType.Accept);
			if (def.FileName != null)
				dialog.SetFilename(def.FileName);
			
			// assemble the filters
			foreach (var ext in def.Extensions)
			{
				var filter = new Gtk.FileFilter();
				var pattern = "*." + ext;
				filter.AddPattern(pattern);
				filter.Name = def.GetDescription(ext) + " (" + pattern + ")";
				dialog.AddFilter(filter);
			}
			
			var ret = dialog.Run() == (int)Gtk.ResponseType.Accept;
			def.FileName = dialog.Filename;
			dialog.Destroy();
			return ret;
		}
				
		#endregion
		
		

	}
}
