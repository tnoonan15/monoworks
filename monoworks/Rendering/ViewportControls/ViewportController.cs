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


namespace MonoWorks.Rendering.ViewportControls
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
        public ViewportController(Viewport viewport)
            : base()
        {
			this.viewport = viewport;
			viewport.InteractionStateChanged += ExternalInteractionStateChanged;
			viewport.Camera.ProjectionChanged += ExternalProjectionChanged;

			ResourceManagerBase.LoadAssembly("MonoWorks.Rendering");

			UiManager = new UiManager(this);
			UiManager.LoadStream(ResourceHelper.GetStream("Viewport.ui"));

			viewport.RenderList.AddOverlay(UiManager.ContextLayer);
			
			// make the interaction buttons
			interactionButtons = new CornerButtons(Corner.NW);
			interactionButtons.Image1 = new Image(ResourceHelper.GetStream("3d-interact.png"));
			interactionButtons.Action1 += OnEnablePrimaryInteractor;
			interactionButtons.Image2 = new Image(ResourceHelper.GetStream("3d-view.png"));
			interactionButtons.Action2 += OnDisablePrimaryInteractor;
			interactionButtons.IsTogglable = true;
			viewport.RenderList.AddOverlay(interactionButtons);
        }

        protected Viewport viewport;

		/// <summary>
		/// The UiManager used by this controller.
		/// </summary>
		public UiManager UiManager { get; set; }

		/// <summary>
		/// The context layer containing all of the toolbars.
		/// </summary>
		public ContextLayer ContextLayer
		{
			get { return UiManager.ContextLayer; }
		}

		/// <summary>
		/// Loads the standard (View and Interaction) toolbars from the UiManager.
		/// </summary>
		protected void LoadStandardToolbars()
		{
			ContextLayer.AddContext(ContextLocation.N, "View");
//			ContextLayer.AddContext(ContextLocation.N, "Interaction");
			ContextLayer.AddContext(ContextLocation.N, "Export");
			OnProjectionChanged();
			OnInteractionStateChanged();
		}
		
#region View Direction Actions
		
		
		[Action("Standard View")]
		public void OnStandardView()
		{
			viewport.RenderList.ResetBounds();
			viewport.Camera.AnimateTo(ViewDirection.Standard);
		}
		
		[Action("Front View")]
		public void OnFrontView()
		{
			viewport.RenderList.ResetBounds();
			viewport.Camera.AnimateTo(ViewDirection.Front);
		}
		
		[Action("Back View")]
		public void OnBackView()
		{
			viewport.RenderList.ResetBounds();
			viewport.Camera.AnimateTo(ViewDirection.Back);
		}
		
		[Action("Left View")]
		public void OnLeftView()
		{
			viewport.RenderList.ResetBounds();
			viewport.Camera.AnimateTo(ViewDirection.Left);
		}
		
		[Action("Right View")]
		public void OnRightView()
		{
			viewport.RenderList.ResetBounds();
			viewport.Camera.AnimateTo(ViewDirection.Right);
		}
		
		[Action("Top View")]
		public void OnTopView()
		{
			viewport.RenderList.ResetBounds();
			viewport.Camera.AnimateTo(ViewDirection.Top);
		}
		
		[Action("Bottom View")]
		public void OnBottomView()
		{
			viewport.RenderList.ResetBounds();
			viewport.Camera.AnimateTo(ViewDirection.Bottom);
		}
		
#endregion


#region Projection Actions


		[Action("Projection")]
		public void OnChangeProjection()
		{
			viewport.Camera.ToggleProjection();
			OnProjectionChanged();
		}

		/// <summary>
		/// Updates the projection button based on the current projection.
		/// </summary>
		public void OnProjectionChanged()
		{
			ToolBar toolbar = UiManager.GetToolbar("View");
			if (toolbar != null)
			{
				Button projButton = toolbar.GetButton("Projection");
				projButton.IsSelected = viewport.Camera.Projection == Projection.Perspective;
			}
		}

		/// <summary>
		/// Handles the projection changing from an external source.
		/// </summary>
		private void ExternalProjectionChanged(object sender, EventArgs args)
		{
			OnProjectionChanged();
		}

#endregion


#region Interaction Actions

		/// <summary>
		/// The buttons in the upper left corner that allow the user to select whether or not to use th primary interactor. 
		/// </summary>
		protected CornerButtons interactionButtons;
		
		/// <summary>
		/// The name of each interaction state.
		/// </summary>
//		protected readonly Dictionary<InteractionState, string> interactionNames = new Dictionary<InteractionState, string>{
//			{InteractionState.Interact2D, "2D Interaction"}, 
//			{InteractionState.Interact3D, "3D Interaction"},
//			{InteractionState.View3D, "3D View"}};


		/// <summary>
		/// Handles the interaction state changing from an external source.
		/// </summary>
		protected virtual void ExternalInteractionStateChanged(object sender, EventArgs args)
		{
			if (!InternalUpdate)
				OnInteractionStateChanged();
		}

		
//		[Action("2D Interaction")]
//		public void On2dInteract()
//		{
//			BeginInternalUpdate();
//			viewport.Camera.SetViewDirection(ViewDirection.Front);
//			viewport.Camera.Projection = Projection.Parallel;
//			viewport.SetInteractionState(InteractionState.Interact2D);
//		 	OnInteractionStateChanged();
//			EndInternalUpdate();
//		}
		
//		[Action("3D Interaction")]
//		public void On3dInteract()
//		{
//			BeginInternalUpdate();
//			viewport.UsePrimaryInteractor = true;
//			OnInteractionStateChanged();
//			EndInternalUpdate();
//		}
//		
//		[Action("3D View")]
//		public void On3dView()
//		{
//			BeginInternalUpdate();
//			viewport.UsePrimaryInteractor = false;
//			OnInteractionStateChanged();
//			EndInternalUpdate();
//		}	

		/// <summary>
		/// Enables the primary interactor in the viewport. 
		/// </summary>
        void OnEnablePrimaryInteractor(object sender, EventArgs e)
        {
			BeginInternalUpdate();
        	viewport.UsePrimaryInteractor = true;
			EndInternalUpdate();
        }

		/// <summary>
		/// Disables the primary interactor in the viewport. 
		/// </summary>
        void OnDisablePrimaryInteractor(object sender, EventArgs e)
        {
			BeginInternalUpdate();
        	viewport.UsePrimaryInteractor = false;
			EndInternalUpdate();
        }
		
		/// <summary>
		/// Updates the controls after the interaction state has changed.
		/// </summary>
		public void OnInteractionStateChanged()
		{	
			if (InternalUpdate)
				return;
			if (viewport.UsePrimaryInteractor)
				interactionButtons.SelectedRegion = CornerButtons.Region.Button1;
			else
				interactionButtons.SelectedRegion = CornerButtons.Region.Button2;
		}
		
#endregion


#region Exporting

		/// <summary>
		/// Exports the viewport to a file, prompting the user for the file location.
		/// </summary>
		[Action("Export")]
		public void OnExport()
		{
			viewport.Export();
		}

#endregion

	}
}
