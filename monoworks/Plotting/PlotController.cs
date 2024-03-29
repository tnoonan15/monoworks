// PlotController.cs - MonoWorks Project
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


using MonoWorks.Base;
using MonoWorks.Rendering;
using MonoWorks.Controls;
using MonoWorks.Controls.World;

namespace MonoWorks.Plotting
{
	public class PlotController : WorldController<Scene>
	{

		public PlotController(Scene scene)
			: base(scene)
		{
			Legend = new Legend();
			ContextLayer.AnchorControl(Legend, AnchorLocation.NE);

			Context(Side.N, "ShortViewToolbar");
			Context(Side.N, "ExportToolbar");
		}

		/// <summary>
		/// The plot legend.
		/// </summary>
		public Legend Legend { get; private set; }


	}
}
