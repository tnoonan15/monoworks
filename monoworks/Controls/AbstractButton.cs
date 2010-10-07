// 
//  AbstractButton.cs - MonoWorks Project
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
using MonoWorks.Rendering.Events;
using MonoWorks.Rendering;

namespace MonoWorks.Controls
{
	
	/// <summary>
	/// Defines how the button image and label are laid out.
	/// </summary>
	public enum ButtonStyle {Image, Label, ImageOverLabel, ImageNextToLabel};

	/// <summary>
	/// Base class for buttons which handle user clicks.
	/// </summary>
	public class AbstractButton : Control2D, IActionPopulatable
	{

		public AbstractButton() : base()
		{
			IsHoverable = true;
		}
		/// <summary>
		/// Create a button with the given text.
		/// </summary>
		public AbstractButton(string text) : this(new Label(text))
		{

		}

		/// <summary>
		/// Create a button with the given label.
		/// </summary>
		public AbstractButton(Label label) : this(label, null)
		{
			ButtonStyle = ButtonStyle.Label;
		}

		/// <summary>
		/// Create a button with the given image.
		/// </summary>
		public AbstractButton(Image image) : this("", image)
		{
			ButtonStyle = ButtonStyle.Image;
		}

		/// <summary>
		/// Creates a button with the given label text and image.
		/// </summary>
		public AbstractButton(string text, Image image) : this(new Label(text), image)
		{
		}

		/// <summary>
		/// Create a button with the given label and image.
		/// </summary>
		public AbstractButton(Label label, Image image) 
			: this()
		{
			this.label = label;
			label.IsHoverable = false;
			Image = image;
			ButtonStyle = ButtonStyle.ImageOverLabel;
		}

		private Label label;
		/// <value>
		/// The label on the button.
		/// </value>
		public Label Label
		{
			get { return label; }
			set
			{
				label = value;
				label.IsHoverable = false;
				MakeDirty();
			}
		}

		/// <value>
		/// The string of the label.
		/// </value>
		[MwxProperty]
		public string LabelString
		{
			get
			{
				if (label == null)
					return "";
				return label.Body;
			}
			set
			{
				if (label == null)
				{
					label = new Label();
					label.IsHoverable = false;
				}
				label.Body = value;
			}
		}
		
		
		/// <value>
		/// The image on the button.
		/// </value>
		[MwxProperty]
		public Image Image { get; set; }
		
			
		#region Layout		

		/// <value>
		/// The style used to layout the image and label.
		/// </value>
		[MwxProperty]
		public ButtonStyle ButtonStyle {get; set;}
		
		
		/// <value>
		/// The button style to use to render.
		/// </value>
		/// <remarks>This may be different than the current style
		/// if some of the controls are missing.</remarks>
		protected ButtonStyle StyleToUse
		{
			get
			{
				// if one of the correct controls isn't present
				if (ButtonStyle == ButtonStyle.ImageOverLabel || 
				          ButtonStyle == ButtonStyle.ImageNextToLabel)
				{
					if (label == null)
						return ButtonStyle.Image;
					else if (Image == null)
						return ButtonStyle.Label;
				}
				return ButtonStyle;
			}
		}		
		
		#endregion
			
		
		#region Interaction
		
		protected override void OnEnter(MouseEvent evt)
		{
			base.OnEnter(evt);
			
			evt.Scene.SetCursor(CursorType.Normal);
		}


		protected bool isTogglable = false;
		/// <summary>
		/// Whether the button toggles or just clicks.
		/// </summary>
		public bool IsTogglable
		{
			get { return isTogglable; }
			set { isTogglable = value; }
		}

		protected bool justClicked = false;

		public override void OnButtonPress(MouseButtonEvent evt)
		{
			base.OnButtonPress(evt);
			
			if (evt.IsHandled || !IsEnabled)
				return;

			if (evt.Button == 1 && HitTest(evt.Pos) && !justClicked)
			{
				evt.Handle(this);
				justClicked = true;
				IsSelected = true;
				IsFocused = true;
				QueuePaneRender();
			}
		}

		public override void OnButtonRelease(MouseButtonEvent evt)
		{
			base.OnButtonRelease(evt);

			if (!IsEnabled)
				return;
			
			if (IsSelected && !IsTogglable)
				Deselect();

			// if we were just clicked, we get to handle the next button release event
			if (evt.Button == 1 && justClicked)
			{
				justClicked = false;
				evt.Handle(this);
				QueuePaneRender();
				Click();
			}

		}

		/// <summary>
		/// Called when the button is clicked by the user.
		/// </summary>
		public event EventHandler Clicked;
		
		/// <summary>
		/// Activates the button clicked event.
		/// </summary>
		public void Click()
		{
			if (Clicked != null)
				Clicked(this, new EventArgs());
		}
		
		/// <summary>
		/// Activates the button clicked event with the given args.
		/// </summary>
		public void Click(object sender, EventArgs args)
		{
			if (Clicked != null)
				Clicked(sender, args);
		}
		
		private bool _justKeyActivated;
		
		public override void OnKeyPress(KeyEvent evt)
		{
			base.OnKeyPress(evt);
			
			if (!IsEnabled)
				return;
			
			if (evt.SpecialKey == SpecialKey.Enter || evt.SpecialKey == SpecialKey.Space)
			{
				evt.Handle(this);
				_justKeyActivated = true;
				IsSelected = true;
				IsFocused = true;
				QueuePaneRender();
				Click();
			}
		}
		
		#endregion
		
		
		#region Rendering
		
		public override void ComputeGeometry()
		{
			base.ComputeGeometry();
			
			if (Image != null)
				Image.IsGrayed = !IsEnabled;				
		}

		protected override void Render(RenderContext context)
		{
			base.Render(context);
			
			if (Label != null && Label.IsVisible)
				Label.RenderCairo(context);
			
			if (Image != null && Image.IsVisible)
				Image.RenderCairo(context);
			
			if (IsSelected && !IsTogglable && _justKeyActivated)
			{
				_justKeyActivated = false;
				IsSelected = false;
				MakeDirty();
			}
		}
		
		#endregion
		
		
		#region Action Population
		
		/// <summary>
		/// Populates the button from an action.
		/// </summary>
		public void Populate(MonoWorks.Base.UiAction action)
		{
			LabelString = action.Name;
			if (action.IconName != null)
			{
				Image = new Image();
				Image.Parse(action.IconName);
			}
			
			Clicked += action.Activate;
			IsTogglable = action.IsTogglable;
		}
		
		#endregion
		
	}
}
