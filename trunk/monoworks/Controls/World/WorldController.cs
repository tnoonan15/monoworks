// WorldController.cs - MonoWorks Project
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

namespace MonoWorks.Controls.World
{
    /// <summary>
    /// Implements a controller for a "standard" scene.
    /// </summary>
    public class WorldController<SceneType> : AbstractController<SceneType> where SceneType : Scene
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="scene">The scene that this controller controls.</param>
        public WorldController(SceneType scene)
            : base(scene)
        {
        	Scene.Camera.ProjectionChanged += ExternalProjectionChanged;

			Mwx.Parse(ResourceHelper.GetStream("World.mwx"));
   
			_worldInfoLabel = Mwx.Get<Label>("WorldInfoLabel");

        	ContextLayer = new ContextLayer();
			Scene.RenderList.AddOverlay(ContextLayer);
        }

		/// <summary>
		/// The context layer containing all of the toolbars.
		/// </summary>
		public ContextLayer ContextLayer { get; private set;}
		
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
		
		
		#region World Info
		
		private Label _worldInfoLabel;

		private bool _showWorldInfo;
		/// <summary>
		/// Whether or not to show the world information in the bottom right corner. 
		/// </summary>
		public bool ShowWorldInfo {
			set {
				_showWorldInfo = value;
				if (_showWorldInfo) {
					ContextLayer.AnchorControl(_worldInfoLabel, AnchorLocation.SE);
					Scene.Rendered += OnSceneRendered;
				}
				else
				{
					Scene.Rendered -= OnSceneRendered;
				}
			}
		}

		private DateTime _lastRenderTime = DateTime.Now;
		
		private void OnSceneRendered(object sender, SceneRenderEvent evt)
		{
			var now = DateTime.Now;
			var dif = now - _lastRenderTime;
			var frameRate = 1000 / dif.TotalMilliseconds;
			_worldInfoLabel.Body = String.Format("{0} x {1} at {2:##.#} fps", Scene.Width, Scene.Height, frameRate);
			_lastRenderTime = now;
		}
		
		#endregion
		
				
		#region View Direction Actions		
		
		[ActionHandler("Standard View")]
		public void OnStandardView(object sender, EventArgs args)
		{
			Scene.RenderList.ResetBounds();
			Scene.Camera.AnimateTo(ViewDirection.Standard);
		}
		
		[ActionHandler("Front View")]
		public void OnFrontView(object sender, EventArgs args)
		{
			Scene.RenderList.ResetBounds();
			Scene.Camera.AnimateTo(ViewDirection.Front);
		}
		
		[ActionHandler("Back View")]
		public void OnBackView(object sender, EventArgs args)
		{
			Scene.RenderList.ResetBounds();
			Scene.Camera.AnimateTo(ViewDirection.Back);
		}
		
		[ActionHandler("Left View")]
		public void OnLeftView(object sender, EventArgs args)
		{
			Scene.RenderList.ResetBounds();
			Scene.Camera.AnimateTo(ViewDirection.Left);
		}
		
		[ActionHandler("Right View")]
		public void OnRightView(object sender, EventArgs args)
		{
			Scene.RenderList.ResetBounds();
			Scene.Camera.AnimateTo(ViewDirection.Right);
		}
		
		[ActionHandler("Top View")]
		public void OnTopView(object sender, EventArgs args)
		{
			Scene.RenderList.ResetBounds();
			Scene.Camera.AnimateTo(ViewDirection.Top);
		}
		
		[ActionHandler("Bottom View")]
		public void OnBottomView(object sender, EventArgs args)
		{
			Scene.RenderList.ResetBounds();
			Scene.Camera.AnimateTo(ViewDirection.Bottom);
		}
		
		#endregion
		
		
		#region Projection Actions

		[ActionHandler("Projection")]
		public void OnChangeProjection(object sender, EventArgs args)
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
