// DockableNode.cs - Slate Mono Application Framework
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
	/// Dock framework node that represents a dockable widget.
	/// </summary>
	public class DockableNode : Node
	{
		
		public DockableNode(Dockable dockable) : base()
		{
			this.dockable = dockable;
		}
		
		protected Dockable dockable;
		/// <value>
		/// The dockable.
		/// </value>
		public Dockable Dockable
		{
			get { return dockable; }
		}

		public override Gtk.Widget Widget
		{
			get { return dockable; }
		}
		
		public override void Refresh()
		{
			base.Refresh();
			dockable.NewParent();
		}

		
		public override void Remove(Node child)
		{
			throw new Exception("Dockables don't have children, so they can't be removed.");
		}

		
		
		public override void Swap (Node existingChild, Node newChild)
		{
			throw new Exception("Swap() shouldn't be called on DockableNodes as they have no children.");
		}


	}
}
