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

using gl = Tao.OpenGl.Gl;
using ft=Tao.FreeType;
using ISE;

using MonoWorks.Base;


namespace MonoWorks.Rendering
{
	
	/// <summary>
	/// Class for rendering text as an overlay.
	/// </summary>
	public class TextRenderer
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public TextRenderer() : this(16)
		{
		}
		
		~TextRenderer()
		{
//			font.Dispose();
		}
		
		/// <summary>
		/// Constructor that takes the font size.
		/// </summary>
		/// <param name="size"> The font size. </param>
		public TextRenderer(int size)
		{
			this.size = size;
			font = GetFont(size);
			color = new Color(0, 0, 0);
			position = new Coord();
			text = "";
		}
		
		
#region Attributes

		protected string text;
		/// <value>
		/// The text to render.
		/// </value>
		public string Text
		{
			get {return text;}
			set
			{
				if (size != 14)
				{
					// HACK - all but this size has a character offset, no idea why
					char[] chars = value.ToCharArray();
					text = "";
					foreach (char c in chars)
					{
						char newC = (char)((int)c + 1);
						text += newC;
					}
				}
				else
					text = value;
			}
		}
		
		
		protected int size;
		/// <value>
		/// The font size.
		/// </value>
		public int Size
		{
			get {return size;}
		}
		
		protected Color color;
		/// <value>
		/// The text color.
		/// </value>
		public Color Color
		{
			get {return color;}
			set {color = value;}
		}
		
		protected Coord position;
		/// <value>
		/// The position of the text.
		/// </value>
		public Coord Position
		{
			get {return position;}
			set {position = value;}
		}
		
		protected Angle angle = new Angle();
		/// <value>
		/// The angle from horizontal.
		/// </value>
		public Angle Angle
		{
			get {return angle;}
			set {angle = value;}
		}
		
#endregion
		

#region The Font
		
		/// <summary>
		/// Fonts used by the application.
		/// </summary>
		protected static Dictionary<int,FTFont> fonts = new Dictionary<int,FTFont>();
		
		/// <summary>
		/// Gets the font for a given size.
		/// </summary>
		/// <param name="size"> The font size. </param>
		/// <returns> A <see cref="FTFont"/>. </returns>
		protected static FTFont GetFont(int size)
		{
			if (fonts.ContainsKey(size))
				return fonts[size];
			else
			{
				int Errors = 0;
				FTFont font = new FTFont("FreeSans.ttf", out Errors);
				font.ftRenderToTexture(size, 92);
				fonts[size] = font;
				return font;
			}
		}
				
		/// <value>
		/// The font for this renderer.
		/// </value>
		protected FTFont font;

		/// <value>
		/// The font alignment.
		/// </value>
		public FTFontAlign Alignment
		{
			get {return font.FT_ALIGN;}
			set
			{
				font.FT_ALIGN = value;
			}
		}		
		
#endregion
		
		/// <summary>
		/// Renders the text to the viewport.
		/// </summary>
		/// <param name="viewport"> A <see cref="IViewport"/> to render to. </param>
		public void RenderOverlay(IViewport viewport)
		{
			gl.glMatrixMode(gl.GL_MODELVIEW);
			gl.glPushMatrix();

			gl.glTranslated(position.X, position.Y, 0);
			
			if (angle.Value != 0)
				gl.glRotated(angle.Degrees, 0, 0, 1);
			
			font.ftBeginFont();
			color.Setup(); 
			font.ftWrite(text);
//			font.ftWrite("Hello");
			font.ftEndFont();

			gl.glMatrixMode(gl.GL_MODELVIEW);
			gl.glPopMatrix();
		}
		
		
	}
}
