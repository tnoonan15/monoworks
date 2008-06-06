// AngleControl.cs - MonoWorks Project
//
// Copyright (C) 2008 Andy Selvig
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA

using System;

using Qyoto;

using MonoWorks.Base;
using MonoWorks.Model;

namespace MonoWorks.Gui.Attributes
{
	
	/// <summary>
	/// Control for a text attribute.
	/// </summary>
	public class AngleControl : Control
	{
		QDoubleSpinBox spinBox;
		
		/// <summary>
		/// Default constructor.
		/// </summary>
		/// <param name="parent"> The parent <see cref="Item"/>. </param>
		public AngleControl(Item parent) : base(parent)
		{
			spinBox = new QDoubleSpinBox(this);
			spinBox.SetRange(-720, 720);
			spinBox.SingleStep = 15;
			hbox.AddWidget(spinBox);
			Connect(spinBox, SIGNAL("valueChanged(double)"), this, SLOT("OnAttributeUpdated()"));
		}
		
		public override void PopulateValue(Entity entity, string name)
		{
			base.PopulateValue(entity, name);

			Angle val = (Angle)entity.GetAttribute(name);
			spinBox.Value = val.DisplayValue;
		}
				
		public override void CommitValue()
		{
			base.CommitValue();
			Angle val = (Angle)entity.GetAttribute(name);
			val.DisplayValue = spinBox.Value;
			entity.MakeDirty();
		}

		
	}
}
