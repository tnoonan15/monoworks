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
		}
		
		
		protected InteractionState interactionState = new InteractionState();
		/// <value>
		/// The interaction state.
		/// </value>
		public InteractionState InteractionState
		{
			get {return interactionState;}
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
				
		
#region Renderable Registry
		
		/// <summary>
		/// Renderables to render.
		/// </summary>
		protected List<Renderable> renderables = new List<Renderable>();
		
		/// <summary>
		/// Adds a renderable to the rendering list.
		/// </summary>
		/// <param name="renderable"> A <see cref="Renderable"/>. </param>
		public void AddRenderable(Renderable renderable)
		{
			if (!renderables.Contains(renderable))
				renderables.Add(renderable);
		}
		
		/// <summary>
		/// Removes a renderable from the rendering list.
		/// </summary>
		/// <param name="renderable"> A <see cref="Renderable"/>. </param>
		public void RemoveRenderable(Renderable renderable)
		{
			if (!renderables.Contains(renderable))
				throw new Exception("The renderable is not a part of this viewport's rendering list.");
			renderables.Remove(renderable);
		}
		
		/// <value>
		/// The bounds of all renderables.
		/// </value>
		public Bounds Bounds
		{
			get
			{
				Bounds bounds = new Bounds();
				foreach (Renderable renderable in renderables)
					bounds.Resize(renderable.Bounds);
				return bounds;
			}
		}

		/// <summary>
		/// Reset the bounds of all renderables.
		/// </summary>
		public void ResetBounds()
		{
			foreach (Renderable renderable in renderables)
				renderable.ResetBounds();
		}
		
		/// <summary>
		/// Alerts the renderables that the viewport has been modified.
		/// </summary>
		public void OnResized()
		{
			foreach (Renderable renderable in renderables)
				renderable.OnViewportResized(this);
		}
		
		/// <summary>
		/// Alerts the renderables that the viewport has been modified.
		/// </summary>
		public void OnDirectionChanged()
		{
			foreach (Renderable renderable in renderables)
				renderable.OnViewDirectionChanged(this);
		}
		
#endregion


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
	
		/// <summary>
		/// The rubber band used for selection and viewing.
		/// </summary>
		private RubberBand rubberBand = new RubberBand();
		
		protected void OnButtonPress(object sender, Gtk.ButtonPressEventArgs args)
		{
			// look for the double-click reset
			if (args.Event.Type == Gdk.EventType.TwoButtonPress && args.Event.Button == 1)
			{
				if (interactionState.Mode == InteractionMode.Select2D)
					camera.SetViewDirection(ViewDirection.Front);
				else
					camera.SetViewDirection(ViewDirection.Standard);
				PaintGL();
			}
			
			Coord coord = new Coord(args.Event.X, args.Event.Y);
			interactionState.OnButtonPress(coord, (int)args.Event.Button);

			// handle selection and zooming 
			if (interactionState.MouseType == InteractionType.Select ||
				interactionState.MouseType == InteractionType.Zoom)
			{
				rubberBand.Start = new Coord(args.Event.X, HeightGL - args.Event.Y);;
				rubberBand.Enabled = true;
			}
		}
		
		protected void OnButtonRelease(object sender, Gtk.ButtonReleaseEventArgs args)
		{			
//			Coord coord = new Coord(args.Event.X, HeightGL - args.Event.Y);
			Coord coord = new Coord(args.Event.X, args.Event.Y);
			
			switch (interactionState.MouseType)
			{
			case InteractionType.Select:
				break;

			case InteractionType.Zoom:
				bool blocked = false;
				foreach (Renderable renderable in renderables)
				{
					if (renderable.HandleZoom(this, rubberBand))
						blocked = true;
				}
				if (!blocked)
				{
					// TODO: zoom
				}
				break;
			}

			interactionState.OnButtonRelease(coord);
			
			rubberBand.Enabled = false;
			PaintGL();
		}
		
		protected virtual void OnMotionNotify(object sender, Gtk.MotionNotifyEventArgs args)
		{
			bool blocked = false;
			
//			Coord coord = new Coord(args.Event.X, HeightGL - args.Event.Y);
			Coord coord = new Coord(args.Event.X, args.Event.Y);
			
			switch (interactionState.MouseType)
			{
			case InteractionType.Select:
				rubberBand.Stop = new Coord(args.Event.X, HeightGL - args.Event.Y);;
				break;
				
			case InteractionType.Rotate:
				camera.Rotate(coord - interactionState.LastPos);
				break;
				
			case InteractionType.Pan:
				Coord diff = coord - interactionState.LastPos;
				
				// allow the renderables to deal with the interaction
				foreach (Renderable renderable in renderables)
				{
					if (renderable.HandlePan(this, diff.X, diff.Y))
						blocked = true;
				}
				
				if (!blocked)
					camera.Pan(diff.X, diff.Y);
				break;
				
			case InteractionType.Dolly:
				double factor = (args.Event.Y - interactionState.LastPos.Y) / (int)Allocation.Height;
				
				// allow the renderables to deal with the interaction
				foreach (Renderable renderable in renderables)
				{
					if (renderable.HandleDolly(this, factor))
						blocked = true;
				}
				
				if (!blocked)
					camera.Dolly(factor);
				break;

			case InteractionType.Zoom:
				rubberBand.Stop = new Coord(args.Event.X, HeightGL - args.Event.Y);;
				break;
			}
			
			interactionState.OnMouseMotion(coord);
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
					factor = -camera.DollyFactor;
				else if (args.Event.Direction == Gdk.ScrollDirection.Down)
					factor = camera.DollyFactor;
				
				// allow the renderables to deal with the interaction
				foreach (Renderable renderable in renderables)
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

		
#region Selection
		
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
			foreach (Renderable renderable in renderables)
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

			
			// render the rendering list
			camera.Place();
			foreach (Renderable renderable in renderables)
				renderable.RenderOpaque(this);
			foreach (Renderable renderable in renderables)
				renderable.RenderTransparent(this);
			camera.PlaceOverlay();
			foreach (Renderable renderable in renderables)
				renderable.RenderOverlay(this);

			// render the rubber band
			rubberBand.Render(this);
													
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
