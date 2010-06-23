// 
//  SingleActorInteractor.cs - MonoWorks Project
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

using MonoWorks.Base;
using MonoWorks.Rendering.Events;


namespace MonoWorks.Rendering.Interaction
{
	/// <summary>
	/// Interactor that interacts with a single (generic) actor.
	/// </summary>
	/// <remarks>Unlike ActorInteractor, which acts on all actors in the scene, 
	/// a SingleActorInteractor is meant to act on one actor in the scene at a time.</remarks>
	public class SingleActorInteractor<ActorType> : AbstractInteractor where ActorType : Actor
	{
		public SingleActorInteractor(Scene scene) : base(scene)
		{
		}
		
		public SingleActorInteractor(Scene scene, ActorType actor) : base(scene)
		{
			Actor = actor;
		}		
		
		/// <summary>
		/// The actor being acted upon with this interactor.
		/// </summary>
		public ActorType Actor { get; set; }
		
		
		#region Interaction
		
		public override void OnButtonPress(MouseButtonEvent evt)
		{
			base.OnButtonPress(evt);
			
			if (Actor != null)
				Actor.OnButtonPress(evt);
		}
		
		public override void OnButtonRelease(MouseButtonEvent evt)
		{
			base.OnButtonRelease(evt);
			
			if (Actor != null)
				Actor.OnButtonRelease(evt);
		}

		public override void OnMouseMotion(MouseEvent evt)
		{
			base.OnMouseMotion(evt);
			
			if (Actor != null)
				Actor.OnMouseMotion(evt);
		}
		
		public override void OnMouseWheel(MouseWheelEvent evt)
		{
			base.OnMouseWheel(evt);
			
			if (Actor != null)
				Actor.OnMouseWheel(evt);
		}
		
		public override void OnKeyPress(KeyEvent evt)
		{			
			if (Actor != null)
				Actor.OnKeyPress(evt);
		}
		
		public override void OnKeyRelease(KeyEvent evt)
		{
			if (Actor != null)
				Actor.OnKeyRelease(evt);			
		}
		
		#endregion
		
	}
}

