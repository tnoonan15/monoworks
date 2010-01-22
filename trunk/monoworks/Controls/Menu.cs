// 
//  Menu.cs - MonoWorks Project
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

using System;
using MonoWorks.Base;
using MonoWorks.Rendering;

namespace MonoWorks.Controls
{
	/// <summary>
	/// Menu that displays a list of menu items for the user to select.
	/// </summary>
	public class Menu : GenericStack<MenuItem>
	{
		public Menu()
		{
			Orientation = Orientation.Vertical;
			FontSize = 12;
		}


		#region Rendering
        
		private double _fontSize;
		/// <summary>
		/// The font size (in pixels).
		/// </summary>
		[MwxProperty]
		public double FontSize
		{
			get { return _fontSize; }
			set
			{
				_fontSize = value;
				MakeDirty();
			}
		}

		protected override void Render(RenderContext context)
		{
			context.Cairo.Save();
			context.Cairo.SetFontSize(FontSize);
			base.Render(context);
			context.Cairo.Restore();
		}

		#endregion

	}
}

