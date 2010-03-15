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
			IsHoverable = true;
			Padding = 6;
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

		
		#region Rendering
		
		/// <summary>
		/// The default font size if there is no parent menu.
		/// </summary>
		private const double DefaultFontSize = 12;
		
		/// <summary>
		/// The font size used during the last geometry computation.
		/// </summary>
		private double _lastFontSize;
		
		/// <summary>
		/// A dummy surface used by Cairo to compute the text extents.
		/// </summary>
		private static readonly Cairo.ImageSurface DummySurface = new Cairo.ImageSurface(Cairo.Format.ARGB32, 128, 128);

		public override void ComputeGeometry()
		{
			base.ComputeGeometry();

			// compute the font size
			if (ParentControl != null && ParentControl is Menu)
				_lastFontSize = (ParentControl as Menu).FontSize;
			else
				_lastFontSize = DefaultFontSize;
			
			// compute the text extents
			using (var cr = new Cairo.Context(DummySurface))
			{
				cr.SetFontSize(_lastFontSize);
				var extents = cr.TextExtents(Text);
				MinSize.X = extents.Width + 2 * Padding;
				MinSize.Y = extents.Height + 2 * Padding;
			}
			RenderSize = MinSize;
		}

		protected override void Render(RenderContext context)
		{
			base.Render(context);

			// render the text
			context.Cairo.SetFontSize(_lastFontSize);
			context.Cairo.Color = new Cairo.Color(0, 0, 0);
			var point = context.Cairo.CurrentPoint;
			context.Cairo.Color = context.Decorator.GetColor(ColorType.Text, HitState.None).Cairo;
			context.Cairo.MoveTo(point.X + Padding, point.Y + Padding + _lastFontSize - 2);
			context.Cairo.ShowText(Text);
			
			// highlight if the user is hovering
			if (IsHovering)
			{
				context.Cairo.Color = context.Decorator.SelectionColor.Cairo;
				context.Cairo.Rectangle(point, RenderWidth, RenderHeight);
				context.Cairo.Fill();
			}
			
			context.Cairo.MoveTo(point);
		}
		
		#endregion
		
		
		#region Interaction
		
		public override void OnButtonPress(MonoWorks.Rendering.Events.MouseButtonEvent evt)
		{
			base.OnButtonPress(evt);
			
			if (!HitTest(evt.Pos))
				return;
			
			evt.Handle(this);
			
			if (ParentControl != null && ParentControl is Menu)
				(ParentControl as Menu).ActivateItem(this);
		}

		#endregion
		
	}
}

