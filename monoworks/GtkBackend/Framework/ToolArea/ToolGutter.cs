// ToolGutter.cs - Slate Mono Application Framework
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

namespace MonoWorks.GtkBackend.Framework.Tools
{

	/// <summary>
	/// Data structure to hold the position of a tool.
	/// </summary>
	struct ToolPos : IComparable
	{
		public int Offset;
		public int Length;
		
		public HandleBox HandleBox;
		

		public int CompareTo(object obj)
		{
			return Offset - ((ToolPos)obj).Offset;
		}

	}
	
	
	/// <summary>
	/// The tool gutter sits along the edges of the tool area and hold toolbars.
	/// </summary>
	public class ToolGutter : Gtk.Fixed
	{

		/// <summary>
		/// Default constructor.
		/// </summary>
		/// <param name="orientation"> The gutter's orientation.</param>
		public ToolGutter(ToolArea toolArea, Gtk.Orientation orientation)
			: base()
		{
			this.toolArea = toolArea;
			this.orientation = orientation;
		}

		protected ToolArea toolArea;
		/// <summary>
		/// The tool area that this gutter belongs to.
		/// </summary>
		public ToolArea ToolArea
		{
			get {return toolArea;}
		}

		protected Gtk.Orientation orientation;
		/// <summary>
		/// The gutter orientation.
		/// </summary>
		public Gtk.Orientation Orientation
		{
			get { return orientation; }
		}

		
#region Tools

		protected List<HandleBox> handleBoxes = new List<HandleBox>();

		/// <summary>
		/// Adds a tool to the gutter.
		/// </summary>
		/// <param name="tool"></param>
		/// <remarks> Automatically creates a handle box.</remarks>
		public void AddTool(ITool tool)
		{
			tool.ToolArea = toolArea;
			HandleBox handleBox = new HandleBox(tool);	
			AddHandleBox(handleBox);
//			LayoutTools();
		}

		/// <summary>
		/// Removes a tool and strips its HandleBox.
		/// </summary>
		/// <param name="tool"> </param>
		public void RemoveTool(ITool tool)
		{
			HandleBox handleBox = null;
			foreach (HandleBox box in handleBoxes)
			{
				if (box.Tool == tool)
				{
					handleBox = box;
					break;
				}
			}
			if (handleBox == null)
				throw new Exception("The tool is not present in this gutter");
			
			RemoveHandleBox(handleBox);
			handleBox.RemoveTool();
		}
		
		/// <summary>
		/// Adds an already created handle box.
		/// </summary>
		/// <param name="handleBox"> A <see cref="HandleBox"/>. </param>
		public void AddHandleBox(HandleBox handleBox)
		{
			handleBox.Orientation = Orientation;
			Add(handleBox);			
			handleBoxes.Insert(0, handleBox);
			ShowAll();
		}

		/// <summary>
		/// Removes an existing handle box.
		/// </summary>
		/// <param name="handleBox"> A <see cref="HandleBox"/> to remove.
		/// </param>
		/// <remarks> handleBox should belong to the gutter.</remarks>
		public void RemoveHandleBox(HandleBox handleBox)
		{
			handleBoxes.Remove(handleBox);
			Remove(handleBox);
		}

#endregion
		
		
#region Tool Movement
		
		/// <summary>
		/// Checks for a cursor position that should tear a handle box off the gutter.
		/// </summary>
		/// <param name="x"> The x position relative to the gutter. </param>
		/// <param name="y"> The y position relative to the gutter. </param>
		/// <returns> True if the handle box should be torn off. </returns>
		protected bool CheckForTearOff(int x, int y)
		{
			if (orientation == Gtk.Orientation.Horizontal)
			{
				if (y < -Allocation.Height || y > Allocation.Height)
					return true;
			}
			else // vertical
			{
				if (x < -Allocation.Width || x > Allocation.Width)
					return true;
			}
			return false;
		}
		
		/// <summary>
		/// Called when a tool tries to move within the gutter.
		/// </summary>
		/// <param name="handleBox"> A <see cref="HandleBox"/> that's moving. </param>
		/// <param name="x"> The requested x position. </param>
		/// <param name="y"> The requested y position. </param>
		public void OnMoveRequest(HandleBox handleBox, int x, int y)
		{
			if (!handleBoxes.Contains(handleBox))
				throw new Exception("The given handle box is not in this gutter.");
			
			
			// check for tearing off
			if (CheckForTearOff(x, y))
			{
				handleBox.Float();
				return;
			}
			
			// range check the requested position
			if (orientation == Gtk.Orientation.Horizontal)
			{
				// clamp to the lower and upper bounds
				if (x < 0)
					x = 0;
				if (x > Allocation.Width - handleBox.Allocation.Width)
					x = Allocation.Width - handleBox.Allocation.Width;
			}
			else // vertical
			{
				// clamp to the lower and upper bounds
				if (y < 0)
					y = 0;
				if (y > Allocation.Height - handleBox.Allocation.Height)
					y = Allocation.Height - handleBox.Allocation.Height;
			}
			
			// get the position of all handle boxes
			ToolPos[] toolPos = new ToolPos[handleBoxes.Count];
			int count = 0;
			foreach (HandleBox box in handleBoxes)
			{
				toolPos[count].HandleBox = box;
				if (orientation == Gtk.Orientation.Horizontal)
				{
					toolPos[count].Length = box.Allocation.Width;
					if (box == handleBox)
						toolPos[count].Offset = x;
					else
						toolPos[count].Offset = box.Allocation.Left - Allocation.Left;
				}
				else // vertical
				{
					toolPos[count].Length = box.Allocation.Height;
					if (box == handleBox)
						toolPos[count].Offset = y;
					else
						toolPos[count].Offset = box.Allocation.Top - Allocation.Top;
				}
				count++;
			}	
			
			LayoutTools(toolPos);
		}
		
		/// <summary>
		/// Performs the layout so that no tools overlap.
		/// </summary>
		public void LayoutTools()
		{
			ToolPos[] toolPos = new ToolPos[handleBoxes.Count];
			int count = 0;
			foreach (HandleBox box in handleBoxes)
			{
				toolPos[count].HandleBox = box;
				if (orientation == Gtk.Orientation.Horizontal)
				{
					toolPos[count].Length = box.Allocation.Width;
					toolPos[count].Offset = box.Allocation.Left - Allocation.Left;
				}
				else // vertical
				{
					toolPos[count].Length = box.Allocation.Height;
					toolPos[count].Offset = box.Allocation.Top - Allocation.Top;
				}
				count++;
			}
			LayoutTools(toolPos);
		}
		
		/// <summary>
		/// Performs tool layout with all the tool positions already known.
		/// </summary>
		/// <param name="toolPos"> An array of <see cref="ToolPos"/>. </param>
		private void LayoutTools(ToolPos[] toolPos)
		{			
			// sort the positions in order of offset
			Array.Sort(toolPos);
			
			// ensure there is no overlapping
			int lastAvailPos = 0;
			for (int i=0; i < toolPos.Length; i++)
			{
				if (toolPos[i].Offset < lastAvailPos)
					toolPos[i].Offset = lastAvailPos + 1;
				lastAvailPos = toolPos[i].Offset + toolPos[i].Length;
							
				// move the handle box to it's new position
				if (orientation == Gtk.Orientation.Horizontal)
					Move(toolPos[i].HandleBox, toolPos[i].Offset, 0);
				else // vertical
					Move(toolPos[i].HandleBox, 0, toolPos[i].Offset);
			}
		}
		
		/// <summary>
		/// Returns true if the cursor is hitting the gutter.
		/// </summary>
		public bool CursorHitTest()
		{
			int x, y;
			GetPointer(out x, out y);
			return x >= 0 && x < Allocation.Width && y >= 0 && y < Allocation.Height;
		}
		
		/// <summary>
		/// Ensure that the layout is done when the gutter is realized.
		/// </summary>
		protected override void OnRealized()
		{
			base.OnRealized();
			LayoutTools();
			base.OnShown();
		}

		
#endregion

	}
}
