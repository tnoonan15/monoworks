// 
//  NumericControl.cs - MonoWorks Project
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

namespace MonoWorks.Controls
{

	/// <summary>
	/// Event for a double value changing.
	/// </summary>
	public class DoubleChangedEvent : ValueChangedEvent<double>
	{
		public DoubleChangedEvent(double oldVal, double newVal) : base(oldVal, newVal)
		{
			
		}
	}


	/// <summary>
	/// Delegate for handling double changed events.
	/// </summary>
	public delegate void DoubleChangedHandler(object sender, DoubleChangedEvent evt);
	
	
	/// <summary>
	/// Base class for controls that adjust a number (Spinner and Slider).
	/// </summary>
	public abstract class NumericControl : Control2D
	{
		public NumericControl()
		{
			IsHoverable = true;
			
			Max = 10;
			Step = 2;
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
		public virtual double Value {
			get { return _value; }
			set {
				var oldVal = _value;
				var newVal = value.MinMax(Min, Max);
				
				// perform step interpolation
				if (ForceStep) {
					_value = Math.Round((newVal - Min) / Step) * Step + Min;
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
			get { return _min; }
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
			get { return _max; }
			set {
				_max = value;
				MakeDirty();
			}
		}

		/// <summary>
		/// Max - Min.
		/// </summary>
		public double Range {
			get { return Max - Min; }
		}

		private double _step;
		/// <summary>
		/// The amount to change the value when arrow keys are used.
		/// </summary>
		/// <remarks>Also used when ForceStep is true to force the value
		/// to specific increments.</remarks>
		[MwxProperty]
		public double Step {
			get { return _step; }
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
			get { return _forceStep; }
			set {
				_forceStep = value;
				MakeDirty();
			}
		}

		/// <summary>
		/// Increase the value by one step.
		/// </summary>
		public void StepUp()
		{
			Value = Math.Min(Value + Step, Max);
		}

		/// <summary>
		/// Decrease the value by one step.
		/// </summary>
		public void StepDown()
		{
			Value = Math.Max(Value - Step, Min);
		}
	}
}

