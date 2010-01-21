// Button.cs - MonoWorks Project
//
//  Copyright (C) 2008 Andy Selvig
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
using MonoWorks.Base;
using MonoWorks.Rendering;
using MonoWorks.Rendering.Events;


namespace MonoWorks.Controls
{
	
	/// <summary>
	/// Defines how the button image and label are laid out.
	/// </summary>
	public enum ButtonStyle {Image, Label, ImageOverLabel, ImageNextToLabel};
	
	/// <summary>
	/// Control that can be clicked by the user.
	/// </summary>
	/// <remarks> Buttons can contain an image and/or label.</remarks>
	public class Button : Control2D
	{

		/// <summary>
		/// Default constructor.
		/// </summary>
		public Button() : base()
		{
			IsHoverable = true;
		}
		
		/// <summary>
		/// Create a button with the given text.
		/// </summary>
		public Button(string text) : this(new Label(text))
		{

		}

		/// <summary>
		/// Create a button with the given label.
		/// </summary>
		public Button(Label label) : this(label, null)
		{
			ButtonStyle = ButtonStyle.Label;
		}

		/// <summary>
		/// Create a button with the given image.
		/// </summary>
		public Button(Image image) : this("", image)
		{
			ButtonStyle = ButtonStyle.Image;
		}

		/// <summary>
		/// Creates a button with the given label text and image.
		/// </summary>
		public Button(string text, Image image) : this(new Label(text), image)
		{
		}

		/// <summary>
		/// Create a button with the given label and image.
		/// </summary>
		public Button(Label label, Image image) 
			: this()
		{
			this.label = label;
			label.IsHoverable = false;
			this.image = image;
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
		
		private Image image;
		/// <value>
		/// The image on the button.
		/// </value>
		[MwxProperty]
		public Image Image
		{
			get
			{
				if (image == null)
					image = new Image();
				return image;
			}
			set {image = value;}
		}
		
			
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
					else if (image == null)
						return ButtonStyle.Label;
				}
				return ButtonStyle;
			}
		}		
		
		#endregion
		
		
		#region Rendering

		public override void ComputeGeometry()
		{
			base.ComputeGeometry();

			Coord pad = new Coord(Padding, Padding);
			Coord pad2 = new Coord(Padding, Padding) * 2;
			
			// the correct control is not present
			if ((ButtonStyle == ButtonStyle.Label && label == null) || 
			    (ButtonStyle == ButtonStyle.Image && image == null) ||
			    (label == null && image == null))
			{
				MinSize = pad2;
				return;
			}
				
			if (image != null && image.IsDirty)
				image.ComputeGeometry();
			if (label != null && label.IsDirty)
				label.ComputeGeometry();

			switch (StyleToUse)
			{
			case ButtonStyle.Label: // only show the label
				if (image != null)
					image.IsVisible = false;
				label.Origin = pad;
				MinSize = label.RenderSize + pad2;
				ApplyUserSize();
				break;
				
			case ButtonStyle.Image: // only show the image
				if (label != null)
					label.IsVisible = false;
				image.IsVisible = true;
				image.Origin = pad;
				MinSize = image.RenderSize + pad2;
				ApplyUserSize();
				break;
				
			case ButtonStyle.ImageOverLabel: // place the image over the label
				image.IsVisible = true;
				label.IsVisible = true;
				MinSize = new Coord(Math.Max(image.RenderWidth, label.RenderWidth),
								 image.RenderHeight + label.RenderHeight + Padding) + pad2;
				ApplyUserSize();
				image.Origin = pad + new Coord((RenderWidth - image.RenderWidth) / 2.0 - Padding, 0);
				label.Origin = pad + new Coord((RenderWidth - label.RenderWidth) / 2.0 - Padding, image.RenderHeight + Padding);
				break;
				
			case ButtonStyle.ImageNextToLabel: // place the image to the right of the label
				image.IsVisible = true;
				label.IsVisible = true;
				MinSize = new Coord(image.RenderWidth + label.RenderWidth + Padding, 
				                 Math.Max(image.RenderHeight, label.RenderHeight)) + pad2;
				ApplyUserSize();
				image.Origin = pad;
				label.Origin = pad + new Coord(image.RenderWidth + Padding, (RenderHeight - label.RenderHeight) / 2.0 - Padding);
				break;
			}
		}

		protected override void Render(RenderContext context)
		{
			base.Render(context);
			
			if (label != null && label.IsVisible)
				label.RenderCairo(context);
			
			if (image != null && image.IsVisible)
				image.RenderCairo(context);
		}

		#endregion
				
		
		#region Mouse Interaction

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

			if (HitTest(evt.Pos) && !justClicked)
			{
				evt.Handle();
				justClicked = true;
				IsSelected = true;
				IsFocused = true;
				MakeDirty();
			}
		}

		public override void OnButtonRelease(MouseButtonEvent evt)
		{
			base.OnButtonRelease(evt);

			if (IsSelected && !IsTogglable)
				Deselect();

			// if we were just clicked, we get to handle the next button release event
			if (justClicked)
			{
				justClicked = false;
				evt.Handle();
				MakeDirty();
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
		
		#endregion


		
	}
}
