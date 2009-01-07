// SwfViewport.cs - MonoWorks Project
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
using MonoWorks.Rendering.Events;
using MonoWorks.Rendering.Interaction;

namespace MonoWorks.GuiWpf
{

	/// <summary>
	/// Viewport for WPF.
	/// </summary>
	public class SwfViewport : SimpleOpenGlControl, IViewport
	{
		public SwfViewport()
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

			// initialize the interactors
			renderableInteractor = new RenderableInteractor(this);
			overlayInteractor = new OverlayInteractor(this);

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
			foreach (Renderable3D renderable in renderList.Renderables)
				renderable.OnViewDirectionChanged(this);
		}

#endregion


		/// <summary>
		/// Export the viewport to a bitmap in the given file name.
		/// </summary>
		/// <param name="fileName"></param>
		public void Export(string fileName)
		{
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


		private RenderList renderList = new RenderList();
		/// <summary>
		/// The rendering list for this viewport.
		/// </summary>
		public RenderList RenderList
		{
			get { return renderList; }
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

		public InteractionState InteractionState { get; set; }

        public AbstractInteractor PrimaryInteractor { get; set; }

		protected RenderableInteractor renderableInteractor;
		/// <summary>
		/// The renderable interactor.
		/// </summary>
		public RenderableInteractor RenderableInteractor
		{
			get { return renderableInteractor; }
		}

		protected OverlayInteractor overlayInteractor;
		/// <summary>
		/// The overlay interactor.
		/// </summary>
		public OverlayInteractor OverlayInteractor
		{
			get { return overlayInteractor; }
		}

		/// <summary>
		/// Convenience method that converts a mouse event point into a proper viewport coord.
		/// </summary>
		/// <param name="point"></param>
		/// <returns></returns>
		protected Coord MouseToViewport(System.Drawing.Point point)
		{
			return new Coord((double)point.X, (double)(HeightGL - point.Y));
		}


		protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs args)
		{
			base.OnMouseDown(args);

			MouseButtonEvent evt = new MouseButtonEvent(MouseToViewport(args.Location),
									SwfExtensions.ButtonNumber(args.Button));
            overlayInteractor.OnButtonPress(evt);
            if (PrimaryInteractor!=null && !evt.Handled)
                PrimaryInteractor.OnButtonPress(evt);
			if (!evt.Handled)
				renderableInteractor.OnButtonPress(evt);

			PaintGL();
		}

		protected override void OnMouseUp(System.Windows.Forms.MouseEventArgs args)
		{
			base.OnMouseUp(args);

            MouseButtonEvent evt = new MouseButtonEvent(MouseToViewport(args.Location),
                                    SwfExtensions.ButtonNumber(args.Button));
            overlayInteractor.OnButtonRelease(evt);
            if (PrimaryInteractor != null && !evt.Handled)
                PrimaryInteractor.OnButtonRelease(evt);
			if (!evt.Handled)
				renderableInteractor.OnButtonRelease(evt);

			PaintGL();
		}

		protected override void OnMouseMove(System.Windows.Forms.MouseEventArgs args)
		{
			base.OnMouseMove(args);

			MouseEvent evt = new MouseEvent(MouseToViewport(args.Location));
            overlayInteractor.OnMouseMotion(evt);
            if (PrimaryInteractor != null && !evt.Handled)
                PrimaryInteractor.OnMouseMotion(evt);
			if (!evt.Handled)
				renderableInteractor.OnMouseMotion(evt);

			PaintGL();
		}


		protected override void OnMouseWheel(System.Windows.Forms.MouseEventArgs e)
		{
			base.OnMouseWheel(e);
			bool blocked = false;

			// use the default dolly factor
			double factor;
			if (e.Delta > 0)
				factor = camera.DollyFactor;
			else
				factor = -camera.DollyFactor;

			// allow the renderables to deal with the interaction
			foreach (Renderable3D renderable in renderList.Renderables)
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

			if (InteractionState == InteractionState.Interact2D)
				camera.SetViewDirection(ViewDirection.Front);
			else
				camera.SetViewDirection(ViewDirection.Standard);
			PaintGL();
		}


#endregion



#region GL Stuff

		/// <summary>
		/// Initialize the OpenGL context.
		/// </summary>
		public void InitializeGL()
		{
			MakeCurrent();

			renderManager.Initialize();

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

			//Console.WriteLine("viewport resized to {0}, {1}", WidthGL, HeightGL);

			foreach (Renderable renderable in renderList.Renderables)
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

			Render();

			base.OnPaint(e);
		}

		public void PaintGL()
		{
			Draw();
		}

		/// <summary>
		/// Renders the scene.
		/// </summary>
		public void Render()
		{
			MakeCurrent();

			renderManager.ClearScene();


			// render the rendering list
			renderList.Render(this);

			// render the rubber band
			renderableInteractor.RubberBand.Render(this);

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
