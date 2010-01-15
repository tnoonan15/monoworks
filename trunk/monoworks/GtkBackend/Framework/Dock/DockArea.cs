// DockArea.cs - Slate Mono Application Framework
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

namespace MonoWorks.GtkBackend.Framework.Dock
{
	
	/// <summary>
	/// The main dock area.
	/// </summary>
	public class DockArea : Gtk.Frame
	{
		
		/// <summary>
		/// Default constructor.
		/// </summary>
		/// <remarks> Sets HandlesDocuments to false.</remarks>
		public DockArea() : this(false)
		{
		}
		
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="handlesDocuments"> Whether or not the area handle documents. </param>
		public DockArea(bool handlesDocuments) : base()
		{
			ShadowType = Gtk.ShadowType.None;
			Shadow = Gtk.ShadowType.None;
			this.AddEvents((int)Gdk.EventMask.AllEventsMask);
			this.handlesDocuments = handlesDocuments;
		}
		
		
		protected DockManager manager;
		/// <value>
		/// The dock manager.
		/// </value>
		public DockManager Manager
		{
			get {return manager;}
			set	{manager = value;}
		}
		
		protected bool handlesDocuments = false;
		/// <value>
		/// Set true to make the area handle only documents instead of general dockables.
		/// </value>
		public bool HandlesDocuments
		{
			get {return handlesDocuments;}
			set {handlesDocuments = value;}
		}

				
	}
}
