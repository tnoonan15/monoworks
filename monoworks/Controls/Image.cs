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
using MonoWorks.Framework;
using MonoWorks.Rendering;


namespace MonoWorks.Controls
{
	
	/// <summary>
	/// Control containing an image.
	/// </summary>
	public class Image : Control2D
	{
		/// <summary>
		/// Loads an image from a stream.
		/// </summary>
		/// <param name="stream"></param>
		public Image(Stream stream)
			: base()
		{

			LoadStream(stream);
		}

		/// <summary>
		/// Loads an image from a file.
		/// </summary>
		/// <param name="fileName"> The name of the image file. </param>
		public Image(string fileName) : base()
		{
			
			LoadFile(fileName);
		}


		/// <summary>
		/// Loads an image from the stream.
		/// </summary>
		/// <param name="stream"></param>
		/// <remarks>Writes it to a temporary file first, 
		/// then reads it with LoadFile(). Kinda hackish 
		/// but I'm too lazy to figure out how to
		/// load it directly from the stream.</remarks>
		public void LoadStream(Stream stream)
		{
			// read the data
			int N = (int)stream.Length;
			byte[] data = new byte[N];
			stream.Read(data, 0, N);

			// write to a file
			string fileName = System.IO.Path.GetTempPath() + "temp.png";
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
			surface = new Cairo.ImageSurface(fileName);
			
		}
		
		/// <value>
		/// The surface containing the image.
		/// </value>
		protected Cairo.ImageSurface surface;

		public override void ComputeGeometry()
		{
			base.ComputeGeometry();
			
			if (surface != null)
				MinSize = new Coord(surface.Width, surface.Height);
			else
				MinSize = new Coord();
			
			if (UserSize != null)
				RenderSize = Coord.Max(MinSize, UserSize);
			else
				RenderSize = MinSize;
		}

		
		protected override void Render(RenderContext context)
		{
			base.Render(context);

			context.Cairo.Save();
			context.Cairo.SetSourceSurface(surface, (int)LastPosition.X, (int)LastPosition.Y);
			context.Cairo.Paint();
			context.Cairo.Restore();
		}

		
		
		#region Convenience Methods
		
		
		/// <summary>
		/// Gets an icon with the given name and size from the resources.
		/// </summary>
		public static Image GetIcon(string name, int size)
		{
			return new Image(ResourceHelper.GetStream(String.Format("icons{0}.{1}.png", size, name), "MonoWorks.Resources"));
		}
		
		
		#endregion
		
		
		}
		
}
	