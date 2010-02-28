// SceneController.cs - MonoWorks Project
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

using MonoWorks.Base;
using MonoWorks.Rendering;
using MonoWorks.Controls;
using MonoWorks.Rendering.Interaction;

namespace MonoWorks.Controls.StandardScene
{
    /// <summary>
    /// Implements a controller for a "standard" scene.
    /// </summary>
    public class StandardSceneController : AbstractController<Scene>
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="scene">The scene that this controller controls.</param>
        public StandardSceneController(Scene scene)
            : base(scene)
        {
        	Scene.Camera.ProjectionChanged += ExternalProjectionChanged;

//			Mwx.Parse(ResourceHelper.GetStream("Scene.ui"));

        	ContextLayer = new ContextLayer();
			Scene.RenderList.AddOverlay(ContextLayer);
        }

		/// <summary>
		/// The context layer containing all of the toolbars.
		/// </summary>
		public ContextLayer ContextLayer { get; private set;}

		/// <summary>
		/// Loads the standard (View and Interaction) toolbars from the UiManager.
		/// </summary>
		protected void LoadStandardToolbars()
		{
			Context(Side.N, "View");
			//			ContextLayer.AddContext(Side.N, "Interaction");
			Context(Side.N, "Export");
			OnProjectionChanged();
		//			OnInteractionStateChanged();
		}
		
		/// <summary>
		/// Loads the control with the given name into the context layer at the given location. 
		/// </summary>
		public void Context(Side loc, string name)
		{
			var obj = Mwx.Get(name);
			if (obj is Control2D)
				ContextLayer.AddContext(loc, obj as Control2D);
			else
				throw new Exception("Context " + name + " is not a control!");
		}
		
#region View Direction Actions
		
		
		[ActionHandler("Standard View")]
		public void OnStandardView()
		{
			Scene.RenderList.ResetBounds();
			Scene.Camera.AnimateTo(ViewDirection.Standard);
		}
		
		[ActionHandler("Front View")]
		public void OnFrontView()
		{
			Scene.RenderList.ResetBounds();
			Scene.Camera.AnimateTo(ViewDirection.Front);
		}
		
		[ActionHandler("Back View")]
		public void OnBackView()
		{
			Scene.RenderList.ResetBounds();
			Scene.Camera.AnimateTo(ViewDirection.Back);
		}
		
		[ActionHandler("Left View")]
		public void OnLeftView()
		{
			Scene.RenderList.ResetBounds();
			Scene.Camera.AnimateTo(ViewDirection.Left);
		}
		
		[ActionHandler("Right View")]
		public void OnRightView()
		{
			Scene.RenderList.ResetBounds();
			Scene.Camera.AnimateTo(ViewDirection.Right);
		}
		
		[ActionHandler("Top View")]
		public void OnTopView()
		{
			Scene.RenderList.ResetBounds();
			Scene.Camera.AnimateTo(ViewDirection.Top);
		}
		
		[ActionHandler("Bottom View")]
		public void OnBottomView()
		{
			Scene.RenderList.ResetBounds();
			Scene.Camera.AnimateTo(ViewDirection.Bottom);
		}
		
#endregion


#region Projection Actions


		[ActionHandler("Projection")]
		public void OnChangeProjection()
		{
			Scene.Camera.ToggleProjection();
			OnProjectionChanged();
		}

		/// <summary>
		/// Updates the projection button based on the current projection.
		/// </summary>
		public void OnProjectionChanged()
		{
//			ToolBar toolbar = UiManager.GetToolbar("View");
//			if (toolbar != null)
//			{
//				Button projButton = toolbar.GetButton("Projection");
//				projButton.IsSelected = Scene.Camera.Projection == Projection.Perspective;
//			}
		}

		/// <summary>
		/// Handles the projection changing from an external source.
		/// </summary>
		private void ExternalProjectionChanged(object sender, EventArgs args)
		{
			OnProjectionChanged();
		}

#endregion


#region Exporting

		/// <summary>
		/// Exports the scene to a file, prompting the user for the file location.
		/// </summary>
		[ActionHandler("Export")]
		public void OnExport()
		{
//			Scene.Export();
		}

#endregion

	}
}
