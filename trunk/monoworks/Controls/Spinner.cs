// 
//  Spinner.cs - MonoWorks Project
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
	/// Control that lets the user adjust a number through a text box or by pushing up/down buttons.
	/// </summary>
	public class Spinner : NumericControl
	{
		public Spinner()
		{
			_textBox = new TextBox();
			_upButton = new Button(new Image(ResourceHelper.GetStream("up.png")));
			_downButton = new Button(new Image(ResourceHelper.GetStream("down.png")));
			_textBox.Body = Value.ToString();
			_upButton.Clicked += delegate {
				StepUp();
			};
			_downButton.Clicked += delegate {
				StepDown();
			};
			
			_textBox.BodyChanged += OnTextBoxBodyChanged;
			_goodColor = _textBox.TextColor;
		}

		
		#region Value Changing
		
		private bool _internalUpdate = false;
		
		public override double Value {
			get {
				return base.Value;
			}
			set {
				if (!_internalUpdate)
				{
					_internalUpdate = true;
					base.Value = value;
					_textBox.Body = Value.ToString();
					_internalUpdate = false;
				}
			}
		}
		
		
		/// <summary>
		/// Color to use when the text box has a bad value.
		/// </summary>
		private static Color _badColor = new Color(1f, 0f, 0f);
		
		/// <summary>
		/// The text box's preferred color.
		/// </summary>
		private Color _goodColor;

		/// <summary>
		/// Handles the text box body changing.
		/// </summary>
		private void OnTextBoxBodyChanged(object sender, TextChangedEvent evt)
		{
			double val;
			if (double.TryParse(_textBox.Body, out val))
			{
				Value = val;
				_textBox.TextColor = _goodColor;
			}
			else
			{
				_textBox.TextColor = _badColor;
			}
		}

		#endregion
		
		
		#region Rendering
		
		private TextBox _textBox;
		
		private Button _upButton, _downButton;
		
		public override void ComputeGeometry()
		{
			base.ComputeGeometry();
		
			_textBox.ComputeGeometry();
			_upButton.ComputeGeometry();
			_downButton.ComputeGeometry();
			MinWidth = _textBox.RenderWidth + _upButton.RenderWidth;
			var buttonHeight = _upButton.RenderHeight + _downButton.RenderHeight;
			MinHeight = buttonHeight + 2 * Padding;
			_textBox.Origin.Y = (buttonHeight - _textBox.RenderHeight)/2.0;
			_upButton.Origin.X = _textBox.RenderWidth;
			_downButton.Origin.X = _textBox.RenderWidth;
			_downButton.Origin.Y = _upButton.RenderHeight;
			ApplyUserSize();
		}
		
		protected override void Render(RenderContext context)
		{
			base.Render(context);
			
			_textBox.RenderCairo(context);
			_upButton.RenderCairo(context);
			_downButton.RenderCairo(context);
		}		
		
		#endregion
		
		
		#region Interaction
		
		public override void OnButtonPress(MouseButtonEvent evt)
		{
			base.OnButtonPress(evt);
			
			if (!evt.IsHandled)
				_textBox.OnButtonPress(evt);
			if (!evt.IsHandled)
				_upButton.OnButtonPress(evt);
			if (!evt.IsHandled)
				_downButton.OnButtonPress(evt);
		}
		
		public override void OnButtonRelease(MouseButtonEvent evt)
		{
			base.OnButtonRelease(evt);
			
			if (!evt.IsHandled)
				_textBox.OnButtonRelease(evt);
			if (!evt.IsHandled)
				_upButton.OnButtonRelease(evt);
			if (!evt.IsHandled)
				_downButton.OnButtonRelease(evt);
		}
		
		public override void OnMouseMotion(MouseEvent evt)
		{
			base.OnMouseMotion(evt);
			
			if (!evt.IsHandled)
				_textBox.OnMouseMotion(evt);
			if (!evt.IsHandled)
				_upButton.OnMouseMotion(evt);
			if (!evt.IsHandled)
				_downButton.OnMouseMotion(evt);
		}

		public override void OnKeyPress(KeyEvent evt)
		{
			base.OnKeyPress(evt);
			
			_textBox.OnKeyPress(evt);
		}
		
		public override void OnKeyRelease(KeyEvent evt)
		{
			base.OnKeyRelease(evt);
			
			_textBox.OnKeyRelease(evt);
		}
		
		
		#endregion
		
	}
}

