// DimensionalControl.cs - MonoWorks Project
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
using MonoWorks.Modeling.SceneControls;

namespace MonoWorks.GtkBackend.AttributeControls
{
	/// <summary>
	/// Attribute control for dimensional values.
	/// </summary>	
	public class DimensionalControl<T> : AttributeControl where T : Dimensional
	{
		
		public DimensionalControl(Entity entity, AttributeMetaData metaData) : base(entity, metaData)
		{
			
			var hbox = new Gtk.HBox(false, 2);
			PackStart(hbox, true, true, Padding);
			
			spin = new Gtk.SpinButton(-10, 10, 0.1);
			spin.Changed += HandleChanged;
			hbox.PackStart(spin, true, true, Padding);
			
			unitsLabel = new Gtk.Label();
			hbox.PackEnd(unitsLabel, false, true, Padding);
			
			Update();
		}
		
		
		private Gtk.SpinButton spin = null;
		
		private Gtk.Label unitsLabel = null;
		
		public override void Update ()
		{
			T val = Entity.GetAttribute(MetaData.Name) as T;
			spin.Value = val.Value;
			unitsLabel.Text = val.DisplayUnits;
		}
		
		/// <summary>
		/// Handles the spins value changing.
		/// </summary>
		private void HandleChanged(object sender, EventArgs e)
		{
			T val = Entity.GetAttribute(MetaData.Name) as T;
			val.Value = spin.Value;
			Entity.MakeDirty();
			RaiseAttributeChanged();
		}

	}
}
