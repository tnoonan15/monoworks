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

using MonoWorks.Rendering;
using MonoWorks.Plotting;

namespace MonoWorks.GtkBackend
{
	
	/// <summary>
	/// Pane that holds controls for a point plot.
	/// </summary>
	public class PointPlotPane : Gtk.Frame
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		/// <param name="plot"> A <see cref="PointPlot"/> to control. </param>
		public PointPlotPane(PointPlot plot) : base("Point Plot")
		{
			this.plot = plot;
			
			Gtk.Table pointTable = new Gtk.Table((uint)Enum.GetValues(typeof(ColumnIndex)).Length, 2, false);
			Add(pointTable);
			
			uint count = 0;
			foreach (ColumnIndex column in Enum.GetValues(typeof(ColumnIndex)))
			{
				pointTable.Attach(new Gtk.Label(column.ToString()), 0, 1, count, count+1);
				Gtk.ComboBox combo = Gtk.ComboBox.NewText();
				combos[column] = combo;
				combo.Changed += OnComboChanged;
				pointTable.Attach(combo, 1, 2, count, count+1);
				count++;
				
				// append the column entries				
				foreach (string name in plot.DataSet.ColumnNames)
					combo.AppendText(name);
				
				// append the entries specific to the parameter
				if ((int)column > 2)
				{
					combo.AppendText("---");
					switch (column)
					{
					case ColumnIndex.Color:
						foreach (string name in ColorManager.Global.Names)
							combo.AppendText(name);
						break;
					case ColumnIndex.Shape:
						foreach (string name in Enum.GetNames(typeof(PlotShape)))
							combo.AppendText(name);
						break;
					case ColumnIndex.Size:
						foreach (float val in PointPlot.PossibleMarkerSizes)
							combo.AppendText(val.ToString());
						break;
					}
				}
			}
			
			UpdateCombos();
		}
		
		

		/// <summary>
		/// The plot this control is controlling.
		/// </summary>
		protected PointPlot plot;
		
		/// <summary>
		/// Gets called when the plot state changed.
		/// </summary>
		public event ControlChangedHandler ControlChanged;
		
		/// <summary>
		/// The combo boxes to select the column indices.
		/// </summary>
		protected Dictionary<ColumnIndex,Gtk.ComboBox> combos = new Dictionary<ColumnIndex,Gtk.ComboBox>();
		
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
					combos[column].Active = plot[column];
				}
				else // the parameter is explicitely defined
				{
					switch (column)
					{
					case ColumnIndex.Color:
						int colorIndex = ColorManager.Global.Names.IndexOf(plot.Color.Name);
						combos[column].Active = numColumns + colorIndex + 1;
						break;
						
					case ColumnIndex.Shape:
						int shapeIndex = Array.IndexOf(Enum.GetValues(typeof(PlotShape)), plot.Shape);
						combos[column].Active = numColumns + shapeIndex + 1;
						break;
						
					case ColumnIndex.Size:
						int sizeIndex = Array.IndexOf(PointPlot.PossibleMarkerSizes, plot.MarkerSize);
						combos[column].Active = numColumns + sizeIndex + 1;
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
		protected void OnComboChanged(object sender, EventArgs args)
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

			
			int active = combos[column].Active; // the index of the active entry
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
				string activeName = combos[column].ActiveText; // the name of the active parameter
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
			
			if (ControlChanged != null)
				ControlChanged();
			
		}
		
		
	}
}
