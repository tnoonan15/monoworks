// AbstractDecorator.cs - MonoWorks Project
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

using Cairo;

using MonoWorks.Base;
using MonoWorks.Rendering;
using MonoWorks.Rendering.Events;


namespace MonoWorks.Rendering.Controls
{
	
	/// <summary>
	/// Abstract base class for decorators that decorate the controls by
	/// deciding how render their background and outline.
	/// </summary>
	public abstract class AbstractDecorator
	{
		
		public AbstractDecorator()
		{
		}
		
		
		/// <summary>
		/// Renders the background for the given control.
		/// </summary>
		public abstract void RenderBackground(Context cr, Control2D control);
		
		
	}
}
