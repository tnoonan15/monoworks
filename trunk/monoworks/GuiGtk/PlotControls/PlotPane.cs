// ControlPane.cs - MonoWorks Project
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

using MonoWorks.Rendering;
using MonoWorks.Plotting;

namespace MonoWorks.GuiGtk
{
		
	/// <summary>
	/// Delegate for handling state changed events for custom controls.
	/// </summary>
	public delegate void ControlChangedHandler();
	
	/// <summary>
	/// Pane that contains controls for the 
	/// </summary>
	public class PlotPane : Gtk.VBox
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public PlotPane(TestAxes axes) : base()
		{
			testAxes = axes;
			
			// create the point plot pane
			PointPlotPane pointPane = new PointPlotPane(axes.PointPlot);
			PackStart(pointPane, false, true, 6);
			pointPane.ControlChanged += OnControlChanged;
		}
		
		/// <summary>
		/// The test axes.
		/// </summary>
		protected TestAxes testAxes;
		
		
		/// <summary>
		/// Gets called when the state of any of the child controls has changed.
		/// </summary>
		public event ControlChangedHandler ControlChanged;
		
		/// <summary>
		/// Handler for state changed events from the child controls.
		/// </summary>
		protected void OnControlChanged()
		{
			ControlChanged();
		}
		
	}
}
