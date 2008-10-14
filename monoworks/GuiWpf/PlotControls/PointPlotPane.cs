// PointPlotPane.cs - MonoWorks Project
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

namespace MonoWorks.GuiWpf.PlotControls
{
	/// <summary>
	/// Contains controls for controlling a point plot's columns.
	/// </summary>
	public class PointPlotPane : StackPanel
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		/// <param name="plot">The plot to control.</param>
		public PointPlotPane(PointPlot plot)
			: base()
		{
			Orientation = Orientation.Vertical;
			

			this.plot = plot;

			foreach (ColumnIndex column in Enum.GetValues(typeof(ColumnIndex)))
			{
				ComboBox combo = new ComboBox();
				
				// append the column entries				
				foreach (string name in plot.DataSet.ColumnNames)
				{
					combo.AddItem(name);
				}

				// append entries specific to the parameter
				if ((int)column > 2)
				{
					combo.AddItem("---");
					switch (column)
					{
					case ColumnIndex.Color:
						foreach (string name in ColorManager.Global.Names)
							combo.AddItem(name);
						break;
					case ColumnIndex.Shape:
						foreach (string name in Enum.GetNames(typeof(PlotShape)))
							combo.AddItem(name);
						break;
					case ColumnIndex.Size:
						foreach (float val in PointPlot.PossibleMarkerSizes)
							combo.AddItem(val.ToString());
						break;
					}
				}

				combos[column] = combo;
				combo.Margin = new Thickness(8);
				combo.SelectionChanged += OnComboChanged;
				Children.Add(combo);
			}
			UpdateCombos();
		}


		/// <summary>
		/// The plot this control acts on.
		/// </summary>
		protected PointPlot plot;

		/// <summary>
		/// Combo boxes used to select the columns.
		/// </summary>
		protected Dictionary<ColumnIndex, ComboBox> combos = new Dictionary<ColumnIndex, ComboBox>();

		/// <summary>
		/// Called when one of the controls is changed.
		/// </summary>
		public event UpdateHandler ControlUpdated;

		/// <summary>
		/// Updates the selection of the point plot combos to correspond with the point plot.
		/// </summary>
		protected void UpdateCombos()
		{
			// the number of columns in the data set
			int numColumns = plot.DataSet.NumColumns;

			foreach (ColumnIndex column in combos.Keys)
			{
				if (plot[column] >= 0) // the parameter is defined by a column
				{
					combos[column].SelectedIndex = plot[column];
				}
				else // the parameter is explicitely defined
				{
					switch (column)
					{
					case ColumnIndex.Color:
						int colorIndex = ColorManager.Global.Names.IndexOf(plot.Color.Name);
						combos[column].SelectedIndex = numColumns + colorIndex + 1;
						break;

					case ColumnIndex.Shape:
						int shapeIndex = Array.IndexOf(Enum.GetValues(typeof(PlotShape)), plot.Shape);
						combos[column].SelectedIndex = numColumns + shapeIndex + 1;
						break;

					case ColumnIndex.Size:
						int sizeIndex = Array.IndexOf(PointPlot.PossibleMarkerSizes, plot.MarkerSize);
						combos[column].SelectedIndex = numColumns + sizeIndex + 1;
						break;
					}
				}
			}
		}


		/// <summary>
		/// Handles a combo box changing value.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"> </param>
		protected void OnComboChanged(object sender, SelectionChangedEventArgs e)
		{
			// get the column being changed
			ColumnIndex column = ColumnIndex.Color;
			foreach (ColumnIndex column_ in combos.Keys)
			{
				if (combos[column_] == sender)
				{
					column = column_;
					break;
				}
			}

			// the number of columns in the data set
			int numColumns = plot.DataSet.NumColumns;


			int active = combos[column].SelectedIndex; // the index of the active entry
			if (active == numColumns) // handle selecting the divider
			{
				UpdateCombos();
				return;
			}
			else if (active < numColumns) // handle selecting a column as the parameter
			{
				plot[column] = active;
			}
			else // handle parameters set explicitely
			{
				string activeName = combos[column].GetSelectedText(); // the name of the active parameter
				switch (column)
				{
				case ColumnIndex.Color:
					foreach (string colorName in ColorManager.Global.Names)
					{
						if (activeName == colorName)
						{
							plot.Color = ColorManager.Global.GetColor(colorName);
							plot[ColumnIndex.Color] = -1;
							break;
						}
					}
					break;

				case ColumnIndex.Shape:
					foreach (PlotShape shape in Enum.GetValues(typeof(PlotShape)))
					{
						if (activeName == shape.ToString())
						{
							plot.Shape = shape;
							plot[ColumnIndex.Shape] = -1;
							break;
						}
					}
					break;

				case ColumnIndex.Size:
					plot.MarkerSize = Convert.ToSingle(activeName);
					plot[ColumnIndex.Size] = -1;
					break;
				}
			}

			if (ControlUpdated != null)
				ControlUpdated();

		}


	}
}
