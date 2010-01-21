// 
//  MenuItem.cs - MonoWorks Project
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
	/// An item in an Menu.
	/// </summary>
	public class MenuItem : Control2D, IStringParsable
	{
		public MenuItem()
		{
		}
		
		/// <summary>
		/// The text portion of the item.
		/// </summary>
		[MwxProperty]
		public string Text {
			get;
			set;
		}
		
		/// <summary>
		/// Parses the text out of a string.
		/// </summary>
		public void Parse(string valString)
		{
			Text = valString;
		}


		/// <summary>
		/// A dummy surface used by Cairo to compute the text extents.
		/// </summary>
		private static readonly Cairo.ImageSurface DummySurface = new Cairo.ImageSurface(Cairo.Format.ARGB32, 128, 128);

		public override void ComputeGeometry()
		{
			base.ComputeGeometry();

			using (var cr = new Cairo.Context(DummySurface))
			{
				var extents = cr.TextExtents(Text);
				MinSize.X = extents.Width + 2 * Padding;
				MinSize.Y = extents.Height + 2 * Padding;
			}
			RenderSize = MinSize;
		}

		protected override void Render(RenderContext context)
		{
			base.Render(context);

			// render the text (assume the font size is set by the parent menu
			context.Cairo.Color = new Cairo.Color(0, 0, 0);
			var point = context.Cairo.CurrentPoint;
			context.Cairo.Color = context.Decorator.GetColor(ColorType.Text, HitState.None).Cairo;
			context.Cairo.MoveTo(point.X + Padding, point.Y + Padding);
			context.Cairo.ShowText(Text);
			context.Cairo.MoveTo(point);
		}
		
	}
}

