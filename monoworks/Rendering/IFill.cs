//   IFill.cs - MonoWorks Project
//
//    Copyright Andy Selvig 2008
//
//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU Lesser General Public License as published 
//    by the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU Lesser General Public License for more details.
//
//    You should have received a copy of the GNU Lesser General Public 
//    License along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;

using MonoWorks.Base;

namespace MonoWorks.Rendering
{
	/// <summary>
	/// A corner of the viewport.
	/// </summary>
	public enum Corner {NE, NW, SE, SW};

	/// <summary>
	/// Interface for objects that can be used to fill overlays.
	/// </summary>
	public interface IFill : ICloneable
	{
		/// <summary>
		/// Draw a rectangle at the given position with the given size.
		/// </summary>
		void DrawRectangle(Coord pos, Coord size);

		/// <summary>
		/// Draw a triangle of the given size with the specified corner.
		/// </summary>
		void DrawCorner(Coord size, Corner corner);
	}
}
