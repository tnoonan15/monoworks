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
using MonoWorks.Rendering.Interaction;

namespace MonoWorks.Rendering.Events
{

	/// <summary>
	/// The number or times the button was clicked.
	/// </summary>
	public enum ClickMultiplicity { Single, Double, Triple };
	
	/// <summary>
	/// Mouse button press event.
	/// </summary>
	public class MouseButtonEvent : MouseEvent
	{
		
		public MouseButtonEvent(Scene scene, Coord pos, int button) 
			: this(scene, pos, button, InteractionModifier.None)
		{
		}
		
		public MouseButtonEvent(Scene scene, Coord pos, int button, InteractionModifier modifier) 
			: this(scene, pos, button, modifier, ClickMultiplicity.Single)
		{
		}

		public MouseButtonEvent(Scene scene, Coord pos, int button, InteractionModifier modifier, ClickMultiplicity multiplicity)
			: base(scene, pos, modifier)
		{
			Button = button;
			Multiplicity = multiplicity;
		}


		/// <value>
		/// The mouse button that was pressed.
		/// </value>
		public int Button { get; private set; }

		/// <value>
		/// The multiplicity for the click.
		/// </value>
		public ClickMultiplicity Multiplicity { get; private set; }
		
		
		public override string ToString()
		{
			return string.Format("[MouseButtonEvent: Button={0}, Multiplicity={1}, Modifier={2}, Handled={3}]", 
				Button, Multiplicity, Modifier, Handled);
		}

		
	}
}
