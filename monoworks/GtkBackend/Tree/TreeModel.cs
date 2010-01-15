// TreeModel.cs - MonoWorks Project
//
//  Copyright (C) 2009 Andy Selvig
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
using System.Collections.Generic;

using MonoWorks.Modeling;

namespace MonoWorks.GtkBackend.Tree
{
	
	
	public class TreeModel : Gtk.TreeStore, IEntityListener
	{	

		
		public TreeModel() : base(typeof(String), typeof(String))
		{
		}
		

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
				drawing.EntityManager.RegisterEntityListener(this);
//				drawing.EntityManager.RegisterSelectionListener(this);
				GenerateItems();
			}
		}
		
		/// <summary>
		/// Regenerates the entire model.
		/// </summary>
		public void GenerateItems()
		{
			Clear();
			
			foreach (Entity entity in drawing.Children)
				AddEntity(entity);
		}
		
		/// <summary>
		/// Adds an entity to the tree.
		/// </summary>
		public void AddEntity(Entity entity)
		{
			// try to remove it first
			RemoveEntity(entity);
			
			// add it
			if (entity.ParentEntity == null || entity.ParentEntity is Drawing) // this is the root level entity
				AddEntity(entity, null);
			else // this is a child entity
			{
				Gtk.TreeIter iter;
				if (TryGetIter(entity.ParentEntity, out iter)) // only proceed if the parent is in the model
					AddEntity(entity, iter);
			}
		}

		/// <summary>
		/// Adds an entity to the tree with the given parent.
		/// </summary>
		protected void AddEntity(Entity entity, Gtk.TreeIter? parentIter)
		{
			Gtk.TreeIter iter;
			if (parentIter == null)
				iter = AppendValues(entity.ClassName.ToLower(), entity.Name);
			else
				iter = AppendValues((Gtk.TreeIter)parentIter, entity.ClassName.ToLower(), entity.Name);

			// add the children
			foreach (Entity child in entity.Children)
				AddEntity(child, iter);
		}
		
		/// <summary>
		/// Removes an entity from the tree model.
		/// </summary>
		public void RemoveEntity(Entity entity)
		{
			Gtk.TreeIter iter;
			if (TryGetIter(entity, out iter))
				Remove(ref iter);
		}
		
		/// <summary>
		/// Tries to get an iter pointing to an existing entity in the tree. Returns true if it succeeded.
		/// </summary>
		public bool TryGetIter(Entity entity, out Gtk.TreeIter iter)
		{
			Gtk.TreeIter theIter = new Gtk.TreeIter();
			Foreach(delegate(Gtk.TreeModel model, Gtk.TreePath path, Gtk.TreeIter iter_) {
				string name = model.GetValue(iter_, 1) as string;
				if (entity.Name == name)
				{
					theIter = iter_;
					return true;
				}
				else
					return false;
			});
			iter = theIter;
			if (IterIsValid(theIter))
				return true;
			return false;
		}

		
		/// <summary>
		/// Gets the entity associated with the tree iter.
		/// </summary>
		public Entity GetEntity(Gtk.TreeIter iter)
		{
			string name = GetValue(iter, 1) as string;
			return Drawing.EntityManager.GetEntity(name);
		}
		
		
		
	}
}
