// PaneBase.cs - MonoWorks Project
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

using System.Windows;
using swc = System.Windows.Controls;

using MonoWorks.Rendering;
using MonoWorks.Plotting;
using MonoWorks.GuiWpf;
using MonoWorks.GuiWpf.PlotControls;

namespace MonoWorks.DemoWpf
{
	public class PaneBase : swc.Grid
	{

		public PaneBase()
			: base()
		{
			// create the rows
			swc.RowDefinition row = new swc.RowDefinition();
			RowDefinitions.Add(row);
			row = new swc.RowDefinition();
			RowDefinitions.Add(row);

			// create the columns
			swc.ColumnDefinition col = new swc.ColumnDefinition();
			col.Width = new GridLength(200);
			ColumnDefinitions.Add(col);
			col = new swc.ColumnDefinition();
			ColumnDefinitions.Add(col);


			// create the viewport
			tooledViewport = new TooledViewport(ViewportUsage.Plotting);
			viewport = tooledViewport.Viewport;
		}

		protected void DockViewport()
		{
			this.AddAt(tooledViewport, 0, 1);
			swc.Grid.SetRowSpan(tooledViewport, 2);
		}

		/// <summary>
		/// Repaint the viewport after a control is updated.
		/// </summary>
		public void OnUpdated()
		{
			viewport.PaintGL();
		}

		protected Viewport viewport;

		protected TooledViewport tooledViewport;
	}
}
