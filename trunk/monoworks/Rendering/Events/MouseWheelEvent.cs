using System;
using System.Collections.Generic;

using MonoWorks.Base;
using MonoWorks.Rendering.Interaction;
using MonoWorks.Framework;

namespace MonoWorks.Rendering.Events
{
	/// <summary>
	/// Possible mouse wheel directions.
	/// </summary>
	public enum WheelDirection { Up, Down };

	/// <summary>
	/// Event that stores information about mouse wheel events.
	/// </summary>
	public class MouseWheelEvent : Event
	{

		public MouseWheelEvent(Scene scene, WheelDirection direction, InteractionModifier modifier) : base(scene)
		{
			Direction = direction;
			Modifier = modifier;
		}

		public WheelDirection Direction { get; private set; }
		
		public InteractionModifier Modifier {get; private set;}

	}
}
