// 
//  CheckBox.cs
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
using MonoWorks.Rendering.Events;

namespace MonoWorks.Controls
{

	/// <summary>
	/// A button that represents its state with a checked box.
	/// </summary>
	public class CheckBox : Control2D, IStringParsable
	{

		public CheckBox()
		{
			IsHoverable = true;
			
		}
		
		/// <summary>
		/// The label used to render the text.
		/// </summary>
		private readonly Label _label = new Label();
				
		/// <summary>
		/// The text to display next to the check box.
		/// </summary>
		[MwxProperty]
		public string Text
		{
			get { return _label.Body; }
			set {
				_label.Body = value;
				MakeDirty();
			}
		}

		/// <summary>
		/// Assigns the text.
		/// </summary>
		public void Parse(string valString)
		{
			Text = valString;
		}
		
		
		#region Rendering
		
		/// <summary>
		/// The width and height of the box.
		/// </summary>
		public const double BoxSize = 14;
		
		public override void ComputeGeometry()
		{
			base.ComputeGeometry();
			
			if (_label.IsDirty)
				_label.ComputeGeometry();
			
			MinSize = new Coord(_label.RenderSize.X + 3 * Padding + BoxSize, _label.RenderSize.Y);
			_label.Origin = new Coord(2 * Padding + BoxSize, 0);
			
			ApplyUserSize();
		}

		protected override void Render(RenderContext context)
		{
			base.Render(context);
			
			_label.RenderCairo(context);
		}
		
		#endregion
		
		
		#region Interaction
		
		public override void OnButtonPress(MouseButtonEvent evt)
		{
			base.OnButtonPress(evt);
			
			if (HitTest(evt.Pos))
			{
				ToggleSelection();
				GrabFocus();
				MakeDirty();
			}
		}
		
		protected override void OnEnter(MouseEvent evt)
		{
			base.OnEnter(evt);
			
			evt.Viewport.SetCursor(CursorType.Hand);
		}

		protected override void OnLeave(MouseEvent evt)
		{
			base.OnLeave(evt);
			
			evt.Viewport.SetCursor(CursorType.Normal);
		}
		
		#endregion
		
	}
}
