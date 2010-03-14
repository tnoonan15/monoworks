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

using MonoWorks.Rendering;
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
			HitStateChanged += OnHitStateChanged;
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
				_entity.HitStateChanged += OnEntityHitStateChanged;
				Refresh();
			}
		}

		/// <summary>
		/// Handles the entity's HitStateChanged event.
		/// </summary>
		private void OnEntityHitStateChanged(object sender, HitStateChangedEvent evt)
		{
			if (sender != this)
			{
				Console.WriteLine("{0} changed hitstate from {1} to {2} with sender {3}", 
					Entity.Name, evt.OldValue, evt.NewValue, sender);
				SetHitState(this, evt.NewValue);
			}
		}
		
		/// <summary>
		/// Refreshes the item based on the 
		/// </summary>
		public void Refresh()
		{
			Text = Entity.Name;
			IconName = Entity.ClassName.ToLower();
			Clear();
			foreach (var child in Entity.Children)
			{
				AddChild(new EntityTreeItem(child));
			}
		}
		
		
		private void OnHitStateChanged(object sender, HitStateChangedEvent evt)
		{
			Console.WriteLine("tree item for {0} changed from {1} to {2} with sender {3}", 
				Entity.Name, evt.OldValue, evt.NewValue, sender);
			
			if (Entity != null && sender == this)
				Entity.SetHitState(this, evt.NewValue);
		}
		
	}
}

