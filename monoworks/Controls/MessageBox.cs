// 
//  MessageBox.cs - MonoWorks Project
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
using System.Collections.Generic;

using MonoWorks.Base;
using MonoWorks.Rendering;

namespace MonoWorks.Controls
{
	/// <summary>
	/// Possible user responses from a message box.
	/// </summary>
	[Flags]
	public enum MessageBoxResponse {
		Ok = 1, 
		Cancel = 2, 
		Yes = 4, 
		No = 8,
		CloseWithoutSaving = 16,
	};
	
	
	/// <summary>
	/// Possible icons in a message box.
	/// </summary>
	public enum MessageBoxIcon {None, Info, Warning, Error};
	
	
	/// <summary>
	/// Control that has the contents of the message box.
	/// </summary>
	public class MessageBoxFrame : Stack
	{
		public MessageBoxFrame(Orientation orientation) : base(orientation)
		{
			
		}
	}
	
	
	/// <summary>
	/// Displays a simple modal dialog to the user with some text and an optional icon.
	/// </summary>
	public class MessageBox : ModalControlOverlay
	{
		
		public MessageBox() : base()
		{
			_frame = new MessageBoxFrame(Orientation.Vertical);
			Control = _frame;
			
			_bodyStack = new Stack(Orientation.Horizontal);
			_frame.AddChild(_bodyStack);
			_messageLabel = new Label();
			_bodyStack.AddChild(_messageLabel);
			
			_buttonStack = new GenericStack<Button>(Orientation.Horizontal);
			_frame.AddChild(_buttonStack);
			
			GrayScene = true;
			CloseOnOutsideClick = false;
		}

		public override void OnShown(Scene scene)
		{
			base.OnShown(scene);
			Center(scene);
		}
		
		#region Body
		
		private MessageBoxFrame _frame;
		
		private Stack _bodyStack;
		
		private Label _messageLabel;
		
		/// <summary>
		/// The message body.
		/// </summary>
		public string Message {
			get { return _messageLabel.Body; }
			set { _messageLabel.Body = value;}
		}
		
		#endregion
		
		
		#region Icons
		
		private static Dictionary<MessageBoxIcon, string> _iconNames = new Dictionary<MessageBoxIcon, string>() {
			{MessageBoxIcon.Info, "dialog-information.png"},
			{MessageBoxIcon.Warning, "dialog-warning.png"},
			{MessageBoxIcon.Error, "dialog-error.png"}
		};
		
		private Image _icon;
		
		/// <summary>
		/// Sets the icon displayed next to the message.
		/// </summary>
		public MessageBoxIcon Icon {
			set {
				if (_icon != null)
					_bodyStack.RemoveChild(_icon);
				if (value != MessageBoxIcon.None)
				{
					_icon = new Image(ResourceHelper.GetStream(_iconNames[value]));
					_bodyStack.InsertChild(_icon, 0);
				}
			}
		}
		
		#endregion
		
		
		#region Buttons
		
		private GenericStack<Button> _buttonStack;
		
		/// <summary>
		/// Adds a button for the given response type.
		/// </summary>
		public void AddButton(MessageBoxResponse buttonType)
		{
			var button = new Button(buttonType.ToString().InsertCamelSpaces()) {
				ButtonStyle = ButtonStyle.Label
			};
			button.Clicked += delegate {
				CurrentResponse = buttonType;
				Close();
			};
			_buttonStack.AddChild(button);
		}
		
		/// <summary>
		/// The current response, either set when the box was created or selected by the user.
		/// </summary>
		public MessageBoxResponse CurrentResponse { get; set; }
		
		#endregion
		
		
		
		#region Static Invocation
		
		/// <summary>
		/// Shows a message box with the given parameters.
		/// </summary>
		/// <param name="scene"> A <see cref="Scene"/> to show it on. </param>
		/// <param name="icon"> Which <see cref="MessageBoxIcon"/> to show next to the message. </param>
		/// <param name="message"> The bosy of the message. </param>
		/// <param name="responses"> The possible <see cref="MessageBoxResponse"/>. A button will be made for each one. </param>
		/// <returns> The <see cref="MessageBox"/> that was created. </returns>
		public static MessageBox Show(Scene scene, MessageBoxIcon icon, 
			string message, MessageBoxResponse responses)
		{
			var box = new MessageBox() {
				Icon = icon
			};
			
			// add the buttons
			foreach (var val in Enum.GetValues(typeof(MessageBoxResponse)))
			{
				var response = (MessageBoxResponse)val;
				if ((responses & response) == response)
				{
					box.AddButton(response);
				}
			}
			
			// set the message and icon
			box.Message = message;
			
			// show the box
			scene.ShowModal(box);
			
			return box;
		}
		
		#endregion
		
	}
}

