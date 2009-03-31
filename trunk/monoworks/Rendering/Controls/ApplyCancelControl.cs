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
using MonoWorks.Framework;
using MonoWorks.Rendering.Events;

namespace MonoWorks.Rendering.Controls
{
	/// <summary>
	/// Control containing an apply and cancel button that goes in the corner of a viewport.
	/// </summary>
	public class ApplyCancelControl : Control
	{
		/// <summary>
		/// Defines the two regions of the button.
		/// </summary>
		protected enum Region { None, Apply, Cancel }

		public ApplyCancelControl()
		{
			StyleClassName = "applycancel";
			size = MinSize;
			IsHoverable = true;

			applyImage = new Image(ResourceHelper.GetStream("apply.png"));
			cancelImage = new Image(ResourceHelper.GetStream("cancel.png"));
		}


		/// <summary>
		/// Width of the control along the edge of the viewport.
		/// </summary>
		private const double EdgeWidth = 76;
		
		public override Coord MinSize
		{
			get {return new Coord(EdgeWidth, EdgeWidth);}
		}

		protected Image applyImage;

		protected Image cancelImage;

		public override void OnViewportResized(Viewport viewport)
		{
			base.OnViewportResized(viewport);

			Coord corner = new Coord(viewport.WidthGL, viewport.HeightGL);
			Position = corner - size;

			// position the images
			applyImage.Position = corner - new Coord(2.2 * applyImage.MinWidth, applyImage.MinHeight + 4);
			cancelImage.Position = corner - new Coord(cancelImage.MinWidth + 4, 2.2 * cancelImage.MinWidth);
		}


		public override void ComputeGeometry()
		{
			base.ComputeGeometry();
		}

		protected override void Render(Viewport viewport)
		{
			base.Render(viewport);

			RenderBackground();

			RenderOutline();

			applyImage.RenderOverlay(viewport);
			cancelImage.RenderOverlay(viewport);
		}

		protected override void RenderOutline()
		{
			Color fg = styleClass.GetForeground(HitState.None);
			if (fg != null)
			{
				fg.Setup();
				gl.glLineWidth(1f);
				gl.glBegin(gl.GL_LINE_LOOP);
				(Position + new Coord(Width, 0)).glVertex();
				(Position + size).glVertex();
				(Position + new Coord(0, Height)).glVertex();
				gl.glEnd();
			}
		}

		protected override void RenderBackground()
		{
			// the no-hitstate background is always rendered
			IFill bg = styleClass.GetBackground(HitState.None);
			if (bg != null)
				bg.DrawCorner(Position, size, Corner.NE);

			// now apply other styles
			bg = styleClass.GetBackground(hitState);
			if (bg is FillGradient && hitState != HitState.None && hitRegion != Region.None)
			{
				FillGradient grad = bg as FillGradient;
				gl.glBegin(gl.GL_TRIANGLES);
				grad.StartColor.Setup();
				(Position + size).glVertex();
				grad.StopColor.Setup();
				(Position + size/2).glVertex();
				if (hitRegion == Region.Apply)
					(Position + new Coord(0, EdgeWidth)).glVertex();
				else // cancel
					(Position + new Coord(EdgeWidth, 0)).glVertex();
				gl.glEnd();
			}
		}



#region Mouse Interaction

		/// <summary>
		/// Tells which region the mouse is in.
		/// </summary>
		/// <param name="pos"></param>
		/// <returns></returns>
		protected Region HitRegion(Coord pos)
		{
			Coord dPos = pos - LastPosition;
			if (dPos.X > size.X || dPos.Y > size.Y ||
				dPos.X + dPos.Y < EdgeWidth)
				return Region.None;
			else if (dPos.X > dPos.Y)
				return Region.Cancel;
			else
				return Region.Apply;
		}

		protected Region hitRegion = Region.None;


		protected override bool HitTest(Coord pos)
		{
			hitRegion = HitRegion(pos);
			return hitRegion != Region.None;
		}

		public override void OnButtonPress(MouseButtonEvent evt)
		{
			base.OnButtonPress(evt);

			hitRegion = HitRegion(evt.Pos);

			if (hitRegion != Region.None)
			{
				ToggleSelection();
				evt.Handle();
				if (hitRegion == Region.Apply)
					RaiseApply();
				else
					RaiseCancel();

			}
		}


		public override void OnButtonRelease(MouseButtonEvent evt)
		{
			base.OnButtonRelease(evt);

			if (IsSelected)
				Deselect();

			// if we were just clicked, we get to handle the next button release event
			if (hitRegion != Region.None)
			{
				hitRegion = Region.None;
				evt.Handle();
			}

		}


		/// <summary>
		/// Called when the apply button is clicked by the user.
		/// </summary>
		public event EventHandler Apply;

		/// <summary>
		/// Activates the apply event.
		/// </summary>
		public void RaiseApply()
		{
			if (Apply != null)
				Apply(this, new EventArgs());
		}

		/// <summary>
		/// Called when the cancel button is clicked by the user.
		/// </summary>
		public event EventHandler Cancel;

		/// <summary>
		/// Activates the cancel event.
		/// </summary>
		public void RaiseCancel()
		{
			if (Cancel != null)
				Cancel(this, new EventArgs());
		}

#endregion


	}
}
