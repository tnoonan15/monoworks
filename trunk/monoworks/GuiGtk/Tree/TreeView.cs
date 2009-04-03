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
	public class TreeView : Gtk.TreeView
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
			Gtk.CellRendererText nameRenderer = new Gtk.CellRendererText();
			column.PackStart(nameRenderer, true);
			column.AddAttribute(nameRenderer, "text", 0);
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
		
	}
}
