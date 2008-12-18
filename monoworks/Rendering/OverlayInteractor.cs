// OverlayInteractor.cs - MonoWorks Project
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
	/// Handles overlay interaction from the viewport.
	/// </summary>
	public class OverlayInteractor : AbstractInteractor
	{

		public OverlayInteractor(IViewport viewport)
			: base(viewport)
		{

		}


		public override bool OnButtonPress(Coord pos, int button, InteractionModifier modifier)
		{
			base.OnButtonPress(pos, button, modifier);

			bool handled = false;
			return handled;
		}


		public override bool OnButtonRelease(Coord pos)
		{
			base.OnButtonRelease(pos);

			bool handled = false;
			return handled;
		}


		public override bool OnMouseMotion(Coord pos)
		{
			base.OnMouseMotion(pos);

			bool handled = false;

			foreach (Overlay overlay in renderList.Overlays)
			{
				if (!handled && overlay.HoverTest(pos))
					handled = true;
				else // either already been handled or failed the hit test
					overlay.IsHovering = false;
			}

			return handled;
		}

	}
}
