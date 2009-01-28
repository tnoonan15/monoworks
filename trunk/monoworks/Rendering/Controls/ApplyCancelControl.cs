// ApplyCancelControl.cs - MonoWorks Project
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

namespace MonoWorks.Rendering.Controls
{
	/// <summary>
	/// Control containing an apply and cancel button that goes in the corner of a viewport.
	/// </summary>
	public class ApplyCancelControl : Control
	{

		public ApplyCancelControl()
		{
			StyleClassName = "applycancel";
			size = MinSize;
		}

		/// <summary>
		/// Width of the control along the edge of the viewport.
		/// </summary>
		private const double EdgeWidth = 64;


		public override Coord MinSize
		{
			get {return new Coord(EdgeWidth, EdgeWidth);}
		}

		public override void OnViewportResized(Viewport viewport)
		{
			base.OnViewportResized(viewport);

			position = new Coord(viewport.WidthGL, viewport.HeightGL) - size;
			Console.WriteLine("apply-cancel moved to {0} with size {1}", position, size);
		}


		public override void ComputeGeometry()
		{
			base.ComputeGeometry();
		}

		public override void RenderOverlay(Viewport viewport)
		{
			base.RenderOverlay(viewport);

			RenderBackground();

			RenderOutline();
		}

		protected override void RenderOutline()
		{
			Color fg = styleClass.GetForeground(hitState);
			if (fg != null)
			{
				fg.Setup();
				gl.glLineWidth(1f);
				gl.glBegin(gl.GL_LINE_LOOP);
				(position + new Coord(Width, 0)).glVertex();
				(position + size).glVertex();
				(position + new Coord(0, Height)).glVertex();
				gl.glEnd();
			}
		}

		protected override void RenderBackground()
		{

			IFill bg = styleClass.GetBackground(hitState);
			if (bg != null)
				bg.DrawCorner(position, size, Corner.NE);

		}

	}
}
