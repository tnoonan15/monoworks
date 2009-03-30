using System;
using System.Collections.Generic;
using System.Windows.Controls;

using MonoWorks.Modeling;
using MonoWorks.GuiWpf;
using MonoWorks.GuiWpf.Framework;

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
		public EntityTreeItem(Entity entity, EntityTreeItem parent)
		{
			Entity = entity;
			ParentItem = parent;
			GenerateHeader();
		}

		/// <summary>
		/// The entity this item represents.
		/// </summary>
		public Entity Entity { get; private set; }


		/// <summary>
		/// The parent item.
		/// </summary>
		public EntityTreeItem ParentItem { get; private set; }


		/// <summary>
		/// Generate the display for this item.
		/// </summary>
		public void GenerateHeader()
		{
			StackPanel stack = new StackPanel();
			stack.Orientation = Orientation.Horizontal;
			stack.Children.Add(ResourceManager.RenderIcon(Entity.ClassName.ToLower(), 16));
			stack.Children.Add(new Label() { Content = Entity.Name });
			Header = stack;
		}



	}
}
