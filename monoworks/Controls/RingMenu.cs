// 
//  RingMenu.cs - MonoWorks Project
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

using MonoWorks.Rendering.Events;

namespace MonoWorks.Controls
{

	/// <summary>
	/// A context menu that uses a RingBar as its control.
	/// </summary>
	public class RingMenu : GenericModalControlOverlay<RingBar>
	{

		public RingMenu() : base(new RingBar())
		{
			GrayScene = false;
		
		}
		
		/// <summary>
		/// Adds a button to the menu.
		/// </summary>
		public void Add(RingButton button)
		{
			Control.AddChild(button);
			button.Clicked += delegate {
				Close();
			};
		}
		
		/// <summary>
		/// Shows the menu onto the given scene's render list centered at the current location.
		/// </summary>
		public void Show(MouseButtonEvent evt)
		{
			ComputeGeometry();
			Origin.X = evt.Pos.X - Control.RenderWidth / 2.0;
			Origin.Y = evt.Pos.Y - Control.RenderHeight / 2.0;
			evt.Scene.ShowModal(this);
		}
		
	}
}
