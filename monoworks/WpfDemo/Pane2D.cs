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
using MonoWorks.Rendering.Interaction;
using MonoWorks.Plotting;
using MonoWorks.WpfBackend;
using MonoWorks.WpfBackend.PlotControls;

namespace MonoWorks.DemoWpf
{
	/// <summary>
	/// Contains the 2D portion of the plotting demo.
	/// </summary>
	public class Pane2D : PaneBase
	{

		public Pane2D()
			: base()
		{
			// hide the first column
			this.ColumnDefinitions[0].Width = new GridLength(0);

			// create the axes
			axesBox = new TestAxes2D();
			axesBox.Legend = controller.Legend;

			// add the test axes
			viewport.RenderList.AddActor(axesBox);
			viewport.Camera.Projection = Projection.Parallel;
			viewport.Camera.SetViewDirection(ViewDirection.Front);

			DockViewport();

		}

	}
}
