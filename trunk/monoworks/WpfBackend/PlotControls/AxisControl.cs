// AxisControl.cs - MonoWorks Project
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


namespace MonoWorks.WpfBackend.PlotControls
{
	/// <summary>
	/// Controls for a specific axis.
	/// </summary>
	public class AxisControl : swc.GroupBox
	{

		public AxisControl()
			: base()
		{

			Thickness margin = new Thickness(8);

			swc.StackPanel panel = new swc.StackPanel();
			panel.Orientation = swc.Orientation.Vertical;

			visibleCheck = new swc.CheckBox();
			visibleCheck.Content = "Visible";
			visibleCheck.Margin = margin;
			visibleCheck.Click += new RoutedEventHandler(OnvisibleChecked);
			panel.Children.Add(visibleCheck);

			Content = panel;
		}


		swc.CheckBox visibleCheck;


		protected Axis axis = null;
		/// <summary>
		/// The axis the control is controlling.
		/// </summary>
		public Axis Axis
		{
			get { return axis; }
			set
			{
				axis = value;
				RefreshControls();
			}
		}


		bool internalUpdate = false;

		public event UpdateHandler ControlUpdated;

		protected void CallUpdateEvent()
		{
			if (ControlUpdated != null)
				ControlUpdated();
		}


		/// <summary>
		/// Refreshes the controls based on the axis.
		/// </summary>
		public void RefreshControls()
		{
			if (axis == null)
				return;

			internalUpdate = true;

			visibleCheck.IsChecked = axis.IsVisible;

			internalUpdate = false;
		}

		/// <summary>
		/// Handles the axis check being checked.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void OnvisibleChecked(object sender, RoutedEventArgs e)
		{
			if (axis == null || internalUpdate)
				return;

			axis.IsVisible = (bool)visibleCheck.IsChecked;

			CallUpdateEvent();
		}


	}
}
