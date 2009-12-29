// RenderContext.cs - MonoWorks Project
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

using Cairo;

using MonoWorks.Rendering;


namespace MonoWorks.Controls
{
	
	/// <summary>
	/// Contains a Cairo context and a Decorator for rendering controls.
	/// </summary>
	public class RenderContext
	{
		
		public RenderContext(Context cr, AbstractDecorator decorator) 
		{
			this.Cairo = cr;
			this.Decorator = decorator;
			Decorator.Context = this;
		}
		
		/// <value>
		/// The Cairo context to render to.
		/// </value>
		public Context Cairo {get; private set;}
		
		/// <value>
		/// The decorator used to decorate the controls.
		/// </value>
		public AbstractDecorator Decorator {get; private set;}
		
		/// <summary>
		/// Stack to keep track of the current position that rendering operations should use in the context.
		/// </summary>
		private Stack<PointD> positionStack = new Stack<PointD>();
		
		/// <summary>
		/// Saves the state of the context and pushes the current position onto the position stack.
		/// </summary>
		/// <returns> The position that was pushed. </returns>
		/// <remarks>Always call Pop() exactly once at some point after calling this method.</remarks>
		public PointD Push()
		{
			Cairo.Save();
			var pos = Cairo.CurrentPoint;
			positionStack.Push(pos);
			return pos;
		}
		
		/// <summary>
		/// Restores the context state and pops the last position off the stack.
		/// </summary>
		/// <remarks>Always call Push() exactly once at some point before calling this method.</remarks>
		public void Pop()
		{
			if (positionStack.Count < 1)
				throw new Exception("The position stack is empty. This means you forgot to call RenderContext.Push() at some point.");
			Cairo.Restore();
			Cairo.MoveTo(positionStack.Pop());
		}
		
	}
}
