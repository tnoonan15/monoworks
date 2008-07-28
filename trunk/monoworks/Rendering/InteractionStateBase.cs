// InteractionStateBase.cs - MonoWorks Project
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

namespace MonoWorks.Rendering
{
	/// <summary>
	/// Possible user interaction modes.
	/// </summary>
	public enum InteractionMode {None, Rotate, Pan, Dolly, Zoom};
	
	/// <summary>
	/// Base class for mouse interaction state.
	/// </summary>
	/// <remarks> This should be subclassed for every GUI implementation.</remarks>
	public abstract class InteractionStateBase
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public InteractionStateBase()
		{
			mouseMode = InteractionMode.None;
			lastX = 0;
			lastY = 0;
		}
		
		protected InteractionMode mouseMode;
		/// <value>
		/// The current interaction mode.
		/// </value>
		public InteractionMode MouseMode
		{
			get {return mouseMode;}
		}
		
		
		protected double lastX;
		/// <value>
		/// The last x position registered.
		/// </value>
		public double LastX
		{
			get {return lastX;}
			set {lastX = value;}
		}
		
		protected double lastY;
		/// <value>
		/// The last y position registered.
		/// </value>
		public double LastY
		{
			get {return lastY;}
			set {lastY = value;}
		}
		
	}
}
