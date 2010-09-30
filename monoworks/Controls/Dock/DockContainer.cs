// 
//  DockContainer.cs - MonoWorks Project
//  
//  Author:
//       Andy Selvig <ajselvig@gmail.com>
// 
//  Copyright (c) 2010 Andy Selvig
// 
//  This library is free software; you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as
//  published by the Free Software Foundation; either version 2.1 of the
//  License, or (at your option) any later version.
// 
//  This library is distributed in the hope that it will be useful, but
//  WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
//  Lesser General Public License for more details.
// 
//  You should have received a copy of the GNU Lesser General Public
//  License along with this library; if not, write to the Free Software
//  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MonoWorks.Rendering;
using MonoWorks.Rendering.Events;

namespace MonoWorks.Controls.Dock
{
	/// <summary>
	/// A scene container specifically for the docking functionality.
	/// </summary>
	public abstract class DockContainer : SceneContainer
	{
		public DockContainer(Viewport viewport) : base(viewport)
		{

		}

		/// <summary>
		/// Must be implemented by subclasses to determine if the given mouse event is over a potential dock slot.
		/// </summary>
		protected abstract DockSlot SlotTest(MouseEvent evt);

		/// <summary>
		/// Finds a potential slot to dock in based on the mouse event.
		/// </summary>
		/// <remarks>This method traverses the entire scene tree in reverse to determine
		/// which container has the most appropriate slot.</remarks>
		public DockSlot FindSlot(MouseEvent evt)
		{
			foreach (var child in Children)
			{
				if (child is DockContainer)
				{
					var slot = (child as DockContainer).SlotTest(evt);
					if (slot != null)
						return slot;
				}
			}

			return SlotTest(evt);
		}
	}
}
