// 
//  Events.cs - MonoWorks Project
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

namespace MonoWorks.Base
{

	/// <summary>
	/// An event that's used for a value changing.
	/// </summary>
	public class ValueChangedEvent<T> : EventArgs
	{
		public ValueChangedEvent(T oldVal, T newVal)
		{
			OldValue = oldVal;
			NewValue = newVal;
		}
	
		/// <summary>
		/// The old value.
		/// </summary>
		public T OldValue { get; private set; }
	
		/// <summary>
		/// The new value.
		/// </summary>
		public T NewValue { get; private set; }
	}
	
}