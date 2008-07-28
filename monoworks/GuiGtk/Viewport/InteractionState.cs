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


using MonoWorks.Rendering;

namespace MonoWorks.GuiGtk
{
	
	/// <summary>
	/// Keeps track of the state of the user interaction.
	/// </summary>
	public class InteractionState : InteractionStateBase
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public InteractionState() : base()
		{
			ConnectMouseMode(1, InteractionMode.Rotate);
			ConnectMouseMode(2, InteractionMode.Dolly);
			ConnectMouseMode(3, InteractionMode.Pan);
			ConnectMouseMode(3, Gdk.ModifierType.ShiftMask, InteractionMode.Dolly);
			
			ConnectWheelMode(Gdk.ModifierType.None, InteractionMode.Dolly);
			ConnectWheelMode(Gdk.ModifierType.ControlMask, InteractionMode.Pan);
		}
		
		
#region The Registry
		
		/// <summary>
		/// Maps the button/modifiers to mouse interaction types.
		/// </summary>
		protected Dictionary<uint, InteractionMode> mouseModes = new Dictionary<uint,InteractionMode>();
		
		/// <summary>
		/// Maps the modifiers to wheel interaction types.
		/// </summary>
		protected Dictionary<Gdk.ModifierType, InteractionMode> wheelModes = new Dictionary<Gdk.ModifierType,InteractionMode>();
		
		/// <summary>
		/// Produces a unit key for the button and modifier combo.
		/// </summary>
		/// <param name="button"> The button number. </param>
		/// <param name="modifier"> A <see cref="Gdk.ModifierType"/>. </param>
		/// <returns> A unique <see cref="System.UInt32"/> to this combo. </returns>
		protected uint GetKey(uint button, Gdk.ModifierType modifier)
		{
			return 255*button + (uint)modifier;
		}

		/// <summary>
		/// Connects the given button (without modifier) to the interaction mode.
		/// </summary>
		/// <param name="button"> The button number. </param>
		/// <param name="mode"> A <see cref="InteractionMode"/> </param>
		public void ConnectMouseMode(uint button, InteractionMode mode)
		{
			ConnectMouseMode(button, Gdk.ModifierType.None, mode);
		}		
		
		/// <summary>
		/// Connects the given button and modifier to the interaction mode.
		/// </summary>
		/// <param name="button"> The button number. </param>
		/// <param name="modifier"> A <see cref="Gdk.ModifierType"/>. </param>
		/// <param name="mode"> A <see cref="InteractionMode"/> </param>
		public void ConnectMouseMode(uint button, Gdk.ModifierType modifier, InteractionMode mode)
		{
			mouseModes[GetKey(button, modifier)] = mode;
		}
		
		/// <summary>
		/// Connects the given modifier to the wheel interaction mode.
		/// </summary>
		/// <param name="modifier"> A <see cref="Gdk.ModifierType"/>. </param>
		/// <param name="mode"> A <see cref="InteractionMode"/>. </param>
		public void ConnectWheelMode(Gdk.ModifierType modifier, InteractionMode mode)
		{
			wheelModes[modifier] = mode;
		}
		
#endregion
		

		/// <summary>
		/// Registers a button press event.
		/// </summary>
		/// <param name="evnt"> A <see cref="Gdk.EventButton"/>. </param>
		public void RegisterButtonPress(Gdk.EventButton evnt)
		{
			lastX = evnt.X;
			lastY = evnt.Y;
						
			if (mouseModes.ContainsKey(GetKey(evnt.Button, evnt.State)))
				mouseMode = mouseModes[GetKey(evnt.Button, evnt.State)];
			else
				mouseMode = InteractionMode.None;
		}

		/// <summary>
		/// Registers a button release event.
		/// </summary>
		/// <param name="evnt"> A <see cref="Gdk.EventButton"/>. </param>
		public void RegisterButtonRelease(Gdk.EventButton evnt)
		{
			mouseMode = InteractionMode.None;
		}
		
		/// <summary>
		/// Registers a motion notify event.
		/// </summary>
		/// <param name="evnt"> A <see cref="Gdk.EventMotion"/>. </param>
		public void RegisterMotion(Gdk.EventMotion evnt)
		{
			lastX = evnt.X;
			lastY = evnt.Y;
		}
		
		/// <summary>
		/// Gets the interaction mode for the given wheel modifier.
		/// </summary>
		/// <param name="modifier"> The <see cref="Gdk.ModifierType"/>. </param>
		/// <returns> The <see cref="InteractionMode"/> for modifier. </returns>
		public InteractionMode GetWheelMode(Gdk.ModifierType modifier)
		{
			if (wheelModes.ContainsKey(modifier))
				return wheelModes[modifier];
			else
				return InteractionMode.None;
		}
		
	}
}
