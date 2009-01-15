// SpinControl.cs - MonoWorks Project
//
//  Copyright (C) 2009 Andy Selvig
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

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using MonoWorks.GuiWpf;

namespace MonoWorks.GuiWpf.Utilities
{
	/// <summary>
	/// Since WPF is ridiculous and doesn't have 
	/// a spin control, I had to roll my own.
	/// </summary>
	public class SpinControl : Grid
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public SpinControl()
		{
			// add the rows and columns
			this.AddRow();
			this.AddRow();
			this.AddAutoColumn();
			this.AddColumn(16);

			textBox = new TextBox();
			this.AddAt(textBox, 0, 0, 2, 1);
			textBox.TextChanged += OnTextchanged;
			textBox.Width = Double.NaN;

			upButton = new Button();
			this.AddAt(upButton, 0, 1);
			upButton.Click += OnUp;

			downButton = new Button();
			this.AddAt(downButton, 1, 1);
			downButton.Click += OnDown;

			IsValid = true;
			Value = 0;
			Step = 1;
		}

		TextBox textBox;

		Button upButton;

		Button downButton;

		double val;
		/// <summary>
		/// The value of the control.
		/// </summary>
		public double Value
		{
			get { return val; }
			set
			{
				BeginUpdate();
				val = value;
				RangeCheck();
				IsValid = true;
				textBox.Text = val.ToString();
				EndUpdate();
			}
		}


		#region Range

		double min = Double.MinValue;
		/// <summary>
		/// The minimum allowed value.
		/// </summary>
		public double Min
		{
			get { return min; }
			set
			{
				min = value;
				if (val < min)
					Value = min;
			}
		}

		double max = Double.MaxValue;
		/// <summary>
		/// The maximum allowed value.
		/// </summary>
		public double Max
		{
			get { return max; }
			set
			{
				max = value;
				if (val > max)
					Value = max;
			}
		}

		/// <summary>
		/// Checks the value against the range and will adjust it if necessary.
		/// </summary>
		void RangeCheck()
		{
			if (val < min)
				val = min;
			else if (val > max)
				val = max;
		}

		#endregion


		#region Stepping

		/// <summary>
		/// The step taken when a button is pressed.
		/// </summary>
		public double Step { get; set; }

		/// <summary>
		/// Handles up button clicks.
		/// </summary>
		void OnDown(object sender, RoutedEventArgs e)
		{
			val -= Step;
			RangeCheck();
			Value = val; 
			if (ValueChanged != null)
				ValueChanged(val);
		}

		/// <summary>
		/// Handles down button clicks.
		/// </summary>
		void OnUp(object sender, RoutedEventArgs e)
		{
			val += Step;
			RangeCheck();
			Value = val;
			if (ValueChanged != null)
				ValueChanged(val);
		}
		
		#endregion


		/// <summary>
		/// Whether the control is in a valid state.
		/// </summary>
		public bool IsValid { get; private set; }

		public delegate void ValueChangedHandler(double val);

		/// <summary>
		/// Gets called when the value is changed (and is valid).
		/// </summary>
		public event ValueChangedHandler ValueChanged;

		/// <summary>
		/// Whether the control is updating itself.
		/// </summary>
		bool internalUpdate = false;

		/// <summary>
		/// Begin updating the control internally.
		/// </summary>
		void BeginUpdate()
		{
			internalUpdate = true;
		}

		/// <summary>
		/// End updating the control internally.
		/// </summary>
		void EndUpdate()
		{
			internalUpdate = false;
		}

		/// <summary>
		/// Handles text changed events.
		/// </summary>
		void OnTextchanged(object sender, TextChangedEventArgs e)
		{
			if (!internalUpdate)
			{
				IsValid = Double.TryParse(textBox.Text, out val);
				RangeCheck();

				if (IsValid && ValueChanged != null)
					ValueChanged(val);
			}

			// adjust the color based on the validity
			if (IsValid)
				textBox.Foreground = Brushes.Black;
			else
				textBox.Foreground = Brushes.Red;

		}


	}
}
