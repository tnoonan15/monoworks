using System;
using System.Collections.Generic;

//using gl = Tao.OpenGl.Gl;
//using glu = Tao.OpenGl.Glu;
using Tao.Platform.Windows;

using MonoWorks.Base;
using MonoWorks.Rendering;

namespace MonoWorks.GuiWpf
{
	/// <summary>
	/// Viewport for WPF.
	/// </summary>
	public class Viewport : SimpleOpenGlControl, IViewport
	{
		public Viewport()
			: base()
		{
			InitializeContexts();
			camera = new Camera(this);

			camera.Center = new Vector(1, 1, 1);

			camera.Configure();

			// initialize the interactionState
			interactionState = new InteractionState();

			InitializeGL();

		}


		#region IViewport Implementation

		protected Camera camera;
		/// <summary>
		/// The camera.
		/// </summary>
		public Camera Camera
		{
			get { return camera; }
		}

		/// <summary>
		/// Int width.
		/// </summary>
		/// <returns></returns>
		public int WidthGL
		{
			get {return Size.Width;}
		}

		/// <summary>
		/// Int height.
		/// </summary>
		/// <returns></returns>
		public int HeightGL
		{
			get { return Size.Height; }
		}

		protected RenderManager renderManager = new RenderManager();
		/// <summary>
		/// The render manager.
		/// </summary>
		public RenderManager RenderManager
		{
			get { return renderManager; }
		}

		protected Lighting lighting = new Lighting();
		/// <summary>
		/// The lighting.
		/// </summary>
		public Lighting Lighting
		{
			get { return lighting; }
		}



		#endregion



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

		protected InteractionState interactionState;
		/// <summary>
		/// The interaction state.
		/// </summary>
		public InteractionStateBase InteractionState
		{
			get { return interactionState; }
		}


		protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
		{
			base.OnMouseDown(e);
			interactionState.OnMouseDown(e);
		}

		protected override void OnMouseUp(System.Windows.Forms.MouseEventArgs e)
		{
			base.OnMouseUp(e);
			interactionState.OnMouseUp(e);
		}

		protected override void OnMouseMove(System.Windows.Forms.MouseEventArgs e)
		{
			base.OnMouseMove(e);

			switch (interactionState.MouseType)
			{
			case InteractionType.Rotate:
				camera.Rotate(e.Location.X - interactionState.LastLoc.X, e.Location.Y - interactionState.LastLoc.Y);
				Draw();
				break;
			case InteractionType.Pan:
				camera.Pan(e.Location.X - interactionState.LastLoc.X, e.Location.Y - interactionState.LastLoc.Y);
				Draw();
				break;
			case InteractionType.Dolly:
				double factor = (e.Location.Y - interactionState.LastLoc.Y) / (double)Size.Height;
				camera.Dolly(factor);
				Draw();
				break;
			}
			interactionState.OnMouseMove(e);
		}


		protected override void OnMouseWheel(System.Windows.Forms.MouseEventArgs e)
		{
			base.OnMouseWheel(e);

			if (e.Delta > 0)
				camera.DollyIn();
			else
				camera.DollyOut();
			Draw();
		}


		#endregion


		#region GL Stuff

		/// <summary>
		/// Initialize the OpenGL context.
		/// </summary>
		public void InitializeGL()
		{
			renderManager.InitializeGL();

			camera.Configure();

			PaintGL();
		}

		/// <summary>
		/// Called when the widget is resized.
		/// </summary>
		public void ResizeGL()
		{
			camera.Configure();
			PaintGL();
		}

		protected override void OnPaintBackground(System.Windows.Forms.PaintEventArgs e)
		{
			//base.OnPaintBackground(e);
		}

		protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
		{
			base.OnPaint(e);
			
			if (e.ClipRectangle.Width == 0)
			{
				return;
			}

			//camera.Configure();
			PaintGL();
		}

		/// <summary>
		/// Renders the scene.
		/// </summary>
		public void PaintGL()
		{
			MakeCurrent();

			renderManager.ClearScene();


			// render the rendering list
			camera.Place(); // place the camera for 3D rendering
			foreach (Renderable renderable in renderables)
				renderable.RenderOpaque(this);
			foreach (Renderable renderable in renderables)
				renderable.RenderTransparent(this);
			camera.PlaceOverlay(); // place the camera for overlay rendering
			foreach (Renderable renderable in renderables)
				renderable.RenderOverlay(this);

			SwapBuffers();
		}

		#endregion

		private void InitializeComponent()
		{
			this.SuspendLayout();
			// 
			// Viewport
			// 
			this.AutoSize = true;
			this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.Name = "Viewport";
			this.Size = new System.Drawing.Size(0, 0);
			this.ResumeLayout(false);

		}

	}
}
