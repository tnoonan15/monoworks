// Pane3D.cs - MonoWorks Project
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

namespace MonoWorks.DemoWpf
{
	/// <summary>
	/// Contains the 3D portion of the plotting demo.
	/// </summary>
	public class Pane3D : PaneBase
	{

		public Pane3D() : base()
		{
			// create the axes
			axesBox = new TestAxes3D();

			// add the plot control
			PointPlotControl plotControl = new PointPlotControl();
			plotControl.Plot = (axesBox as TestAxes3D).PointPlot;
			plotControl.ControlUpdated += OnUpdated;
			this.AddAt(plotControl, 0, 0);


			// add the axes control
			AxesControl axesControl = new AxesControl();
			axesControl.Axes = axesBox;
			axesControl.ControlUpdated += OnUpdated;
			this.AddAt(axesControl, 1, 0);

			// add the test axes
			viewport.RenderList.AddActor(axesBox);
			viewport.Camera.SetViewDirection(ViewDirection.Standard);

			DockViewport();

		}
	}
}
