// PointListControl.cs - MonoWorks Project
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

using swc = System.Windows.Controls;

using MonoWorks.Base;
using MonoWorks.Model;
using MonoWorks.GuiWpf.Utilities;

namespace MonoWorks.GuiWpf.AttributeControls
{

	public class PointListControl : AttributeControl
	{
		public PointListControl(Entity entity, AttributeMetaData metaData)
			: base(entity, metaData)
		{
		}

		/// <summary>
		/// The widgets for each point.
		/// </summary>
		protected List<PointWidget> widgets = new List<PointWidget>();

		/// <summary>
		/// Repopulates the point widgets.
		/// </summary>
		protected void Repopulate(List<Point> points)
		{
			// clear the existing widgets
			foreach (var widget in widgets)
				Children.Remove(widget);
			widgets.Clear();

			// create the new ones
			foreach (var point in points)
			{
				PointWidget widget = new PointWidget(point);
				widgets.Add(widget);
				Children.Add(widget);
			}
		}

		public override void Update()
		{
			List<Point> points = Entity.GetAttribute(MetaData.Name) as List<Point>;
			if (points == null)
				return;

			if (points.Count != widgets.Count)
				Repopulate(points);

			for (int i = 0; i < points.Count; i++ )
				widgets[i].Update(points[i]);
		}

	}

}
