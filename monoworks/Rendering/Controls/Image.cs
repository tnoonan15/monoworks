// Image.cs - MonoWorks Project
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


using MonoWorks.Rendering;
using gl=Tao.OpenGl.Gl;
using il=Tao.DevIl.Il;
using ilu=Tao.DevIl.Ilu;

namespace MonoWorks.Rendering.Controls
{
	
	/// <summary>
	/// Control containing an image.
	/// </summary>
	public class Image : Control
	{

		/// <summary>
		/// Loads an image from a file.
		/// </summary>
		/// <param name="fileName"> The name of the image file. </param>
		public Image(string fileName) : base()
		{
			LoadFile(fileName);
		}

		int ilId;

		/// <summary>
		/// Loads a file image.
		/// </summary>
		public void LoadFile(string fileName)
		{
			il.ilInit();
//			ilId = il.ilGenImage();
			il.ilGenImages(1, out ilId);
            il.ilBindImage(ilId);
            il.ilLoadImage(fileName);
		}

		
		
		public override void ComputeGeometry()
		{
			base.ComputeGeometry();
		}

		
		public override void RenderOverlay(IViewport viewport)
		{
			base.RenderOverlay(viewport);

//			gl.glMatrixMode(gl.GL_MODELVIEW);
//			gl.glPushMatrix();

//			gl.glTranslated(position.X, position.Y, 0);

			gl.glRasterPos2d(position.X, position.Y);
			
            il.ilBindImage(ilId);
			gl.glDrawPixels(48, 48, gl.GL_RGBA, gl.GL_UNSIGNED_BYTE, il.ilGetData());

//			gl.glPopMatrix();
		}

		
		
	}
}
