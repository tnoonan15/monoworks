// ColorMap.cs - Slate Mono Application Framework
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

namespace MonoWorks.Rendering
{
	
	public enum ColorMapMode {Hsv};
	
	/// <summary>
	/// Maps a number of values to a distribution of colors.
	/// </summary>
	/// <remarks> Used for automatically generating a set of unique colors for a set of points.</remarks>
	public class ColorMap
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public ColorMap()
		{
		}
		
		protected ColorMapMode mode;
		/// <value>
		/// The color map mode.
		/// </value>
		public ColorMapMode Mode
		{
			get {return mode;}
			set {mode = value;}
		}
		

		/// <summary>
		/// Gets num evenly distributed colors.
		/// </summary>
		/// <param name="num"> The number of colors to get. </param>
		/// <returns> The colors. </returns>
		public Color[] GetColors(int num)
		{
			float s = 1f;
			float v = 1f;
			Color[] colors = new Color[num];
			for (int i=0; i<num; i++)
			{
				colors[i] = Color.FromHsv((float)i/num * 360f, s, v);
			}
			return colors;
		}
		
		
	}
}
