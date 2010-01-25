// 
//  CairoHelper.cs - MonoWorks Project
//  
//  Author:
//       Andy Selvig <ajselvig@gmail.com>
// 
//  Copyright (c) 2010 Andy Selvig
// 
//  This library is free software; you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as
//  published by the Free Software Foundation; either version 2.1 of the
//  License, or (at your option) any later version.
// 
//  This library is distributed in the hope that it will be useful, but
//  WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
//  Lesser General Public License for more details.
// 
//  You should have received a copy of the GNU Lesser General Public
//  License along with this library; if not, write to the Free Software
//  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA

using Cairo;
using System.IO;

using MonoWorks.Base;

namespace MonoWorks.Rendering
{
	/// <summary>
	/// Helper methods for cairo rendering
	/// </summary>
	public static class CairoHelper
	{
		/// <summary>
		/// Converts a Cairo point to a Coord. 
		/// </summary>
		public static Coord Coord(this PointD point)
		{
			return new Coord(point.X, point.Y);
		}

		/// <summary>
		/// Converts a coord to a Cairo point. 
		/// </summary>
		public static PointD PointD(this Coord coord)
		{
			return new PointD(coord.X, coord.Y);
		}
		
		/// <summary>
		/// Creates an image surface from an image inside a stream.
		/// </summary>
		public static ImageSurface ImageSurfaceFromStream(Stream stream)
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
			
			return new ImageSurface(fileName);
		}
		
	}
}

