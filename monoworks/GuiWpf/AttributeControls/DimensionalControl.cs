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
using System.Collections.Generic;

using System.Windows;
using System.Windows.Controls;

using MonoWorks.Base;
using MonoWorks.Model;

namespace MonoWorks.GuiWpf.AttributeControls
{
	/// <summary>
	/// Control for dimensional attributes.
	/// </summary>
	/// <typeparam name="T">A dimensional type.</typeparam>
	public class DimensionalControl<T> : NumericControl where T : Dimensional
	{
		public DimensionalControl(Entity entity, AttributeMetaData metaData) 
			: base(entity, metaData)
		{
			Update();

			// add the unit label
			dockPanel.AddLabel(dimVal.DisplayUnits);

			// custom settings for the spinner
			if (dimVal is Angle)
				spin.Step = 15;
		}

		T dimVal;

		protected override void OnValueChanged(double val)
		{
			dimVal.DisplayValue = val;
			Entity.MakeDirty();
			RaiseAttributeChanged();
		}


		public override void Update()
		{
			if (!InternalUpdate)
			{
				dimVal = Entity.GetAttribute(MetaData.Name) as T;
				spin.Value = dimVal.DisplayValue;
			}
		}

	}
}
