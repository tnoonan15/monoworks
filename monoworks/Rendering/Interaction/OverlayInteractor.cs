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
using MonoWorks.Rendering.Events;

namespace MonoWorks.Rendering.Interaction
{
	/// <summary>
	/// Handles overlay interaction from the viewport.
	/// </summary>
	public class OverlayInteractor : AbstractInteractor
	{

		public OverlayInteractor(Viewport viewport)
			: base(viewport)
		{

		}


		public override void OnButtonPress(MouseButtonEvent evt)
		{
			base.OnButtonPress(evt);

			foreach (Overlay overlay in renderList.OverlayCopy)
				overlay.OnButtonPress(evt);
		}


		public override void OnButtonRelease(MouseButtonEvent evt)
		{
			base.OnButtonRelease(evt);

			foreach (Overlay overlay in renderList.OverlayCopy)
				overlay.OnButtonRelease(evt);
		}


		public override void OnMouseMotion(MouseEvent evt)
		{
			base.OnMouseMotion(evt);

			foreach (Overlay overlay in renderList.OverlayCopy)
				overlay.OnMouseMotion(evt);

			//if (!evt.Handled)
			//    viewport.ToolTip = "";
		}

	}
}
