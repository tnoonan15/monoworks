// ViewInteractor.cs - MonoWorks Project
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

using MonoWorks.Base;
using MonoWorks.Rendering.Events;
using MonoWorks.Rendering;


namespace MonoWorks.Rendering.Interaction
{	
	/// <summary>
	/// Interactor that handles viewing interaction (rotating, panning, zooming, dollying).
	/// </summary>
	public class ViewInteractor : GenericInteractor<Scene>
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public ViewInteractor(Scene scene) : base(scene)
		{
			mouseType = InteractionType.None;

			ConnectMouseType(InteractionType.Rotate, 1);
			ConnectMouseType(InteractionType.Rotate, 1, InteractionModifier.Control);
			ConnectMouseType(InteractionType.Dolly, 2);
			ConnectMouseType(InteractionType.Pan, 3);
			ConnectMouseType(InteractionType.Dolly, 3, InteractionModifier.Shift);
		}


#region Mouse Types

		/// <summary>
		/// Gets the unique key for the button/modifier combo.
		/// </summary>
		/// <param name="button"></param>
		/// <param name="modifier"></param>
		/// <returns> A unique int representing button and modifier.</returns>
		protected int GetKey(int button, InteractionModifier modifier)
		{
			return button + (int)modifier;
		}


		/// <summary>
		/// The mouse interaction hashes for each button.
		/// </summary>
		protected Dictionary<int, InteractionType> mouseTypes = new Dictionary<int, InteractionType>();

		/// <summary>
		/// Associates the given mouse button with an interaction type.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="button"></param>
		public void ConnectMouseType(InteractionType type, int button)
		{
			ConnectMouseType(type, button, InteractionModifier.None);
		}

		/// <summary>
		/// Associates the given mouse button and modifier with an interaction type.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="button"></param>
		/// <param name="modifier"></param>
		public void ConnectMouseType(InteractionType type, int button, InteractionModifier modifier)
		{
			int key = GetKey(button, modifier);
			mouseTypes[key] = type;
		}

#endregion


#region Mouse Interaction

		protected InteractionType mouseType;
		/// <value>
		/// The current interaction mode.
		/// </value>
		/// <remarks> This will be overriden with Select if it should be rotate but the mode is Interact2D or Interact3D.</remarks>
		public InteractionType MouseType
		{
			get
			{
				if (Scene.Use2dInteraction
					&& mouseType == InteractionType.Dolly)
					return InteractionType.Zoom;
				else
					return mouseType;
			}
		}
		
		public override void Cancel()
		{
			base.Cancel();
			
			mouseType = InteractionType.None;
		}

		
		/// <summary>
		/// Gets set to true if an interaction has actually been performed during a mouse motion event.
		/// </summary>
		private bool interactionPerformed = false;

		/// <summary>
		/// Registers a button press event.
		/// </summary>
		/// <param name="evt"></param>
		public override void OnButtonPress(MouseButtonEvent evt)
		{
			base.OnButtonPress(evt);

			// don't interact if modal overlays are present
			if (Scene.RenderList.ModalCount > 0)
				return;
			
			int key = GetKey(evt.Button, evt.Modifier);
			if (mouseTypes.ContainsKey(key))
				mouseType = mouseTypes[key];
			else
				mouseType = InteractionType.None;

			// don't rotate in 2d mode
			if (Scene.Use2dInteraction && mouseType == InteractionType.Rotate)
				mouseType = InteractionType.None;

			// rubber hand zoom
			if (MouseType == InteractionType.Zoom)
			{
				RubberBand.Reset(evt.Pos);
				RubberBand.Enabled = true;
				evt.Handle(this);
			}

			// handle double click
			if (!evt.IsHandled && evt.Multiplicity == ClickMultiplicity.Double)
			{
				if (Scene.Use2dInteraction)
					Scene.Camera.AnimateTo(ViewDirection.Front);
				else
					Scene.Camera.AnimateTo(ViewDirection.Standard);
			}

		}

		/// <summary>
		/// Registers a button release event.
		/// </summary>
		/// <param name="evt"></param>
		public override void OnButtonRelease(MouseButtonEvent evt)
		{
			// don't interact if modal overlays are present
			if (Scene.RenderList.ModalCount > 0)
				return;
			
			switch (MouseType)
			{
			case InteractionType.Zoom:
				bool blocked = false;
				foreach (Actor actor in RenderList.Actors)
				{
					if (actor.HandleZoom(Scene, RubberBand))
						blocked = true;
				}
				if (!blocked)
				{
					// TODO: Rendering - implement unblocked zoom in ViewInteractor
				}
				break;
			}


			RubberBand.Enabled = false;
			base.OnButtonRelease(evt);
			
			// reset the interaction type.
			// if we've interacted, handle the event so others don't respond to it.
			if (mouseType != InteractionType.None)
			{
				mouseType = InteractionType.None;
				if (interactionPerformed)
					evt.Handle(this);
				interactionPerformed = false;
			}
		}

		/// <summary>
		/// Mouse motion handler that looks for child actors to handle the motion event.
		/// </summary>
		/// <remarks>If no one handles the event, then OnMouseMotion(MouseEvent, Camera) is called.</remarks>
		public override void OnMouseMotion(MouseEvent evt)
		{
			if (evt.IsHandled)
				return;

			// don't interact if modal overlays are present
			if (Scene.RenderList.ModalCount > 0)
				return;
			
			bool blocked = false;

			switch (MouseType)
			{
			case InteractionType.Select:
			case InteractionType.Zoom:
				RubberBand.Stop = evt.Pos;
				break;

			case InteractionType.Pan:
				Coord diff = evt.Pos - lastPos;

				// allow the renderables to deal with the interaction
				foreach (Actor renderable in RenderList.Actors)
				{
					if (renderable.HandlePan(Scene, diff.X, diff.Y))
						blocked = true;
				}
				break;

			case InteractionType.Dolly:
				double factor = (evt.Pos.Y - lastPos.Y) / Scene.Height;

				// allow the renderables to deal with the interaction
				foreach (Renderable renderable in RenderList.Actors)
				{
					if (renderable.HandleDolly(Scene, factor))
						blocked = true;
				}
				break;
			}
			
			// register whether or not an interactino has been performed
			if (mouseType != InteractionType.None)
				interactionPerformed = true;

			if (!blocked)
				OnMouseMotion(evt, Scene.Camera);

			base.OnMouseMotion(evt);
		}

		/// <summary>
		/// Registers a mouse motion event and performs the appropriate interaction.
		/// </summary>
		/// <param name="evt"></param>
		/// <param name="camera"> The camera that will handle the interaction.</param>
		protected void OnMouseMotion(MouseEvent evt, Camera camera)
		{
			switch (MouseType)
			{
			case InteractionType.Rotate:
				Scene.Camera.Rotate(evt.Pos - lastPos);
				break;

			case InteractionType.Pan:
				Scene.Camera.Pan(evt.Pos - lastPos);
				break;

			case InteractionType.Dolly:
				double factor = (evt.Pos.Y - lastPos.Y) / (double)camera.ViewportHeight;
				Scene.Camera.Dolly(factor);
				break;
			}
		}


#endregion

	}
}
