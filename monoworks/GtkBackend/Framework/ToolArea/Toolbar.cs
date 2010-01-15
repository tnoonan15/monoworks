// Toolbar.cs - Slate Mono Application Framework
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

using MonoWorks.Framework;

namespace MonoWorks.GtkBackend.Framework.Tools
{
	/// <summary>
	/// Toolbar that implements ITool interface so it can be used in the tool area.
	/// </summary>
	public class Toolbar : Gtk.Toolbar, ITool
	{

		/// <summary>
		/// Default constructor.
		/// </summary>
		public Toolbar()
			: base()
		{
			this.ToolbarStyle = Gtk.ToolbarStyle.Icons;
		}
		
		
		protected ToolArea toolArea;
		/// <summary>
		/// The tool area that this tool belongs to.
		/// </summary>
		public ToolArea ToolArea
		{
			get {return toolArea;}
			set {toolArea = value;}
		}

		private ToolPosition lastPosition;
		//// <value>
		/// The tools last docked position.
		/// </value>
		public ToolPosition LastPosition
		{
			get {return lastPosition;}
			set {lastPosition = value;}
		}

		//// <value>
		/// The visibility of the tool.
		/// </value>
		public bool ToolVisible
		{
			get {return !toolArea.ToolIsHidden(this);}
			set
			{
				if (value)
					toolArea.ShowTool(this);
				else
					toolArea.HideTool(this);
			}
		}
		
		
		protected override void OnSizeRequested (ref Gtk.Requisition requisition)
		{
			base.OnSizeRequested(ref requisition);

			int pad = 6;
			
			if (Orientation == Gtk.Orientation.Horizontal)
			{
				requisition.Width = pad;
				requisition.Height = 0;
				foreach (Gtk.Widget child in Children)
				{
					Gtk.Requisition req = child.SizeRequest();
					requisition.Width += req.Width;
					requisition.Height = Math.Max(requisition.Height, req.Height);
				}
				requisition.Height += pad;
			}
			else // vertical
			{			
				requisition.Width = 0;
				requisition.Height = pad;	
				foreach (Gtk.Widget child in Children)
				{
					Gtk.Requisition req = child.SizeRequest();
					requisition.Height += req.Height;
					requisition.Width = Math.Max(requisition.Width, req.Width);
				}
				requisition.Width += pad;
			}			
		}


	}
}
