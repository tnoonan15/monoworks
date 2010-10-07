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
using MonoWorks.Controls;

namespace MonoWorks.Plotting
{
	/// <summary>
	/// Interactor for plots.
	/// </summary>
	public class PlotInteractor : GenericInteractor<Scene>
	{

		public PlotInteractor(Scene scene)
			: base(scene)
		{

		}


		public override void OnButtonPress(MouseButtonEvent evt)
		{
			base.OnButtonPress(evt);
	
			// handle double click
			if (!evt.IsHandled && evt.Button == 1) // unhandled left click
			{
				if (evt.Multiplicity == ClickMultiplicity.Double) //  double click
				{
					if (Scene.Use2dInteraction)
					{
						Scene.RenderList.ResetBounds();
						Scene.Resize();
						evt.Handle(this);
					}
				}
				else if (evt.Multiplicity == ClickMultiplicity.Single) // single click
				{
					// TODO: Plotting - handle multiple hits with depth checking
					Actor hitRend = null;
					foreach (Actor rend in RenderList.Actors)
					{
						rend.OnButtonRelease(evt);
						if (evt.IsHandled)
							hitRend = rend;
					}

					// show the selection tooltip
					if (hitRend != null)
					{
						string description = hitRend.SelectionDescription;
						if (description.Length > 0)
							Scene.SetToolTip(description, false);
						evt.Handle(this);
					}
					else
						Scene.ClearToolTip();
				} 
				// handle the event anyway, we don't want the view interactor to do anything
				//evt.Handle(this);
			} // button 1
		}

		public override void OnButtonRelease(MouseButtonEvent evt)
		{
			if (evt.IsHandled)
				return;
	
			base.OnButtonRelease(evt);
	
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
