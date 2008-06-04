// AttributeFrame.cs - MonoWorks Project
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
using System.Collections.Generic;

using Qyoto;

using MonoWorks.Model;

namespace MonoWorks.Gui.Attributes
{
	
	/// <summary>
	/// Frame that contains the attribute controls for an entity.
	/// </summary>
	public class Frame : QFrame
	{
		QVBoxLayout vbox;
		List<Item> items;
		DocFrame docFrame;
		
		/// <summary>
		/// Default constructor.
		/// </summary>
		/// <param name="parent"> The parent <see cref="QWidget"/>. </param>
		/// <param name="entity"> The <see cref="Entity"/> associated with this widget. </param>
		public Frame(DocFrame parent) : base(parent)
		{	
			docFrame = parent;
			items = new List<Item>();
			vbox = new QVBoxLayout(this);
			vbox.Spacing = 0;
			FrameShadow = Shadow.Raised;
			FrameShape = Shape.Panel;
			
			// add the apply/cancel buttons
			QFrame buttonFrame = new QFrame(this);
			vbox.AddWidget(buttonFrame);
			QHBoxLayout buttonBox = new QHBoxLayout(buttonFrame);
			buttonFrame.SetLayout(buttonBox);
			// cancel button
			QPushButton cancelButton = new QPushButton(buttonFrame);
			cancelButton.icon = ResourceManager.GetIcon("cancel");
			cancelButton.IconSize = new QSize(48, 48);
			buttonBox.AddWidget(cancelButton);
			Connect(cancelButton, SIGNAL("clicked()"), docFrame, SLOT("EntityAttributesCancel()"));
			// okay button
			QPushButton applyButton = new QPushButton(buttonFrame);
			applyButton.icon = ResourceManager.GetIcon("apply");
			applyButton.IconSize = new QSize(48, 48);
			buttonBox.AddWidget(applyButton);
			Connect(applyButton, SIGNAL("clicked()"), docFrame, SLOT("EntityAttributesApply()"));
		}
		
		/// <summary>
		/// Constructor without a parent.
		/// </summary>
		public Frame() : this(null)
		{	
		}

#region The Entity
		
		/// <summary>
		/// Clears the items.
		/// </summary>
		public void Clear()
		{
			// clear existing items
			foreach (Item item in items)
			{
				vbox.RemoveWidget(item);
				item.DeleteLater();
			}
			items.Clear();
		}
		
		protected Entity entity;
		/// <summary>
		/// The entity associated with this widget.
		/// </summary>
		public Entity Entity
		{
			set				
			{
				this.entity = value;
				Clear();
				
				foreach (string name in entity.AttributeNames)
				{
					Item item = new Item(this, name);
					vbox.AddWidget(item, 0);
					items.Add(item);
				}
			}
			get {return entity;}
		}
		
#endregion
		
		
#region Updating
		
		[Q_SLOT]
		public void OnAttributeUpdated()
		{
			Console.WriteLine("attribute updated");
			docFrame.Viewport.Paint();
		}
		
#endregion
		
	}
}
