// TreeWidget.cs - MonoWorks Project
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
using System.Collections.Generic;

using Qyoto;

using MonoWorks.Model;

namespace MonoWorks.Gui
{
	
	/// <summary>
	/// The TreeWidget class is a subclass of QTreeWidget that provides a tree structure view of the entities.
	/// </summary>
	public class TreeWidget : QTreeWidget
	{
		
		/// <summary>
		/// Default constructor.
		/// </summary>
		/// <param name="parent">The prarent <see cref="QWidget"/>. </param>
		public TreeWidget(QWidget parent, Document document) : base(parent)
		{
			bool isRegistered = QMetaType.IsRegistered(QMetaType.type("QModelIndex"));
			Console.WriteLine("QModelIndex is registered? {0}", isRegistered);
			
			this.ColumnCount = 1;
			this.SetHeaderLabel("Name");
			
			this.Size = new QSize(150, 200);
			
			this.Document = document;
		}
		
		
		protected Document document;
		/// <value>
		/// The document associated with this tree.
		/// </value>
		public Document Document
		{
			get {return document;}
			set
			{
				document = value;
				Refresh();
			}
		}
		
		protected TreeWidgetItem docItem;
		
#region Display
		
		/// <summary>
		/// Refreshes the entire tree.
		/// </summary>
		public virtual void Refresh()
		{
			this.Clear();
			docItem = new TreeWidgetItem(this, document);
//			TreeWidgetItem item = new TreeWidgetItem(this, document);
//			this.AddTopLevelItem(docItem);
//			TreeWidgetItem item;
//			foreach (Entity child in document.Children)
//				item = new TreeWidgetItem(this, child);
//				AddEntity(child, docItem);
			this.Show();
		}
		
		/// <summary>
		/// Adds an entity at the given parent location.
		/// Recursively adds all children of the entity.
		/// </summary>
		/// <param name="entity"> A <see cref="Entity"/> </param>
		/// <param name="parent"> The <see cref="TreeWidgetItem"/> representing the entity's parent. </param>
		public virtual void AddEntity(Entity entity, TreeWidgetItem parent)
		{
			Console.WriteLine("adding child named {0} to the tree", entity.Name);
//			TreeWidgetItem item = new TreeWidgetItem(this, parent.Entity);
//			TreeWidgetItem item = new TreeWidgetItem(parent, parent.Entity);
//			this.AddTopLevelItem(item);
//			parent.InsertChild(parent.Entity.ChildIndex(entity), item);
//			foreach (Entity child in entity.Children)
//			{
//				AddEntity(child, item);
//			}
		}
		
#endregion
		
	}
}
