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

using MonoWorks.Base;
using MonoWorks.Model;

namespace MonoWorks.Gui.Attributes
{
	/// <summary>
	/// Custom signals for attribute controls.
	/// </summary>
	public interface IAttributeControlSignals
	{
		[Q_SIGNAL]
		void AttributeUpdated();
	}
	
	/// <summary>
	/// Base class for attribute controls.
	/// </summary>
	public class Control : QFrame
	{
		/// <summary>
		/// The base layout.
		/// </summary>
		protected QHBoxLayout hbox;
		
		/// <summary>
		/// Default constructor.
		/// </summary>
		/// <param name="parent"> The parent <see cref="QWidget"/>. </param>
		public Control(QWidget parent) : base(parent)
		{
			hbox = new QHBoxLayout(this); 
		}
				
		/// <value>
		/// Emits custom signals.
		/// </value>
		protected new IAttributeControlSignals Emit
		{ 
			get { return (IAttributeControlSignals)Q_EMIT; }
		}
		
		/// <summary>
		/// Populates the control with the attribute.
		/// </summary>
		/// <param name="entity"> A <see cref="Entity"/>. </param>
		/// <param name="name"> The attribute name. </param>
		public virtual void PopulateValue(Entity entity, string name) {}
		
		/// <summary>
		/// Commits the modified value back to the entity.
		/// </summary>
		public virtual void CommitValue() {}
	}
		
}
