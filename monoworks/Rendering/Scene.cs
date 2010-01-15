// 
//  Scene.cs
//  
//  Author:
//       Andy Selvig <ajselvig@gmail.com>
// 
//  Copyright (c) 2010 Andy Selvig
// 
//  This library is free software; you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as
//  published by the Free Software Foundation; either version 2.1 of the
//  License, or (at your option) any later version.
// 
//  This library is distributed in the hope that it will be useful, but
//  WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
//  Lesser General Public License for more details.
// 
//  You should have received a copy of the GNU Lesser General Public
//  License along with this library; if not, write to the Free Software
//  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA

using System;

using MonoWorks.Rendering.Interaction;
using MonoWorks.Rendering.Events;

namespace MonoWorks.Rendering
{

	/// <summary>
	/// Represents a 3D scene where renderables can be rendered.
	/// </summary>
	public class Scene
	{

		public Scene(Viewport viewport)
		{
			Viewport = viewport;
			
			Camera = new Camera(this);
			
			RenderManager = new RenderManager();
			
			// initialize the interactors
			ViewInteractor = new ViewInteractor(this);
			OverlayInteractor = new OverlayInteractor(this);
			
			Animator = new Animator(this);
		}

		private RenderList renderList = new RenderList();
		/// <summary>
		/// The rendering list for this viewport.
		/// </summary>
		public RenderList RenderList
		{
			get { return renderList; }
		}
		
		/// <summary>
		/// The viewport associated with this scene.
		/// </summary>
		public Viewport Viewport { get; private set; }

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
		public Lighting Lighting
		{
			get { return RenderManager.Lighting; }
		}

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
		public Animator Animator
		{
			get;
			private set;
		}

		/// <summary>
		/// The renderable width of the scene.
		/// </summary>
		public double Width { get; set;}

		/// <summary>
		/// The renderable height of the scene.
		/// </summary>
		public double Height { get; set;	}


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
			
			renderList.OnSceneResized(this);
		}

		private bool queueResize = false;
		/// <summary>
		/// Tells the viewport to resize the next render cycle.
		/// </summary>
		/// <remarks>This is safe to call from non-GUI threads.</remarks>
		public void QueueResize()
		{
			queueResize = true;
		}
		
		
		public void RemotePaint()
		{
			Viewport.RemotePaintGL();
		}
		
		
		public void Paint()
		{
			Viewport.PaintGL();
		}

		/// <summary>
		/// Render the viewport.
		/// </summary>
		public void Render()
		{
			
			RenderManager.ClearScene();
			
			// resize if needed
			if (queueResize) {
				Resize();
				queueResize = false;
			}
			
			// render the rendering list
			renderList.Render(this);
			
			// let the interactors render themselves
			Camera.Place();
			ViewInteractor.RenderOpaque(this);
			ViewInteractor.RenderTransparent(this);
			OverlayInteractor.RenderOpaque(this);
			OverlayInteractor.RenderTransparent(this);
			if (PrimaryInteractor != null) {
				PrimaryInteractor.RenderOpaque(this);
				PrimaryInteractor.RenderTransparent(this);
			}
			Camera.PlaceOverlay();
			ViewInteractor.RenderOverlay(this);
			OverlayInteractor.RenderOverlay(this);
			if (PrimaryInteractor != null)
				PrimaryInteractor.RenderOverlay(this);
			
			//SwapBuffers();
		}
		
		#endregion


		#region Interactors

		private bool use2dInteraction = false;
		/// <value>
		/// Whether or not the user interaction should be 2-dimensional. 
		/// </value>
		public bool Use2dInteraction
		{
			get { return use2dInteraction; }
			set {
				use2dInteraction = value;
				if (InteractionStateChanged != null)
					InteractionStateChanged(this, new EventArgs());
				Resize();
			}
		}

		/// <summary>
		/// Raised when the interaction state changes.
		/// </summary>
		public EventHandler InteractionStateChanged;

		/// <summary>
		/// The primary interactor, generally specific to the content of the viewport.
		/// </summary>
		public AbstractInteractor PrimaryInteractor
		{
			get;
			set;
		}

		/// <summary>
		/// The renderable interactor.
		/// </summary>
		public ViewInteractor ViewInteractor
		{
			get;
			private set;
		}

		/// <summary>
		/// The overlay interactor.
		/// </summary>
		public OverlayInteractor OverlayInteractor
		{
			get;
			private set;
		}

		#endregion


		#region Mouse Interaction

		public void OnButtonPress(MouseButtonEvent evt)
		{
			evt.HitLine = Camera.ScreenToWorld(evt.Pos);
			
			OverlayInteractor.OnButtonPress(evt);
			
			if (PrimaryInteractor != null)
				PrimaryInteractor.OnButtonPress(evt);
			
			ViewInteractor.OnButtonPress(evt);
		}

		public void OnButtonRelease(MouseButtonEvent evt)
		{
			evt.HitLine = Camera.ScreenToWorld(evt.Pos);
			
			OverlayInteractor.OnButtonRelease(evt);
			
			if (PrimaryInteractor != null)
				PrimaryInteractor.OnButtonRelease(evt);
			
			ViewInteractor.OnButtonRelease(evt);
		}

		public void OnMouseMotion(MouseEvent evt)
		{
			evt.HitLine = Camera.ScreenToWorld(evt.Pos);
			
			OverlayInteractor.OnMouseMotion(evt);
			
			if (PrimaryInteractor != null)
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
			foreach (Actor renderable in renderList.Actors) {
				if (renderable.HandleDolly(this, factor))
					blocked = true;
			}
			
			if (!blocked)
				Camera.Dolly(factor);
		}

		/// <summary>
		/// Sets the tooltip on the viewport.
		/// </summary>
		public string ToolTip
		{
			set {  }
		}

		/// <summary>
		/// Clears the tooltip on the viewport.
		/// </summary>
		public void ClearToolTip()
		{
		}

		/// <summary>
		/// Set the current cursor to the given type.
		/// </summary>
		public void SetCursor(CursorType type)
		{
			Viewport.SetCursor(type);
		}

		#endregion


		#region Keyboard Interaction

		public void OnKeyPress(KeyEvent evt)
		{
			OverlayInteractor.OnKeyPress(evt);
			
			if (PrimaryInteractor != null)
				PrimaryInteractor.OnKeyPress(evt);
			
			ViewInteractor.OnKeyPress(evt);
		}

		#endregion


		#region Modal Overlay

		/// <summary>
		/// Shows a modal overlay on top of the rest of the renderables. 
		/// </summary>
		public void ShowModal(ModalOverlay modalOverlay)
		{
			RenderList.PushModal(modalOverlay);
			modalOverlay.Closed += OnCloseModalOverlay;
		}

		/// <summary>
		/// Handles the Closed event on the current modal overlay.
		/// </summary>
		private void OnCloseModalOverlay(object sender, EventArgs e)
		{
			ModalOverlay modalOverlay;
			if (sender is ModalOverlay)
				modalOverlay = sender as ModalOverlay;
			else
				modalOverlay = RenderList.TopModal;
			RenderList.PopModal(modalOverlay);
			modalOverlay.Closed -= OnCloseModalOverlay;
		}
		
		#endregion
	}
}
