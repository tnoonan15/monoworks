// Toolshelf.cs - MonoWorks Project
//
// Copyright (C) 2008 Andy Selvig
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA

using System;

using Qyoto;

namespace MonoWorks.Gui
{
	
	/// <summary>
	/// A toolshelf is contained inside a toolbox. 
	/// It contains a list of icons representing different tools.
	/// </summary>
	public class Toolshelf : QToolBar
	{

		/// <summary>
		/// Default constructor.
		/// </summary>
		/// <param name="parent"> The  <see cref="Toolbox"/> this shelf belongs to. </param>
		public Toolshelf(Toolbox parent) : base(parent)
		{
			this.IconSize = new QSize(48, 48);
			this.Orientation = Orientation.Vertical;
			this.SetToolButtonStyle( ToolButtonStyle.ToolButtonTextUnderIcon);
		}
				
 
		
	}
}
