// ControlExtensions.cs - MonoWorks Project
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

using System.Windows;
using System.Windows.Controls;
using sd = System.Drawing;
using wf = System.Windows.Forms;

using MonoWorks.Base;
using MonoWorks.Rendering;

namespace MonoWorks.GuiWpf
{

	/// <summary>
	/// Extensions for the System.Windows.Forms.
	/// </summary>
	public static class SwfExtensions
	{
		/// <summary>
		/// Generates a coord from the point.
		/// </summary>
		/// <param name="point"></param>
		/// <returns></returns>
		public static Coord Coord(this sd.Point point)
		{
			return new Coord(point.X, point.Y);
		}


		/// <summary>
		/// Returns the button number of the MouseButtons enum.
		/// </summary>
		/// <param name="buttons"></param>
		/// <returns></returns>
		public static int ButtonNumber(wf.MouseButtons buttons)
		{
			switch (buttons)
			{
			case wf.MouseButtons.Left:
				return 1;
			case wf.MouseButtons.Middle:
				return 2;
			case wf.MouseButtons.Right:
				return 3;
			}
			return 0;
		}

	}



	/// <summary>
	/// Extensions for the combo box control.
	/// </summary>
	public static class ComboBoxExtensions
	{
		/// <summary>
		/// Convenience method for adding a text item.
		/// </summary>
		/// <param name="combo"></param>
		/// <param name="text"></param>
		public static void AddItem(this ComboBox combo, string text)
		{
			ComboBoxItem item = new ComboBoxItem();
			item.Content = text;
			combo.Items.Add(item);
		}

		/// <summary>
		/// Convenience method to get the selected item as a string.
		/// </summary>
		/// <param name="combo"></param>
		/// <returns></returns>
		public static string GetSelectedText(this ComboBox combo)
		{
			return (string)((combo.SelectedItem as ComboBoxItem).Content); 
		}
	}

}