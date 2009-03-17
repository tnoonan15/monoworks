// PointControl.cs - MonoWorks Project
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
using System.Windows.Controls;

using MonoWorks.Base;
using MonoWorks.Model;
using MonoWorks.GuiWpf.Utilities;

namespace MonoWorks.GuiWpf.AttributeControls
{
	/// <summary>
	/// Widget for controlling a point.
	/// </summary>
	public class PointWidget : StackPanel
	{

		public PointWidget(Point point)
			: base()
		{
			for (int i = 0; i < spins.Length; i++)
			{
				spins[i] = new SpinControl();
				Children.Add(spins[i]);
			}
		}
		
		protected SpinControl[] spins = new SpinControl[3];

		/// <summary>
		/// The point being controlled.
		/// </summary>
		public Point Point { get; set; }

		public void Update()
		{
			if (Point == null)
				return;
			for (int i = 0; i < spins.Length; i++)
			{
				spins[i].Value = Point[i].DisplayValue;
			}
		}

	}
}
