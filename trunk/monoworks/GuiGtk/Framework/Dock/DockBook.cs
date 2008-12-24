// DockBook.cs - Slate Mono Application Framework
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

namespace MonoWorks.GuiGtk.Framework.Dock
{
	
	/// <summary>
	/// The notebook widget for dockables.
	/// </summary>
	public class DockBook : Gtk.Notebook
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public DockBook() : base()
		{
		}


		/// <value>
		/// The orientation of the page titles.
		/// </value>
		public Gtk.Orientation Orientation
		{
			get
			{
				if (TabPos == Gtk.PositionType.Top || TabPos == Gtk.PositionType.Bottom)
					return Gtk.Orientation.Horizontal;
				else // vertical
					return Gtk.Orientation.Vertical;
			}
		}
		

		/// <summary>
		/// Called when the user tries to move a dockable within the dockbook.
		/// </summary>
		/// <param name="dockable"> The <see cref="Dockable"/> being moved. </param>
		public void OnChildMotion(Dockable dockable)
		{
			
			// get the cursor position in local coords
			int x, y;
			GetPointer(out x, out y);
			
			// test for undocking
			// this basically happens when the cursor outside of the allowed tab area by a certain tolerance
			int undockTol = dockable.TitleBar.Allocation.Height;
			if (x < -undockTol || x > Allocation.Width+undockTol ||
			    y < -undockTol || y > Allocation.Height+undockTol)
				dockable.Float();
			else if (TabPos == Gtk.PositionType.Top && y > 2*undockTol)
				dockable.Float();
			else if (TabPos == Gtk.PositionType.Left && x > 2*undockTol)
				dockable.Float();
			else if (TabPos == Gtk.PositionType.Bottom && y < Allocation.Height-2*undockTol)
				dockable.Float();
			else if (TabPos == Gtk.PositionType.Right && x < Allocation.Width-2*undockTol)
				dockable.Float();
			if (dockable.DockFloating) // if it's floating we're done
				return;
			
			// get the page number of the dockable
			int pageNum = PageNum(dockable);
			
			// test for moving the page within the book
			int n=0;
			foreach (Gtk.Widget child in Children)
			{
				Gtk.Widget labelWidget = GetTabLabel(child);
				int labelX, labelY, labelWidth, labelHeight;
				labelWidget.GetPointer(out labelX, out labelY);
				labelWidth = labelWidget.Allocation.Width;
				labelHeight = labelWidget.Allocation.Height;
				if (Orientation == Gtk.Orientation.Horizontal && 
				    ((n < pageNum && labelX < labelWidth/2) ||
					    (n > pageNum && labelX > labelWidth/2))) 
				{
					ReorderChild(dockable, n);
					return;
				}
				else if (Orientation == Gtk.Orientation.Vertical && 
				    ((n < pageNum && labelY < labelHeight/2) ||
					    (n > pageNum && labelY > labelHeight/2))) 
				{
					ReorderChild(dockable, n);
					return;
				}
				n++;
			}
		}
		
	}
}
