// AbstractSketcher.cs - MonoWorks Project
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

using MonoWorks.Rendering.Events;

namespace MonoWorks.Model
{
	/// <summary>
	/// Base class for skecthers (classes that handle the user interface of sketching).
	/// </summary>
	public abstract class AbstractSketcher<T> : ISketcher where T : Sketchable
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		/// <param name="sketchble"></param>
		public AbstractSketcher(Sketch sketch, T sketchble)
		{
			Sketch = sketch;
			Sketchable = sketchble;
		}

		/// <summary>
		/// The sketch being sketched on.
		/// </summary>
		public Sketch Sketch { get; private set; }

		/// <summary>
		/// The sketchable being sketched.
		/// </summary>
		public T Sketchable { get; private set; }

		/// <summary>
		/// Apply the current sketching operation.
		/// </summary>
		public abstract void Apply();

	
#region Mouse and Keyboard Handling

		public virtual void OnButtonPress(MouseButtonEvent evt)
		{
		}

		public virtual void OnButtonRelease(MouseButtonEvent evt)
		{
		}

		public virtual void OnMouseMotion(MouseEvent evt)
		{
		}

		public virtual void OnMouseWheel(MouseWheelEvent evt)
		{
		}

		public virtual void OnKeyPress(KeyEvent evt)
		{
		}

#endregion

	}
}
