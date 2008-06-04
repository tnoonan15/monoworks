// StringControl.cs - ScratchNotes
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
	/// Control for string attributes.
	/// </summary>
	public class StringControl : Control
	{
		protected QLineEdit lineEdit;
		
		/// <summary>
		/// Default constructor.
		/// </summary>
		/// <param name="parent"> The parent <see cref="QWidget"/>. </param>
		public StringControl(QWidget parent) : base(parent)
		{
			lineEdit = new QLineEdit(this);
			hbox.AddWidget(lineEdit);
			Connect(lineEdit, SIGNAL("returnPressed()"), this, SLOT("OnTextChanged()"));
		}
		
		public override void PopulateValue(Entity entity, string name)
		{
			lineEdit.SetText( (string)entity.GetAttribute(name));
		}
		
		
		[Q_SLOT]
		protected virtual void OnTextChanged()
		{
			Console.WriteLine("text changed");
		}
		

	}
}
