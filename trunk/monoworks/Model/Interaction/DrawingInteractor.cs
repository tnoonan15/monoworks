// DrawingInteractor.cs - MonoWorks Project
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

using MonoWorks.Rendering;
using MonoWorks.Rendering.Events;
using MonoWorks.Rendering.Interaction;

namespace MonoWorks.Model.Interaction
{
	
	/// <summary>
	/// Interactor for top level drawing entities (features, reference entities, sketches).
	/// </summary>
	public class DrawingInteractor : AbstractInteractor
	{
		
		public DrawingInteractor(Viewport viewport, Drawing drawing) : base(viewport)
		{
			this.drawing = drawing;
		}
		
		protected Drawing drawing;
		
		public override void OnButtonPress (MouseButtonEvent evt)
		{
			base.OnButtonPress(evt);
		}

		
		public override void OnButtonRelease(MouseButtonEvent evt)
		{
			base.OnButtonRelease(evt);

			if (evt.Handled || evt.Button != 1)
				return;

			// deselect everything, if necessary
			if (evt.Modifier != InteractionModifier.Shift)
			{
				drawing.EntityManager.DeselectAll(null);
			}
			
			Entity hitEntity = HitEntity(evt);
			
			if (hitEntity != null)
			{
				drawing.EntityManager.Select(null, hitEntity);
				evt.Handle();
			}
		}
		
		
		public override void OnMouseMotion(MouseEvent evt)
		{
			base.OnMouseMotion(evt);

			foreach (Entity entity in drawing.Children)
				entity.IsHovering = false;

			if (evt.Handled)
				return;
			
			
			Entity hitEntity = HitEntity(evt);
			if (hitEntity != null)
				hitEntity.IsHovering = true;
		}
		
		/// <summary>
		/// Determines which, if any, entity was hit.
		/// </summary>
		protected Entity HitEntity(MouseEvent evt)
		{
			
			HitLine hitLine = viewport.Camera.ScreenToWorld(evt.Pos);

			// gather a list of entities that were hit
			List<Entity> hits = new List<Entity>();
			foreach (Entity entity in drawing.Children)
			{
				if (entity.HitTest(hitLine))
					hits.Add(entity);
			}
			
			// perform depth test
			Entity front = null;
			double frontDist = 0;
			foreach (Entity entity in hits)
			{
				//double dist_ = viewport.Camera.GetDistance(entity.LastHit);
				double dist_ = viewport.Camera.GetDistance(entity.Bounds.Center);
				if (front == null || dist_ < frontDist)
				{
					front = entity;
					frontDist = dist_;
				}
			}
			return front;
		}


	}
}
