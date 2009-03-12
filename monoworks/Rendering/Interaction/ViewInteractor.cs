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
using MonoWorks.Framework;
using MonoWorks.Rendering.Events;
using MonoWorks.Rendering;


namespace MonoWorks.Rendering.Interaction
{	
	/// <summary>
	/// Interactor that handles viewing interaction (rotating, panning, zooming, dollying).
	/// </summary>
	public class ViewInteractor : AbstractInteractor
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public ViewInteractor(Viewport viewport) : base(viewport)
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
                if (viewport.InteractionState == InteractionState.Interact2D 
					&& mouseType == InteractionType.Dolly)
                    return InteractionType.Zoom;
                else
					return mouseType;
			}
		}

		/// <summary>
		/// Registers a button press event.
		/// </summary>
		/// <param name="evt"></param>
		public override void OnButtonPress(MouseButtonEvent evt)
		{
			base.OnButtonPress(evt);

			// Ctrl swaps first button view and interact types
			if (evt.Handled || 
				(evt.Button == 1 && evt.Modifier != InteractionModifier.Control &&
				viewport.InteractionState != InteractionState.View3D) || 
				(evt.Button == 1 && evt.Modifier == InteractionModifier.Control &&
				viewport.InteractionState == InteractionState.View3D))
				return;

			int key = GetKey(evt.Button, evt.Modifier);
			if (mouseTypes.ContainsKey(key))
				mouseType = mouseTypes[key];
			else
				mouseType = InteractionType.None;

			// TODO: make this work for rubber band selection
			if (MouseType == InteractionType.Zoom)
			{
				RubberBand.Reset(evt.Pos);
				RubberBand.Enabled = true;
				evt.Handle();
			}

			// handle double click
			if (!evt.Handled && evt.Multiplicity == ClickMultiplicity.Double)
			{
				if (viewport.InteractionState == InteractionState.Interact2D)
					viewport.Camera.AnimateTo(ViewDirection.Front);
				else
					viewport.Camera.AnimateTo(ViewDirection.Standard);
				evt.Handle();
			}

		}

		/// <summary>
		/// Registers a button release event.
		/// </summary>
		/// <param name="evt"></param>
		public override void OnButtonRelease(MouseButtonEvent evt)
		{
			switch (MouseType)
			{
			case InteractionType.Zoom:
				bool blocked = false;
				foreach (Actor actor in renderList.Actors)
				{
					if (actor.HandleZoom(viewport, RubberBand))
						blocked = true;
				}
				if (!blocked)
				{
					// TODO: unblocked zoom
				}
				break;
			}


			RubberBand.Enabled = false;
			base.OnButtonRelease(evt);
			mouseType = InteractionType.None;
		}

		/// <summary>
		/// Mouse motion handler that looks for child actors to handle the motion event.
		/// </summary>
		/// <remarks>If no one handles the event, then OnMouseMotion(MouseEvent, Camera) is called.</remarks>
		public override void OnMouseMotion(MouseEvent evt)
		{
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
				foreach (Actor renderable in renderList.Actors)
				{
					if (renderable.HandlePan(viewport, diff.X, diff.Y))
						blocked = true;
				}
				break;

			case InteractionType.Dolly:
				double factor = (evt.Pos.Y - lastPos.Y) / (double)viewport.HeightGL;

				// allow the renderables to deal with the interaction
				foreach (Renderable renderable in renderList.Actors)
				{
					if (renderable.HandleDolly(viewport, factor))
						blocked = true;
				}
				break;
			}

			if (!blocked)
				OnMouseMotion(evt, viewport.Camera);

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
				viewport.Camera.Rotate(evt.Pos - lastPos);
				break;

			case InteractionType.Pan:
				viewport.Camera.Pan(evt.Pos - lastPos);
				break;

			case InteractionType.Dolly:
				double factor = (evt.Pos.Y - lastPos.Y) / (double)camera.ViewportHeight;
				viewport.Camera.Dolly(factor);
				break;
			}
		}


#endregion

	}
}
