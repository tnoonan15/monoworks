// ToolBox.cs - Slate Mono Application Framework
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

using MonoWorks.Framework;

namespace MonoWorks.GuiGtk.Framework.Tools
{
	
	/// <summary>
	/// A toolbox holds toolshelves and allows the user to switch between them.
	/// </summary>
	public class ToolBox : DynamicBox, ITool
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public ToolBox(string name) : base()
		{
			Name = name;
			
			PackStart(label, false, true, 0);
		}
		
		
		protected ToolArea toolArea;
		/// <summary>
		/// The tool area that this tool belongs to.
		/// </summary>
		public ToolArea ToolArea
		{
			get {return toolArea;}
			set {toolArea = value;}
		}

		private ToolPosition lastPosition;
		//// <value>
		/// The tools last docked position.
		/// </value>
		public ToolPosition LastPosition
		{
			get {return lastPosition;}
			set {lastPosition = value;}
		}


		//// <value>
		/// The visibility of the tool.
		/// </value>
		public bool ToolVisible
		{
			get {return !toolArea.ToolIsHidden(this);}
			set
			{
				if (value)
					toolArea.ShowTool(this);
				else
					toolArea.HideTool(this);
			}
		}
		
		protected Gtk.Label label = new Gtk.Label();
		/// <value>
		/// The name of the toolbox.
		/// </value>
		public new string Name
		{
			get {return label.Text;}
			set {label.Text = value;}
		}
		
		/// <summary>
		/// The shelves.
		/// </summary>
		protected List<ToolShelf> shelves = new List<ToolShelf>();
		
		/// <summary>
		/// Create a tool shelf associated with this tool box.
		/// </summary>
		/// <param name="name"> The name of the shelf. </param>
		/// <returns> A <see cref="ToolShelf"/>. </returns>
		public ToolShelf CreateShelf(string name)
		{
			ToolShelf shelf = new ToolShelf(this, name);
			shelves.Add(shelf);
			shelf.Orientation = Orientation;
			PackStart(shelf, false, true, 0);
			shelf.VisibilityChanged += ShelfVisibilityChanged;
			ShelfVisibilityChanged(shelf); // make this shelf visible
			return shelf;
		}
		
		
		public override Gtk.Orientation Orientation
		{
			set
			{
				base.Orientation = value;
				
				if (orientation == Gtk.Orientation.Horizontal)
					label.Angle = 90;
				else
					label.Angle = 0;
				
				ToolShelf visibleShelf = null;
				foreach (ToolShelf shelf in shelves)
				{
					shelf.Orientation = orientation;
					if (shelf.ShelfVisible)
						visibleShelf = shelf;
				}
				
				ShowAll();
				
				if (visibleShelf != null)
					ShelfVisibilityChanged(visibleShelf);
			}
		}

		/// <summary>
		/// Handles a shelve's visibility being changed.
		/// </summary>
		/// <param name="shelf"> A <see cref="ToolShelf"/>. </param>
		private void ShelfVisibilityChanged(ToolShelf shelf)
		{
			bool visible = shelf.ShelfVisible;
			foreach (ToolShelf shelf_ in shelves)
			{
				if (shelf_ != shelf)
					shelf_.ShelfVisible = !visible;
			}
		}
		
	}
}
