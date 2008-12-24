// PanedNode.cs - Slate Mono Application Framework
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
	/// Docking framework node that represents a Stack.
	/// </summary>
	public class PanedNode : Node
	{
		public PanedNode(Gtk.Orientation orientation) : base()
		{
			GeneratePaned(orientation);
		}
		
		/// <summary>
		/// Contructor that makes the paned compatible with the position.
		/// </summary>
		/// <param name="position"> A <see cref="Position"/>. </param>
		public PanedNode(Position position) : base()
		{
			if (position == Position.Left || position == Position.Right)
				GeneratePaned(Gtk.Orientation.Horizontal);
			else if (position == Position.Top || position == Position.Bottom)
				GeneratePaned(Gtk.Orientation.Vertical);
			else
				throw new Exception(String.Format("Paned nodes do not support positions like {0}", position));
		}

		/// <summary>
		/// Generates the paned for the given orientation.
		/// </summary>
		/// <param name="orientation"> </param>
		/// <returns>/ </returns>
		protected void GeneratePaned(Gtk.Orientation orientation)
		{
			this.orientation = orientation;
			if (orientation == Gtk.Orientation.Horizontal)
				paned = new Gtk.HPaned();
			else
				paned = new Gtk.VPaned();
		}
		
		protected Gtk.Paned paned;
		/// <value>
		/// The paned widget.
		/// </value>
		public override Gtk.Widget Widget
		{
			get { return paned; }
		}


		private Gtk.Orientation orientation;
		/// <value>
		/// The orientation of the paned.
		/// </value>
		public Gtk.Orientation Orientation
		{
			get {return orientation;}
			set {orientation = value;}
		}
		
		/// <summary>
		/// Returns true if the position is compatible with the orientation of the paned.
		/// </summary>
		/// <param name="position"> A <see cref="Position"/>. </param>
		public bool PositionIsCompatible(Position position)
		{
			if (Orientation==Gtk.Orientation.Horizontal && (position==Position.Left || position==Position.Right))
				return true;
			if (Orientation==Gtk.Orientation.Vertical && (position==Position.Top || position==Position.Bottom))
				return true;
			return false;			
		}

		//// <value>
		/// The node in the first slot, or null if it's empty.
		/// </value>
		public Node StartNode
		{
			get
			{
				if (paned.Child1 == null)
					return null;
				foreach (Node node in children)
				{
					if (node.Widget == paned.Child1)
						return node;
				}
				return null;
			}
		}

		//// <value>
		/// The node in the second slot, or null if it's empty.
		/// </value>
		public Node EndNode
		{
			get
			{
				if (paned.Child2 == null)
					return null;
				foreach (Node node in children)
				{
					if (node.Widget == paned.Child2)
						return node;
				}
				return null;
			}
		}


		/// <summary>
		/// Reparents the children if they contains a document area.
		/// </summary>
		/// <param name="node"> </param>
		public override void Refresh()
		{
			base.Refresh();

			// re-assign the other one if it has a document area
			if (EndNode != null && EndNode.ContainsDocumentArea)
			{
				Node endNode = EndNode;
				paned.Remove(endNode.Widget);
				paned.Pack2(endNode.Widget, true, true);
			}			

			// re-assign the other one if it has a document area
			if (StartNode != null && StartNode.ContainsDocumentArea)
			{
				Node startNode = StartNode;
				paned.Remove(startNode.Widget);
				paned.Pack1(startNode.Widget, true, true);
			}

			paned.ShowAll();
		}

		

#region Adding Nodes

		public override void Add (Node node)
		{
			if (StartNode == null)
				AddStart(node);
			else if (EndNode == null)
				AddEnd(node);
			else
				throw new Exception("No empty slots in the paned.");
		}

		
		/// <summary>
		/// Adds a node to the start of the paned.
		/// </summary>
		/// <param name="node"> </param>
		public void AddStart(Node node)
		{
			base.Add(node);
			if (node.ContainsDocumentArea)
				paned.Pack1(node.Widget, true, true);
			else
				paned.Pack1(node.Widget, false, false);
			
			Refresh();
		}
		
		/// <summary>
		/// Add a node to the end.
		/// </summary>
		/// <param name="node"> A <see cref="Node"/> to add. </param>
		/// <remarks> If the paned has other children, it will be appended to the right/bottom. </remarks>
		public void AddEnd(Node node)
		{			
			base.Add(node);
			if (node.ContainsDocumentArea)
				paned.Pack2(node.Widget, true, true);
			else
				paned.Pack2(node.Widget, false, false);
			
			Refresh();
		}
		
//		/// <summary>
//		/// Appends the node to one side or the other of the paned.
//		/// </summary>
//		/// <param name="node"> A <see cref="Node"/> to append. </param>
//		/// <param name="position"> A <see cref="Position"/> that's compatible with the orientation. </param>
//		public void AddAtPos(Node node, Position position)
//		{
//			if (!PositionIsCompatible(position))
//				throw new Exception(String.Format("The position {0} is not compatible with the paned orientation {1}", position, Orientation));
//			
//			if (position == Position.Left || position == Position.Top) // insert at the beginning
//			{
//				children.Insert(0, node);
//				paned.InsertNode(node, 0);
//			}
//			else // add to the end
//			{
//				children.Add(node);
//				paned.AddNode(node);
//			}
//			node.Parent = this;
//			node.Refresh();
//		}
		
//		/// <summary>
//		/// Adds the node next to another node.
//		/// </summary>
//		/// <param name="node"> A <see cref="Node"/> to append. </param>
//		/// <param name="position"> A <see cref="Position"/> that's compatible with the orientation. </param>
//		public void AddAtPos(Node node, Node otherNode, Position position)
//		{
//			if (!PositionIsCompatible(position))
//				throw new Exception(String.Format("The position {0} is not compatible with the paned orientation {1}", position, Orientation));
//			
//			if (!children.Contains(otherNode))
//				throw new Exception("Paned must contain otherNode");
//			
//			int index = children.IndexOf(otherNode);
//			
//			if (position == Position.Left || position == Position.Top) // insert before otherNode
//			{
//				children.Insert(index, node);
//				paned.InsertNode(node, index);
//			}
//			else // insert after otherNode
//			{
//				if (index == children.Count - 1) // add to the end
//				{
//					children.Add(node);
//					paned.AddNode(node);
//				}
//				else // insert after otherNode, not at the end
//				{
//					children.Insert(index+1, node);
//					paned.InsertNode(node, index+1);
//				}
//			}
//			node.Parent = this;
//			node.Refresh();
//		}
//		
//		
		public override void Insert(Node node, int index)
		{
			base.Insert(node, index);

			if (index == 0)
				paned.Pack1(node.Widget, false, false);
			else if (index == 1)
				paned.Pack2(node.Widget, false, false);
			else
				throw new Exception(index.ToString() + " is not a valid index for a paned.");
			Refresh();
		}

		
		public override void Remove(Node child)
		{
			base.Remove(child);
			
//			Console.WriteLine("  removing {0} from {1}", child.Widget, paned);
			paned.Remove(child.Widget);
		}


		/// <summary>
		/// This needs to be overriden since the paned node doesn't used the children indices
		/// to identify their position.
		/// </summary>
		/// <param name="existingChild"> </param>
		/// <param name="newChild"> </param>
		public override void Swap (Node existingChild, Node newChild)
		{
			if (!children.Contains(existingChild))
				throw new Exception("Node does not contain child to swap");

			// determine where the existing child is
			int index;
			if (paned.Child1 != null && existingChild.Widget == paned.Child1)
				index = 0;
			else
				index = 1;
			
			Remove(existingChild);
			if (newChild.Parent != null)
				newChild.Parent.Remove(newChild);
			Insert(newChild, index);		
		}


		
#endregion
		
		
	}
}
