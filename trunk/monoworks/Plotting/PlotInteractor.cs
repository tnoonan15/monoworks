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

			// handle double click
			if (!evt.Handled && evt.Multiplicity == ClickMultiplicity.Double)
			{
				if (viewport.Use2dInteraction)
				{
					viewport.RenderList.ResetBounds();
					viewport.Resize();
					evt.Handle();
				}
			}
		}

		public override void OnButtonRelease(MouseButtonEvent evt)
		{
			if (evt.Handled)
				return;

			base.OnButtonRelease(evt);


			if (evt.Button == 1)
			{
				// TODO: handle multiple hits with depth checking
				Actor hitRend = null;
				foreach (Actor rend in renderList.Actors)
				{
					rend.OnButtonRelease(evt);
					if (evt.Handled)
						hitRend = rend;
				}

				// show the selection tooltip
				if (hitRend != null)
				{
					string description = hitRend.SelectionDescription;
					if (description.Length > 0)
						viewport.ToolTip = description;
					evt.Handle();
				}
				else
					viewport.ClearToolTip();
			} // button 1
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
