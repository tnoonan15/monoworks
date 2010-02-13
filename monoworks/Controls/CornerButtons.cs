// CornerButtons.cs - MonoWorks Project
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
using MonoWorks.Rendering;
using MonoWorks.Rendering.Events;

namespace MonoWorks.Controls
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
			RenderSize = MinSize;
			IsHoverable = true;
			IsTogglable = false;
		}

		/// <value>
		/// The corner on which to put the control.
		/// </value>
		public Corner Corner { get; set; }

		/// <summary>
		/// Width of the control along the edge of the viewport.
		/// </summary>
		private const double EdgeWidth = 76;

		/// <value>
		/// Top image.
		/// </value>
		public Image Image1 { get; set; }

		/// <value>
		/// Bottom image.
		/// </value>
		public Image Image2 { get; set; }

		/// <value>
		/// Whether or not the buttons will toggle when clicked.
		/// </value>
		public bool IsTogglable { get; set; }

		public override void ComputeGeometry()
		{
			base.ComputeGeometry();

			double shift = 1.1; // ratio to shift the images from the corner to put them in the right position

			MinSize = new Coord(EdgeWidth, EdgeWidth);
			RenderSize = MinSize;

			// position the images			
			if (Image1 != null)
			{
				Image1.ComputeGeometry();
				switch (Corner)
				{
				case Corner.NE:
					Image1.Origin = new Coord(RenderWidth - (shift + 1) * Image1.RenderWidth, Padding);
					break;
				case Corner.NW:
					Image1.Origin = new Coord(shift * Image1.RenderWidth, Padding);
					break;
				}
			}
			if (Image2 != null)
			{
				Image2.ComputeGeometry();
				switch (Corner)
				{
				case Corner.NE:
					Image2.Origin = new Coord(RenderWidth - Padding - Image2.RenderWidth, shift * Image2.RenderHeight);
					break;
				case Corner.NW:
					Image2.Origin = new Coord(Padding, shift * Image2.RenderHeight);
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


		#region Mouse Interaction

		/// <summary>
		/// Tells which region the mouse is in.
		/// </summary>
		/// <param name="pos"></param>
		/// <returns></returns>
		protected Region HitRegion(Coord pos)
		{
			if (LastPosition == null)
				return Region.None;
			Coord dPos = pos - LastPosition;
			switch (Corner)
			{
			case Corner.NW:
				if (dPos.X > RenderSize.X || dPos.Y > RenderSize.Y ||
					dPos.X + dPos.Y > EdgeWidth || dPos.X < 0 || dPos.Y < 0)
					return Region.None;
				return dPos.X > dPos.Y ? Region.Button1 : Region.Button2;
			case Corner.NE:
				if (dPos.Y / dPos.X > 1 || dPos.X < 0 || dPos.Y < 0)
					return Region.None;
				else if (dPos.X + dPos.Y < EdgeWidth)
					return Region.Button1;
				return Region.Button2;
			default:
				throw new NotImplementedException();
			}
		}

		protected Region hitRegion = Region.None;

		/// <value>
		/// The hit state of Region1.
		/// </value>
		public HitState HitState1 { get; set; }

		/// <value>
		/// The hit state of Region2.
		/// </value>
		public HitState HitState2 { get; set; }

		/// <summary>
		/// Selects the given region (and deselects the other one).
		/// </summary>
		public void Select(Region region)
		{
			Select();
			if (region == Region.Button1)
			{
				HitState1 |= HitState.Selected;
				HitState2 &= ~HitState.Selected;
			}
			else if (region == Region.Button2)
			{
				HitState2 |= HitState.Selected;
				HitState1 &= ~HitState.Selected;
			}
			else // nothing is selected
			{
				HitState1 &= ~HitState.Selected;
				HitState2 &= ~HitState.Selected;
			}
		}

		/// <summary>
		/// Sets the hovering state on a region (and unsets it on the other one).
		/// </summary>
		protected void Hover(Region region)
		{
			if (region == Region.Button1)
			{
				if ((HitState1 &= HitState.Hovering) == 0) // wasn't hovering before
					MakeDirty();
				HitState1 |= HitState.Hovering;
				HitState2 &= ~HitState.Hovering;
			}
			else if (region == Region.Button2)
			{
				if ((HitState2 &= HitState.Hovering) == 0) // wasn't hovering before
					MakeDirty();
				HitState2 |= HitState.Hovering;
				HitState1 &= ~HitState.Hovering;
			}
			else // nothing is hovering
			{
				HitState1 &= ~HitState.Hovering;
				HitState2 &= ~HitState.Hovering;
			}
		}

		public override void Deselect()
		{
			base.Deselect();

			HitState1 &= ~HitState.Selected;
			HitState2 &= ~HitState.Selected;
		}


		public override bool HitTest(Coord pos)
		{
			hitRegion = HitRegion(pos);
			Hover(hitRegion);
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
				{
					if (IsSelected)
						Deselect();
				}

				Select(hitRegion);

				evt.Handle();
				if (hitRegion == Region.Button1)
					RaiseAction1();
				else
					RaiseAction2();

				MakeDirty();
			}
		}


		public override void OnButtonRelease(MouseButtonEvent evt)
		{
			base.OnButtonRelease(evt);

			if (IsSelected && !IsTogglable)
			{
				Deselect();
			}

			// if we were just clicked, we get to handle the next button release event
			if (hitRegion != Region.None)
			{
				hitRegion = Region.None;
				evt.Handle();
				MakeDirty();
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
