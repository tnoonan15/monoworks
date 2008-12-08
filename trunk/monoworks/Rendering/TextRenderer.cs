// TextRenderer.cs - MonoWorks Project
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
using System.Collections.Generic;
using System.Reflection;
using System.IO;

using gl = Tao.OpenGl.Gl;
using ft=Tao.FreeType;
using ISE;

using MonoWorks.Base;


namespace MonoWorks.Rendering
{
	/// <summary>
	/// Possible values for horizontal text alignment.
	/// </summary>
	public enum HorizontalAlignment {Left = FTFontAlign.FT_ALIGN_LEFT, 
									Right = FTFontAlign.FT_ALIGN_RIGHT,
									Center = FTFontAlign.FT_ALIGN_CENTERED};

	/// <summary>
	/// Possible values for vertical text alignment.
	/// </summary>
	public enum VerticalAlignment {Top, Bottom, Middle};
	
	/// <summary>
	/// Class for rendering text as an overlay.
	/// </summary>
	public class TextRenderer
	{	
		
		/// <summary>
		/// Default constructor.
		/// </summary>
		/// <remarks> there should be one of these for every viewport.</remarks>
		public TextRenderer()
		{
		}
			

#region The Font
		
				
		/// <value>
		/// The font for this renderer.
		/// </value>
		protected FTFont font = null;


		/// <summary>
		/// Fonts used by the application.
		/// </summary>
		protected Dictionary<int, FTFont> fonts = new Dictionary<int, FTFont>();

		/// <summary>
		/// Gets the font for a given size.
		/// </summary>
		/// <param name="size"> The font size. </param>
		/// <returns> A <see cref="FTFont"/>. </returns>
		protected FTFont GetFont(int size)
		{
			if (fonts.ContainsKey(size))
				return fonts[size];
			else
			{
				int Errors = 0;
				string dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
				FTFont font = new FTFont(dir + @"/FreeSans.ttf", out Errors);
				font.ftRenderToTexture(size, 92);
				fonts[size] = font;
				return font;
			}
		}
		
#endregion


#region Rendering

		/// <summary>
		/// Renders a single piece of text.
		/// </summary>
		/// <param name="text"></param>
		public void Render(TextDef text)
		{
			gl.glMatrixMode(gl.GL_MODELVIEW);
			gl.glPushMatrix();

			gl.glTranslated(text.Position.X, text.Position.Y, 0);

			if (text.Angle.Value != 0)
				gl.glRotated(text.Angle.Degrees, 0, 0, 1);

			FTFont font = GetFont(text.Size);
			font.ftBeginFont();
			text.Color.Setup();
			font.ftWrite(text.Text);
			font.FT_ALIGN = (FTFontAlign)text.HorizontalAlignment;
			font.ftEndFont();

			gl.glMatrixMode(gl.GL_MODELVIEW);
			gl.glPopMatrix();
		}

		/// <summary>
		/// Renders many pieces of text.
		/// </summary>
		public void Render(TextDef[] text)
		{
			foreach (TextDef text_ in text)
				Render(text_);
		}


#endregion


	}
}
