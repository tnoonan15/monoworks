// 
//  HitState.cs
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
using System.Collections.Generic;

namespace MonoWorks.Rendering
{

	/// <summary>
	/// Renderable states with respect to mouse interaction.
	/// </summary>
	[Flags]
	public enum HitState {
		None = 0, 
		Hovering = 1,
		Selected = 2,
		Focused = 4
	};
	
	/// <summary>
	/// Delegate for handling hit state changed events.
	/// </summary>
	public delegate void HitStateChangedHandler(Renderable sender, HitState oldVal);
	
	/// <summary>
	/// Extension methods for HitState.
	/// </summary>
	public static class HitStateExtensions
	{
		/// <summary>
		/// Tests the hit state for Hovering.
		/// </summary>
		public static bool IsHovering(this HitState hitState)
		{
			return (hitState & HitState.Hovering) > 0;
		}
		
		/// <summary>
		/// Tests the hit state for Selected.
		/// </summary>
		public static bool IsSelected(this HitState hitState)
		{
			return (hitState & HitState.Selected) > 0;
		}
		
		/// <summary>
		/// Tests the hit state for Focused.
		/// </summary>
		public static bool IsFocused(this HitState hitState)
		{
			return (hitState & HitState.Focused) > 0;
		}
	}
	
}