// PlotInteractor.cs - MonoWorks Project
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

using MonoWorks.Rendering;
using MonoWorks.Rendering.Interaction;
using MonoWorks.Rendering.Events;

namespace MonoWorks.Plotting
{
	/// <summary>
	/// Interactor for plots.
	/// </summary>
	public class PlotInteractor : AbstractInteractor
	{

		public PlotInteractor(Viewport viewport)
			: base(viewport)
		{

		}


		public override void OnButtonPress(MouseButtonEvent evt)
		{
			base.OnButtonPress(evt);
		}

		public override void OnButtonRelease(MouseButtonEvent evt)
		{
			base.OnButtonRelease(evt);

			// determine the 3D position of the hit
			viewport.Camera.Place();
			HitLine hitLine = evt.HitLine;

			// TODO: handle multiple hits with depth checking
			Renderable3D hitRend = null;
			foreach (Renderable3D rend in renderList.Renderables)
			{
				rend.OnButtonPress(evt);
				if (rend.HitTest(hitLine))
					hitRend = rend;
			}

			// show the selection tooltip
			if (hitRend != null)
			{
				string description = hitRend.SelectionDescription;
				if (description.Length > 0)
				{
					//toolTip.SetToolTip(this, description);
					Console.WriteLine("tooltip: {0}", description);
				}
			}
		}

		public override void OnMouseMotion(MouseEvent evt)
		{
			base.OnMouseMotion(evt);
		}

		public override void OnMouseWheel(MouseWheelEvent evt)
		{
			base.OnMouseWheel(evt);
		}

	}
}
