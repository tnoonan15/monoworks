// IAnimatable.cs - MonoWorks Project
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


namespace MonoWorks.Rendering
{
	/// <summary>
	/// Interface for objects that can animate their behavior.
	/// </summary>
	public interface IAnimatable
	{

		/// <summary>
		/// Recomputes the geometry based on the progress of the animation.
		/// </summary>
		/// <param name="progress">From 0 to 1.</param>
		void Animate(double progress);

		/// <summary>
		/// Notifies the animatable it's done with the animation.
		/// </summary>
		void EndAnimation();

	}
}
