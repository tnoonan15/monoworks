// Event.cs - MonoWorks Project
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

namespace MonoWorks.Rendering.Events
{

	/// <summary>
	/// Possible interaction modifiers.
	/// </summary>
	[Flags()]
	public enum InteractionModifier
	{
		None = 256,
		Shift = 512,
		Control = 1024,
		Alt = 2048
	};
	
	/// <summary>
	/// Base class for all rendering events.
	/// </summary>
	public abstract class Event
	{
		
		public Event(Scene scene)
		{
			Scene = scene;
		}
		
		/// <summary>
		/// The scene associated with this event.
		/// </summary>
		public Scene Scene { get; set; }
		
		/// <value>
		/// Whether the event has been handled.
		/// </value>
		public bool IsHandled
		{
			get; 
			private set;
		}
		
		/// <summary>
		/// The last object that handled this event.
		/// </summary>
		public Renderable LastHandler {get; private set;}
		
		/// <summary>
		/// Marks the event as handled.
		/// </summary>
		/// <param name="handler">The object responsible for handling the event.</param>
		public void Handle(Renderable handler)
		{
			IsHandled = true;
			LastHandler = handler;
		}
		
	}
}
