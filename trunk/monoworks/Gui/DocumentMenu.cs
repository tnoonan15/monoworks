// DocumentMenu.cs - ScratchNotes
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
	/// The DocumentMenu is the base class for context menus meant to operate 
	/// at the document (not entity) level.
	/// </summary>
	public class DocumentMenu : QMenu
	{
		protected Document document;
		
		/// <summary>
		/// Default constructor.
		/// </summary>
		/// <param name="parent"> The parent <see cref="QWidget"/>. </param>
		/// <param name="document"> The <see cref="Document"/>. </param>
		public DocumentMenu(QWidget parent, Document document) : base(parent)
		{
			this.document = document;
			
			CreateActions();
			
			// add actions
			AddAction(actions["paste"]);
			AddSeparator();
		}
		

#region Actions
		
		protected Dictionary<string, QAction> actions;
		
		/// <summary>
		/// Create the actions.
		/// </summary>
		protected void CreateActions()
		{
			actions = new Dictionary<string,QAction>();
			
			actions["paste"] = new QAction(ResourceManager.GetIcon("edit-paste"), "Paste", this);
			Connect(actions["paste"], SIGNAL("triggered()"), this, SLOT("OnPaste()"));
		}
		
#endregion
		
		
#region Slots
		
		/// <summary>
		/// Paste from the clipboard.
		/// </summary>
		[Q_SLOT]
		protected void OnPaste()
		{
			Console.WriteLine("paste");
		}
		
#endregion
		
	}
}
