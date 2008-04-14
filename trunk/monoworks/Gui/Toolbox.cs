// Toolbox.cs - MonoWorks Project
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
	/// A toolbox shows a list of shelves, with one visible at a time.
	/// Each shelve contains an icon view that has tools.
	/// </summary>
	public class Toolbox : QToolBox
	{

		/// <summary>
		/// Default constructor.
		/// </summary>
		public Toolbox(QWidget parent) : base(parent)
		{
		}
		
		
		/// <summary>
		/// Creates a toolshelf in the toolbox.
		/// </summary>
		/// <param name="name"> The name of the shelf. </param>
		/// <returns> The new <see cref="Toolshelf"/>. </returns>
		public Toolshelf AddShelf(string name)
		{
			Toolshelf shelf = new Toolshelf(this);
			this.AddItem(shelf, name);
			return shelf;
		}
	}
}
