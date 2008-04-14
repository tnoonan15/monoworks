// TreeWidgetItem.cs - MonoWorks Project
//
// Copyright (C) 2008 Andy Selvig
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA

using System;

using Qyoto;

using MonoWorks.Model;

namespace MonoWorks.Gui
{
	
	/// <summary>
	/// The TreeWidgetItem is subclasses from QTreeWidgetItem.
	/// It represents a single entity in the TreeWidget.
	/// </summary>
	public class TreeWidgetItem : QTreeWidgetItem
	{
		
		public TreeWidgetItem(TreeWidget treeWidget, Entity entity) : base(treeWidget)
		{
			this.Entity = entity;
		}
		
		public TreeWidgetItem(TreeWidgetItem parentItem, Entity entity) : base(parentItem)
		{
			this.Entity = entity;
		}
		
		
#region The Entity
		
		private Entity entity;
		/// <value>
		/// The entity represented by this item.
		/// </value>
		public Entity Entity
		{
			get {return entity;}
			set
			{
				entity = value;
				Refresh();
			}
		}
		
#endregion
		
		
#region Display

		/// <summary>
		/// Refresh the item's attributes based on the entity.
		/// </summary>
		public virtual void Refresh()
		{
			SetText(0, entity.Name);
			SetIcon(0, ResourceManager.GetIcon("block"));
		}
		
#endregion
		
	}
}
