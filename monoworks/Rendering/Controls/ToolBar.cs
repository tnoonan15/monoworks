// ToolBar.cs - MonoWorks Project
//
//  Copyright (C) 2008 Andy Selvig
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

using MonoWorks.Base;
using MonoWorks.Rendering;

namespace MonoWorks.Rendering.Controls
{
	
	/// <summary>
	/// A stack of buttons.
	/// </summary>
	public class ToolBar : Stack
	{
		
		public ToolBar() : base()
		{
			StyleClassName = "toolbar";
			ToolStyle = "tool";
		}
		
		
		public override void Add(Control child)
		{
			base.Add(child);
			
			child.StyleClassName = toolStyle;
			
			if (child is Button)
				(child as Button).ButtonStyle = buttonStyle;
		}

		
		private string toolStyle = "tool";
		/// <value>
		/// The style class to use for the child controls.
		/// </value>
		public string ToolStyle
		{
			get {return toolStyle;}
			set
			{
				toolStyle = value;
				foreach (Control child in Children)
					child.StyleClassName = value;
			}
		}
		
		
		public override void RenderOverlay(IViewport viewport)
		{	
			if (dirty)
				ComputeGeometry();
			
			RenderBackground();
			RenderOutline();
			
			base.RenderOverlay(viewport);
			
		}
		
		protected ButtonStyle buttonStyle = ButtonStyle.ImageOverLabel;
		/// <value>
		/// Set the button style of all buttons in the toolbar.
		/// </value>
		public ButtonStyle ButtonStyle
		{
			set
			{
				buttonStyle = value;
				foreach (Control child in Children)
				{
					if (child is Button)
						(child as Button).ButtonStyle = buttonStyle;
				}
			}
			get {return buttonStyle;}
		}

		
	}
}
