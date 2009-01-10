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
using MonoWorks.Rendering.Interaction;


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

		
		protected ViewportUsage viewportUsage = ViewportUsage.Custom;
		
		/// <summary>
		/// Sets the usage and resets the context layer appropriately.
		/// </summary>
		/// <param name="usage"></param>
		public void SetUsage(ViewportUsage usage)
		{
			if (ContextLayer == null) // add the context layer
			{				
				ContextLayer = UiManager.CreateContextLayer();
				viewport.RenderList.AddOverlay(ContextLayer);
			}
			else // the context layer already exists
				ContextLayer.ClearAllContexts();
			switch (usage)
			{
				case ViewportUsage.CAD:
					ContextLayer.AddContext(ContextLocation.N, "CadView");
					ContextLayer.AddContext(ContextLocation.N, "CadInteraction");
					ContextLayer.AddContext(ContextLocation.N, "Shading");
					break;
				case ViewportUsage.Plotting:
					ContextLayer.AddContext(ContextLocation.N, "PlotView");
					ContextLayer.AddContext(ContextLocation.N, "PlotInteraction");
					break;
			}
			viewportUsage = usage;
			
		 	OnInteractionStateChanged();
			OnProjectionChanged();
		}

		
#region View Direction Actions
		
		/// <summary>
		/// The name of the view toolbar on the viewport, or null if there isn't one.
		/// </summary>
		protected string ViewToolbarName
		{
			get				
			{			
				if (viewportUsage == ViewportUsage.CAD)
					return "CadView";
				else if (viewportUsage == ViewportUsage.Plotting)
					return "PlotView";	
				else
					return null;
			}
		}
		
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
		
		[Action("Projection")]
		public void OnChangeProjection()
		{
			Console.WriteLine("projection");
			viewport.Camera.ToggleProjection();
				
		}
		
		/// <summary>
		/// Updates the projection button based on the current projection.
		/// </summary>
		public void OnProjectionChanged()
		{
			if (ViewToolbarName != null)
			{
				ToolBar toolbar = UiManager.GetToolbar(ViewToolbarName);
				Button projButton = toolbar.GetButton("Projection");
				projButton.IsSelected = viewport.Camera.Projection == Projection.Perspective;
			}
		}
		
#endregion
		
		
#region Interaction Actions
		
		/// <summary>
		/// The name of the interaction toolbar on the viewport, or null if there isn't one.
		/// </summary>
		protected string InteractionToolbarName
		{
			get				
			{			
				if (viewportUsage == ViewportUsage.CAD)
					return "CadInteraction";
				else if (viewportUsage == ViewportUsage.Plotting)
					return "PlotInteraction";	
				else
					return null;
			}
		}
		
		protected readonly Dictionary<InteractionState, string> interactionNames = new Dictionary<InteractionState, string>
		{{InteractionState.Interact2D,"2D Interaction"}, {InteractionState.Interact3D,"3D Interaction"}, {InteractionState.View3D,"3D View"}};
		
		
		[Action("2D Interaction")]
		public void On2dInteract()
		{
			viewport.InteractionState = InteractionState.Interact2D;
		 	OnInteractionStateChanged();
		}
		
		[Action("3D Interaction")]
		public void On3dInteract()
		{
			viewport.InteractionState = InteractionState.Interact3D;
		 	OnInteractionStateChanged();
		}
		
		[Action("3D View")]
		public void On3dView()
		{
			viewport.InteractionState = InteractionState.View3D;
		 	OnInteractionStateChanged();
		}	
		
		/// <summary>
		/// Updates the controls after the interaction state has changed.
		/// </summary>
		public void OnInteractionStateChanged()
		{			
			if (InteractionToolbarName != null)
			{
				ToolBar toolbar = UiManager.GetToolbar(InteractionToolbarName);
				string interactionName = interactionNames[viewport.InteractionState];
				foreach (Button button in toolbar)
				{
					if (button.LabelString == interactionName)
						button.IsSelected = true;
					else
						button.IsSelected = false;
					button.MakeDirty();
				}
			}
		}
		
#endregion
		
		
		
    }
}
