// Axes.cs - MonoWorks Project
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

using MonoWorks.Base;
using MonoWorks.Rendering;

namespace MonoWorks.Plotting
{

	/// <summary>
	/// A single axis (a part of an axes box).
	/// </summary>
	public class Axis : Plottable
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		/// <param name="parent"></param>
		public Axis(AxesBox parent)
			: base(parent)
		{
		}


		protected ScreenCoord start = new ScreenCoord();
		/// <summary>
		/// The starting position of the axis.
		/// </summary>
		public ScreenCoord Start
		{
			get { return start; }
			set { start = value; }
		}

		protected ScreenCoord stop = new ScreenCoord();
		/// <summary>
		/// The stopping position of the axes.
		/// </summary>
		public ScreenCoord Stop
		{
			get { return stop; }
			set { stop = value; }
		}


		public override void RenderOverlay(IViewport viewport)
		{
			base.RenderOverlay(viewport);

			Console.WriteLine("rendering axis from {0} to {1}", start, stop);

			gl.glBegin(gl.GL_LINES);

			gl.glVertex2i(start.X, start.Y);
			gl.glVertex2i(stop.X, stop.Y);

			gl.glEnd();
		}
	

	}
}
