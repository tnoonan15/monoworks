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

		/// <summary>
		/// Converts a model index to its associated entity.
		/// </summary>
		/// <param name="index"> A <see cref="QModelIndex"/>. </param>
		/// <returns> The <see cref="Entity"/> associated with the index. </returns>
		public static Entity IndexToEntity(QModelIndex index)
		{
//			Console.WriteLine("begin index to entity");
			Entity entity = (Entity)index.InternalPointer();
//			Console.WriteLine("entity name {0}", entity.Name);
			return entity;
		}
		
		
		/// <summary>
		/// Default constructor.
		/// </summary>
		public TreeModel(Document document) : base()
		{
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
		/// Returns the index for a given position.
		/// </summary>
		/// <param name="row"> The row number. </param>
		/// <param name="column"> The column number. </param>
		/// <param name="parent"> The parent <see cref="QModelIndex"/>. </param>
		/// <returns> A <see cref="QModelIndex"/> representing the given position in the model. </returns>
		public override QModelIndex Index(int row, int column, QModelIndex parent)
		{
//			Console.WriteLine("begin index");
			// get the parent entity
			Entity parentEntity;
			if (parent.IsValid())
			{
				parentEntity = IndexToEntity(parent);
			}
			else
			{
//				Console.WriteLine("invalid parent");
				parentEntity = document;
			}
			
			Entity entity = parentEntity.GetNthChild(row);
			QModelIndex index = CreateIndex(row, column, entity);
//			Console.WriteLine("end index");
			return index;
		}
		

		/// <summary>
		/// Returns the number of rows (entities) for a given parent.
		/// </summary>
		/// <param name="parent"> A <see cref="QModelIndex"/> representing the entity parent. </param>
		/// <returns> The number of children of the parent. </returns>
		public override int RowCount(QModelIndex parent)
		{
//			Console.WriteLine("begin row count");
			Entity parentEntity;
			if (parent.IsValid())
			{
				parentEntity = IndexToEntity(parent);
			}
			else
			{
//				Console.WriteLine("parent is document");
				parentEntity = document;
			}
//			Console.WriteLine("{0} rows", parentEntity.NumChildren);
			return parentEntity.NumChildren;
		}
		
		/// <summary>
		/// Returns the number of columns (always 1).
		/// </summary>
		public override int ColumnCount(QModelIndex parent)
		{
			return 1;
		}

		/// <summary>
		/// Gets the index of the parent of the given entity.
		/// </summary>
		/// <param name="index"> A <see cref="QModelIndex"/> representing an entity. </param>
		/// <returns> The <see cref="QModelIndex"/> of the parent. </returns>
		public override QModelIndex Parent(QModelIndex index)
		{
//			Console.WriteLine("begin parent");
			if (!index.IsValid())
			{
//				Console.WriteLine("invalid index");
				return new QModelIndex();
			}

			Entity entity = IndexToEntity(index);
			Entity parent = entity.Parent;
			if (parent==document)
			{
//				Console.WriteLine("parent is document");
				return new QModelIndex();
			}
			int row = entity.Parent.ChildIndex(entity);
			QModelIndex parentIndex =CreateIndex(row, 0, parent);
//			Console.WriteLine("end parent");
			return parentIndex;
		}

		
		/// <summary>
		/// Creates an index for the given entity.
		/// </summary>
		/// <param name="entity"> A <see cref="Entity"/>. </param>
		/// <returns> A <see cref="QModelIndex"/> representing the entity. </returns>
		public QModelIndex GetIndex(Entity entity)
		{
//			Console.WriteLine("begin get index");
			int row = entity.Parent.ChildIndex(entity);
			QModelIndex index = CreateIndex(row, 0, entity);
//			Console.WriteLine("end get index");
			return index;
		}

		
#endregion
		
		
#region Data Access
		
	
		public override QVariant HeaderData(int section, Qt.Orientation orientation, int role)
		{
			if (orientation == Qt.Orientation.Horizontal && role == (int) Qt.ItemDataRole.DisplayRole)
				return document.Name;
			
			return new QVariant();
		}
		
		/// <summary>
		/// Accesses the data for the given model index.
		/// </summary>
		/// <param name="index"> A <see cref="QModelIndex"/> representing an entity. </param>
		/// <param name="role"> The role. </param>
		/// <returns> A <see cref="QVariant"/> representing the data. </returns>
		public override QVariant Data (QModelIndex index, int role)
		{
//			Console.WriteLine("begin data");
			if (!index.IsValid())
				return new QVariant();
			Entity entity = IndexToEntity(index);
			switch (role)
			{
			case (int)Qt.ItemDataRole.DisplayRole:
//				Console.WriteLine("entity name: {0}", entity.Name);
				return entity.Name;
			case (int)Qt.ItemDataRole.DecorationRole:
//				Console.WriteLine("entity decoration");
				return ResourceManager.GetIcon(entity.TypeName);
			default:
//				Console.WriteLine("other role");
				return new QVariant();
			}
		}

		
#endregion
		
		
	}
}
