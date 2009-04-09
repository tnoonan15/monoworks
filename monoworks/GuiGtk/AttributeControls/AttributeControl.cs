// AttributeControl.cs - MonoWorks Project
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

using MonoWorks.Base;
using MonoWorks.Modeling;
using MonoWorks.Modeling.ViewportControls;

namespace MonoWorks.GuiGtk.AttributeControls
{	
	/// <summary>
	/// Base class for Gtk attribute controls.
	/// </summary>
	public abstract class AttributeControl : Gtk.VBox, IAttributeControl
	{		
		public AttributeControl(Entity entity, AttributeMetaData metaData)
		{
			Entity = entity;
			MetaData = metaData;
			
			// add the label
			var label = new Gtk.Label(metaData.Name);
			label.SetAlignment(0,0);
			PackStart(label, false, true, Padding);
			
			
		}
		
		/// <value>
		/// Internal padding for controls.
		/// </value>
		protected static uint Padding = 6;
		
		/// <value>
		/// The entity that the attribute belongs to.
		/// </value>
		public Entity Entity {get; private set;}
		
		/// <value>
		/// The meta data for this control.
		/// </value>
		public AttributeMetaData MetaData {get; private set;}
		
		/// <summary>
		/// Gets the appropriate control for the given attribute.
		/// </summary>
		public static AttributeControl GetControl(Entity entity, AttributeMetaData metaData)
		{
			switch (metaData.TypeName)
			{
			case "System.String":
				return new StringControl(entity, metaData);
			case "MonoWorks.Base.Length":
				return new DimensionalControl<Length>(entity, metaData);
			default:
				return new NullControl(entity, metaData);
			}
		}
		
		
		
#region Attribute Changed Event

		/// <summary>
		/// Raised when the value of the attribute changed.
		/// </summary>
		public event AttributeChangedHandler AttributeChanged;

		/// <summary>
		/// Raise the attribute changed handler, if anyone is listening.
		/// </summary>
		protected void RaiseAttributeChanged()
		{
			if (AttributeChanged != null)
				AttributeChanged(this);
		}

#endregion


#region Updating

		/// <summary>
		/// Update the control based on the attribute.
		/// </summary>
		public abstract void Update();

		/// <summary>
		/// Whether or not an internal update is occuring.
		/// </summary>
		protected bool InternalUpdate { get; private set; }

		/// <summary>
		/// Begin an internal update.
		/// </summary>
		protected void BeginUpdate()
		{
			InternalUpdate = true;
		}

		/// <summary>
		/// End an internal update.
		/// </summary>
		protected void EndUpdate()
		{
			InternalUpdate = false;
		}

#endregion
		
	}
}
