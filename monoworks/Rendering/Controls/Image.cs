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
using System.IO;

using MonoWorks.Base;
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
		/// Loads an image from a stream.
		/// </summary>
		/// <param name="stream"></param>
		public Image(Stream stream)
			: base()
		{
			if (!ilInitialized)
			{
				il.ilInit();
				ilInitialized = true;
			}

			LoadStream(stream);
		}

		/// <summary>
		/// Loads an image from a file.
		/// </summary>
		/// <param name="fileName"> The name of the image file. </param>
		public Image(string fileName) : base()
		{
			if (!ilInitialized)
			{
				il.ilInit();
				ilInitialized = true;
			}
			
			LoadFile(fileName);
		}

		/// <summary>
		/// Whether or not DevIL has been initialized.
		/// </summary>
		protected static bool ilInitialized = false;
		
		/// <summary>
		/// The DevIL identifier of the image.
		/// </summary>
		protected int ilId;

		/// <summary>
		/// Loads an image from the stream.
		/// </summary>
		/// <param name="stream"></param>
		/// <remarks>Writes it to a temporary file first, 
		/// then reads it with LoadFile(). Kinda hackish 
		/// but I can't figure out how to convince DevIL
		/// to load it directly from the stream.</remarks>
		public void LoadStream(Stream stream)
		{
			// read the data
			int N = (int)stream.Length;
			byte[] data = new byte[N];
			stream.Read(data, 0, N);

			// write to a file
			string fileName = Path.GetTempPath() + "temp.png";
			FileStream fileStream = new FileStream(fileName, FileMode.Create);
			fileStream.Write(data, 0, N);
			fileStream.Close();

			// load the file
			LoadFile(fileName);
		}

		/// <summary>
		/// Loads a file image.
		/// </summary>
		public void LoadFile(string fileName)
		{
			il.ilGenImages(1, out ilId);
            il.ilBindImage(ilId);
            il.ilLoadImage(fileName);
			
			imageSize.X = (double)il.ilGetInteger(il.IL_IMAGE_WIDTH);
			imageSize.Y = (double)il.ilGetInteger(il.IL_IMAGE_HEIGHT);
		}

		protected Coord imageSize;
		/// <value>
		/// The size of the image.
		/// </value>
		public override Coord MinSize
		{
			get {return imageSize;}
		}

		
		
		public override void ComputeGeometry()
		{
			base.ComputeGeometry();
		}

		
		public override void RenderOverlay(Viewport viewport)
		{
			base.RenderOverlay(viewport);

			gl.glRasterPos3d(position.X, position.Y + imageSize.Y, 0);
			gl.glPixelZoom(1f, -1f); // need to flip the image rightside up
			
            il.ilBindImage(ilId);
			gl.glDrawPixels((int)imageSize.X, (int)imageSize.Y, gl.GL_RGBA, gl.GL_UNSIGNED_BYTE, il.ilGetData());

		}

		
		
	}
}
