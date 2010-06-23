// 
//  Action.cs - MonoWorks Project
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

using System.Collections.Generic;

namespace MonoWorks.Base
{
	
	
	public class UiAction : IMwxObject
	{
		public UiAction()
		{
		}
		
		
		public void AddChild(IMwxObject child)
		{
			throw new System.NotImplementedException();
		}

		public IList<IMwxObject> GetMwxChildren()
		{
			return new List<IMwxObject>();
		}
		
		/// <summary>
		/// The name of the action.
		/// </summary>
		[MwxProperty]
		public string Name {get; set;}
		
		/// <value>
		/// The name of the icon to use for the action.
		/// </value>
		[MwxProperty]
		public string IconName {get; set;}

		/// <value>
		/// The tooltip to display when hovering over controls that link to this action.
		/// </value>
		[MwxProperty]
		public string ToolTip { get; set; }
		
		/// <summary>
		/// If true, the action is bistate and can be toggled.
		/// </summary>
		[MwxProperty]
		public bool IsTogglable { get; set; }
		
		public IMwxObject Parent { get; set; }
		
		/// <summary>
		/// Gets called when the event is activated.
		/// </summary>
		public event System.EventHandler Activated;
		
		/// <summary>
		/// Activates the action.
		/// </summary>
		public void Activate(object sender, System.EventArgs args)
		{
			if (Activated != null)
				Activated(sender, args);
		}
		
	}
}

