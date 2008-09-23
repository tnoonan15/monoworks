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
			
			// The GL widget is a minimum of 300x300 pixels 
			this.SetSizeRequest(600,600);
			
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

			// initialize the render manager
			renderManager = new RenderManager();		
		}
		
		
		protected InteractionState interactionState = new InteractionState();
		/// <value>
		/// The interaction state.
		/// </value>
		public InteractionStateBase InteractionState
		{
			get {return interactionState;}
		}
		
		
		/// <value>
		/// The rubber band used for selection and zooming.
		/// </value>
		protected RubberBand rubberBand = new RubberBand();
		
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
		
#endregion
		
		
#region Mouse Interaction
		
		protected void OnButtonPress(object sender, Gtk.ButtonPressEventArgs args)
		{
			interactionState.RegisterButtonPress(args.Event);
		}
		
		protected void OnButtonRelease(object sender, Gtk.ButtonReleaseEventArgs args)
		{
			interactionState.RegisterButtonRelease(args.Event);
		}
		
		protected virtual void OnMotionNotify(object sender, Gtk.MotionNotifyEventArgs args)
		{
			switch (interactionState.MouseType)
			{
			case InteractionType.Rotate:
				camera.Rotate(args.Event.X - interactionState.LastX, args.Event.Y - interactionState.LastY);
				PaintGL();
				break;
			case InteractionType.Pan:
				camera.Pan(args.Event.X - interactionState.LastX, args.Event.Y - interactionState.LastY);
				PaintGL();
				break;
			case InteractionType.Dolly:
				double factor = (args.Event.Y - interactionState.LastY) / (int)Allocation.Height;
				camera.Dolly(factor);
				PaintGL();
				break;
			}
			interactionState.RegisterMotion(args.Event);
		}
		
		protected virtual void OnScroll(object sender, Gtk.ScrollEventArgs args)
		{
			switch(interactionState.GetWheelType(args.Event.State))
			{
			case InteractionType.Dolly:
				if (args.Event.Direction == Gdk.ScrollDirection.Up)
					camera.DollyIn();
				else if (args.Event.Direction == Gdk.ScrollDirection.Down)
					camera.DollyOut();
				PaintGL();
				break;
			}
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
			if (MakeCurrent() == 0)
				return;
			
			// Run the state setup routine
			InitializeGL();
		}

		/// <summary>
		/// Called when the viewport is configured.
		/// </summary>
		void OnConfigure (object o, EventArgs e)
		{	
			if( this.MakeCurrent() == 0)
				return;
				
			camera.Configure();
		}
		
		/// <summary>
		/// Called when the viewport is resized.
		/// </summary>
		void OnSizeAllocated (object o, Gtk.SizeAllocatedArgs e)
		{			
			camera.Configure();
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
			if (this.MakeCurrent() == 0)
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
		

		// Bound in glwidget.glade
		private void OnWindowDeleteEvent(object sender, Gtk.DeleteEventArgs a) 
		{
			Gtk.Application.Quit ();
			a.RetVal = true;
		}

	}
}
