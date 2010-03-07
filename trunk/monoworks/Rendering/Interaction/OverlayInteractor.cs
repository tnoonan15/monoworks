// OverlayInteractor.cs - MonoWorks Project
//
//  Copyright (C) 2008 Andy Selvig
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
using MonoWorks.Rendering.Events;

namespace MonoWorks.Rendering.Interaction
{
	/// <summary>
	/// Handles overlay interaction from the viewport.
	/// </summary>
	public class OverlayInteractor : AbstractInteractor
	{

		public OverlayInteractor(Scene scene)
			: base(scene)
		{

		}


		public override void OnButtonPress(MouseButtonEvent evt)
		{
			base.OnButtonPress(evt);
			
			var wasHandled = evt.Handled;

			// let the modals interact first
			if (_scene.RenderList.ModalCount > 0)
			{
				var top = _scene.RenderList.TopModal;
				top.OnButtonPress(evt);
				if (!wasHandled && evt.Handled)
					Current = top;
				evt.Handle();
				return; // don't interact with anything else if modal overlays are present
			}

			foreach (Overlay overlay in renderList.OverlayCopy)
			{
				overlay.OnButtonPress(evt);
				if (!wasHandled && evt.Handled)
				{
					Current = overlay;
					wasHandled = true;
				}
			}
		}


		public override void OnButtonRelease(MouseButtonEvent evt)
		{
			base.OnButtonRelease(evt);

			// let the modals interact first
			if (_scene.RenderList.ModalCount > 0)
			{
				_scene.RenderList.TopModal.OnButtonRelease(evt);
				evt.Handle();
				return; // don't interact with anything else if modal overlays are present
			}

			foreach (Overlay overlay in renderList.OverlayCopy)
				overlay.OnButtonRelease(evt);
		}


		public override void OnMouseMotion(MouseEvent evt)
		{
			base.OnMouseMotion(evt);

			// let the modals interact first
			if (_scene.RenderList.ModalCount > 0)
			{
				_scene.RenderList.TopModal.OnMouseMotion(evt);
				evt.Handle();
				return; // don't interact with anything else if modal overlays are present
			}

			foreach (Overlay overlay in renderList.OverlayCopy)
				overlay.OnMouseMotion(evt);
		}
		
		/// <summary>
		/// The currently hit overlay. 
		/// </summary>
		public Overlay Current { get; set; }
			
		public override void OnKeyPress(KeyEvent evt)
		{
			// let the modals interact first
			if (_scene.RenderList.ModalCount > 0)
			{
				_scene.RenderList.TopModal.OnKeyPress(evt);
				return; // don't interact with anything else if modal overlays are present
			}
			
			if (Current != null)
				Current.OnKeyPress(evt);
		}


	}
}
