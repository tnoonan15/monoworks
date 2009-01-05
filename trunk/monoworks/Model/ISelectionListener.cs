// ISelectionListener.cs - MonoWorks Project
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

namespace MonoWorks.Model
{
	
	/// <summary>
	/// Interface for objects which would like to listen to the selection state of the drawing.
	/// </summary>
	public interface ISelectionListener
	{            
		/// <summary>
		/// Handles selection events.
		/// </summary>
		/// <param name="entity"> The selected <see cref="Entity"/>. </param>
		void OnSelect(Entity entity);
		
		/// <summary>
		/// Handles deselection events.
		/// </summary>
		/// <param name="entity"> The deselected <see cref="Entity"/>. </param>
		void OnDeselect(Entity entity);
		
		/// <summary>
		/// Handles select all events.
		/// </summary>
		void OnSelectAll();
		
		/// <summary>
		/// Handles deselect all events.
		/// </summary>
		void OnDeselectAll();

	}
}
