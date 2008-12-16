// Container.cs - MonoWorks Project
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

namespace MonoWorks.Rendering.Controls
{
	
	/// <summary>
	/// Base class for control container.
	/// </summary>
	public abstract class Container : Control
	{
		
		public Container() : base()
		{
		}


#region Children


		protected List<Control> children = new List<Control>();
		/// <value>
		/// The container's children.
		/// </value>
		public IEnumerable<Control> Children
		{
			get {return children;}
		}

		/// <value>
		/// Access the children by index.
		/// </value>
		public Control this[int index]
		{
			get {return GetChild(index);}
			set {SetChild(index, value);}
		}

		/// <summary>
		/// Get a child by index.
		/// </summary>
		public Control GetChild(int index)
		{
			if (index < 0 || index >= children.Count)
				throw new IndexOutOfRangeException("Invalid container child index: " + index.ToString());
			return children[index];
		}

		/// <summary>
		/// Set a child by index.
		/// </summary>
		/// <remarks>If index is equal to NumChildren, it will be appended to the end.</remarks>
		public void SetChild(int index, Control child)
		{
			if (index < 0 || index > children.Count)
				throw new IndexOutOfRangeException("Invalid container child index: " + index.ToString());
			if (index == children.Count)
				children.Add(child);
			else
				children[index] = child;
		}

#endregion

		
	}
}
