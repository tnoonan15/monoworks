// Legend.cs - MonoWorks Project
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

using Cairo;

using MonoWorks.Base;
using MonoWorks.Rendering;
using MonoWorks.Controls;

namespace MonoWorks.Plotting
{
	/// <summary>
	/// Legend control for plots.
	/// </summary>
	public class Legend : Expander
	{

		public Legend()
			: base()
		{
			Content = stack;
			ButtonText = "Legend";
		}

		/// <summary>
		/// The stack containing the items.
		/// </summary>
		protected Stack stack = new Stack(Orientation.Vertical);

		/// <summary>
		/// Adds an item to the legend.
		/// </summary>
		public void Add(LegendItem item)
		{
			stack.AddChild(item);
			MakeDirty();
		}

		/// <summary>
		/// Clear the legend items.
		/// </summary>
		public void Clear()
		{
			stack.Clear();
		}

		protected override void Render(RenderContext context)
		{
			base.Render(context);
			
			if (IsDirty)
				ComputeGeometry();

		}

	}
}
