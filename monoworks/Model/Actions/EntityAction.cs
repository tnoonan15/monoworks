// EntityAction.cs - MonoWorks Project
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



namespace MonoWorks.Model
{

	using EntityList = List<Entity>;
	
	/// <summary>
	/// The EntityAction is an action defining editing of entities.
	/// </summary>
	public class EntityAction : Action
	{
		/// <summary>
		/// Default constructor (single entity).
		/// </summary>
		/// <param name="entity"> The <see cref="Entity"/> that was edited.
		/// </param>
		public EntityAction(Entity entity) : base()
		{
		}
		
		/// <summary>
		/// Default constructor (multiple entities).
		/// </summary>
		/// <param name="entities"> A <see cref="EntityList"/> containing all entities that were edited.
		/// </param>
		public EntityAction(EntityList entities) : base()
		{
		}
	}
}
