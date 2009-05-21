// CornerToggle.cs - MonoWorks Project
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

using Cairo;

using MonoWorks.Base;
using MonoWorks.Framework;
using MonoWorks.Rendering.Events;

namespace MonoWorks.Rendering.Controls
{
	
	/// <summary>
	/// A control that contains two buttons and sits in the corner of the viewport.
	/// </summary>
	public class CornerButtons : Control2D
	{
		/// <summary>
		/// Defines the two regions of the button.
		/// </summary>
		public enum Region { None, Button1, Button2 }

		public CornerButtons(Corner corner)
		{
			Corner = corner;
			StyleClassName = "corner";
			size = MinSize;
			IsHoverable = true;
			IsTogglable = true;
		}

		/// <value>
		/// The corner on which to put the control.
		/// </value>
		public Corner Corner {get; set;}

		/// <summary>
		/// Width of the control along the edge of the viewport.
		/// </summary>
		private const double EdgeWidth = 76;
		
		public override Coord MinSize
		{
			get {return new Coord(EdgeWidth, EdgeWidth);}
		}

		/// <value>
		/// Top image.
		/// </value>
		public Image Image1 {get; set;}

		/// <value>
		/// Bottom image.
		/// </value>
		public Image Image2 {get; set;}
		
		/// <value>
		/// Whether or not the buttons will toggle when clicked.
		/// </value>
		public bool IsTogglable {get; set;}

		public override void ComputeGeometry()
		{
			base.ComputeGeometry();

			double shift = 1.1; // ratio to shift the images from the corner to put them in the right position
			
			// position the images			
			if (Image1 != null)
			{
				switch (Corner)
				{
				case Corner.NE:
					Image1.Position = new Coord(Width - (shift+1) * Image1.Width, Padding);
					break;
				case Corner.NW:
					Image1.Position = new Coord(shift * Image1.Width, Padding);
					break;
				}
			}
			if (Image2 != null)
			{
				switch (Corner)
				{
				case Corner.NE:
					Image2.Position = new Coord(Width - Padding - Image2.Width, shift * Image2.Height);
					break;
				case Corner.NW:
					Image2.Position = new Coord(Padding, shift * Image2.Height);
					break;
				}
			}
		}

		protected override void Render(RenderContext context)
		{
			base.Render(context);
			
			if (Image1 != null)
				Image1.RenderCairo(context);
			if (Image2 != null)
				Image2.RenderCairo(context);
		}

		protected override void RenderOutline()
		{
			Color fg = styleClass.GetForeground(HitState.None);
			if (fg != null)
			{
				fg.OutlineCorner(size, Corner);
			}
		}

		protected override void RenderBackground()
		{
			// the no-hitstate background is always rendered
			IFill bg = styleClass.GetBackground(HitState.None);
			if (bg != null)
				bg.DrawCorner(size, Corner);

			// apply the hovering style
			bg = styleClass.GetBackground(hitState);
			if (bg is FillGradient && hitState == HitState.Hovering && hitRegion != Region.None)
			{
				FillRegion(bg as FillGradient, hitRegion);
			}
			
			// apply the selected style
			bg = styleClass.GetBackground(HitState.Selected);
			if (bg is FillGradient && SelectedRegion != Region.None)
			{
				FillRegion(bg as FillGradient, SelectedRegion);
			}
			
		}
		
		/// <summary>
		/// Fills the given region with the given fill.
		/// </summary>
		private void FillRegion(FillGradient grad, Region region)
		{
//			gl.glBegin(gl.GL_TRIANGLES);
//			grad.StartColor.Setup();
//			switch (Corner)
//			{
//			case Corner.NE:
//				gl.glVertex2d(Width, Height);
//				grad.StopColor.Setup();
//				gl.glVertex2d(Width/2, Height/2);
//				if (region == Region.Button1)
//					gl.glVertex2d(0, Height);
//				else
//					gl.glVertex2d(Width, 0);
//				break;
//			case Corner.NW:
//				gl.glVertex2d(0, Height);
//				grad.StopColor.Setup();
//				gl.glVertex2d(Width/2, Height/2);
//				if (region == Region.Button1)
//					gl.glVertex2d(Width, Height);
//				else
//					gl.glVertex2d(0, 0);
//				break;
//			default:
//				throw new NotImplementedException();
//			}
//			gl.glEnd();
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
			switch (Corner)
			{
			case Corner.NW:
				if (dPos.X > size.X || dPos.Y > size.Y ||
					dPos.X + dPos.Y < EdgeWidth)
					return Region.None;
				else if (dPos.X > dPos.Y)
					return Region.Button2;
				return Region.Button1;
			case Corner.NE:
				if (dPos.Y / dPos.X < 1)
					return Region.None;
				else if (dPos.X + dPos.Y < EdgeWidth)
					return Region.Button2;
				return Region.Button1;
			default: 
				throw new NotImplementedException();
			}
		}

		protected Region hitRegion = Region.None;

		//// <value>
		/// The currently selected region of the button, if IsTogglable is true.
		/// </value>
		public Region SelectedRegion {get; set;}

		protected override bool HitTest(Coord pos)
		{
			hitRegion = HitRegion(pos);
			return hitRegion != Region.None;
		}

		public override void OnButtonPress(MouseButtonEvent evt)
		{
			base.OnButtonPress(evt);

			if (evt.Handled)
				return;
			
			hitRegion = HitRegion(evt.Pos);

			if (hitRegion != Region.None)
			{
				if (IsTogglable)
					SelectedRegion = hitRegion;
				
				Select();
				evt.Handle();
				if (hitRegion == Region.Button1)
					RaiseAction1();
				else
					RaiseAction2();

			}
		}


		public override void OnButtonRelease(MouseButtonEvent evt)
		{
			base.OnButtonRelease(evt);

			if (IsSelected && !IsTogglable)
				Deselect();

			// if we were just clicked, we get to handle the next button release event
			if (hitRegion != Region.None)
			{
				hitRegion = Region.None;
				evt.Handle();
			}

		}


		/// <summary>
		/// Called when the button1 button is clicked by the user.
		/// </summary>
		public event EventHandler Action1;

		/// <summary>
		/// Activates the button1 event.
		/// </summary>
		public void RaiseAction1()
		{
			if (Action1 != null)
				Action1(this, new EventArgs());
		}

		/// <summary>
		/// Called when the button2 button is clicked by the user.
		/// </summary>
		public event EventHandler Action2;

		/// <summary>
		/// Activates the button2 event.
		/// </summary>
		public void RaiseAction2()
		{
			if (Action2 != null)
				Action2(this, new EventArgs());
		}

#endregion
	}
}
