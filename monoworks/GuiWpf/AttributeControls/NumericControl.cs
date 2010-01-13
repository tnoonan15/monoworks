// NumericControl.cs - MonoWorks Project
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

using MonoWorks.Modeling;
using MonoWorks.WpfBackend.Utilities;

namespace MonoWorks.WpfBackend.AttributeControls
{
	/// <summary>
	/// Attribute control for numeric (double) values.
	/// </summary>
	public class NumericControl : AttributeControl
	{
		public NumericControl(Entity entity, AttributeMetaData metaData)
			: base(entity, metaData)
		{
			dockPanel = new DockPanel();
			dockPanel.LastChildFill = true;
			dockPanel.Width = Double.NaN;
			dockPanel.HorizontalAlignment = HorizontalAlignment.Stretch;
			Children.Add(dockPanel);

			spin = new SpinControl();
			dockPanel.Children.Add(spin);
			spin.ValueChanged += OnValueChanged;
			spin.HorizontalAlignment = HorizontalAlignment.Stretch;
			spin.Width = Double.NaN;

		}

		protected DockPanel dockPanel;

		protected SpinControl spin;

		/// <summary>
		/// Handles the value being changed.
		/// </summary>
		/// <param name="val"></param>
		protected virtual void OnValueChanged(double val)
		{
			BeginUpdate();
			Entity.SetAttribute(MetaData.Name, val);
			RaiseAttributeChanged();
			EndUpdate();
		}


		public override void Update()
		{
			if (!InternalUpdate)
			{
				spin.Value = (double)Entity.GetAttribute(MetaData.Name);
			}
		}

	}
}
