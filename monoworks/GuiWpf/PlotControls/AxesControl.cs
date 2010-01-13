// AxesControl.cs - MonoWorks Project
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
	/// Contains controls for an AxesBox.
	/// </summary>
	public class AxesControl : swc.StackPanel
	{

		public AxesControl()
			: base()
		{
			Orientation = swc.Orientation.Vertical;


			Thickness margin = new Thickness(8);

			// the title control
			swc.StackPanel stack_ = new swc.StackPanel();
			stack_.Orientation = swc.Orientation.Horizontal;
			swc.Label label_ = new swc.Label();
			label_.Content = "Title: ";
			stack_.Children.Add(label_);
			titleBox = new swc.TextBox();
			titleBox.MinWidth = 100;
			stack_.Children.Add(titleBox);
			Children.Add(stack_);
			titleBox.TextChanged += OnTitleChanged;

			// the grid check box
			gridCheck = new swc.CheckBox();
			gridCheck.Content = "Grid Visible";
			gridCheck.Margin = margin;
			gridCheck.Click += OnGridChecked;
			Children.Add(gridCheck);

			// create the axis controls
			for (int i = 0; i < axisControls.Length; i++)
			{
				axisControls[i] = new AxisControl();
				axisControls[i].ControlUpdated += CallUpdateEvent;
				Children.Add(axisControls[i]);
			}
			axisControls[0].Header = "X Axis";
			axisControls[1].Header = "Y Axis";
			axisControls[2].Header = "Z Axis";
		}

		protected AxesBox axes = null;
		/// <summary>
		/// The axes box currently being controlled.
		/// </summary>
		public AxesBox Axes
		{
			get { return axes; }
			set
			{
				axes = value;
				for (int i = 0; i < axisControls.Length; i++)
				{
					axisControls[i].Axis = axes.Axes[i];
				}
				RefreshControls();
			}
		}



		#region The controls

		swc.TextBox titleBox;
		
		swc.CheckBox gridCheck;

		AxisControl[] axisControls = new AxisControl[3];

		bool internalUpdate = false;


		/// <summary>
		/// Refreshes the controls based on the axes.
		/// </summary>
		public void RefreshControls()
		{
			if (axes == null)
				return;

			internalUpdate = true;

			titleBox.Text = axes.Title;
			gridCheck.IsChecked = axes.GridVisible;

			for (int i = 0; i < axisControls.Length; i++)
				axisControls[i].RefreshControls();

			internalUpdate = false;
		}


		#endregion



		public event UpdateHandler ControlUpdated;
		
		protected void CallUpdateEvent()
		{
			if (ControlUpdated != null)
				ControlUpdated();
		}

		/// <summary>
		/// Handles the user changing the title.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void OnTitleChanged(object sender, swc.TextChangedEventArgs e)
		{
			if (axes == null || internalUpdate)
				return;

			axes.Title = titleBox.Text;

			CallUpdateEvent();
		}



		/// <summary>
		/// Handles the grid check being checked.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void OnGridChecked(object sender, RoutedEventArgs e)
		{
			if (axes == null || internalUpdate)
				return;

			axes.GridVisible = (bool)gridCheck.IsChecked;

			CallUpdateEvent();
		}



	}
}
