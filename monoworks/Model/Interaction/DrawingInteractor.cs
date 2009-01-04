// ModelInteractor.cs - MonoWorks Project
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

namespace MonoWorks.Model.Interaction
{
	
	/// <summary>
	/// Interactor for top level model entities (features, reference entities, sketches).
	/// </summary>
	public class DrawingInteractor : AbstractInteractor
	{
		
		public DrawingInteractor(IViewport viewport) : base(viewport)
		{
		}
		
		public override void OnButtonPress (MouseButtonEvent evt)
		{
			base.OnButtonPress(evt);
		}

		
		public override void OnButtonRelease(MouseEvent evt)
		{
			base.OnButtonRelease(evt);
			
			HitLine hitLine = viewport.Camera.ScreenToWorld(evt.Pos);

			// gather a list of entities that were hit
			List<Entity> hits = new List<Entity>();
			foreach (Renderable3D renderable in renderList.Renderables)
			{
				if (renderable is Entity) // only look at top-level entities
				{
					if (renderable.HitTest(hitLine))
					{
						hits.Add(renderable as Entity);
						Console.WriteLine("entity {0} was hit", (renderable as Entity).Name);
					}
				}
			}
			
			
		}
		
		
		public override void OnMouseMotion(MouseEvent evt)
		{
			base.OnMouseMotion(evt);
		}


	}
}
