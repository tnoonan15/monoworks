using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Linq;

using MonoWorks.Modeling;

namespace MonoWorks.GuiWpf.Tree
{
	/// <summary>
	/// Tree view.
	/// </summary>
	public class EntityTreeView : TreeView, IEntityListener, ISelectionListener
	{

		/// <summary>
		/// Default constructor.
		/// </summary>
		public EntityTreeView()
			: base()
		{
		}

		protected Drawing drawing;

		public Drawing Drawing
		{
			get { return drawing; }
			set
			{
				this.drawing = value;
				drawing.EntityManager.RegisterEntityListener(this);
				drawing.EntityManager.RegisterSelectionListener(this);
				GenerateItems();
			}
		}

		/// <summary>
		/// Clears and regenerats the entire tree.
		/// </summary>
		public void GenerateItems()
		{
			Items.Clear();

			foreach (Entity entity in drawing.Children)
				AddEntity(entity);
		}


		protected Dictionary<Entity, EntityTreeItem> items = new Dictionary<Entity,EntityTreeItem>();


		/// <summary>
		/// Adds an entity to the tree.
		/// </summary>
		public void AddEntity(Entity entity)
		{
			//Console.WriteLine("tree add entity {0}", entity.Name);

			if (entity.Parent != null && items.ContainsKey(entity.Parent))
				AddEntity(entity, GetItem(entity.Parent));
			else
				AddEntity(entity, null);
		}

		/// <summary>
		/// Adds an entity to the tree with the given parent.
		/// </summary>
		protected void AddEntity(Entity entity, EntityTreeItem parent)
		{
			if (items.ContainsKey(entity))
				RemoveEntity(entity);

			EntityTreeItem item = new EntityTreeItem(entity, parent);
			if (parent == null)
				Items.Add(item);
			else
			{
				parent.Items.Add(item);
				parent.IsExpanded = true;
			}
			items[entity] = item;

			item.Selected += OnItemSelected;
			item.Unselected += OnItemDeselected;

			// add the children
			foreach (Entity child in entity.Children)
				AddEntity(child, item);
		}

		/// <summary>
		/// Removes an entity from the tree.
		/// </summary>
		public void RemoveEntity(Entity entity)
		{
			EntityTreeItem item = GetItem(entity);
			if (item != null)
			{
				if (item.ParentItem == null)
					Items.Remove(item);
				else
					item.ParentItem.Items.Remove(item);
				items.Remove(entity);
			}
		}

		/// <summary>
		/// Gets the item associated with the entity.
		/// </summary>
		protected EntityTreeItem GetItem(Entity entity)
		{
			EntityTreeItem item = null;
			items.TryGetValue(entity, out item);
			return item;
		}

		/// <summary>
		/// Handles an item on the tree being selected.
		/// </summary>
		void OnItemSelected(object sender, System.Windows.RoutedEventArgs e)
		{
			if (internalUpdate)
				return;

			EntityTreeItem item = sender as EntityTreeItem;

			// see if any of the children are selected
			bool childSelected = false;
			foreach (var child in item.Items)
			{
				if ((child as EntityTreeItem).IsSelected)
				{
					childSelected = true;
					break;
				}
			}

			//Console.WriteLine("tree item {0} selected, child selected? {1}", item.Entity.Name, childSelected);
			if (!childSelected)
				drawing.EntityManager.Select(this, item.Entity);
		}

		/// <summary>
		/// Handles an item on the tree being deselected.
		/// </summary>
		void OnItemDeselected(object sender, System.Windows.RoutedEventArgs e)
		{
			if (internalUpdate)
				return;

			EntityTreeItem item = sender as EntityTreeItem;
			drawing.EntityManager.Deselect(this, item.Entity);
		}




#region ISelectionListener Members

		protected bool internalUpdate = false;

		/// <summary>
		/// Start an internal update.
		/// </summary>
		protected void BeginInternalUpdate()
		{
			internalUpdate = true;
		}

		/// <summary>
		/// Ends an internal update.
		/// </summary>
		protected void EndInternalUpdate()
		{
			internalUpdate = false;
		}

		public void OnSelect(Entity entity)
		{
			BeginInternalUpdate();
			EntityTreeItem item = items[entity];
			item.IsSelected = true;
			//Console.WriteLine("tree select entity {0}", entity.Name);
			EndInternalUpdate();
		}

		public void OnDeselect(Entity entity)
		{
			BeginInternalUpdate();
			EntityTreeItem item = items[entity];
			item.IsSelected = false;
			//Console.WriteLine("tree deselect entity {0}", entity.Name);
			EndInternalUpdate();
		}

		public void OnSelectAll()
		{
			BeginInternalUpdate();
			foreach (EntityTreeItem item in items.Values)
				item.IsSelected = true;
			EndInternalUpdate();
		}

		public void OnDeselectAll()
		{
			BeginInternalUpdate();
			foreach (EntityTreeItem item in items.Values)
				item.IsSelected = false;
			EndInternalUpdate();
		}

#endregion

	}
}
