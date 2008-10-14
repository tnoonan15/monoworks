// Pane2D.cs - MonoWorks Project
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
using System.Windows.Controls;

using MonoWorks.Rendering;
using MonoWorks.Plotting;
using MonoWorks.GuiWpf;
using MonoWorks.GuiWpf.PlotControls;

namespace MonoWorks.PlottingDemoWpf
{
	/// <summary>
	/// Contains the 3D portion of the plotting demo.
	/// </summary>
	public class Pane2D : DockPanel
	{

		public Pane2D()
			: base()
		{
			// create the viewport
			TooledViewport tooledViewport = new TooledViewport(ViewportUsage.Plotting);
			viewport = tooledViewport.Viewport;

			// create the axes
			TestAxes2D axes = new TestAxes2D();

			// add the control pane
			//PointPlotPane controlPane = new PointPlotPane(axes.PointPlot);
			//controlPane.ControlUpdated += new UpdateHandler(OnControlUpdated);

			// stack the widgets
			//Children.Add(controlPane);
			//DockPanel.SetDock(controlPane, Dock.Left);
			Children.Add(tooledViewport);
			DockPanel.SetDock(tooledViewport, Dock.Right);

			// add the test axes
			viewport.AddRenderable(axes);
			viewport.Camera.Projection = Projection.Parallel;
			viewport.Camera.SetViewDirection(ViewDirection.Front);
			viewport.InteractionState.Mode = InteractionMode.Select2D;
		}

		/// <summary>
		/// Repaint the viewport after a control is updated.
		/// </summary>
		void OnControlUpdated()
		{
			viewport.PaintGL();
		}

		protected Viewport viewport;
	}
}
