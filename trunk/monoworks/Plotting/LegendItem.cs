// LegendItem.cs - MonoWorks Project
//
//  Copyright (C) 2009 Andy Selvig
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

using MonoWorks.Base;
using MonoWorks.Rendering;
using MonoWorks.Rendering.Controls;

namespace MonoWorks.Plotting
{
	/// <summary>
	/// An item in a legend.
	/// </summary>
	public class LegendItem : Stack
	{

		public LegendItem()
			: base(Orientation.Horizontal)
		{
			label = new Label("item");
			Add(label);
		}

		/// <summary>
		/// Privatize adding children. Outsiders don't get to do this.
		/// </summary>
		protected new void Add(Control child)
		{
			base.Add(child);
		}

#region Attributes
		
		protected Label label;

		/// <summary>
		/// The item text.
		/// </summary>
		public string Text
		{
			get { return label.Text; }
			set
			{
				label.Text = value;
				MakeDirty();
			}
		}

		/// <summary>
		/// The color of the marker.
		/// </summary>
		public Color Color { get; set; }

		/// <summary>
		/// The shape of the marker.
		/// </summary>
		public PlotShape MarkerShape { get; set; }

		/// <summary>
		/// The size of the marker.
		/// </summary>
		public float MarkerSize { get; set; }

		/// <summary>
		/// The line width.
		/// </summary>
		public float LineWidth { get; set; }

		/// <summary>
		/// The line style.
		/// </summary>
		public LineStyle LineStyle { get; set; }

		/// <summary>
		/// Whether or not to show the line.
		/// </summary>
		public bool ShowLine { get; set; }

		/// <summary>
		/// Whether or not to show the marker.
		/// </summary>
		public bool ShowMarker { get; set; }

#endregion

	}
}
