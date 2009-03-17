// AttributeControl.cs - MonoWorks Project
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
using MonoWorks.Model.ViewportControls;

namespace MonoWorks.GuiWpf.AttributeControls
{
	/// <summary>
	/// Base class for attribute controls.
	/// </summary>
	public abstract class AttributeControl : StackPanel, IAttributeControl
	{

		public AttributeControl(Entity entity, AttributeMetaData metaData)
			: base()
		{
			Entity = entity;
			MetaData = metaData;

			Orientation = Orientation.Vertical;

			Label label = new Label();
			label.Content = metaData.Name;
			Children.Add(label);

			InternalUpdate = false;
		}

		public Entity Entity { get; private set; }

		public AttributeMetaData MetaData { get; private set; }

		#region Factory

		/// <summary>
		/// Generates an attribute control of the appropriate subclass
		/// for the given entity and attribute name.
		/// </summary>
		public static AttributeControl Generate(Entity entity, AttributeMetaData metaData)
		{
			switch (metaData.TypeName)
			{
			case "System.String":
				return new StringControl(entity, metaData);
			case "System.Double":
				return new NumericControl(entity, metaData);
			case "MonoWorks.Base.Length":
				return new DimensionalControl<MonoWorks.Base.Length>(entity, metaData);
			case "MonoWorks.Base.Angle":
				return new DimensionalControl<MonoWorks.Base.Angle>(entity, metaData);
			case "MonoWorks.Base.Point":
				return new PointControl(entity, metaData);
			default:
				return new NullControl(entity, metaData);
			}
		}

		#endregion


		#region Attribute Changed Event


		public delegate void AttributeChangedHandler(IAttributeControl sender);

		/// <summary>
		/// Raised when the value of the attribute changed.
		/// </summary>
		public event AttributeChangedHandler AttributeChanged;

		/// <summary>
		/// Raise the attribute changed handler, if anyone is listening.
		/// </summary>
		protected void RaiseAttributeChanged()
		{
			if (AttributeChanged != null)
				AttributeChanged(this);
		}

		#endregion


		#region Updating

		/// <summary>
		/// Update the control based on the attribute.
		/// </summary>
		public abstract void Update();

		/// <summary>
		/// Whether or not an internal update is occuring.
		/// </summary>
		protected bool InternalUpdate { get; private set; }

		/// <summary>
		/// Begin an internal update.
		/// </summary>
		protected void BeginUpdate()
		{
			InternalUpdate = true;
		}

		/// <summary>
		/// End an internal update.
		/// </summary>
		protected void EndUpdate()
		{
			InternalUpdate = false;
		}

		#endregion

	}
}
