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
	/// <summary>
	/// The Viewport is the interface between the GUI and rendering pipeline.
	/// </summary>
	public class Viewport : IMouseHandler, IKeyHandler
	{

		public Viewport(ViewportAdapter adapter)
		{
			Camera = new Camera(this);

			RenderManager = new RenderManager();

			// initialize the interactors
			ViewInteractor = new ViewInteractor(this);
			OverlayInteractor = new OverlayInteractor(this);

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

		/// <summary>
		/// The render manager.
		/// </summary>
		public RenderManager RenderManager {get; private set;}

		/// <summary>
		/// The lighting.
		/// </summary>
		public Lighting Lighting {get; private set;}

		/// <summary>
		/// Callback for the view direction changing.
		/// </summary>
		public void OnDirectionChanged()
		{
			foreach (Actor renderable in renderList.Actors)
				renderable.OnViewDirectionChanged(this);
		}

		/// <summary>
		/// The animator for this viewport.
		/// </summary>
		public Animator Animator { get; private set; }


#region Rendering

		/// <summary>
		/// Initialize rendering.
		/// </summary>
		public void Initialize()
		{
			RenderManager.Initialize();

			Camera.Configure();
		}

		/// <summary>
		/// Callback for the viewport being resized.
		/// </summary>
		public void Resize()
		{

			Camera.Configure();

			renderList.OnViewportResized(this);
		}
		
		/// <summary>
		/// Render the viewport.
		/// </summary>
		public void Render()
		{
			adapter.MakeCurrent();

			RenderManager.ClearScene();


			// render the rendering list
			renderList.Render(this);

			// let the interactors render themselves
			Camera.Place();
			ViewInteractor.RenderOpaque(this);
			ViewInteractor.RenderTransparent(this);
			ViewInteractor.RenderOverlay(this);
			if (PrimaryInteractor != null)
			{
				PrimaryInteractor.RenderOpaque(this);
				PrimaryInteractor.RenderTransparent(this);
				PrimaryInteractor.RenderOverlay(this);
			}
			Camera.PlaceOverlay();
			OverlayInteractor.RenderOpaque(this);
			OverlayInteractor.RenderTransparent(this);
			OverlayInteractor.RenderOverlay(this);

			//SwapBuffers();
		}

		/// <summary>
		/// Height of the rendered area.
		/// </summary>
		public int HeightGL
		{
			get { return adapter.HeightGL; }
		}

		/// <summary>
		/// Width of the rendered area.
		/// </summary>
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
		public void RenderText(TextDef text)
		{
			textRenderer.Render(text);
		}

		/// <summary>
		/// Renders lots of text to the viewport.
		/// </summary>
		public void RenderText(TextDef[] text)
		{
			textRenderer.Render(text);
		}

#endregion


#region Interactors

		/// <summary>
		/// The current interaction state (defines which interactor to use).
		/// </summary>
		public InteractionState InteractionState { get; private set; }

		/// <summary>
		/// Set the viewport interaction state.
		/// </summary>
		public void SetInteractionState(InteractionState state)
		{
			InteractionState = state;
			if (InteractionStateChanged != null)
				InteractionStateChanged(this, new EventArgs());
		}

		/// <summary>
		/// Raised when the interaction state changes.
		/// </summary>
		public EventHandler InteractionStateChanged;

		/// <summary>
		/// The primary interactor, generally specific to the content of the viewport.
		/// </summary>
		public AbstractInteractor PrimaryInteractor { get; set; }

		/// <summary>
		/// The renderable interactor.
		/// </summary>
		public ViewInteractor ViewInteractor { get; private set; }

		/// <summary>
		/// The overlay interactor.
		/// </summary>
		public OverlayInteractor OverlayInteractor { get; private set; }


#endregion


#region IMouseHandler Members

		public void OnButtonPress(MouseButtonEvent evt)
		{
			evt.HitLine = Camera.ScreenToWorld(evt.Pos);

			OverlayInteractor.OnButtonPress(evt);

			// primary interactor
			if (PrimaryInteractor != null &&
				((InteractionState != InteractionState.View3D && 
				evt.Modifier != InteractionModifier.Control) ||
				(InteractionState == InteractionState.View3D &&
				evt.Modifier == InteractionModifier.Control)))
				PrimaryInteractor.OnButtonPress(evt);

			ViewInteractor.OnButtonPress(evt);

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

			OverlayInteractor.OnButtonRelease(evt);

			// primary interactor
			if (PrimaryInteractor != null &&
				((InteractionState != InteractionState.View3D &&
				evt.Modifier != InteractionModifier.Control) ||
				(InteractionState == InteractionState.View3D &&
				evt.Modifier == InteractionModifier.Control)))
				PrimaryInteractor.OnButtonRelease(evt);

			ViewInteractor.OnButtonRelease(evt);
		}

		public void OnMouseMotion(MouseEvent evt)
		{
			evt.HitLine = Camera.ScreenToWorld(evt.Pos);

			OverlayInteractor.OnMouseMotion(evt);

			// primary interactor
			if (PrimaryInteractor != null &&
				((InteractionState != InteractionState.View3D &&
				evt.Modifier != InteractionModifier.Control) ||
				(InteractionState == InteractionState.View3D &&
				evt.Modifier == InteractionModifier.Control)))
				PrimaryInteractor.OnMouseMotion(evt);

			ViewInteractor.OnMouseMotion(evt);
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
			foreach (Actor renderable in renderList.Actors)
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
			OverlayInteractor.OnKeyPress(evt);

			if (PrimaryInteractor != null)
				PrimaryInteractor.OnKeyPress(evt);

			ViewInteractor.OnKeyPress(evt);
		}

#endregion


	}
}
