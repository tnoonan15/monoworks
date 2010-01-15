// ToolArea.cs - Slate Mono Application Framework
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

namespace MonoWorks.GtkBackend.Framework.Tools
{

	/// <summary>
	/// The container for the toolbars and all application content.
	/// </summary>
	public class ToolArea : Gtk.VBox
	{

		/// <summary>
		/// Default constructor.
		/// </summary>
		public ToolArea()
			: base(false, 0)
		{

			gutters = new Dictionary<ToolPosition, ToolGutter>();

			// top gutter
			gutters[ToolPosition.Top] = new ToolGutter(this, Gtk.Orientation.Horizontal);
			PackStart(gutters[ToolPosition.Top], false, true, 0);

			// an hbox to contain the left/right gutters and content
			hBox = new Gtk.HBox();
			PackStart(hBox, true, true, 0);

			// left gutter
			gutters[ToolPosition.Left] = new ToolGutter(this, Gtk.Orientation.Vertical);
			hBox.PackStart(gutters[ToolPosition.Left], false, true, 0);

			// content
			contentBin = new Gtk.Frame();
			hBox.PackStart(contentBin, true, true, 0);

			// right gutter
			gutters[ToolPosition.Right] = new ToolGutter(this, Gtk.Orientation.Vertical);
			hBox.PackStart(gutters[ToolPosition.Right], false, true, 0);

			// bottom gutter
			gutters[ToolPosition.Bottom] = new ToolGutter(this, Gtk.Orientation.Horizontal);
			PackStart(gutters[ToolPosition.Bottom], false, true, 0);
		}




#region Containers

		protected Gtk.HBox hBox;

		/// <summary>
		/// Container for the content.
		/// </summary>
		protected Gtk.Frame contentBin;

		/// <summary>
		/// The tool gutters.
		/// </summary>
		protected Dictionary<ToolPosition, ToolGutter> gutters;


		/// <summary>
		/// Adds the tool widget to the given position.
		/// </summary>
		/// <param name="tool"></param>
		/// <param name="position"></param>
		public void AddTool(ITool tool, ToolPosition position)
		{
			tool.LastPosition = position;
			gutters[position].AddTool(tool);
		}

#endregion


#region Tool Hiding

		private List<ITool> hiddenTools = new List<ITool>();

		/// <summary>
		/// Returns true if the tool is hidden.
		/// </summary>
		/// <param name="tool"> A <see cref="ITool"/>. </param>
		/// <returns> </returns>
		public bool ToolIsHidden(ITool tool)
		{
			return hiddenTools.Contains(tool);
		}

		/// <summary>
		/// Hides the given tool.
		/// </summary>
		/// <param name="tool"> A <see cref="ITool"/>. </param>
		public void HideTool(ITool tool)
		{
			gutters[tool.LastPosition].RemoveTool(tool);
			hiddenTools.Add(tool);
		}

		/// <summary>
		/// Shows the given hidden tool.
		/// </summary>
		/// <param name="tool"> A <see cref="ITool"/>. </param>
		public void ShowTool(ITool tool)
		{
			hiddenTools.Remove(tool);
			gutters[tool.LastPosition].AddTool(tool);
			gutters[tool.LastPosition].QueueDraw();
			gutters[tool.LastPosition].LayoutTools();
			gutters[tool.LastPosition].QueueDraw();
		}

#endregion

		
#region Tool Movement
		
		/// <summary>
		/// Called when a handle box is hovering.
		/// </summary>
		/// <param name="handleBox"> A <see cref="HandleBox"/> that's hovering. </param>
		/// <remarks> Decides if the tool should be put in a gutter.</remarks>
		public void OnHover(HandleBox handleBox)
		{
			// see if the cursor is over a gutter
			foreach (ToolPosition position in gutters.Keys)
			{
				if (gutters[position].CursorHitTest())
				{
					handleBox.Tool.LastPosition = position;
					handleBox.Dock(gutters[position]);
				}
			}
		}
		
#endregion


		/// <summary>
		/// The content widget.
		/// </summary>
		public Gtk.Widget Content
		{
			get { return contentBin.Child; }
			set { contentBin.Add(value); }
		}

	}
}
