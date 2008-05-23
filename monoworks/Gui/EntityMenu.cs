// ContextMenu.cs - ScratchNotes
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

using MonoWorks.Model;

using Qyoto;

namespace MonoWorks.Gui
{
	
	/// <summary>
	/// The ContextMenu class represents a context menu that is used to interact with entities.
	/// It is created by the tree view and viewport.
	/// </summary>
	public class EntityMenu : QMenu
	{
		protected Entity entity;
		
		/// <summary>
		/// Default constructor.
		/// </summary>
		/// <param name="parent"> The parent <see cref="QWidget"/>. </param>
		/// <param name="entity"> The <see cref="Entity"/>. </param>
		public EntityMenu(QWidget parent, Entity entity) : base(entity.Name, parent)
		{
			this.entity = entity;

			CreateActions();
			
			// add actions
			AddAction(actions["cut"]);
			AddAction(actions["copy"]);
			AddAction(actions["delete"]);
			AddSeparator();
			AddAction(actions["properties"]);
		}
			               

#region Actions
		
		protected Dictionary<string, QAction> actions;
		
		/// <summary>
		/// Create the actions.
		/// </summary>
		protected void CreateActions()
		{
			actions = new Dictionary<string,QAction>();
			
			actions["cut"] = new QAction(ResourceManager.GetIcon("edit-cut"), "Cut", this);
			Connect(actions["cut"], SIGNAL("triggered()"), this, SLOT("OnCut()"));
			actions["copy"] = new QAction(ResourceManager.GetIcon("edit-copy"), "Copy", this);
			Connect(actions["copy"], SIGNAL("triggered()"), this, SLOT("OnCopy()"));
			actions["delete"] = new QAction(ResourceManager.GetIcon("delete"), "Delete", this);
			Connect(actions["delete"], SIGNAL("triggered()"), this, SLOT("OnDelete()"));
			
			actions["properties"] = new QAction(ResourceManager.GetIcon("preferences"), "Properties", this);
			Connect(actions["properties"], SIGNAL("triggered()"), this, SLOT("OnProperties()"));
		}		
		
#endregion
		
		
#region Slots
		
		/// <summary>
		/// Cut the entity.
		/// </summary>
		[Q_SLOT]
		protected void OnCut()
		{
			Console.WriteLine("cut");
		}
		
		/// <summary>
		/// Copy the entity.
		/// </summary>
		[Q_SLOT]
		protected void OnCopy()
		{
			Console.WriteLine("copy");
		}
		
		/// <summary>
		/// Delete the entity.
		/// </summary>
		[Q_SLOT]
		protected void OnDelete()
		{
			Console.WriteLine("delete");
		}
		
		/// <summary>
		/// Entity properties.
		/// </summary>
		[Q_SLOT]
		protected void OnProperties()
		{
			Console.WriteLine("properties");
		}
		
#endregion
		
		
	}
}
