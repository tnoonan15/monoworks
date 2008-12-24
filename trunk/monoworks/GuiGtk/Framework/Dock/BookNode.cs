// BookNode.cs - Slate Mono Application Framework
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

namespace MonoWorks.GuiGtk.Framework.Dock
{
	
	/// <summary>
	/// Docking framework node that represents a notebook.
	/// </summary>
	public class BookNode : Node
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public BookNode() : base()
		{
			dockBook = new DockBook();
			dockBook.PageReordered += OnPageReordered;
		}

		protected DockBook dockBook;
		/// <value>
		/// The DockBook.
		/// </value>
		public override Gtk.Widget Widget
		{
			get { return dockBook; }
		}

		/// <summary>
		/// Adds a node to the book.
		/// </summary>
		/// <param name="node"> A <see cref="Node"/> to add. </param>
		public override void Add(Node node)
		{
			if (!(node is DockableNode))
				throw new Exception("Only dockable can be added to DockBooks.");
			base.Add(node);
			
			(node as DockableNode).Dockable.RemoveTitleBar();
			dockBook.AppendPage(node.Widget, (node as DockableNode).Dockable.TitleBar);
			
			node.Refresh();
		}

		/// <summary>
		/// Removes a node from the book.
		/// </summary>
		/// <param name="child"> A child <see cref="Node"/>. </param>
		public override void Remove(Node child)
		{
			base.Remove(child);
			
			dockBook.Remove(child.Widget);
			(child as DockableNode).Dockable.AddTitleBar();
		}
		
		
		/// <summary>
		/// Handles when a page is moved within the book.
		/// </summary>
		public void OnPageReordered(object sender, Gtk.PageReorderedArgs args)
		{
			// get the dockable's node
			DockableNode node = (args.P0 as Dockable).Manager.GetNode(args.P0 as Dockable);
			if (!children.Contains(node))
				throw new Exception("DockBook does note contain this node.");
			
			children.Remove(node);
			int newIndex = (int)args.P1;
				children.Insert(newIndex, node);
		}


		
	}
}
