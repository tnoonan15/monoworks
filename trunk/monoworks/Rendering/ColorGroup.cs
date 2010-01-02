// BasicDecorator.cs - MonoWorks Project
//
//  Copyright (C) 2009 Andy Selvig
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
using System.Xml.Serialization;

using MonoWorks.Base;
using MonoWorks.Rendering;

namespace MonoWorks.Rendering
{
	
	/// <summary>
	/// Represents a group of colors corresponding to hit states.
	/// </summary>
	public class ColorGroup
	{
		
		public ColorGroup()
		{
		}
		
		
		public ColorGroup(Color noneColor) : this()
		{
			colors[HitState.None] = noneColor;
		}
		
		public ColorGroup(Color noneColor, Color hoveringColor, Color selectedColor) : this()
		{
			colors[HitState.None] = noneColor;
			colors[HitState.Hovering] = hoveringColor;
			colors[HitState.Selected] = selectedColor;
		}
		
		
		/// <summary>
		/// The colors.
		/// </summary>
		private Dictionary<HitState, Color> colors = new Dictionary<HitState, Color>();
		
		/// <value>
		/// Get the color for the given hit state.
		/// </value>
		/// <remarks>If there is no color for hitState, the one for HitState.None will be returned.</remarks>
		public Color GetColor(HitState hitState)
		{
			if (colors.ContainsKey(hitState))
				return colors[hitState];
			else if ((hitState & HitState.Selected) >0 && colors.ContainsKey(HitState.Selected))
				return colors[HitState.Selected];
			else if (colors.ContainsKey(HitState.None))
				return colors[HitState.None];
			else
				return null;
		}
		
		/// <summary>
		/// Sets the color for the given hit state.
		/// </summary>
		public void SetColor(HitState hitState, Color color)
		{
			colors[hitState] = color;
		}
		
		/// <value>
		/// Get the color for the given hit state.
		/// </value>
		public Color this[HitState hitState]
		{
			get {return GetColor(hitState);}
			set {SetColor(hitState, value);}
		}
		
	}
}
