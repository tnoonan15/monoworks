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
	/// Event for a double value changing.
	/// </summary>
	public class DoubleChangedEvent : ValueChangedEvent<double>
	{
		public DoubleChangedEvent(double oldVal, double newVal)
			: base(oldVal, newVal)
		{
			
		}
	}
	
	
	/// <summary>
	/// Delegate for handling double changed events.
	/// </summary>
	public delegate void DoubleChangedHandler(object sender, DoubleChangedEvent evt);
	
	
	/// <summary>
	/// Lets the user slide an indicator to adjust a floating point value.
	/// </summary>
	public class Slider : Control2D
	{
		
		public Slider()
		{
			IsHoverable = true;
			
			Max = 10;
			Step = 2;
			
			MinSize = new Coord();
			IndicatorPosition = new Coord();
			IndicatorSize = new Coord();
			LineStart = new Coord();
			LineStop = new Coord();
		}
		
		/// <summary>
		/// Gets called whenever the slider value changes.
		/// </summary>
		public event DoubleChangedHandler ValueChanged;
		
		private double _value;
		/// <summary>
		/// The current value of the slider.
		/// </summary>
		[MwxProperty]
		public double Value {
			get {return _value;}
			set {
				var oldVal = _value;
				var newVal = value.MinMax(Min, Max);
				
				// perform step interpolation
				if (ForceStep)
				{
					_value = Math.Round((newVal - Min)/Step) * Step + Min;
				}
				else
					_value = newVal;
				
				MakeDirty();
				if (ValueChanged != null)
					ValueChanged(this, new DoubleChangedEvent(oldVal, _value));
			}
		}
				
		private double _min;
		/// <summary>
		/// The minimum value of the slider.
		/// </summary>
		[MwxProperty]
		public double Min {
			get {return _min;}
			set {
				_min = value;
				MakeDirty();
			}
		}
				
		private double _max;
		/// <summary>
		/// The maximum value of the slider.
		/// </summary>
		[MwxProperty]
		public double Max {
			get {return _max;}
			set {
				_max = value;
				MakeDirty();
			}
		}
		
		/// <summary>
		/// Max - Min.
		/// </summary>
		public double Range
		{
			get {return Max - Min;}
		}
				
		private double _step;
		/// <summary>
		/// The amount to change the value when arrow keys are used.
		/// </summary>
		/// <remarks>Also used when ForceStep is true to force the value
		/// to specific increments.</remarks>
		[MwxProperty]
		public double Step {
			get {return _step;}
			set {
				_step = value;
				MakeDirty();
			}
		}
				
		private bool _forceStep;
		/// <summary>
		/// If true, the value is forced to the closest Min + n * Step,
		/// where n is an integer.
		/// </summary>
		[MwxProperty]
		public bool ForceStep {
			get {return _forceStep;}
			set {
				_forceStep = value;
				MakeDirty();
			}
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
			
			var relPos = evt.Pos - LastPosition;
			if (relPos >= IndicatorPosition && relPos <= IndicatorPosition + IndicatorSize)
			{
				_isDragging = true;
			}
			else
			{
				SetClosestValue(evt.Pos);
			}
			evt.Handle();
		}

		public override void OnButtonRelease(MouseButtonEvent evt)
		{
			base.OnButtonRelease(evt);
			
			if (_isDragging)
			{
				_isDragging = false;
				evt.Handle();
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
		
		#endregion
		
	}
}
