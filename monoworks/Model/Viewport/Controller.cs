// Controller.cs - MonoWorks Project
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

using MonoWorks.Framework;
using MonoWorks.Rendering;
using MonoWorks.Rendering.Viewport;
using MonoWorks.Rendering.Controls;

namespace MonoWorks.Model.Viewport
{
	/// <summary>
	/// Controller for a Model viewport.
	/// </summary>
	public class Controller : ViewportController
	{
		public Controller(IViewport viewport)
			: base(viewport)
		{
			UiManager.LoadStream(ResourceHelper.GetStream("Viewport.ui"));
			OnSolidModeChanged();
		}


#region Shading Actions

		
		protected readonly Dictionary<SolidMode, string> solidModeNames = new Dictionary<SolidMode, string>
		{{SolidMode.None,"No Solid"}, {SolidMode.Flat,"Flat Shaded"}, {SolidMode.Smooth,"Smooth Shaded"}};
		
		
		[Action("Wireframe")]
		public void OnWireframe()
		{
			if (UiManager.HasToolbar("Shading"))
			{
				ToolBar toolbar = UiManager.GetToolbar("Shading");
				viewport.RenderManager.ShowWireframe = toolbar.GetButton("Wireframe").IsSelected;
			}
		}

		[Action("No Solid")]
		public void OnNoSolid()
		{
			viewport.RenderManager.SolidMode = SolidMode.None;
			OnSolidModeChanged();
		}

		[Action("Flat Shaded")]
		public void OnFlatShaded()
		{
			viewport.RenderManager.SolidMode = SolidMode.Flat;
			OnSolidModeChanged();
		}

		[Action("Smooth Shaded")]
		public void OnSmoothShaded()
		{
			viewport.RenderManager.SolidMode = SolidMode.Smooth;
			OnSolidModeChanged();
		}

		/// <summary>
		/// Updates the controls based on a new solid rendering mode.
		/// </summary>
		public void OnSolidModeChanged()
		{
			if (UiManager.HasToolbar("Shading"))
			{
				ToolBar toolbar = UiManager.GetToolbar("Shading");
				string solidString = solidModeNames[viewport.RenderManager.SolidMode];
				foreach (Button button in toolbar)
				{
					if (button.LabelString == solidString)
						button.IsSelected = true;
					else if (button.LabelString != "Wireframe") // don't touch the wireframe button
						button.IsSelected = false;
				}
			}
		}

#endregion



	}
}
