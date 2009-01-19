using System;
using System.Collections.Generic;

using MonoWorks.Rendering;
using MonoWorks.Rendering.Interaction;
using MonoWorks.Rendering.Events;
using MonoWorks.Framework;

namespace MonoWorks.Rendering
{
	public class Viewport : IMouseHandler
	{

		public Viewport(IViewportAdapter adapter)
		{
			Camera = new Camera(this);

			// initialize the interactors
			renderableInteractor = new RenderableInteractor(this);
			overlayInteractor = new OverlayInteractor(this);

			this.adapter = adapter;
		}

		protected IViewportAdapter adapter;

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
					Camera.SetViewDirection(ViewDirection.Front);
				else
					Camera.SetViewDirection(ViewDirection.Standard);
				evt.Handle();
			}
		}

		public void OnButtonRelease(MouseButtonEvent evt)
		{
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




	}
}
