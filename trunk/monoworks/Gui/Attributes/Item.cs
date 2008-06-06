// AttributeItem.cs - MonoWorks Project
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

using MonoWorks.Model;

namespace MonoWorks.Gui.Attributes
{
	
	/// <summary>
	/// An item in a AttributeFrame that represents an attribute.
	/// </summary>
	public class Item : QFrame
	{

		protected Frame frame;
		
		/// <summary>
		/// Default constructor.
		/// </summary>
		/// <param name="parent"> The parent <see cref="AttributeFrame"/>. </param>
		public Item(Frame parent, string name) : base(parent)
		{
			this.frame = parent;
			
			// style
			FrameShape = Shape.Box;
			SetBackgroundRole( QPalette.ColorRole.Light);
			
			// add the label and control
			QVBoxLayout layoutBox = new QVBoxLayout(this);
			layoutBox.AddWidget( new QLabel(name, this));
			Control control = GetControl(parent.Entity, name); 
			layoutBox.AddWidget(control);
		}
		
		/// <summary>
		/// Gets a widget for the given entity attribute.
		/// </summary>
		/// <param name="entity"> A <see cref="Entity"/>. </param>
		/// <param name="name">The name of the attribute. </param>
		/// <returns> A new <see cref="AttributeControl"/>. </returns>
		protected Control GetControl(Entity entity, string name)
		{
			object obj = entity.GetAttribute(name);
			Control widget;
			switch (obj.GetType().ToString())
			{
			case "System.String":
				widget = new StringControl(this);
				break;
			case "MonoWorks.Base.Length":
				widget = new LengthControl(this);
				break;
			case "MonoWorks.Base.Angle":
				widget = new AngleControl(this);
				break;
			default:
				widget = new Control(this);
				break;
			}
			widget.PopulateValue(entity, name);
			return widget;
		}
		
		/// <summary>
		/// Handles an attribute being updated.
		/// </summary>
		public void OnAttributeUpdated()
		{
			frame.OnAttributeUpdated();
		}
	}
}
