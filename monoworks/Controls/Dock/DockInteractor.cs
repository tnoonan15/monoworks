// 
//  DockInteractor.cs - MonoWorks Project
//  
//  Author:
//       Andy Selvig <ajselvig@gmail.com>
// 
//  Copyright (c) 2010 Andy Selvig
// 
//  This library is free software; you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as
//  published by the Free Software Foundation; either version 2.1 of the
//  License, or (at your option) any later version.
// 
//  This library is distributed in the hope that it will be useful, but
//  WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
//  Lesser General Public License for more details.
// 
//  You should have received a copy of the GNU Lesser General Public
//  License along with this library; if not, write to the Free Software
//  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MonoWorks.Rendering;
using MonoWorks.Rendering.Interaction;

namespace MonoWorks.Controls.Dock
{
	/// <summary>
	/// Interactor for the DockSpace.
	/// </summary>
	/// <remarks>Probably not useful for much else.</remarks>
	public class DockInteractor : GenericInteractor<DockSpace>
	{

		public DockInteractor(DockSpace scene) : base(scene)
		{
			scene.PreSceneUndocked += new DockEventHandler(OnSceneUndocked);

			_label = new Label();
			_pane = new OverlayPane(_label);
		}

		private Scene _dragScene;

		private Label _label;

		private OverlayPane _pane;

		private DockSlot _slot;

		void OnSceneUndocked(Scene scene)
		{
			_dragScene = scene;
			var container = _dragScene.Parent as SceneContainer;
			if (container == null)
				throw new Exception("Trying to begin dragging a scene that isn't in a container.");
			container.Remove(_dragScene);
			_label.Body = scene.Name;
		}

		public override void OnButtonPress(Rendering.Events.MouseButtonEvent evt)
		{
			base.OnButtonPress(evt);
		}

		public override void OnButtonRelease(Rendering.Events.MouseButtonEvent evt)
		{
			base.OnButtonRelease(evt);

			// decide what to do with the dragging scene
			if (_dragScene != null)
			{
				evt.Handle(this);
				_dragScene = null;
			}
		}

		public override void OnMouseMotion(Rendering.Events.MouseEvent evt)
		{
			base.OnMouseMotion(evt);

			// move the pane to the cursor location
			if (_dragScene != null)
			{
				_pane.Origin = evt.Pos;
				evt.Handle(this);

				_slot = Scene.FindSlot(evt);
			}
		}


		#region Rendering

		public override void RenderOverlay(Scene scene)
		{
 			base.RenderOverlay(scene);

			if (_dragScene != null)
			{
				_pane.RenderOverlay(scene);
				if (_slot != null)
					_slot.RenderOverlay(scene);
			}
			
		}

		#endregion

	}
}
