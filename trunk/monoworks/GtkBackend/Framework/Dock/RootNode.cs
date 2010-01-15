// RootNode.cs - Slate Mono Application Framework
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

namespace MonoWorks.GtkBackend.Framework.Dock
{
	
	/// <summary>
	/// Docking framework node that represents the root of the node tree.
	/// </summary>
	public class RootNode : Node
	{
		public RootNode(DockArea dockArea) : base()
		{
			this.dockArea = dockArea;
		}
		
		protected DockArea dockArea;
		/// <value>
		/// The dock area.
		/// </value>
		public override Gtk.Widget Widget
		{
			get { return dockArea; }
		}
		
		/// <value>
		/// Sets the child node.
		/// </value>
		public Node Child
		{
			set { Add(value);}
			get { return children[0];}
		}
		
		/// <summary>
		/// Adds a node to the root node.
		/// </summary>
		/// <param name="node"> A <see cref="Node"/>to add. </param>
		public override void Add(Node node)
		{
			if (NumChildren > 0)
				throw new Exception("Root node can only have 1 child.");
			dockArea.Child = node.Widget;
			base.Add(node);
		}

		
		
		public override void Remove(Node child)
		{
			base.Remove(child);
			
			dockArea.Remove(child.Widget);			
		}

		
		public override void Swap(Node existingChild, Node newChild)
		{
			base.Swap(existingChild, newChild);
			
//			dockArea.Remove(existingChild.Widget);
			dockArea.Child = newChild.Widget;
		}


	}
}
