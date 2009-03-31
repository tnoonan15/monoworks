// Bin.cs - MonoWorks Project
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

using MonoWorks.Rendering;
using MonoWorks.Rendering.Events;

namespace MonoWorks.Rendering.Controls
{
	
	/// <summary>
	/// Container control that has a single child.
	/// </summary>
	public class Bin : Control
	{

		public Bin()
			: base()
		{

		}

		public Bin(Control child) : this()
		{
			Child = child;
		}
		
		protected Control child;
		/// <value>
		/// The child control.
		/// </value>
		public Control Child
		{
			get {return child;}
			set
			{
				if (child != null)
					child.Parent = null;
				child = value;
				child.Parent = this;
			}
		}
		
		
		public override void OnButtonPress(MouseButtonEvent evt)
		{
			base.OnButtonPress(evt);

			if (child != null)
				child.OnButtonPress(evt);
		}

		public override void OnButtonRelease(MouseButtonEvent evt)
		{
			base.OnButtonRelease(evt);
			
			if (child != null)
				child.OnButtonRelease(evt);
		}

		public override void OnMouseMotion(MouseEvent evt)
		{
			base.OnMouseMotion(evt);

			if (child != null)
				child.OnMouseMotion(evt);
		}


		protected override void Render(Viewport viewport)
		{
			base.Render(viewport);

			if (child != null && child.IsVisible)
				child.RenderOverlay(viewport);
		}

	}
}
