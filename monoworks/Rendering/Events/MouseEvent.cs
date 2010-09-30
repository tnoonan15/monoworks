// MouseEvent.cs - MonoWorks Project
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
	/// Base class for all mouse events.
	/// </summary>
	public class MouseEvent : Event
	{
		public MouseEvent(Scene scene, Coord viewportPos)
			: this(scene, viewportPos, InteractionModifier.None)
		{
		}

		public MouseEvent(Scene scene, Coord viewportPos, InteractionModifier modifier)
			: base(scene)
		{
			ViewportPos = viewportPos;
			Pos = viewportPos.Copy();
			Modifier = modifier;
		}
		
		/// <value>
		/// The position of the event with respect to the viewport.
		/// </value>
		public Coord ViewportPos { get; private set; }

		/// <value>
		/// The position of the event.
		/// </value>
		public Coord Pos { get; set; }

		/// <value>
		/// The modifier for the event.
		/// </value>
		public InteractionModifier Modifier { get; set; }

		/// <summary>
		/// The hit line in 3D space.
		/// </summary>
		public HitLine HitLine { get; set; }

		/// <summary>
		/// Makes a copy of the event.
		/// </summary>
		public MouseEvent Copy()
		{
			return new MouseEvent(Scene, ViewportPos.Copy(), Modifier);
		}
	}
}
