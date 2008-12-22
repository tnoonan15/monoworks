// MouseButtonEvent.cs - MonoWorks Project
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

using MonoWorks.Base;

namespace MonoWorks.Rendering.Events
{
	
	/// <summary>
	/// Mouse button press event.
	/// </summary>
	public class MouseButtonEvent : MouseEvent
	{
		
		public MouseButtonEvent(Coord pos, int button) : this(pos, button, InteractionModifier.None)
		{
		}
		
		public MouseButtonEvent(Coord pos, int button, InteractionModifier modifier) : base(pos)
		{
			this.button = button;
			this.modifier = modifier;
		}
		
		private int button;
		/// <value>
		/// The mouse button that was pressed.
		/// </value>
		public int Button
		{
			get {return button;}
		}
		
		private InteractionModifier modifier;
		/// <value>
		/// The modifier for the event.
		/// </value>
		public InteractionModifier Modifier
		{
			get {return modifier;}
		}
		
	}
}
