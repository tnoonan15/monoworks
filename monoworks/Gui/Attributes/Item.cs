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
		protected AttributeMetaData attribute;
		
		/// <summary>
		/// Default constructor.
		/// </summary>
		/// <param name="parent"> The parent <see cref="AttributeFrame"/>. </param>
		public Item(Frame parent, AttributeMetaData attribute) : base(parent)
		{
			this.frame = parent;
			this.attribute = attribute;
			
			wasUpdated = false;
			
			// style
			FrameShape = Shape.Box;
			SetBackgroundRole( QPalette.ColorRole.Light);
			
			// add the label and control
			QVBoxLayout layoutBox = new QVBoxLayout(this);
			layoutBox.AddWidget( new QLabel(attribute.Name, this));
			Control control = GetControl(attribute.Type); 
			control.PopulateValue(parent.Entity, attribute.Name);
			layoutBox.AddWidget(control);
			
			// set the description
			this.ToolTip = attribute.Description;
		}
		
		/// <summary>
		/// Gets a widget for the given attribute type.
		/// </summary>
		/// <param name="type"> The type of the control. </param>
		/// <returns> A new <see cref="AttributeControl"/>. </returns>
		protected Control GetControl(string type)
		{
			Control widget;
			switch (type)
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
			return widget;
		}
		
		
#region Attribute Updating
				
		/// <summary>
		/// Handles an attribute being updated.
		/// </summary>
		public void OnAttributeUpdated()
		{
			wasUpdated = true;
			frame.OnAttributeUpdated();
		}

		protected bool wasUpdated;
		/// <value>
		/// Whether the attribute has been updated.
		/// </value>
		public bool WasUpdated
		{
			get {return wasUpdated;}
		}
#endregion
	}
}
