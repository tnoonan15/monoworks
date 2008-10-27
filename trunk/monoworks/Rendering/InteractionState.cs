// InteractionState.cs - MonoWorks Project
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

namespace MonoWorks.Rendering
{
	/// <summary>
	/// The interaction modes.
	/// </summary>
	public enum InteractionMode {View3D, Select3D, Select2D};
	
	/// <summary>
	/// Possible user interaction types.
	/// </summary>
	public enum InteractionType {None, Select, Rotate, Pan, Dolly, Zoom};

	/// <summary>
	/// Possible interactoin modifiers.
	/// </summary>
	public enum InteractionModifier { None=3, Shift, Control, Alt };

	
	/// <summary>
	/// Base class for mouse interaction state.
	/// </summary>
	/// <remarks> This should be subclassed for every GUI implementation.</remarks>
	public class InteractionState
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public InteractionState()
		{
			mouseType = InteractionType.None;

			ConnectMouseType(InteractionType.Rotate, 1);
			ConnectMouseType(InteractionType.Dolly, 2);
			ConnectMouseType(InteractionType.Pan, 3);
			ConnectMouseType(InteractionType.Dolly, 3, InteractionModifier.Shift);
		}
		
		protected InteractionMode mode = InteractionMode.View3D;
		/// <value>
		/// The current interaction mode.
		/// </value>
		public InteractionMode Mode
		{
			get {return mode;}
			set {mode = value;}
		}


		protected Coord anchor;
		/// <summary>
		/// The interaction anchor.
		/// </summary>
		public Coord Anchor
		{
			get { return anchor; }
			set { anchor = value; }
		}

		protected Coord lastPos;
		/// <summary>
		/// The last interaction point.
		/// </summary>
		public Coord LastPos
		{
			get { return lastPos; }
			set { lastPos = value; }
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

		protected InteractionType mouseType;
		/// <value>
		/// The current interaction mode.
		/// </value>
		/// <remarks> This will be overriden with Select if it should be rotate but the mode is Select2D or Select3D.</remarks>
		public InteractionType MouseType
		{
			get
			{
                if (mode != InteractionMode.View3D && mouseType == InteractionType.Rotate)
                    return InteractionType.Select;
                else if (mode == InteractionMode.Select2D && mouseType == InteractionType.Dolly)
                    return InteractionType.Zoom;
                else
					return mouseType;
			}
		}

		/// <summary>
		/// Registers a button press event without modifier.
		/// </summary>
		/// <param name="pos"></param>
		/// <param name="button"></param>
		public void OnButtonPress(Coord pos, int button)
		{
			OnButtonPress(pos, button, InteractionModifier.None);
		}

		/// <summary>
		/// Registers a button press event.
		/// </summary>
		/// <param name="pos"></param>
		/// <param name="button"></param>
		/// <param name="modifier"></param>
		public void OnButtonPress(Coord pos, int button, InteractionModifier modifier)
		{
			anchor = pos;
			lastPos = pos;

			int key = GetKey(button, modifier);
			if (mouseTypes.ContainsKey(key))
				mouseType = mouseTypes[key];
			else
				mouseType = InteractionType.None;
		}

		/// <summary>
		/// Registers a button release event.
		/// </summary>
		/// <param name="pos"></param>
		public void OnButtonRelease(Coord pos)
		{
			mouseType = InteractionType.None;
		}

		/// <summary>
		/// Registers the motion event without performing any interaction.
		/// </summary>
		/// <param name="pos"></param>
		public void OnMouseMotion(Coord pos)
		{
			lastPos = pos;
		}

		/// <summary>
		/// Registers a mouse motion event and performs the appropriate interaction.
		/// </summary>
		/// <param name="pos"></param>
		/// <param name="camera"> The camera that will handle the interaction.</param>
		/// <remarks> Viewport implementations should check for blocking by renderables before calling this.</remarks>
		public void OnMouseMotion(Coord pos, Camera camera)
		{
			switch (MouseType)
			{
			case InteractionType.Select:
				//rubberBand.StopX = interactionState.LastLoc.X;
				//rubberBand.StopY = HeightGL - interactionState.LastLoc.Y;
				break;

			case InteractionType.Rotate:
				camera.Rotate(pos - lastPos);
				break;

			case InteractionType.Pan:
				camera.Pan(pos - lastPos);
				break;

			case InteractionType.Dolly:
				double factor = (pos.Y - lastPos.Y) / (double)camera.ViewportHeight;
				camera.Dolly(factor);
				break;
			}
			lastPos = pos;
		}


#endregion

	}
}
