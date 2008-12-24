// DocumentAreaNode.cs - Slate Mono Application Framework
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
	/// A node in the dock tree that represents a document area.
	/// </summary>
	public class DocumentAreaNode : RootNode
	{

		/// <summary>
		/// Default constructor.
		/// </summary>
		/// <param name="dockArea"> A <see cref="DockArea"/> that only handles documents. </param>
		public DocumentAreaNode(DockArea dockArea) : base(dockArea)
		{
			dockArea.HandlesDocuments = true;
		}


		public override bool ContainsDocumentArea
		{
			get {return true;}
		}


		/// <summary>
		/// Adds a document node to the document area.
		/// </summary>
		/// <param name="node"> </param>
		/// <remarks>Ensures there's a book here.</remarks>
		public override void Add(Node node)
		{
			if (NumChildren != 0 && !(node is DocumentNode))
				throw new Exception("Only DocumentNodes can be added to document areas.");
			
			if (NumChildren == 0) // no document yet
			{
				BookNode book = new BookNode();
				base.Add(book);
				book.Add(node);
//				base.Add(node);
			}
			else // already has children
			{
				if (Child is BookNode) // has a book as it's first child
				{
					Child.Add(node);
				}
				else if (Child is PanedNode) // there's more than one document paned together
				{
					// add the node to the first book
					foreach (Node child in Child.Children)
					{
						if (child is BookNode)
							child.Add(node);
					}
				}
			}
			
		}

		
	}
}
