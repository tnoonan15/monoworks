using System;
using System.Collections.Generic;
using System.Windows.Controls;

using MonoWorks.Model;

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
			AddEntity(entity, null);
		}

		/// <summary>
		/// Adds an entity to the tree with the given parent.
		/// </summary>
		protected void AddEntity(Entity entity, EntityTreeItem parent)
		{
			EntityTreeItem item = new EntityTreeItem(entity);
			if (parent == null)
				Items.Add(item);
			else
				parent.Items.Add(item);
			items[entity] = item;

			item.Selected += OnItemSelected;
			item.Unselected += OnItemDeselected;

			// add the children
			foreach (Entity child in entity.Children)
				AddEntity(child, item);
		}

		void OnItemSelected(object sender, System.Windows.RoutedEventArgs e)
		{
			if (internalUpdate)
				return;

			//Console.WriteLine("tree item selected");
			EntityTreeItem item = sender as EntityTreeItem;
			drawing.EntityManager.Select(this, item.Entity);
		}

		void OnItemDeselected(object sender, System.Windows.RoutedEventArgs e)
		{
			if (internalUpdate)
				return;

			EntityTreeItem item = sender as EntityTreeItem;
			drawing.EntityManager.Deselect(this, item.Entity);
		}





#region IEntityListener Members

		public void OnEntityAdded(Entity entity)
		{
			throw new NotImplementedException();
		}

		public void OnEntityDeleted(Entity entity)
		{
			throw new NotImplementedException();
		}

#endregion


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
