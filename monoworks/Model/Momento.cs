// Momento.cs - MonoWorks Project
//
// Copyright (C) 2008 Andy Selvig
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

namespace MonoWorks.Model
{
	
	/// <summary>
	/// The Momento class represents an entity's attributes at a single point in time.
	/// The Entity class uses momentos to keep track of state and undo/redo state changes.
	/// </summary>
	public class Momento : Dictionary<string, object>
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public Momento() : base()
		{
		}
		
		/// <summary>
		/// Duplicates the momento.
		/// </summary>
		/// <returns> A new <see cref="Momento"/> with the same attributes. </returns>
		public Momento Duplicate()
		{
			Momento other = new Momento();
			foreach (KeyValuePair<string, object> attr in this)
			{
				if (attr.Value is ICopyable)
					other[attr.Key] = ((ICopyable)attr.Value).DeepCopy();
				else
					other[attr.Key] = attr.Value;
			}
			return other;
		}
	}
}
