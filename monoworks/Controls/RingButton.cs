// 
//  RingButton.cs - MonoWorks Project
//  
//  Author:
//       Andy Selvig <ajselvig@gmail.com>
// 
//  Copyright (c) 2010 Andy Selvig
// 
//  This library is free software; you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as
//  published by the Free Software Foundation; either version 2.1 of the
//  License, or (at your option) any later version.
// 
//  This library is distributed in the hope that it will be useful, but
//  WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
//  Lesser General Public License for more details.
// 
//  You should have received a copy of the GNU Lesser General Public
//  License along with this library; if not, write to the Free Software
//  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA

using System;

using MonoWorks.Base;
using MonoWorks.Rendering;

namespace MonoWorks.Controls
{
	/// <summary>
	/// A single button in a ring bar.
	/// </summary>
	public class RingButton : AbstractButton
	{

		/// <summary>
		/// Default constructor.
		/// </summary>
		public RingButton() : base()
		{
		}
		
		/// <summary>
		/// Create a button with the given text.
		/// </summary>
		public RingButton(string text) : base(text)
		{
		}

		/// <summary>
		/// Create a button with the given label.
		/// </summary>
		public RingButton(Label label) : base(label)
		{
		}

		/// <summary>
		/// Create a button with the given image.
		/// </summary>
		public RingButton(Image image) : base(image)
		{
		}

		/// <summary>
		/// Creates a button with the given label text and image.
		/// </summary>
		public RingButton(string text, Image image) : base(text, image)
		{
		}

		/// <summary>
		/// Create a button with the given label and image.
		/// </summary>
		public RingButton(Label label, Image image) : base(label, image)
		{
		}


		#region Geometry

		/// <summary>
		/// The angle around the parent ring where the center of the button is.
		/// </summary>
		internal Angle CenterAngle { get; set; }


		public override void ComputeGeometry()
		{
			base.ComputeGeometry();

			// can't do anything if we're not inside a ring bar
			if (!(ParentControl is RingBar))
				return;
			var ringBar = ParentControl as RingBar;

			// place the image
			if (Image != null)
			{
				if (Image.IsDirty)
					Image.ComputeGeometry();
				var imageCenter = new Coord((ringBar.OuterRadius + ringBar.InnerRadius - 2) / 2, 0).Rotate(CenterAngle);
				Image.Origin.X = ringBar.OuterRadius + imageCenter.X - Image.RenderWidth / 2;
				Image.Origin.Y = ringBar.OuterRadius - imageCenter.Y - Image.RenderHeight / 2;
			}
		}

		public override bool HitTest(Coord pos)
		{
			if (!(ParentControl is RingBar) || LastPosition == null)
				return false;
			var ringBar = ParentControl as RingBar;

			var r = pos - LastPosition - new Coord(ringBar.OuterRadius, ringBar.OuterRadius);
			var angle = r.AngleTo(new Coord(1, 0));
			if (angle.Degrees < -ringBar.DiffAngle.Degrees/2.0)
				angle += Angle.TwoPi;
			//Console.WriteLine("hit at r={0}, angle={1}", r.Magnitude, angle.Degrees);
			return r.MagnitudeSquared <= ringBar.OuterRadius*ringBar.OuterRadius && 
				r.MagnitudeSquared >= ringBar.InnerRadius*ringBar.InnerRadius && 
				Math.Abs((angle-CenterAngle).Radians) <= ringBar.DiffAngle.Radians/2;
		}

		protected override void OnEnter(MonoWorks.Rendering.Events.MouseEvent evt)
		{
			base.OnEnter(evt);

			Console.WriteLine("entered " + Name);
		}

		#endregion


		#region Rendering
		
		/// <summary>
		/// Draws the outline of the button to the given context.
		/// </summary>
		/// <remarks>This can be used by decorators to avoid doing the geometry themselves.</remarks>
		public void DrawOutline(Cairo.Context cr)
		{

			// can't do anything if we're not inside a ring bar
			if (!(ParentControl is RingBar))
				return;
			var ringBar = ParentControl as RingBar;
			var outerRadius = ringBar.OuterRadius;

			var startAngle = CenterAngle - ringBar.DiffAngle / 2;
			var stopAngle = startAngle + ringBar.DiffAngle;
			var inner = new Coord(ringBar.InnerRadius, 0).Rotate(startAngle);
			var outer = new Coord(ringBar.OuterRadius-2, 0).Rotate(startAngle);
			cr.MoveTo(outerRadius + inner.X, outerRadius + inner.Y);
			cr.LineTo(outerRadius + outer.X, outerRadius + outer.Y);
			cr.Arc(outerRadius, outerRadius, outerRadius-2, startAngle.Radians, stopAngle.Radians);
			var rotatedInner = inner.Rotate(ringBar.DiffAngle);
			cr.LineTo(outerRadius + rotatedInner.X, outerRadius + rotatedInner.Y);
			cr.ArcNegative(outerRadius, outerRadius, ringBar.InnerRadius, stopAngle.Radians, startAngle.Radians);
		}

		#endregion

	}
}
