// StyleClass.cs - MonoWorks Project
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

namespace MonoWorks.Rendering
{
	
	/// <summary>
	/// Represents the rendering style applied to a specific class of controls.
	/// </summary>
	public class StyleClass
	{
		
		public StyleClass()
		{
			backgrounds[HitState.None] = new FillGradient(ColorManager.Global["Gray"], ColorManager.Global["Light Gray"]);
			backgrounds[HitState.Hovering] = new FillGradient(ColorManager.Global["Light Blue"], ColorManager.Global["White"]);
			backgrounds[HitState.Selected] = new FillGradient(ColorManager.Global["Light Green"], ColorManager.Global["White"]);
			foregrounds[HitState.None] = ColorManager.Global["Black"];
		}


		protected Dictionary<HitState,IFill> backgrounds = new Dictionary<HitState,IFill>();

		/// <summary>
		/// Get the background fill for the given hitstate.
		/// </summary>
		public IFill GetBackground(HitState hitState)
		{
			if (backgrounds.ContainsKey(hitState))
				return backgrounds[hitState];
			else
				return backgrounds[HitState.None];
		}

		/// <summary>
		/// Modifies the fill used for the background with the given hit state.
		/// </summary>
		/// <param name="hitState"></param>
		/// <param name="fill"></param>
		public void ModifyBackground(HitState hitState, IFill fill)
		{
			backgrounds[hitState] = fill;
		}


		protected Dictionary<HitState, Color> foregrounds = new Dictionary<HitState, Color>();

		/// <summary>
		/// Get the foreground color for the given hitstate.
		/// </summary>
		public Color GetForeground(HitState hitState)
		{
			if (foregrounds.ContainsKey(hitState))
				return foregrounds[hitState];
			else
				return foregrounds[HitState.None];
		}

		/// <summary>
		/// Modifies the color used for the foreground with the given hit state.
		/// </summary>
		/// <param name="hitState"></param>
		/// <param name="fill"></param>
		public void ModifyForeground(HitState hitState, Color color)
		{
			foregrounds[hitState] = color;
		}

	}
}
