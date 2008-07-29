// Plottable.cs - MonoWorks Project
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

using gl = Tao.OpenGl.Gl;

using MonoWorks.Rendering;

namespace MonoWorks.Plotting
{

	/// <summary>
	/// Base class for plottable objects.
	/// </summary>
	public abstract class Plottable : Renderable
	{

		/// <summary>
		/// Default constructor.
		/// </summary>
		public Plottable(AxesBox parent)
			: base()
		{
			if (parent != null)
				parent.AddChild(this);
			displayList = gl.glGenLists(1);
		}

		~Plottable()
		{
			// Windows doesn't like this
			//ClearDisplayList();
		}

		protected AxesBox parent = null;
		/// <value>
		/// The parent axes.
		/// </value>
		public AxesBox Parent
		{
			get { return parent; }
			set { parent = value; }
		}



		protected Color color = new Color();
		/// <summary>
		/// The color.
		/// </summary>
		public Color Color
		{
			get { return color; }
			set { color = value; }
		}




		#region Bounds

		/// <summary>
		/// Updates the plot bounds based on the children.
		/// </summary>
		public virtual void UpdateBounds()
		{

		}

		protected Bounds plotBounds = new Bounds();
		/// <summary>
		/// The bounds of the plottable in plot space.
		/// </summary>
		/// <remarks> This differs from bounds, which is in rendering space.</remarks>
		public Bounds PlotBounds
		{
			get { return plotBounds; }
			set { plotBounds = value; }
		}

		#endregion



		#region Geometry

		/// <summary>
		/// Each plottable gets a display list.
		/// </summary>
		protected int displayList = 0;

		/// <summary>
		/// Ensures the display list is cleared.
		/// </summary>
		/// <remarks> Does nothing if this list hasn't been set yet.</remarks>
		public void ClearDisplayList()
		{
			if (gl.glIsList(displayList) != 0)
			{
				gl.glDeleteLists(displayList, 1);
			}
		}

		public override void ComputeGeometry()
		{
			base.ComputeGeometry();
			ClearDisplayList();

		}

		#endregion



		#region Rendering


		public override void RenderOpaque(IViewport viewport)
		{
			base.RenderOpaque(viewport);

			color.Setup();
		}


		public override void RenderOverlay(IViewport viewport)
		{
			base.RenderOverlay(viewport);

			color.Setup();
		}

		#endregion


	}
}
