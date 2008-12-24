// Node.cs - Slate Mono Application Framework
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
using System.Collections.Generic;

namespace MonoWorks.GuiGtk.Framework.Dock
{
	
	/// <summary>
	/// Base class for all dock framework nodes.
	/// </summary>
	public abstract class Node
	{
		
		protected Node()
		{
		}
		
		/// <summary>
		/// The widget this node represents.
		/// </summary>
		public abstract Gtk.Widget Widget
		{
			get;
		}
		
#region Hierarchy
		
		protected Node parent;
		/// <value>
		/// The parent node.
		/// </value>
		public virtual Node Parent
		{
			get {return parent;}
			set {parent = value;}
		}
		

		protected List<Node> children = new List<Node>();
		/// <summary>
		/// The child nodes.
		/// </summary>
		public List<Node> Children
		{
			get {return children;}
			set {children = value;}
		}
		
		/// <value>
		/// The number of children.
		/// </value>
		public int NumChildren
		{
			get {return children.Count;}
		}
		
		/// <summary>
		/// Refreshes the state of the dockable.
		/// </summary>
		public virtual void Refresh()
		{
			foreach (Node child in children)
				child.Refresh();
		}
		
		/// <summary>
		/// Adds the node to this node's children.
		/// </summary>
		/// <param name="node"> A <see cref="Node"/> to add. </param>
		public virtual void Add(Node node)
		{
			children.Add(node);
			node.Parent = this;
		}
		
		/// <summary>
		/// Inserts the node at the given index.
		/// </summary>
		/// <param name="node"> A <see cref="Node"/> to insert. </param>
		/// <param name="index"> The index at which to insert. </param>
		public virtual void Insert(Node node, int index)
		{
			children.Insert(index, node);
			node.Parent = this;
		}
		
		/// <summary>
		/// Removes a child.
		/// </summary>
		/// <param name="child"> A <see cref="Node"/> that is a child of this node. </param>
		public virtual void Remove(Node child)
		{
			if (!children.Contains(child))
				throw new Exception("Node does not contain child to remove");
			children.Remove(child);
			child.Parent = null;
		}
		
		/// <summary>
		/// Swaps existingChild with newChild.
		/// </summary>
		/// <param name="existingChild"> An existing child <see cref="Node"/>. </param>
		/// <param name="newChild"> A new <see cref="Node"/>. </param>
		public virtual void Swap(Node existingChild, Node newChild)
		{
			if (!children.Contains(existingChild))
				throw new Exception("Node does not contain child to swap");
			
			int index = children.IndexOf(existingChild);
			Remove(existingChild);
			if (newChild.Parent != null)
				newChild.Parent.Remove(newChild);
			Insert(newChild, index);			
		}


		//// <value>
		/// Recursively checks the node's hierarchy to see if it contains a document area.
		/// </value>
		public virtual bool ContainsDocumentArea
		{
			get
			{
				foreach (Node child in children)
				{
					if (child.ContainsDocumentArea)
						return true;
				}
				return false;
			}
		}
		
#endregion

		
		
	}
}
