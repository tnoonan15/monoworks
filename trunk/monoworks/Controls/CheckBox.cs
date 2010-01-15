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
	/// Event containing information about a bool value changing.
	/// </summary>
	public class BoolChangedEvent : ValueChangedEvent<bool>
	{
		public BoolChangedEvent(bool oldVal, bool newVal)
			: base(oldVal, newVal)
		{			
		}
	}
	
	/// <summary>
	/// Delegate for handling bool changed events.
	/// </summary>
	public delegate void BoolChangedHandler(object sender, BoolChangedEvent evt);
	
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
		
		/// <summary>
		/// Gets thrown when the value of IsChecked changes.
		/// </summary>
		public event BoolChangedHandler CheckChanged;
		
		/// <summary>
		/// Whether or not the check box is currently checked.
		/// </summary>
		public bool IsChecked
		{
			get { return IsSelected; }
			set
			{
				var oldVal = IsSelected;
				if (oldVal != value)
				{
					IsSelected = value;
					MakeDirty();
					if (CheckChanged != null)
						CheckChanged(this, new BoolChangedEvent(oldVal, value));
				}
			}
		}
		
		/// <summary>
		/// Checks the box.
		/// </summary>
		public void Check()
		{
			IsChecked = true;
		}

		/// <summary>
		/// Unchecks the box.
		/// </summary>
		public void Uncheck()
		{
			IsChecked = false;
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
				GrabFocus();
				IsChecked = !IsChecked;
			}
		}
		
		protected override void OnEnter(MouseEvent evt)
		{
			base.OnEnter(evt);
			
			evt.Scene.SetCursor(CursorType.Hand);
		}

		protected override void OnLeave(MouseEvent evt)
		{
			base.OnLeave(evt);
			
			evt.Scene.SetCursor(CursorType.Normal);
		}
		
		#endregion
		
	}
}
