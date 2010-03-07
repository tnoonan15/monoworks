// 
//  EntityTreeItem.cs - MonoWorks Project
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

using MonoWorks.Controls;

namespace MonoWorks.Modeling
{
	/// <summary>
	/// Tree item for entity tree views.
	/// </summary>
	public class EntityTreeItem : TreeItem
	{
		public EntityTreeItem()
		{
		}
		
		public EntityTreeItem(Entity entity) : this()
		{
			Entity = entity;
		}

		private Entity _entity;
		/// <summary>
		/// The entity that this item represents.
		/// </summary>
		public Entity Entity {
			get {
				return _entity;
			}
			set {
				_entity = value;
				Refresh();
			}
		}
		
		/// <summary>
		/// Refreshes the item based on the 
		/// </summary>
		public void Refresh()
		{
			Text = Entity.Name;
			Clear();
			foreach (var child in Entity.Children)
			{
				AddChild(new EntityTreeItem(child));
			}
		}
	}
}

