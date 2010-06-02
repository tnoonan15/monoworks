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
	/// Control that can be clicked by the user.
	/// </summary>
	/// <remarks> Buttons can contain an Image and/or Label.</remarks>
	public class Button : AbstractButton
	{

		/// <summary>
		/// Default constructor.
		/// </summary>
		public Button() : base()
		{
		}
		
		/// <summary>
		/// Create a button with the given text.
		/// </summary>
		public Button(string text) : base(text)
		{
		}

		/// <summary>
		/// Create a button with the given Label.
		/// </summary>
		public Button(Label label) : base(label)
		{
		}

		/// <summary>
		/// Create a button with the given Image.
		/// </summary>
		public Button(Image Image) : base(Image)
		{
		}

		/// <summary>
		/// Creates a button with the given label text and Image.
		/// </summary>
		public Button(string text, Image Image) : base(text, Image)
		{
		}

		/// <summary>
		/// Create a button with the given label and Image.
		/// </summary>
		public Button(Label label, Image Image) : base(label, Image)
		{
		}
		
		
		
		#region Rendering

		public override void ComputeGeometry()
		{
			base.ComputeGeometry();

			Coord pad = new Coord(Padding, Padding);
			Coord pad2 = pad * 2;
			
			// the correct control is not present
			if ((ButtonStyle == ButtonStyle.Label && Label == null) || 
			    (ButtonStyle == ButtonStyle.Image && Image == null) ||
			    (Label == null && Image == null))
			{
				MinSize = pad2;
				return;
			}
			
			if (Image != null && Image.IsDirty)
				Image.ComputeGeometry();
			if (Label != null && Label.IsDirty)
				Label.ComputeGeometry();
		
			switch (StyleToUse)
			{
			case ButtonStyle.Label:
				// only show the label
				if (Image != null)
					Image.IsVisible = false;
				Label.Origin = pad;
				MinSize = Label.RenderSize + pad2;
				ApplyUserSize();
				break;
				
			case ButtonStyle.Image: // only show the Image
				if (Label != null)
					Label.IsVisible = false;
				Image.IsVisible = true;
				Image.Origin = pad;
				MinSize = Image.RenderSize + pad2;
				ApplyUserSize();
				break;
				
			case ButtonStyle.ImageOverLabel: // place the Image over the label
				Image.IsVisible = true;
				Label.IsVisible = true;
				MinSize = new Coord(Math.Max(Image.RenderWidth, Label.RenderWidth),
								 Image.RenderHeight + Label.RenderHeight + Padding) + pad2;
				ApplyUserSize();
				Image.Origin = pad + new Coord((RenderWidth - Image.RenderWidth) / 2.0 - Padding, 0);
				Label.Origin = pad + new Coord((RenderWidth - Label.RenderWidth) / 2.0 - Padding, Image.RenderHeight + Padding);
				break;
				
			case ButtonStyle.ImageNextToLabel: // place the Image to the right of the label
				Image.IsVisible = true;
				Label.IsVisible = true;
				MinSize = new Coord(Image.RenderWidth + Label.RenderWidth + Padding, 
				                 Math.Max(Image.RenderHeight, Label.RenderHeight)) + pad2;
				ApplyUserSize();
				Image.Origin = pad;
				Label.Origin = pad + new Coord(Image.RenderWidth + Padding, (RenderHeight - Label.RenderHeight) / 2.0 - Padding);
				break;
			}
		}

		#endregion
				
		


		
	}
}
