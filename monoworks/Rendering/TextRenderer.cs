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
		
		/// <summary>
		/// Fonts used by the GL context this renderer belongs to.
		/// </summary>
		protected Dictionary<int, FTFont> fonts = new Dictionary<int, FTFont>();
		
		/// <summary>
		/// Fonts used by the application for extent information.
		/// </summary>
		protected static Dictionary<int, FTFont> globalFonts = new Dictionary<int, FTFont>();

		/// <summary>
		/// Creates a new font of the given size.
		/// </summary>
		/// <remarks>In general, use GetFont() and GetGlobalFont() instead, 
		/// as they invoke this method if necessary.</remarks>
		protected static FTFont CreateFont(int size)
		{
			int Errors = 0;
			string dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			FTFont font = new FTFont(dir + @"/FreeSans.ttf", out Errors);
			font.ftRenderToTexture(size, 92);
			return font;
		}
		
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
				fonts[size] = CreateFont(size); // store the font for use later
				globalFonts[size] = fonts[size]; // overwrite the last global font of this size, if there was one
				return fonts[size];
			}
		}

		/// <summary>
		/// Gets a global font of the given size.
		/// </summary>
		/// <returns>Note that global fonts are for size computation 
		/// only and should NOT be used for rendering.</returns>
		protected static FTFont GetGlobalFont(int size)
		{
			if (globalFonts.ContainsKey(size))
				return globalFonts[size];
			else
			{
				globalFonts[size] = CreateFont(size);
				return globalFonts[size];
			}
		}

		/// <summary>
		/// Gets the extents for the given text definition.
		/// </summary>
		public static Coord GetExtents(TextDef textDef)
		{
			if (textDef.Text == null)
				return new Coord();

			FTFont font = GetGlobalFont(textDef.Size);
			return new Coord((double)font.ftExtent(ref textDef.Text), (double)textDef.Size);
		}
		
#endregion


#region Rendering

		/// <summary>
		/// Renders a single piece of text.
		/// </summary>
		/// <param name="text"></param>
		public void Render(TextDef text)
		{
			if (text.Text == null)
				return;

			gl.glMatrixMode(gl.GL_MODELVIEW);
			gl.glPushMatrix();

			//gl.glTranslated(text.Position.X, text.Position.Y, 0);
			gl.glTranslated(Math.Round(text.Position.X), Math.Round(text.Position.Y), 0);

			if (text.Angle.Value != 0)
				gl.glRotated(text.Angle.Degrees, 0, 0, 1);

			FTFont font = GetFont(text.Size);
			font.ftBeginFont();
			text.Color.Setup();
			font.FT_ALIGN = (FTFontAlign)text.HorizontalAlignment;
			font.ftWrite(text.Text);
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
