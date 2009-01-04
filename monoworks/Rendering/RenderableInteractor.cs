// RenderableInteractor.cs - MonoWorks Project
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

namespace MonoWorks.Rendering
{
	/// <summary>
	/// The interaction modes.
	/// </summary>
	public enum InteractionState {View3D, Select3D, Select2D};
	
	/// <summary>
	/// Possible user interaction types.
	/// </summary>
	public enum InteractionType {None, Select, Rotate, Pan, Dolly, Zoom};

	
	/// <summary>
	/// Base class for mouse interaction state.
	/// </summary>
	public class RenderableInteractor : AbstractInteractor
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public RenderableInteractor(IViewport viewport) : base(viewport)
		{
			mouseType = InteractionType.None;

			ConnectMouseType(InteractionType.Rotate, 1);
			ConnectMouseType(InteractionType.Dolly, 2);
			ConnectMouseType(InteractionType.Pan, 3);
			ConnectMouseType(InteractionType.Dolly, 3, InteractionModifier.Shift);
		}
		
		protected InteractionState state = InteractionState.View3D;
		/// <value>
		/// The current interaction state.
		/// </value>
		public InteractionState State
		{
			get {return state;}
			set {state = value;}
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
			return button + (int)Math.Pow(2, (int)modifier);
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

		protected RubberBand rubberBand = new RubberBand();
		/// <summary>
		/// The rubber band used for select and zoom interactions.
		/// </summary>
		public RubberBand RubberBand
		{
			get { return rubberBand; }
		}

		protected InteractionType mouseType;
		/// <value>
		/// The current interaction mode.
		/// </value>
		/// <remarks> This will be overriden with Select if it should be rotate but the mode is Select2D or Select3D.</remarks>
		public InteractionType MouseType
		{
			get
			{
                if (state != InteractionState.View3D && mouseType == InteractionType.Rotate)
                    return InteractionType.Select;
                else if (state == InteractionState.Select2D && mouseType == InteractionType.Dolly)
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
			int key = GetKey(evt.Button, evt.Modifier);
			if (mouseTypes.ContainsKey(key))
				mouseType = mouseTypes[key];
			else
				mouseType = InteractionType.None;

			// TODO: make this work for rubber band selection
			if (MouseType == InteractionType.Zoom)
			{
				rubberBand.Start = evt.Pos;
				rubberBand.Enabled = true;
				evt.Handle();
			}

		}

		/// <summary>
		/// Registers a button release event.
		/// </summary>
		/// <param name="evt"></param>
		public override void OnButtonRelease(MouseEvent evt)
		{
			switch (MouseType)
			{
			case InteractionType.Select:
				// determine the 3D position of the hit
				viewport.Camera.Place();


				// TODO: handle multiple hits with depth checking

				// show the selection tooltip
				//if (hitRend != null)
				//{
				//    string description = hitRend.SelectionDescription;
				//    if (description.Length > 0)
				//    {
				//        toolTip.SetToolTip(this, description);
				//    }
				//}

				break;

			case InteractionType.Zoom:
				bool blocked = false;
				foreach (Renderable3D renderable in renderList.Renderables)
				{
					if (renderable.HandleZoom(viewport, rubberBand))
						blocked = true;
				}
				if (!blocked)
				{
					// TODO: unblocked zoom
				}
				break;
			}


			rubberBand.Enabled = false;
			base.OnButtonRelease(evt);
			mouseType = InteractionType.None;
		}

		/// <summary>
		/// Registers the motion event without performing any interaction.
		/// </summary>
		/// <param name="pos"></param>
		public override void OnMouseMotion(MouseEvent evt)
		{
			bool blocked = false;

			switch (MouseType)
			{
			case InteractionType.Select:
			case InteractionType.Zoom:
				rubberBand.Stop = evt.Pos;
				break;

			case InteractionType.Pan:
				Coord diff = evt.Pos - lastPos;

				// allow the renderables to deal with the interaction
				foreach (Renderable3D renderable in renderList.Renderables)
				{
					if (renderable.HandlePan(viewport, diff.X, diff.Y))
						blocked = true;
				}
				break;

			case InteractionType.Dolly:
				double factor = (evt.Pos.Y - lastPos.Y) / (double)viewport.HeightGL;

				// allow the renderables to deal with the interaction
				foreach (Renderable renderable in renderList.Renderables)
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
		/// <remarks> Viewport implementations should check for blocking by renderables before calling this.</remarks>
		public void OnMouseMotion(MouseEvent evt, Camera camera)
		{
			switch (MouseType)
			{
			case InteractionType.Select:
				//rubberBand.StopX = interactionState.LastLoc.X;
				//rubberBand.StopY = HeightGL - interactionState.LastLoc.Y;
				break;

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
