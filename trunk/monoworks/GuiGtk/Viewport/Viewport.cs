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

using Cairo;

using gl=Tao.OpenGl.Gl;
using glu=Tao.OpenGl.Glu;

using GtkGL;

using MonoWorks.Base;
using MonoWorks.Rendering;
using MonoWorks.Rendering.Events;

namespace MonoWorks.GuiGtk
{
	
	/// <summary>
	/// Gtk viewport.
	/// </summary>
	public class Viewport : GLArea, IViewport
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
		public Viewport() : this(attrlist)
		{
		}
		
		/// <summary>
		/// Constructor that takes an attribute list.
		/// </summary>
		/// <param name="attrList"> A <see cref="System.Int32"/>. </param>
		public Viewport(System.Int32[] attrList) : base(attrList)
		{
			// make sure the viewport catches all events
			AddEvents((int)Gdk.EventMask.AllEventsMask);
			
			camera = new Camera(this);
			
			DeleteEvent += OnWindowDeleteEvent;
			
			// mouse interaction signals
			ButtonPressEvent += OnButtonPress;
			ButtonReleaseEvent += OnButtonRelease;
			MotionNotifyEvent += OnMotionNotify;
			ScrollEvent += OnScroll;
			
			// Connect some other signals		
			ExposeEvent += OnExposed;
			Realized += OnRealized;
			SizeAllocated += OnSizeAllocated;
			ConfigureEvent += OnConfigure;			
//			SizeAllocated += OnResize;

			// initialize the render manager
			renderManager = new RenderManager();

			renderableInteractor = new RenderableInteractor(this);
			overlayInteractor = new OverlayInteractor(this);
		}
		
		
		protected RenderManager renderManager;
		/// <value>
		/// The viewport render manager.
		/// </value>
		public RenderManager RenderManager
		{
			get {return renderManager;}
		}
		
		protected Camera camera;
		/// <value>
		/// Accesses the viewport camera.
		/// </value>
		public Camera Camera
		{
			get {return camera;}
		}
		
		protected Lighting lighting;
		/// <value>
		/// The viewport lighting.
		/// </value>
		public Lighting Lighting
		{
			get {return lighting;}
		}
		
		
		private RenderList renderList = new RenderList();
		/// <summary>
		/// The rendering list for this viewport.
		/// </summary>
		public RenderList RenderList
		{
			get {return renderList;}
		}

		private RenderableInteractor renderableInteractor;
		//// <value>
		/// The renderable interactor.
		/// </value>
		public RenderableInteractor RenderableInteractor
		{
			get {return renderableInteractor;}
		}

		private OverlayInteractor overlayInteractor;
		//// <value>
		/// The overlay interactor.
		/// </value>
		public OverlayInteractor OverlayInteractor
		{
			get {return overlayInteractor;}
		}
		
		
		/// <summary>
		/// Alerts the renderables that the viewport has been modified.
		/// </summary>
		public void OnResized()
		{
			foreach (Renderable3D renderable in renderList.Renderables)
				renderable.OnViewportResized(this);
		}
		
		/// <summary>
		/// Alerts the renderables that the viewport has been modified.
		/// </summary>
		public void OnDirectionChanged()
		{
			foreach (Renderable3D renderable in renderList.Renderables)
				renderable.OnViewDirectionChanged(this);
		}




#region Text Renderering

		/// <summary>
		/// Renders text to the viewport.
		/// </summary>
		protected TextRenderer textRenderer = new TextRenderer();

		/// <summary>
		/// Renders text to the viewport.
		/// </summary>
		/// <param name="size"></param>
		/// <returns></returns>
		public void RenderText(TextDef text)
		{
			textRenderer.Render(text);
		}

		/// <summary>
		/// Renders lots of text to the viewport.
		/// </summary>
		/// <param name="size"></param>
		/// <returns></returns>
		public void RenderText(TextDef[] text)
		{
			textRenderer.Render(text);
		}

#endregion
		
		
#region Mouse Interaction
			
		protected void OnButtonPress(object sender, Gtk.ButtonPressEventArgs args)
		{
			// look for the double-click reset
			if (args.Event.Type == Gdk.EventType.TwoButtonPress && args.Event.Button == 1)
			{
				if (renderableInteractor.State == InteractionState.Select2D)
					camera.SetViewDirection(ViewDirection.Front);
				else
					camera.SetViewDirection(ViewDirection.Standard);
				PaintGL();
			}
			
			MouseButtonEvent evt = new MouseButtonEvent(new Coord(args.Event.X, HeightGL - args.Event.Y), (int)args.Event.Button);
			overlayInteractor.OnButtonPress(evt);
			if (!evt.Handled) // the overlays didn't handle the event
				renderableInteractor.OnButtonPress(evt);

		}
		
		protected void OnButtonRelease(object sender, Gtk.ButtonReleaseEventArgs args)
		{			
			MouseEvent evt = new MouseEvent(new Coord(args.Event.X, HeightGL - args.Event.Y));
			overlayInteractor.OnButtonRelease(evt);
			if (!evt.Handled) // the overlays didn't handle the event
				renderableInteractor.OnButtonRelease(evt);
			PaintGL();
		}
		
		protected virtual void OnMotionNotify(object sender, Gtk.MotionNotifyEventArgs args)
		{			
			MouseEvent evt = new MouseEvent(new Coord(args.Event.X, HeightGL - args.Event.Y));
			overlayInteractor.OnMouseMotion(evt);
			if (!evt.Handled) // the overlays didn't handle the event
				renderableInteractor.OnMouseMotion(evt);
			PaintGL();
		}
		
		protected virtual void OnScroll(object sender, Gtk.ScrollEventArgs args)
		{
			bool blocked = false;

			// TODO: Make different scroll interactions in the InteractionState
//			switch(interactionState.GetWheelType(args.Event.State))
//			{
//			case InteractionType.Dolly:
				double factor = 0;
				if (args.Event.Direction == Gdk.ScrollDirection.Up)
					factor = camera.DollyFactor;
				else if (args.Event.Direction == Gdk.ScrollDirection.Down)
					factor = -camera.DollyFactor;
				
				// allow the renderables to deal with the interaction
				foreach (Renderable3D renderable in renderList.Renderables)
				{
					if (renderable.HandleDolly(this, factor))
						blocked = true;
				}
				
				if (!blocked)
					camera.Dolly(factor);
//				break;
//			}
			PaintGL();
		}

		
#endregion

		
		
#region OpenGL Methods
		
		/// <summary>
		/// Initialize the OpenGL rendering.
		/// </summary>
		public void InitializeGL()
		{			
			renderManager.InitializeGL();
			
			camera.Configure();
						
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
		void OnConfigure (object o, EventArgs e)
		{	
			if(base.MakeCurrent() == 0)
				return;
				
			camera.Configure();
		}
		
		/// <summary>
		/// Called when the viewport is resized.
		/// </summary>
		void OnSizeAllocated (object o, Gtk.SizeAllocatedArgs e)
		{
			ResizeGL();
		}
		
		public void ResizeGL()
		{
			if( !IsRealized || base.MakeCurrent() == 0)
				return;	
			camera.Configure();
			foreach (Renderable3D renderable in renderList.Renderables)
				renderable.OnViewportResized(this);
		}
		
		public new void MakeCurrent()
		{
			base.MakeCurrent();
		}
		
		/// <summary>
		/// Called when the viewport is exposed.
		/// </summary>
		protected void OnExposed (object o, EventArgs e)
		{
			PaintGL();
		}
		

		/// <summary>
		/// Renders the scene.
		/// </summary>
		public void PaintGL()
		{
			if (base.MakeCurrent() == 0)
				return;
			
			// Clear the scene
			renderManager.ClearScene();

			// render the render list
			renderList.Render(this);

			renderableInteractor.RubberBand.Render(this);
													
			// bring back buffer to front, put front buffer in back
			SwapBuffers ();
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
		

		
		private void OnWindowDeleteEvent(object sender, Gtk.DeleteEventArgs a) 
		{
			Gtk.Application.Quit ();
			a.RetVal = true;
		}

		
		

	}
}
