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
using MonoWorks.Controls;

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
			Icon = new LegendIcon();
			Add(Icon);

			label = new Label("item");
			Add(label);
		}

		protected Label label;

		/// <summary>
		/// The item text.
		/// </summary>
		public string Text
		{
			get { return label.Body; }
			set
			{
				label.Body = value;
				MakeDirty();
			}
		}

		/// <summary>
		/// Privatize adding children. Outsiders don't get to do this.
		/// </summary>
		protected void Add(Control2D child)
		{
			base.AddChild(child);
		}

		/// <summary>
		/// The icon used to show the plot settings for this item.
		/// </summary>
		public LegendIcon Icon { get; private set; }

	}
}
