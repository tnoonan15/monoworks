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


namespace MonoWorks.Controls
{
	
	/// <summary>
	/// Exception for when image.Parse() fails.
	/// </summary>
	public class ImageParseException : Exception
	{
		public ImageParseException() : 
			base("Image literals must be a comma-separated resource and assembly names, like \"myImage.png,MyAssembly\" ")
		{
			
		}
	}
	
	/// <summary>
	/// Control containing an image.
	/// </summary>
	public class Image : Control2D, IStringParsable
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public Image() : base()
		{			
		}
		
		/// <summary>
		/// Loads an image from a stream.
		/// </summary>
		/// <param name="stream"></param>
		public Image(Stream stream)
			: this()
		{

			LoadStream(stream);
		}

		/// <summary>
		/// Loads an image from a file.
		/// </summary>
		/// <param name="fileName"> The name of the image file. </param>
		public Image(string fileName) : this()
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
			surface = CairoHelper.ImageSurfaceFromStream(stream);
		}

		/// <summary>
		/// Loads a file image.
		/// </summary>
		public void LoadFile(string fileName)
		{	
			surface = new Cairo.ImageSurface(fileName);
			
		}
		
		/// <summary>
		/// Parses a string to load an image.
		/// </summary>
		/// <remarks>The string shoud be a comma-separated resource and assembly name,
		/// like "myImage.png,MyAssembly".
		/// </remarks>
		public void Parse(string valString)
		{
			var comps = valString.Split(',');
			if (comps.Length != 2)
				throw new ImageParseException();
			LoadStream(ResourceHelper.GetStream(comps[0], comps[1]));
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
			ApplyUserSize();
		}

		
		protected override void Render(RenderContext context)
		{
			base.Render(context);

			context.Cairo.Save();
			context.Cairo.SetSourceSurface(surface, (int)LastPosition.X, (int)LastPosition.Y);
			context.Cairo.Paint();
			context.Cairo.Restore();
		}

		
		/// <summary>
		/// Renders the image at the current position in the context.
		/// </summary>
		public void RenderHere(RenderContext context)
		{
			var pos = context.Cairo.CurrentPoint;
			context.Cairo.Save();
			context.Cairo.SetSourceSurface(surface, (int)pos.X, (int)pos.Y);
			context.Cairo.Paint();
			context.Cairo.Restore();
		}
				
		
	}
		
}
	