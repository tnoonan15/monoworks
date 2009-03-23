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
using System.Collections.Generic;

using System.Windows;
using System.Windows.Controls;

using MonoWorks.Model;

namespace MonoWorks.GuiWpf.AttributeControls
{
	/// <summary>
	/// Control for attributes that store references to other entities.
	/// </summary>
	public class EntityControl<T> : AttributeControl where T : Entity
	{

		public EntityControl(Entity entity, AttributeMetaData metaData)
			: base(entity, metaData)
		{
			combo = new ComboBox();
			Children.Add(combo);
			combo.SelectionChanged += OnSelectionChanged;

			Update();
		}


		private ComboBox combo;

		/// <summary>
		/// Handles the combo's selection being changed by the user.
		/// </summary>
		void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (e.AddedItems.Count > 0 && !InternalUpdate)
			{
				Label selected = e.AddedItems[0] as Label;
				foreach (var child in Entity.TheDrawing.GetChildren<T>())
				{
					if (child.Name == selected.Content as string)
					{
						BeginUpdate();
						Entity.SetAttribute(MetaData.Name, child);
						Entity.MakeDirty();
						EndUpdate();
						RaiseAttributeChanged();
						break;
					}
				}
			}
		}

		public override void Update()
		{
			if (InternalUpdate)
				return;

			combo.Items.Clear();

			BeginUpdate();
			foreach (var child in Entity.TheDrawing.GetChildren<T>())
			{
				Label item = new Label() { Content = child.Name };
				combo.Items.Add(item);
				Entity entity = Entity.GetAttribute(MetaData.Name) as T;
				if (entity != null && child == Entity.GetAttribute(MetaData.Name))
					combo.SelectedItem = item;
			}
			EndUpdate();

		}

	}
}
