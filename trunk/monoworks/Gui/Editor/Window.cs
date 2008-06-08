// Window.cs - MonoWorks Project
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

namespace MonoWorks.Gui.Editor
{
	/// <summary>
	/// The main editor window.
	/// </summary>	
	public class Window : QMainWindow
	{
				
		public Window() : base()
		{
			// load the UI
			uiManager = new UiManager<Window>(this);
			uiManager.Load();
			
			// add the document area
			docArea = new QMdiArea(this);
			this.SetCentralWidget(docArea);
		}

		
#region UI 

		/// <summary>
		/// User interface manager.
		/// </summary>
		protected UiManager<Window> uiManager;

		protected QMdiArea docArea;
		
#endregion
		
		
#region File I/O
		
		/// <summary>
		/// Create a new Python script.
		/// </summary>
		[Q_SLOT()]
		public void NewPython()
		{
			Document document = new Document(this);
			docArea.AddSubWindow(document);
			document.ShowMaximized();
		}
		
#endregion
		
	}
}
