// TreeView.cs - MonoWorks Project
//
//  Copyright (C) 2008 Andy Selvig
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
//
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA 

using System;

using MonoWorks.Modeling;

namespace MonoWorks.GuiGtk.Tree
{
	
	/// <summary>
	/// The tree view that acts on a document tree.
	/// </summary>
	public class TreeView : Gtk.TreeView, ISelectionListener
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public TreeView() : base()
		{
			WidthRequest = 200;
			
			
			model = new TreeModel();
			this.Model = model;		
			
			// Create a column 
			Gtk.TreeViewColumn column = new Gtk.TreeViewColumn();
			column.Title = "Entity";
			AppendColumn(column);

			// create the renderers
			var iconRenderer = new Gtk.CellRendererPixbuf();
			column.PackStart(iconRenderer, false);
			column.AddAttribute(iconRenderer, "stock-id", 0);
			var nameRenderer = new Gtk.CellRendererText();
			column.PackStart(nameRenderer, true);
			column.AddAttribute(nameRenderer, "text", 1);
			
			this.Selection.Changed += OnSelectionChanged;
		}
		
		protected TreeModel model;
		
		protected Drawing drawing;
		//// <value>
		/// The drawing.
		/// </value>
		public Drawing Drawing
		{
			get { return drawing; }
			set
			{
				this.drawing = value;
				model.Drawing = value;
			}
		}
		
		
		
#region Selection
		
		/// <summary>
		/// Set true when the selection state is being updated by the control itself.
		/// </summary>
		private bool internalUpdate = false;
		
		/// <summary>
		/// Handles the selection changing by the user.
		/// </summary>
		void OnSelectionChanged(object sender, EventArgs e)
		{
			if (!internalUpdate)
			{
				internalUpdate = true;
				Drawing.EntityManager.DeselectAll(this);
				Gtk.TreeIter iter;
				Gtk.TreeModel treeModel;
				if (Selection.GetSelected(out treeModel, out iter))
				{
					var entity = model.GetEntity(iter);
					Drawing.EntityManager.Select(this, entity);
				}
				internalUpdate = false;
			}
		}
		
		public void OnSelect(Entity entity)
		{
			Gtk.TreeIter iter = model.GetIter(entity);
			Selection.SelectIter(iter);
		}
				
		public new void OnSelectAll()
		{
			Selection.UnselectAll();
		}
		
		public void OnDeselect(Entity entity)
		{
			Gtk.TreeIter iter = model.GetIter(entity);
			Selection.UnselectIter(iter);
		}
				
		public void OnDeselectAll()
		{
			Selection.UnselectAll();
		}
		
#endregion
		
		
	}
}
