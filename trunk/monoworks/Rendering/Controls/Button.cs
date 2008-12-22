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
				if ( label != null)
					label.Position = value;
				if ( image != null)
					image.Position = value;
				MakeDirty();
			}
		}


#region Rendering


		public override void ComputeGeometry()
		{
			base.ComputeGeometry();
			
			if (label != null)
				label.ComputeGeometry();
			if (image != null)
				image.ComputeGeometry();

			size = label.Size;
		}

		public override void RenderOverlay(IViewport viewport)
		{
			base.RenderOverlay(viewport);

			if (label != null)
			{
			
				if (IsHovering)
				{
					ColorManager.Global["Red"].Setup();
					gl.glBegin(gl.GL_QUADS);
					gl.glVertex2d(position.X, position.Y);
					gl.glVertex2d(position.X + size.X, position.Y);
					gl.glVertex2d(position.X + size.X, position.Y + size.Y);
					gl.glVertex2d(position.X, position.Y + size.Y);
					gl.glEnd();
				}
				label.RenderOverlay(viewport);
			}
		}



#endregion
		
	}
}
