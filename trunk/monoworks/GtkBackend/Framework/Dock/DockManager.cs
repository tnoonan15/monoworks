// DockManager.cs - Slate Mono Application Framework
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


namespace MonoWorks.GtkBackend.Framework.Dock
{
	
	/// <summary>
	/// Manages the docking framework.
	/// </summary>
	public class DockManager
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public DockManager(DockArea dockArea)
		{
			this.dockArea = dockArea;
			dockArea.Manager = this;
			
			rootNode = new RootNode(dockArea);
			
			// create the overlays	
			foreach (Position pos in Enum.GetValues(typeof(Position)))
			{
				localOverlays[pos] = new DockOverlay(pos);
				if (pos != Position.Tab)
					globalOverlays[pos] = new DockOverlay(pos);
			}
		}
		
		
		protected DockArea dockArea;
		/// <value>
		/// The dock area that the manager is managing.
		/// </value>
		public DockArea DockArea
		{
			get {return dockArea;}
		}
		
		protected RootNode rootNode;
		/// <value>
		/// The root node of the docking tree.
		/// </value>
		public RootNode RootNode
		{
			get {return rootNode;}
			set {rootNode = value;}
		}
		
		
#region Dockable Factory

		/// <summary>
		/// Creates a dockable with ths manager.
		/// </summary>
		/// <param name="widget"> A <see cref="DockableBase"/> to dock. </param>
		/// <param name="name"> The name of the dockable. </param>
		/// <returns> A new <see cref="Dockable"/>. </returns>
		public Dockable CreateDockable(DockableBase widget, string name)
		{
			return new Dockable(this, widget, name);
		}
		
#endregion
		
		
#region Dockable Registry

		protected Dictionary<Dockable, DockableNode> dockableNodes = new Dictionary<Dockable,DockableNode>();
		
		/// <summary>
		/// Registers a dockable with the registry so that the manager knows about it.
		/// </summary>
		/// <param name="dockable"> A <see cref="Dockable"/>. </param>
		/// <param name="node"> The <see cref="DockableNode"/> associated with dockable. </param>
		public void RegisterDockable(Dockable dockable, DockableNode node)
		{
			dockableNodes[dockable] = node;
		}

		protected Dictionary<Dockable, DocumentNode> documentNodes = new Dictionary<Dockable,DocumentNode>();
		
		/// <summary>
		/// Registers a document with the registry so that the manager knows about it.
		/// </summary>
		/// <param name="document"> A <see cref="Dockable"/>. </param>
		/// <param name="node"> The <see cref="DockableNode"/> associated with document. </param>
		public void RegisterDocument(Dockable document, DocumentNode node)
		{
			documentNodes[document] = node;
		}
		
		/// <summary>
		/// Gets the node corresponding to the dockable.
		/// </summary>
		/// <param name="dockable"> A <see cref="Dockable"/>. </param>
		/// <returns> The <see cref="DockableNode"/> corresponding to dockable. </returns>
		public DockableNode GetNode(Dockable dockable)
		{			
			if (dockableNodes.ContainsKey(dockable))
			    return dockableNodes[dockable];
			else if (documentNodes.ContainsKey(dockable))
			    return documentNodes[dockable];
			else 
				throw new Exception("DockManager doesn't contain this dockable.");
		}
		
#endregion
		
		
#region Docking

		/// <summary>
		/// Docks the dockable to the main docking area at the given position.
		/// </summary>
		/// <param name="dockable"> A <see cref="Dockable"/> to dock. </param>
		/// <param name="position"> The <see cref="Position"/> to dock it. </param>
		public void Dock(Dockable dockable, Position position)
		{
			DockableNode node = GetNode(dockable);
			if (rootNode.NumChildren == 0) // there isn't anything there yet
			{
				rootNode.Add(node);
			}
			else // there's something there already
			{
				DockNode(rootNode.Child, node, position);
			}
		}
		
		/// <summary>
		/// Docks dockable to other at the given position.
		/// </summary>
		/// <param name="dockable"> A <see cref="Dockable"/> to dock. </param>
		/// <param name="other"> The <see cref="Dockable"/> to dock to. </param>
		/// <param name="position"> The <see cref="Position"/> to dock it. </param>
		public void Dock(Dockable dockable, Dockable other, Position position)
		{
//			Console.WriteLine("docking {0} to {1} at {2}", dockable.Name, other.Name, position);
			
			// handle book docking
			Gtk.Widget parent = other.Parent;
			if (parent is DockBook && position != Position.Tab)
			{
				DockableNode node = GetNode(dockable);
				Node otherNode = GetNode(other).Parent;
				DockNode(otherNode, node, position);
			}
			else // the parent isn't a book, dock normally
			{
				DockableNode node = GetNode(dockable);
				DockableNode otherNode = GetNode(other);
				DockNode(otherNode, node, position);
			}
		}
		
		/// <summary>
		/// Docks node to baseNode at the given position.
		/// </summary>
		/// <param name="baseNode"> A <see cref="Node"/> to be docked to. </param>
		/// <param name="node"> A <see cref="DockableNode"/> to dock. </param>
		/// <param name="position"> The <see cref="Position"/> to dock at. </param>
		/// <remarks> This is a utility method used by the various overloads of Dock() to do the heavy lifting.</remarks>
		protected void DockNode(Node baseNode, DockableNode node, Position position)
		{
			if (position == Position.Tab) // this is a tab docking
			{
//				Console.WriteLine("baseNode parent {0}", baseNode.Parent);
				if (baseNode.Parent is BookNode) // the base's parent is already a book
				{
//					Console.WriteLine("tab docking {0} to {1}", node, baseNode.Parent);
					baseNode.Parent.Add(node);
				}
				else // make a new book above the parent
				{
//					Console.WriteLine("tab docking {0} to a new book", node);
					BookNode book = new BookNode();
					baseNode.Parent.Swap(baseNode, book); // replace the base with the new book
					book.Add(baseNode);
					book.Add(node);
				}
			}
			else // this is a side docking
			{
				// make a paned
//				Console.WriteLine("making a new paned with {0} and {1}", baseNode, node.Dockable.Name);
				PanedNode paned = new PanedNode(position);
				baseNode.Parent.Swap(baseNode, paned); // replace the base with the new paned
				if (position == Position.Top || position == Position.Left)
				{
					paned.AddStart(node);
					paned.AddEnd(baseNode);
				}
				else
				{
					paned.AddStart(baseNode);
					paned.AddEnd(node);
				}
			}
			
		}
		
		/// <summary>
		/// Detaches a dockable from the dockable tree.
		/// </summary>
		/// <param name="dockable"> A <see cref="Dockable"/> to detach. </param>
		public void Detach(Dockable dockable)
		{
			DockableNode node = GetNode(dockable);
			Node parent = node.Parent;
//			Console.WriteLine("removing {0} from {1}", node, parent);
			parent.Remove(node);
			
			// if the parent is a paned or book and only has one remaining child, remove it
			if ((parent is PanedNode) || (parent is BookNode) && parent.NumChildren == 1)
			{
				parent.Parent.Swap(parent, parent.Children[0]);
			}

		}
		
#endregion
		
				
#region Documents
		
		protected DocumentAreaNode documentAreaNode = null;
		/// <value>
		/// The node for the document area.
		/// </value>
		public DocumentAreaNode DocumentAreaNode
		{
			get {return documentAreaNode;}
		}
		
		protected DockArea documentArea = null;
		/// <value>
		/// The dock area for documents.
		/// </value>
		public DockArea DocumentArea
		{
			get {return documentArea;}
			set
			{
				if (documentArea != null)
					throw new Exception("The dock manager already has a document area.");
				if (!value.HandlesDocuments)
					throw new Exception("The DocumentArea must have HandlesDocuments set to true.");
				documentArea = value;
				documentArea.Manager = this;
				documentAreaNode = new DocumentAreaNode(documentArea);
			}
		}
				
		/// <summary>
		/// Add a document.
		/// </summary>
		/// <param name="document"> A <see cref="DocumentBase"/>. </param>
		public void AddDocument(DocumentBase document)
		{
			if (documentArea == null)
				throw new Exception("There is no document area.");
			
			Dockable dockable = new Dockable(this, document, document.Name);
			DocumentNode node = new DocumentNode(dockable);
			RegisterDocument(dockable, node);
			
			documentAreaNode.Add(node);
			documentArea.ShowAll();
		}
		
		/// <value>
		/// Gets the currently visible document.
		/// </value>
		public Dockable CurrentDocument
		{
			get
			{
				// TODO: Implement CurrentDocument functionality for Gtk
//				var current = documentArea.Ac
				return null;
			}
		}
				
#endregion
		
		
#region Overlays
		
		
		/// <summary>
		/// The docking position overlays for local docking.
		/// </summary>
		protected Dictionary<Position, DockOverlay> localOverlays = new Dictionary<Position, DockOverlay>();
		
		/// <summary>
		/// The docking position overlays for global docking.
		/// </summary>
		protected Dictionary<Position, DockOverlay> globalOverlays = new Dictionary<Position, DockOverlay>();
		
		/// <summary>
		/// Updates the positions of the local overlays based on the position of the dockable currently under the cursor.
		/// </summary>
		public void UpdateLocalOverlays()
		{
			// get the size and position of the dockable relative to its Gdk window
			int w = dockableUnderCursor.Allocation.Width;
			int h = dockableUnderCursor.Allocation.Height;
			int dx = dockableUnderCursor.Allocation.X;
			int dy = dockableUnderCursor.Allocation.Y;
			
			// get the absolute position of the Gdk window
			int x0, y0;
			dockableUnderCursor.GdkWindow.GetPosition(out x0, out y0);
			int x = x0 + dx + (int)(w/2.0) - DockOverlay.Width/2;
			int y = y0 + dy + (int)(h/2.0) - DockOverlay.Width/2;
			
			// position the local overlays
			int offset = 48;
			localOverlays[Position.Left].Move(x - offset, y);
			localOverlays[Position.Right].Move(x + offset, y);
			localOverlays[Position.Top].Move(x, y - offset);
			localOverlays[Position.Bottom].Move(x, y + offset);
			localOverlays[Position.Tab].Move(x, y);
		}

		/// <summary>
		/// Update the positions of the global overlays.
		/// </summary>
		public void UpdateGlobalOverlays()
		{
			// get the size and position of the root dock widget relative to its Gdk window
			int w = rootNode.Widget.Allocation.Width;
			int h = rootNode.Widget.Allocation.Height;
			int dx = rootNode.Widget.Allocation.X;
			int dy = rootNode.Widget.Allocation.Y;
			
			// get the absolute position of the Gdk window
			int x0, y0;
			rootNode.Widget.GdkWindow.GetPosition(out x0, out y0);
			int x = x0 + dx;
			int y = y0 + dy;
			
			// position the local overlays
			int offset = 48;
			globalOverlays[Position.Left].Move(x + offset, y + h/2);
			globalOverlays[Position.Right].Move(x + w - offset - DockOverlay.Width/2, y + h/2);
			globalOverlays[Position.Top].Move(x + w/2, y + offset);
			globalOverlays[Position.Bottom].Move(x + w/2, y + h - offset - DockOverlay.Width/2);
		}
		
		/// <value>
		/// Whether the local overlays are visible.
		/// </value>
		protected bool LocalOverlaysVisible
		{
			set
			{
				foreach (DockOverlay overlay in localOverlays.Values)
					overlay.Visible = value;
			}
		}
		
		/// <value>
		/// Whether the global overlays are visible.
		/// </value>
		protected bool GlobalOverlaysVisible
		{
			set
			{
				foreach (DockOverlay overlay in globalOverlays.Values)
					overlay.Visible = value;
			}
			get {return globalOverlays[Position.Top].Visible;}
		}
		
#endregion
		
		
#region Hovering
		
		protected Dockable dockableUnderCursor = null;
		
		/// <summary>
		/// Called when a main dockable is hovering.
		/// </summary>
		/// <param name="dockable"> The <see cref="MainDockable"/> that is hovering. </param>
		/// <param name="cursorX"> The x position of the cursor. </param>
		/// <param name="cursorY"> The y position of the cursor. </param>
		public void OnHover(Dockable dockable, int cursorX, int cursorY)
		{

			// keep track of the top hit dockable
			Dockable hitDockable = null;
			DockableNode node = GetNode(dockable);
			
			if (node is DocumentNode) // this is a document dockable
			{
				// cycle through registered dockables
				foreach (DocumentNode otherNode in documentNodes.Values)
				{
					Dockable otherDockable = otherNode.Dockable;
					
					if (otherDockable != dockable && !otherDockable.DockFloating) // can't hover over yourself or a floating dockable
					{
						// get the dockable's size and relative pointer position
						Gdk.Rectangle alloc = otherDockable.Allocation;
						int dockW = alloc.Width;
						int dockH = alloc.Height;
						int relX, relY;
						otherDockable.GetPointer(out relX, out relY);
						
						// test if the cursor is inside this dockable
						if (relX > 0 && relX < dockW  &&  relY > 0 && relY < dockH)
						{
							// keep track of this dockable if it's in front of the last one that was hit
							if (hitDockable==null || otherDockable.GdkWindow.Depth < hitDockable.GdkWindow.Depth)
							{
								hitDockable = otherDockable;
							}
						}
						
					}
					
				} 
			}
			else // this is a regular dockable
			{
				// cycle through registered dockables
				foreach (DockableNode otherNode in dockableNodes.Values)
				{
					Dockable otherDockable = otherNode.Dockable;
					
					if (otherDockable != dockable && !otherDockable.DockFloating) // can't hover over yourself or a floating dockable
					{
						// get the dockable's size and relative pointer position
						Gdk.Rectangle alloc = otherDockable.Allocation;
						int dockW = alloc.Width;
						int dockH = alloc.Height;
						int relX, relY;
						otherDockable.GetPointer(out relX, out relY);
						
						// test if the cursor is inside this dockable
						if (relX > 0 && relX < dockW  &&  relY > 0 && relY < dockH)
						{
							// keep track of this dockable if it's in front of the last one that was hit
							if (hitDockable==null || otherDockable.GdkWindow.Depth < hitDockable.GdkWindow.Depth)
							{
								hitDockable = otherDockable;
							}
						}
						
					}
					
				}
				
			}
			
			// handle the hit
			if (hitDockable == null) // nothing is being hovered over
			{
				LocalOverlaysVisible = false;
				dockableUnderCursor = null;
			}
			else // something was hit
			{
				if (dockableUnderCursor == null || hitDockable != dockableUnderCursor)
				{
					dockableUnderCursor = hitDockable;
					UpdateLocalOverlays();					
					LocalOverlaysVisible = true;
				}	
				
			
				// test for local hovering
				foreach (DockOverlay overlay in localOverlays.Values)
				{
					if (overlay.HitTest())
						overlay.Hovering = true;
					else
						overlay.Hovering = false;
				}
			}		

			if (!dockable.ContainsADocument)
			{			
				// handle the global overlays
				if (!GlobalOverlaysVisible)
				{
					UpdateGlobalOverlays();
					GlobalOverlaysVisible = true;
				}
				
				// test for global hovering
				foreach (DockOverlay overlay in globalOverlays.Values)
				{
					if (overlay.HitTest())
						overlay.Hovering = true;
					else
						overlay.Hovering = false;
				}
			}
		}
		
		/// <summary>
		/// Called when a dockable is released by the user.
		/// </summary>
		/// <param name="dockable"> A <see cref="Dockable"/> that was hovering. </param>
		public void OnRelease(Dockable dockable)
		{	
			// if the cursor is hovering over an overlay, dock the dokable
			if (dockableUnderCursor != null)
			{
				// check the local overlays
				foreach (DockOverlay overlay in localOverlays.Values)
				{
					if (overlay.Hovering)
						dockable.Dock( dockableUnderCursor, overlay.Position);
				}
			}

			// check the global overlays
			if (dockable.DockFloating)
			{
				foreach (DockOverlay overlay in globalOverlays.Values)
				{
					if (overlay.Hovering)
						dockable.Dock( overlay.Position);
				}
			}
			
			LocalOverlaysVisible = false;
			dockableUnderCursor = null;
			
			GlobalOverlaysVisible = false;
		}
		
#endregion
		
	}
}
