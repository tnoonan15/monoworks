// Action.cs - MonoWorks Project
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

namespace MonoWorks.Model
{
	
	/// <summary>
	/// The Action class is the base class for editing actions that are tracked by the document. 
	/// </summary>
	public class Action
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public Action()
		{
		}
		

		/// <summary>
		/// Undo the action.
		/// </summary>
		public virtual void Undo()
		{
			throw new Exception("Undo() must be implemented by subclasses.");
		}
		
		/// <summary>
		/// Redo the action.
		/// </summary>
		public virtual void Redo()
		{
			throw new Exception("Redo() must be implemented by subclasses.");
		}
		
	}
}
