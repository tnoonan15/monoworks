// TreeModel.cs - MonoWorks Project
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
	/// The TreeModel implements the QAbtractItemModel interface for a document.
	/// It is used by the TreeView to provide a tree representation of the document.
	/// </summary>
	public class TreeModel : QAbstractItemModel
	{
			
//		Dictionary<int, Entity> indexRegistry;
		
		/// <summary>
		/// Default constructor.
		/// </summary>
		public TreeModel(Document document) : base()
		{
//			indexRegistry = new Dictionary<int,Entity>();
			this.document = document;
		}
		
		
		protected Document document;
		/// <value>
		/// The document associated with this model.
		/// </value>
		public Document Document
		{
			get { return document;}
			set {document = value;}
		}
		

#region Model Indexing

		/// <summary>
		/// Gets the entity represented by the given model index.
		/// </summary>
		/// <param name="index"> A <see cref="QModelIndex"/> representing an entity. </param>
		/// <returns>The <see cref="Entity"/> represented by the index. </returns>
		public Entity GetEntity(QModelIndex index)
		{
			long id = ((ModelIndex)index).Id; // get the entity id
			return document.GetEntity(id);
			
//			return indexRegistry[index.GetHashCode()];
			
			// create a list of indices that trace back to the document
//			List<QModelIndex> indices = new List<QModelIndex>();
//			while (index.Parent().IsValid()) // while the index still has a parent
//			{
//				Console.WriteLine("index parent valid: {0}", index.Parent().IsValid());
//				indices.Add(index);
//				index = index.Parent();
//			}
//			
//			// traverse the indices to find the entity
//			Entity theEntity = document;
//			foreach (QModelIndex ind in indices)
//				theEntity = theEntity.GetNthChild(ind.Row());
//			return theEntity;
		}
		
		protected void RegisterIndex(QModelIndex index, Entity entity)
		{
//			indexRegistry[index.GetHashCode()] = entity;
			
		}
		
		
		/// <summary>
		/// Generates a dictionary of variants that represent the item.
		/// </summary>
		/// <param name="index"> A <see cref="QModelIndex"/>. </param>
		/// <returns> A <see cref="Dictionary`2"/> containing an entry for each role. </returns>
//		public override Dictionary<int, QVariant> ItemData (QModelIndex index)
//		{
//			Dictionary<int, QVariant> data = base.ItemData(index);
//			Entity entity = GetEntity(index); // get the associated entity
//			data[(int)Qt.ItemDataRole.DisplayRole] = entity.Name;
//			return data;
//		}

		
		
		/// <summary>
		/// Returns the index for a given position.
		/// </summary>
		/// <param name="row"> The row number. </param>
		/// <param name="column"> The column number. </param>
		/// <param name="parent"> The parent <see cref="QModelIndex"/>. </param>
		/// <returns> A <see cref="QModelIndex"/> representing the given position in the model. </returns>
		public override QModelIndex Index (int row, int column, QModelIndex parent)
		{
			Entity entity;
			if (parent.IsValid())
				entity = GetEntity(parent).GetNthChild(row);
			else
				entity = document;
			ModelIndex index = new ModelIndex(CreateIndex(row, column, entity.Id));
			index.Id = entity.Id;
			RegisterIndex(index, entity);
			return index;
		}
		

		/// <summary>
		/// Returns the number of rows (entities) for a given parent.
		/// </summary>
		/// <param name="parent"> A <see cref="QModelIndex"/> representing the entity parent. </param>
		/// <returns> The number of children of the parent. </returns>
		public override int RowCount (QModelIndex parent)
		{
			Entity entity = GetEntity(parent);
			return entity.NumChildren;
		}
		
		/// <summary>
		/// Returns the number of columns (always 1).
		/// </summary>
		public override int ColumnCount (QModelIndex parent)
		{
			return 1;
		}

		/// <summary>
		/// Gets the index of the parent of the given entity.
		/// </summary>
		/// <param name="child"> A <see cref="QModelIndex"/> representing an entity. </param>
		/// <returns> The <see cref="QModelIndex"/> of the parent. </returns>
		public override QModelIndex Parent(QModelIndex child)
		{			
			Entity entity = GetEntity(child);
			int row = entity.Parent.ChildIndex(entity);
			ModelIndex index = new ModelIndex(CreateIndex(row, 0, entity.Parent.Id));
			index.Id = entity.Id;
			RegisterIndex(index, entity);
			return index;
		}


		
#endregion
		
		
#region Data Access
		
		/// <summary>
		/// Accesses the data for the given model index.
		/// </summary>
		/// <param name="index"> A <see cref="QModelIndex"/> representing an entity. </param>
		/// <param name="role"> The role. </param>
		/// <returns> A <see cref="QVariant"/> representing the data. </returns>
		public override QVariant Data (QModelIndex index, int role)
		{
			Entity entity = GetEntity(index);
			Dictionary<int, QVariant> data = ItemData(index);
			switch (role)
			{
			case (int)Qt.ItemDataRole.DisplayRole:
				return entity.Name;
			case (int)Qt.ItemDataRole.DecorationRole:
				return ResourceManager.GetIcon("block");
			default:
				return data[role];
			}
		}

		
#endregion
		
		
	}
}
