// TreeView.cs - MonoWorks Project
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
	/// Custom signals for the tree model.
	/// </summary>
	public interface ITreeSignals
	{
		[Q_SIGNAL]
		void PaintViewport();
	}
	
	/// <summary>
	/// The tree view is the view component of the model/view framework used to
	/// represent the document as a tree structure.
	/// </summary>
	public class TreeView : QTreeView
	{
		
		/// <summary>
		/// Default constructor.
		/// </summary>
		/// <param name="parent"> The tree view's parent widget. </param>
		public TreeView(QWidget parent) : base(parent)
		{
			this.selectionMode = SelectionMode.ExtendedSelection;
		}
	
		
		/// <value>
		/// Emits custom signals.
		/// </value>
		protected new ITreeSignals Emit
		{ 
			get { return (ITreeSignals)Q_EMIT; }
		}


		
#region Selection
		
		/// <summary>
		/// Gets called when the selection has changed.
		/// </summary>
		/// <param name="selected"> A <see cref="QItemSelection"/> with the newly selected items. </param>
		/// <param name="deselected"> A <see cref="QItemSelection"/> with the newly deselected items. </param>
		protected override void SelectionChanged(QItemSelection selected, QItemSelection deselected)
		{
			base.SelectionChanged(selected, deselected);
			
			foreach (QModelIndex index in selected.Indexes())
				TreeModel.IndexToEntity(index).IsSelected = true;
			foreach (QModelIndex index in deselected.Indexes())
				TreeModel.IndexToEntity(index).IsSelected = false;

			this.Emit.PaintViewport();
			
		}
		
		/// <summary>
		/// Slot for external change of the selection.
		/// </summary>
		[Q_SLOT]
		public void OnExternalSelectionChanged()
		{
			TreeModel model = (TreeModel)this.Model();
			List<Entity> selected = model.Document.Selected;
			
			this.SelectionModel().Clear();
			
			foreach (Entity entity in selected)
			{
				QModelIndex index = model.GetIndex(entity);
				this.SelectionModel().Select(index, (uint)QItemSelectionModel.SelectionFlag.Select);
			}
		}

				
#endregion

	}
}
