// Extensions.cs - MonoWorks Project
//
//  Copyright (C) 2010 Andy Selvig
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

using System.Windows.Forms;
using Point = System.Drawing.Point;

using MonoWorks.Base;
using MonoWorks.Rendering.Events;

namespace MonoWorks.SwfBackend
{

	/// <summary>
	/// Extensions for the System.Windows.Forms.
	/// </summary>
	public static class Extensions
	{
		/// <summary>
		/// Generates a coord from the point.
		/// </summary>
		/// <param name="point"></param>
		/// <returns></returns>
		public static Coord Coord(this Point point)
		{
			return new Coord(point.X, point.Y);
		}


		/// <summary>
		/// Returns the button number of the MouseButtons enum.
		/// </summary>
		/// <param name="buttons"></param>
		/// <returns></returns>
		public static int ButtonNumber(MouseButtons buttons)
		{
			switch (buttons)
			{
			case MouseButtons.Left:
				return 1;
			case MouseButtons.Middle:
				return 2;
			case MouseButtons.Right:
				return 3;
			}
			return 0;
		}

		/// <summary>
		/// Gets the interaction modifier associated with the swf keys.
		/// </summary>
		/// <param name="keys"></param>
		/// <returns></returns>
		public static InteractionModifier GetModifier(Keys keys)
		{
			switch (keys)
			{
			case Keys.Control:
				return InteractionModifier.Control;
			case Keys.Shift:
				return InteractionModifier.Shift;
			case Keys.Alt:
				return InteractionModifier.Alt;
			default:
				return InteractionModifier.None;
			}
		}


	}
}
