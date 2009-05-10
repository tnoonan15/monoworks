// DockableBase.cs - Slate Mono Application Framework
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

namespace MonoWorks.GuiGtk.Framework.Dock
{
	
	/// <summary>
	/// Base class for dockables.
	/// </summary>
	public class DockableBase : Gtk.Frame
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public DockableBase() : base()
		{
			Shadow = Gtk.ShadowType.None;
			SetSizeRequest(200, 200);
		}
		
		/// <value>
		/// The dockable this widget is inside.
		/// </value>
		public Dockable Dockable {get; set;}

		private string iconName = null;
		/// <value>
		/// The name of the icon associated with this dockable.
		/// </value>
		public string IconName
		{
			get {return iconName;}
			set {iconName = value;}
		}
		

#region Closing


		/// <summary>
		/// Returns true if it's alright to close the dockable.
		/// </summary>
		/// <returns> </returns>
		public virtual bool VerifyClose()
		{
			return true;
		}

#endregion
	}
}
