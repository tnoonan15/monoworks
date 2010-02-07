// 
//  IPane.cs
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

namespace MonoWorks.Controls
{
	
	/// <summary>
	/// Interface for Panes.
	/// Panes are invisible renderables that holds a 2D control, like a pane of glass.
	/// It renders the control to a texture then places it in the scene, as well as pass 
	/// keyboard and mouse events to it.
	/// </summary>
	public interface IPane
	{
		
		/// <value>
		/// The control that gets rendered on the pane.
		/// </value>
		Control2D Control {get; set;}
		
		/// <value>
		/// Whether or not the pane is visible.
		/// </value>
		bool IsVisible {get; set;}
		
		/// <summary>
		/// The control currently in focus.
		/// </summary>
		Control2D InFocus {get; set;}
		
		/// <summary>
		/// Queues the pane to redraw its control during the next render cycle.
		/// </summary>
		void QueueRender();
		
	}
}
