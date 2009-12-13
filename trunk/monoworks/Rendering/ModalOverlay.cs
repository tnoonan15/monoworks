// ModalOverlay.cs - MonoWorks Project
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
	/// Overlays that can be displayed in a modal (blocking all other UI) form in the viewport.
	/// </summary>
	public class ModalOverlay : Overlay
	{
		
		/// <summary>
		/// This event gets raised when the modal overlay whishes to be closed.
		/// </summary>
		public event EventHandler Closed;
		
		/// <summary>
		/// Closes the modal overlay.
		/// </summary>
		public void Close()
		{
			if (Closed)
				Closed(this, new EventArgs());
		}
		
	}
}
