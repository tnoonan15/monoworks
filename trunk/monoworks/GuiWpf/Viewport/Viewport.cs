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
using swf = System.Windows.Forms;
using sd = System.Drawing;

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

			this.DoubleBuffered = false;

			// create the camera
			camera = new Camera(this);
			camera.Configure();

			// create the tooltip
			toolTip = new swf.ToolTip();
			toolTip.SetToolTip(this, "");

			// initialize the interactionState
			interactionState = new InteractionState();

			InitializeGL();

		}

		swf.ToolTip toolTip;


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
		/// Rendering width.
		/// </summary>
		/// <returns></returns>
		public int WidthGL
		{
			get { return this.Size.Width; }
		}

		/// <summary>
		/// Rendering height.
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


		/// <summary>
		/// Export the viewport to a bitmap in the given file name.
		/// </summary>
		/// <param name="fileName"></param>
		public void Export(string fileName)
		{
			Console.WriteLine("export to {0}", fileName);
			//sd.Bitmap bitmap = new sd.Bitmap(Width, Height);
			//DrawToBitmap(bitmap, new sd.Rectangle(0, 0, Width, Height));
			//bitmap.Save(fileName);

			sd.Bitmap bmp = new sd.Bitmap(this.ClientSize.Width, this.ClientSize.Height);
			sd.Imaging.BitmapData data =
			bmp.LockBits(this.ClientRectangle, sd.Imaging.ImageLockMode.WriteOnly,
			sd.Imaging.PixelFormat.Format24bppRgb);
			Tao.OpenGl.Gl.glReadPixels(0, 0, this.ClientSize.Width, this.ClientSize.Height, Tao.OpenGl.Gl.GL_BGR, Tao.OpenGl.Gl.GL_UNSIGNED_BYTE,
			data.Scan0);
			bmp.UnlockBits(data);
			bmp.RotateFlip(sd.RotateFlipType.RotateNoneFlipY);
			bmp.Save(fileName);
		}


#region Renderable Registry

        protected List<Renderable> renderables = new List<Renderable>();
        /// <summary>
        /// Renderables to render.
        /// </summary>
        public IEnumerable<Renderable> Renderables
        {
            get { return renderables; }
        }

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


		public void ResetBounds()
		{
			foreach (Renderable renderable in renderables)
				renderable.ResetBounds();
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

			// TODO: make this work for rubber band selection
			// handle selection and zooming 
			if (//interactionState.MouseType == InteractionType.Select || 
				interactionState.MouseType == InteractionType.Zoom)
			{
				rubberBand.Start = new Coord(args.Location.X, HeightGL - args.Location.Y);
				rubberBand.Enabled = true;
			}
		}

		protected override void OnMouseUp(System.Windows.Forms.MouseEventArgs args)
		{
			base.OnMouseUp(args);

			switch (interactionState.MouseType)
			{
			case InteractionType.Select:
				// determine the 3D position of the hit
				camera.Place();
				HitLine hitLine = camera.ScreenToWorld(args.Location.Coord());

				//List<Renderable> hitRends = new List<Renderable>();
				Renderable hitRend = null;
				foreach (Renderable renderable in renderables)
				{
					renderable.Deselect();
					if (renderable.HitTest(hitLine))
					{
						hitRend = renderable;
						//hitRends.Add(renderable);
						break;
					}
				}

				// TODO: handle multiple hits with depth checking

				// show the selection tooltip
				if (hitRend != null)
				{
					string description = hitRend.SelectionDescription;
					if (description.Length > 0)
					{
						toolTip.SetToolTip(this, description);
					}
				}

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
					// TODO: unblocked zoom
				}
				break;
			}

			interactionState.OnButtonRelease(args.Location.Coord());

			rubberBand.Enabled = false;

			PaintGL();
		}

		protected override void OnMouseMove(System.Windows.Forms.MouseEventArgs args)
		{
			base.OnMouseMove(args);
			bool blocked = false;

			switch (interactionState.MouseType)
			{
			case InteractionType.Select:
			case InteractionType.Zoom:
				rubberBand.Stop = new Coord(args.Location.X, HeightGL - args.Location.Y);
				break;

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

			PaintGL();
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
			PaintGL();
		}

		protected override void OnMouseDoubleClick(System.Windows.Forms.MouseEventArgs e)
		{
			base.OnMouseDoubleClick(e);

			if (interactionState.Mode == InteractionMode.Select2D)
				camera.SetViewDirection(ViewDirection.Front);
			else
				camera.SetViewDirection(ViewDirection.Standard);
			PaintGL();
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

			Console.WriteLine("viewport resized to {0}, {1}", WidthGL, HeightGL);

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
			
			if (e.ClipRectangle.Width == 0)
			{
				return;
			}

			//camera.Configure();
			Render();

			base.OnPaint(e);
		}

		public void PaintGL()
		{
			Draw();
			//Draw();
			//Render();
		}

		/// <summary>
		/// Renders the scene.
		/// </summary>
		public void Render()
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

			//SwapBuffers();
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
