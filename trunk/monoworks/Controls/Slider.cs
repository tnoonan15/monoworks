// 
//  Slider.cs
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
	/// Lets the user slide an indicator to adjust a floating point value.
	/// </summary>
	public class Slider : NumericControl
	{
		
		public Slider()
		{
			
			MinSize = new Coord();
			IndicatorPosition = new Coord();
			IndicatorSize = new Coord();
			LineStart = new Coord();
			LineStop = new Coord();
		}
		
		private bool _showTicks;
		/// <summary>
		/// If true, ticks will be drawn at all Step increments between Min and Max.
		/// </summary>
		[MwxProperty]
		public bool ShowTicks {
			get {return _showTicks;}
			set {
				_showTicks = value;
				MakeDirty();
			}
		}
				
		private bool _showLabels;
		/// <summary>
		/// If true, the Min, Max, and ticks (if shown) will have their values labeled.
		/// </summary>
		[MwxProperty]
		public bool ShowLabels {
			get {return _showLabels;}
			set {
				_showLabels = value;
				MakeDirty();
			}
		}
				
		private Orientation _orientation;
		/// <summary>
		/// The orientation of the slider movement.
		/// </summary>
		[MwxProperty]
		public Orientation Orientation {
			get {return _orientation;}
			set {
				_orientation = value;
				MakeDirty();
			}
		}
		
		
		#region Rendering
		
		/// <summary>
		/// The thickness of the indicator.
		/// </summary>
		public const double Thickness = 16;
		
		/// <summary>
		/// The lateral width of the indicator.
		/// </summary>
		public const double IndicatorWidth = 10;
		
		/// <summary>
		/// The size of the indicator.
		/// </summary>
		public Coord IndicatorSize {get; private set;}
		
		/// <summary>
		/// The position of the indicator.
		/// </summary>
		public Coord IndicatorPosition {get; private set;}
		
		/// <summary>
		/// The start of the slider line.
		/// </summary>
		public Coord LineStart {get; private set;}	
		
		/// <summary>
		/// The stop of the slider line.
		/// </summary>
		public Coord LineStop {get; private set;}
		
		/// <summary>
		/// The length of the line.
		/// </summary>
		public double LineLength {get; private set;}	
				
		public override void ComputeGeometry()
		{
			base.ComputeGeometry();
			
			// compute the render size
			if (Orientation == Orientation.Horizontal)
			{
				MinSize.X = 100;
				MinSize.Y = Thickness + 2 * Padding;
				ApplyUserSize();
				LineLength = RenderWidth - 2 * Padding;
				LineStart.X = Padding;
				LineStart.Y = Padding + Thickness / 2.0 + 0.5;
				LineStop.X = RenderWidth - Padding;
				LineStop.Y = LineStart.Y;
			}
			else // vertical
			{
				MinSize.X = Thickness + 2 * Padding;
				MinSize.Y = 100;
				ApplyUserSize();
				LineLength = RenderHeight - 2 * Padding;
				LineStart.Y = Padding;
				LineStart.X = Padding + Thickness / 2.0 + 0.5;
				LineStop.Y = RenderWidth - Padding;
				LineStop.X = LineStart.X;
			}
			
			// position the indicator
			var indicatorOffset = LineLength * (Value - Min) / Range;
			if (Orientation == Orientation.Horizontal)
			{
				IndicatorSize.X = IndicatorWidth;
				IndicatorSize.Y = Thickness;
				IndicatorPosition.X = Padding + indicatorOffset - IndicatorWidth / 2;
				IndicatorPosition.Y = Padding;
			}
			else // vertical
			{
				IndicatorSize.X = Thickness;
				IndicatorSize.Y = IndicatorWidth;
				IndicatorPosition.X = Padding;
				IndicatorPosition.Y = Padding + indicatorOffset - IndicatorWidth / 2;
			}
		}

		protected override void Render(RenderContext context)
		{			
			base.Render(context);
		}
		
		#endregion
		
		
		#region Interaction

		/// <summary>
		/// Whether or not the user is currently dragging the slider.
		/// </summary>
		private bool _isDragging = false;
		
		public override void OnButtonPress(MouseButtonEvent evt)
		{
			base.OnButtonPress(evt);
			
			if (LastPosition == null)
				return;
			var relPos = evt.Pos - LastPosition;
			if (relPos >= IndicatorPosition && relPos <= IndicatorPosition + IndicatorSize)
			{
				_isDragging = true;
				evt.Handle(this);
				GrabFocus();
			}
			else if (HitTest(evt.Pos))
			{
				SetClosestValue(evt.Pos);
				evt.Handle(this);
				GrabFocus();
			}
		}

		public override void OnButtonRelease(MouseButtonEvent evt)
		{
			base.OnButtonRelease(evt);
			
			if (_isDragging)
			{
				_isDragging = false;
				evt.Handle(this);
			}
		}

		public override void OnMouseMotion(MouseEvent evt)
		{
			base.OnMouseMotion(evt);
			
			if (!_isDragging)
				return;
			
			SetClosestValue(evt.Pos);
		}

		/// <summary>
		/// Sets the closest value to the given hit coord.
		/// </summary>
		protected void SetClosestValue(Coord pos)
		{			
			var relPos = pos - LastPosition;
			double ratio;
			if (Orientation == Orientation.Horizontal)
			{
				ratio = (relPos.X - LineStart.X) / LineLength;
			}
			else // vertical
			{
				ratio = (relPos.Y - LineStart.Y) / LineLength;
			}
			Value = ratio.MinMax(0,1) * Range + Min;
		}
		
		
		public override void OnKeyPress(KeyEvent evt)
		{
			base.OnKeyPress(evt);
			
			if (IsFocused)
			{
				if (evt.SpecialKey == SpecialKey.Right || evt.SpecialKey == SpecialKey.Up)
				{
					StepUp();
				}
				else if (evt.SpecialKey == SpecialKey.Left || evt.SpecialKey == SpecialKey.Down)
				{
					StepDown();
				}
			}
		}

		
		#endregion
		
	}
}
