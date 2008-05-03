// Toolbox.cs - MonoWorks Project
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



namespace MonoWorks.Gui
{
	
	/// <summary>
	/// A toolbox shows a list of shelves, with one visible at a time.
	/// Each shelve contains an icon view that has tools.
	/// </summary>
	public class Toolbox : QToolBox
	{

		/// <summary>
		/// Default constructor.
		/// </summary>
		public Toolbox(QWidget parent) : base(parent)
		{
			shelves = new List<Toolshelf>();
			SetOrientation(Orientation.Vertical);
		}

		
		/// <summary>
		/// Re-orients the toolbox based on what side of the main window it's located on.
		/// </summary>
		/// <param name="arg1"> A <see cref="QResizeEvent"/>. </param>
		protected override void ResizeEvent(QResizeEvent arg1)
		{
			base.ResizeEvent(arg1);
			
			QDockWidget parent = (QDockWidget)ParentWidget();
			QMainWindow mainWindow = (QMainWindow)parent.ParentWidget();
			switch ( mainWindow.DockWidgetArea(parent) )
			{
			case Qt.DockWidgetArea.TopDockWidgetArea:
				this.SetOrientation(Orientation.Horizontal);
				break;
			case Qt.DockWidgetArea.BottomDockWidgetArea:
				this.SetOrientation(Orientation.Horizontal);
				break;
			case Qt.DockWidgetArea.LeftDockWidgetArea:
				this.SetOrientation(Orientation.Vertical);
				break;
			case Qt.DockWidgetArea.RightDockWidgetArea:
				this.SetOrientation(Orientation.Vertical);
				break;
			default:
				this.SetOrientation(Orientation.Vertical);
				break;
			}
		}

		
#region Orientation
		
		protected Orientation orientation;
		/// <value>
		/// The orientation of the toolbox.
		/// </value>
		public void SetOrientation(Orientation newOrientation)
		{
			if (newOrientation != orientation)
			{
				orientation = newOrientation;
				foreach (Toolshelf shelf in shelves)
				{
					shelf.Orientation = orientation;
				}
			}
		}
		
#endregion
			
							
#region Shelves

		protected List<Toolshelf> shelves;
		
		/// <summary>
		/// Creates a toolshelf in the toolbox.
		/// </summary>
		/// <param name="name"> The name of the shelf. </param>
		/// <returns> The new <see cref="Toolshelf"/>. </returns>
		public Toolshelf AddShelf(string name)
		{
			Toolshelf shelf = new Toolshelf(this);
			this.AddItem(shelf, name);
			shelves.Add(shelf);
			return shelf;
		}
		
		
#endregion
		
		

		[Q_SLOT("OnLocationChanged(Qt.DockWidgetArea area)")]
		public void OnLocationChanged(Qt.DockWidgetArea area)
		{
			Console.WriteLine("toolbox location changed to {0}", area);
		}
		
	}
}
