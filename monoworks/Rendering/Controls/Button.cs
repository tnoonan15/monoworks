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

using gl=Tao.OpenGl.Gl;

namespace MonoWorks.Rendering.Controls
{
	
	/// <summary>
	/// Control that can be clicked by the user.
	/// </summary>
	/// <remarks> Buttons can contain an image and/or label.</remarks>
	public class Button : Control
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public Button() : this(null, null)
		{
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

		}

		/// <summary>
		/// Create a button with the given image.
		/// </summary>
		public Button(Image image) : this(null, image)
		{

		}

		/// <summary>
		/// Create a button with the given label and image.
		/// </summary>
		public Button(Label label, Image image)
		{
			this.label = label;
			this.image = image;
		}


		private Label label;
		/// <value>
		/// The label on the button.
		/// </value>
		public Label Label
		{
			get {return label;}
			set
			{
				label = value;
				MakeDirty();
			}
		}

		private Image image;
		/// <value>
		/// The image on the button.
		/// </value>
		public Image Image
		{
			get {return image;}
			set {image = value;}
		}

		public override Coord Position
		{
			get {return base.Position;}
			set
			{
				base.Position = value;
				MakeDirty();
			}
		}

		protected double padding = 4;
		/// <summary>
		/// The padding to place on the inside of the button.
		/// </summary>
		public double Padding
		{
			get { return padding; }
			set { padding = value; }
		}



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



		public override void OnButtonPress(MouseButtonEvent evt)
		{
			base.OnButtonPress(evt);

			if (HitTest(evt.Pos))
			{
				ToggleSelection();
				if (Clicked != null)
					Clicked(this, new EventArgs());
			}
		}


		public override void OnButtonRelease(MouseEvent evt)
		{
			base.OnButtonRelease(evt);

			if (IsSelected && !IsTogglable)
				Deselect();

		}


		/// <summary>
		/// Called when the button is clicked by the user.
		/// </summary>
		public event EventHandler Clicked;
		

#endregion



#region Rendering


		public override void ComputeGeometry()
		{
			base.ComputeGeometry();

			Coord pad = new Coord(padding, padding);

			if (label != null)
			{
				label.Position = position + pad;
				label.ComputeGeometry();
			}
			if (image != null)
			{
				image.Position = position + pad;
				image.ComputeGeometry();
			}

			size = label.Size + pad*2;
		}

		public override void RenderOverlay(IViewport viewport)
		{
			base.RenderOverlay(viewport);

			if (label != null)
			{

				IFill bg = styleClass.GetBackground(hitState);
				if (bg != null)
					bg.DrawRectangle(position, size);
				label.RenderOverlay(viewport);
			}
		}



#endregion
		
	}
}
