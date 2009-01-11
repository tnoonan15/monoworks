using System;
using System.Collections.Generic;
using System.Windows.Controls;

using MonoWorks.Model;

namespace MonoWorks.GuiWpf.Tree
{
	/// <summary>
	/// Tree view item representing an entity.
	/// </summary>
	public class EntityTreeItem : TreeViewItem
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		/// <param name="entity"></param>
		public EntityTreeItem(Entity entity)
		{
			Entity = entity;
			GenerateHeader();
		}

		/// <summary>
		/// The entity this item represents.
		/// </summary>
		public Entity Entity { get; private set; }

		/// <summary>
		/// Generate the display for this item.
		/// </summary>
		public void GenerateHeader()
		{
			Header = Entity.Name;
		}

	}
}
