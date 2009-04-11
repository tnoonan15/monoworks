// EntityControl.cs - MonoWorks Project
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
	/// Control for attributes that are entities.
	/// </summary>	
	public class EntityControl<T> : AttributeControl where T : Entity
	{		
		public EntityControl(Entity entity, AttributeMetaData metaData) : base(entity, metaData)
		{
			combo = Gtk.ComboBox.NewText();
			combo.Changed += HandleChanged;
			PackStart(combo, true, true, Padding);
			
			Update();
		}
		
		private Gtk.ComboBox combo;
	
		public override void Update ()
		{
			// populate the combo
			combo.Clear();
			T attr = Entity.GetAttribute(MetaData.Name) as T;
			int i = 0;
			foreach (var entity in Entity.TheDrawing.GetChildren<T>())
			{
				Console.WriteLine("adding " + entity.Name);
				combo.AppendText(entity.Name);
				if (attr != null && attr.Name == entity.Name)
					combo.Active = i;
				i++;
			}
		}

		/// <summary>
		/// Handles the value of the combo changing.
		/// </summary>
		private void HandleChanged(object sender, EventArgs e)
		{
			if (InternalUpdate)
				return;
			string active = combo.ActiveText;			
			foreach (var entity in Entity.TheDrawing.GetChildren<T>())
			{
				if (entity.Name == active)
				{
					Entity.SetAttribute(MetaData.Name, entity);
					Entity.MakeDirty();
					RaiseAttributeChanged();
				}
			}
		}
		
	}
}
