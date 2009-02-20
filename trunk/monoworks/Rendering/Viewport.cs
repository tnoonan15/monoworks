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

using MonoWorks.Rendering;
using MonoWorks.Rendering.Interaction;
using MonoWorks.Rendering.Events;
using MonoWorks.Framework;

namespace MonoWorks.Rendering
{
	public class Viewport : IMouseHandler, IKeyHandler
	{

		public Viewport(ViewportAdapter adapter)
		{
			Camera = new Camera(this);

			// initialize the interactors
			renderableInteractor = new RenderableInteractor(this);
			overlayInteractor = new OverlayInteractor(this);

			this.adapter = adapter;

			Animator = new Animator(this);
		}

		protected ViewportAdapter adapter;

		private RenderList renderList = new RenderList();
		/// <summary>
		/// The rendering list for this viewport.
		/// </summary>
		public RenderList RenderList
		{
			get { return renderList; }
		}

		/// <summary>
		/// The camera.
		/// </summary>
		public Camera Camera { get; private set; }

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

		/// <summary>
		/// The animator for this viewport.
		/// </summary>
		public Animator Animator { get; private set; }


#region Rendering

		public void Initialize()
		{
			renderManager.Initialize();

			Camera.Configure();
		}


		public void Resize()
		{

			Camera.Configure();

			renderList.OnViewportResized(this);
		}


		public void Render()
		{
			adapter.MakeCurrent();

			renderManager.ClearScene();


			// render the rendering list
			renderList.Render(this);

			// render the rubber band
			renderableInteractor.RubberBand.Render(this);

			//SwapBuffers();
		}


		public int HeightGL
		{
			get { return adapter.HeightGL; }
		}

		public int WidthGL
		{
			get { return adapter.WidthGL; }
		}

		/// <summary>
		/// Passes the paint command to the viewport adapter.
		/// </summary>
		public void PaintGL()
		{
			adapter.PaintGL();
		}

		/// <summary>
		/// Passes the remote paint command to the viewport adapter.
		/// </summary>
		public void RemotePaintGL()
		{
			adapter.PaintGL();
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


#region Interactors


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


#endregion


#region IMouseHandler Members

		public void OnButtonPress(MouseButtonEvent evt)
		{
			evt.HitLine = Camera.ScreenToWorld(evt.Pos);

			overlayInteractor.OnButtonPress(evt);

			// primary interactor
			if (PrimaryInteractor != null &&
				((InteractionState != InteractionState.View3D && 
				evt.Modifier != InteractionModifier.Control) ||
				(InteractionState == InteractionState.View3D &&
				evt.Modifier == InteractionModifier.Control)))
				PrimaryInteractor.OnButtonPress(evt);

			renderableInteractor.OnButtonPress(evt);

			// handle double click
			if (!evt.Handled && evt.Multiplicity == ClickMultiplicity.Double)
			{
				if (InteractionState == InteractionState.Interact2D)
					Camera.AnimateTo(ViewDirection.Front);
				else
					Camera.AnimateTo(ViewDirection.Standard);
				evt.Handle();
			}
		}

		public void OnButtonRelease(MouseButtonEvent evt)
		{
			evt.HitLine = Camera.ScreenToWorld(evt.Pos);

			overlayInteractor.OnButtonRelease(evt);

			// primary interactor
			if (PrimaryInteractor != null &&
				((InteractionState != InteractionState.View3D &&
				evt.Modifier != InteractionModifier.Control) ||
				(InteractionState == InteractionState.View3D &&
				evt.Modifier == InteractionModifier.Control)))
				PrimaryInteractor.OnButtonRelease(evt);

			renderableInteractor.OnButtonRelease(evt);
		}

		public void OnMouseMotion(MouseEvent evt)
		{
			evt.HitLine = Camera.ScreenToWorld(evt.Pos);

			overlayInteractor.OnMouseMotion(evt);

			// primary interactor
			if (PrimaryInteractor != null &&
				((InteractionState != InteractionState.View3D &&
				evt.Modifier != InteractionModifier.Control) ||
				(InteractionState == InteractionState.View3D &&
				evt.Modifier == InteractionModifier.Control)))
				PrimaryInteractor.OnMouseMotion(evt);

			renderableInteractor.OnMouseMotion(evt);
		}


		public void OnMouseWheel(MouseWheelEvent evt)
		{
			bool blocked = false;

			// use the default dolly factor
			double factor;
			if (evt.Direction == WheelDirection.Up)
				factor = Camera.DollyFactor;
			else
				factor = -Camera.DollyFactor;

			// allow the renderables to deal with the interaction
			foreach (Renderable3D renderable in renderList.Renderables)
			{
				if (renderable.HandleDolly(this, factor))
					blocked = true;
			}

			if (!blocked)
				Camera.Dolly(factor);
		}

#endregion



#region IKeyHandler Members

		public void OnKeyPress(KeyEvent evt)
		{
			overlayInteractor.OnKeyPress(evt);

			if (PrimaryInteractor != null)
				PrimaryInteractor.OnKeyPress(evt);

			renderableInteractor.OnKeyPress(evt);
		}

#endregion
	}
}
