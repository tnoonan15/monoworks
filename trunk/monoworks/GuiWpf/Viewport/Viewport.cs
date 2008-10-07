// Viewport.cs - MonoWorks Project
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

		public void OnDirectionChanged()
		{
			foreach (Renderable renderable in renderables)
				renderable.OnViewDirectionChanged(this);
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

		/// <value>
		/// The bounds of all renderables.
		/// </value>
		public new Bounds Bounds
		{
			get
			{
				Bounds bounds = new Bounds();
				foreach (Renderable renderable in renderables)
					bounds.Resize(renderable.Bounds);
				return bounds;
			}
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

		protected InteractionState interactionState;
		/// <summary>
		/// The interaction state.
		/// </summary>
		public InteractionState InteractionState
		{
			get { return interactionState; }
		}


		protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs args)
		{
			base.OnMouseDown(args);
			interactionState.OnButtonPress(args.Location.Coord(), SwfExtensions.ButtonNumber(args.Button));
		}

		protected override void OnMouseUp(System.Windows.Forms.MouseEventArgs args)
		{
			base.OnMouseUp(args);
			interactionState.OnButtonRelease(args.Location.Coord());
		}

		protected override void OnMouseMove(System.Windows.Forms.MouseEventArgs args)
		{
			base.OnMouseMove(args);
			bool blocked = false;

			switch (interactionState.MouseType)
			{
			case InteractionType.Pan:
				Coord diff = args.Location.Coord() - interactionState.LastPos;

				// allow the renderables to deal with the interaction
				foreach (Renderable renderable in renderables)
				{
					if (renderable.HandlePan(this, diff.X, diff.Y))
						blocked = true;
				}
				break;

			case InteractionType.Dolly:
				double factor = (args.Location.Y - interactionState.LastPos.Y) / (double)Size.Height;

				// allow the renderables to deal with the interaction
				foreach (Renderable renderable in renderables)
				{
					if (renderable.HandleDolly(this, factor))
						blocked = true;
				}
				break;
			}

			if (!blocked)
				interactionState.OnMouseMotion(args.Location.Coord(), camera);
			else
				interactionState.OnMouseMotion(args.Location.Coord());

			Draw();
		}


		protected override void OnMouseWheel(System.Windows.Forms.MouseEventArgs e)
		{
			base.OnMouseWheel(e);
			bool blocked = false;

			// use the default dolly factor
			double factor;
			if (e.Delta > 0)
				factor = -camera.DollyFactor;
			else
				factor = camera.DollyFactor;

			// allow the renderables to deal with the interaction
			foreach (Renderable renderable in renderables)
			{
				if (renderable.HandleDolly(this, factor))
					blocked = true;
			}

			if (!blocked)
				camera.Dolly(factor);
			Draw();
		}


#endregion


#region Selection


		protected RubberBand rubberBand = new RubberBand();


#endregion


#region GL Stuff

		/// <summary>
		/// Initialize the OpenGL context.
		/// </summary>
		public void InitializeGL()
		{
			MakeCurrent();

			renderManager.InitializeGL();

			camera.Configure();

			PaintGL();
		}

		/// <summary>
		/// Called when the widget is resized.
		/// </summary>
		public void ResizeGL()
		{
			MakeCurrent();
			camera.Configure();

			foreach (Renderable renderable in renderables)
				renderable.OnViewportResized(this);

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

			// render the rubber band
			rubberBand.Render(this);

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
