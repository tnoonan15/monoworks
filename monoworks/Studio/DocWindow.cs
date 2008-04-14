// DocWindow.cs - MonoWorks Project
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
using MonoWorks.Gui;

namespace MonoWorks.Studio
{
	
	/// <summary>
	/// The DocWindow is an MDI subwindow to the main Studio window.
	/// It contains a MonoWorks document.
	/// </summary>
	public class DocWindow : QMdiSubWindow
	{
		protected MainWindow mainWindow;
		protected DocFrame docFrame;
		
		/// <summary>
		/// Default constructor.
		/// </summary>
		public DocWindow(MainWindow parent) : base(parent)
		{
			mainWindow = parent;
			
			docFrame = new DocFrame(this);			
			this.SetWidget(docFrame);
			
		}
		
		
		protected override void MouseReleaseEvent (QMouseEvent arg1)
		{
			base.MouseReleaseEvent (arg1);
			Console.WriteLine("DocWindow mouse released");
			docFrame.Viewport.Repaint();
		}

		
	}
}
