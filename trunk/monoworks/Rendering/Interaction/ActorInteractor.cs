// ActorInteractor.cs - MonoWorks Project
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

using MonoWorks.Base;
using MonoWorks.Rendering.Events;

namespace MonoWorks.Rendering.Interaction
{

	/// <summary>
	/// Provides a simple implementation of AbstractInteractor that passes 
	/// interaction events to all actors in the viewport.
	/// </summary>
	/// <remarks>
	/// For most usages of the MonoWorks rendering library, it will likely
	/// be necessary to provide a custom implementation. However, this can 
	/// be used for simple situations and as a demonstration of how the 
	/// interaction system works.
	/// </remarks>
	public class ActorInteractor : AbstractInteractor
	{
		/// <summary>
		/// Default constructor that takes the viewport.
		/// </summary>
		public ActorInteractor(Viewport viewport) : base(viewport)
		{
			
		}
		
		


		public override void OnButtonPress(MouseButtonEvent evt)
		{
			base.OnButtonPress(evt);

			// don't interact if modal overlays are present
			if (viewport.RenderList.ModalCount > 0)
				return;
			
			foreach (var actor in viewport.RenderList.ActorCopy)
				actor.OnButtonPress(evt);
		}


		public override void OnButtonRelease(MouseButtonEvent evt)
		{
			base.OnButtonRelease(evt);

			// don't interact if modal overlays are present
			if (viewport.RenderList.ModalCount > 0)
				return;
			
			foreach (var actor in viewport.RenderList.ActorCopy)
				actor.OnButtonRelease(evt);
		}


		public override void OnMouseMotion(MouseEvent evt)
		{
			base.OnMouseMotion(evt);

			// don't interact if modal overlays are present
			if (viewport.RenderList.ModalCount > 0)
				return;
			
			foreach (var actor in viewport.RenderList.ActorCopy)
				actor.OnMouseMotion(evt);
		}
		
		
	}
}
