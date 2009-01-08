// ViewportController.cs - MonoWorks Project
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
using MonoWorks.Rendering.Controls;


namespace MonoWorks.Rendering.Viewport
{
    /// <summary>
    /// Implements a Framework controller for a viewport.
    /// </summary>
    public class ViewportController : AbstractController
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="viewport">The viewport that this controller controls.</param>
        public ViewportController(IViewport viewport)
            : base()
        {
			this.viewport = viewport;
			UiManager = new UiManager(this);
			UiManager.LoadStream(ResourceHelper.GetStream("Viewport.ui"));
			
			// add the context layer
			ContextLayer = UiManager.CreateContextLayer();
			viewport.RenderList.AddOverlay(ContextLayer);
        }

        protected IViewport viewport;

		/// <summary>
		/// The UiManager used by this controller.
		/// </summary>
		public UiManager UiManager { get; set; }

		/// <summary>
		/// The context layer containing all of the toolbars.
		/// </summary>
		public ContextLayer ContextLayer { get; private set; }

		/// <summary>
		/// Sets the usage and resets the context layer appropriately.
		/// </summary>
		/// <param name="usage"></param>
		public void SetUsage(ViewportUsage usage)
		{
			ContextLayer.ClearAllContexts();
			switch (usage)
			{
				case ViewportUsage.CAD:
					ContextLayer.AddContext(ContextLocation.N, "CadView");
					break;
				case ViewportUsage.Plotting:
					ContextLayer.AddContext(ContextLocation.N, "PlotView");
					break;
			}
		}

		
#region View Actions
		
		[Action("Standard View")]
		public void OnStandardView()
		{
			Console.WriteLine("standard");
			viewport.Camera.SetViewDirection(ViewDirection.Standard);
		}
		
		[Action("Front View")]
		public void OnFrontView()
		{
			Console.WriteLine("front");
			viewport.Camera.SetViewDirection(ViewDirection.Front);
		}
		
		[Action("Back View")]
		public void OnBackView()
		{
			Console.WriteLine("back");
			viewport.Camera.SetViewDirection(ViewDirection.Back);
		}
		
		[Action("Left View")]
		public void OnLeftView()
		{
			Console.WriteLine("left");
			viewport.Camera.SetViewDirection(ViewDirection.Left);
		}
		
		[Action("Right View")]
		public void OnRightView()
		{
			Console.WriteLine("right");
			viewport.Camera.SetViewDirection(ViewDirection.Right);
		}
		
		[Action("Top View")]
		public void OnTopView()
		{
			Console.WriteLine("top");
			viewport.Camera.SetViewDirection(ViewDirection.Top);
		}
		
		[Action("Bottom View")]
		public void OnBottomView()
		{
			Console.WriteLine("bottom");
			viewport.Camera.SetViewDirection(ViewDirection.Bottom);
		}
		
#endregion
		
    }
}
