// KeyEvent.cs - MonoWorks Project
//
//  Copyright (C) 2009 Andy Selvig
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

using MonoWorks.Rendering;
using MonoWorks.Framework;

namespace MonoWorks.Rendering.Events
{
	/// <summary>
	/// Enumerated representation for special characters.
	/// </summary>
	/// <remarks>The value are standard ASCII, except for the 
	/// directions which aren't represented in ascii.</remarks>
	public enum SpecialKey {
		Up = 38,
		Right = 39,
		Down = 40,
		Left = 41,
		Backspace = 8,
		Enter = 13,
		Delete = 46,
		Escape = 27,
		Home,
		End,
		Tab = 9,
		None = -1
	};

	/// <summary>
	/// Keyboard event.
	/// </summary>
	public class KeyEvent : Event
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		/// <param name="keyVal"></param>
		/// <param name="modifier"></param>
		public KeyEvent(int keyVal, InteractionModifier modifier)
		{
			Value = keyVal;
			Modifier = modifier;
		}

		/// <summary>
		/// The ASCII key value.
		/// </summary>
		public int Value { get; private set; }

		/// <summary>
		/// The special key representation of the event, or none if it's a character.
		/// </summary>
		public SpecialKey SpecialKey
		{
			get
			{
				foreach (SpecialKey val in Enum.GetValues(typeof(SpecialKey)))
				{
					if ((int)val == Value)
						return val;
				}
				return SpecialKey.None;
			}
		}

		/// <summary>
		/// Key modifier.
		/// </summary>
		public InteractionModifier Modifier { get; private set; }
		
		
		public override string ToString()
		{
			return string.Format("[KeyEvent: Value={0}, SpecialKey={1}, Modifier={2}]", Value, SpecialKey, Modifier);
		}


	}
}
