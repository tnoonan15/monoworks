// Axes.cs - MonoWorks Project
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

using MonoWorks.Base;

namespace MonoWorks.Rendering
{

	/// <summary>
	/// Definition of some text.
	/// </summary>
	public struct TextDef
	{

		public TextDef(int size)
		{
			Text = "";
			Size = size;
			Color = ColorManager.Global["Black"];
			Position = new Coord();
			Angle = new Angle();
			HorizontalAlignment = HorizontalAlignment.Left;
		}

		/// <value>
		/// The text to render.
		/// </value>
		public string Text;

		/// <value>
		/// The font size.
		/// </value>
		public int Size;

		/// <value>
		/// The text color.
		/// </value>
		public Color Color;

		/// <value>
		/// The position of the text.
		/// </value>
		public Coord Position;

		/// <value>
		/// The angle from horizontal.
		/// </value>
		public Angle Angle;

		/// <value>
		/// The horizontal alignment.
		/// </value>
		public HorizontalAlignment HorizontalAlignment;

	}

}