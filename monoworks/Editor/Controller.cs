// Controller.cs - MonoWorks Project
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

using MonoWorks.Gui;

namespace MonoWorks.Editor
{
	
	/// <summary>
	/// The controller for the editor window.
	/// </summary>
	public class Controller : AbstractController<Window>
	{
		
		public Controller(Window window) : base (window)
		{
		}
		
#region File I/O
		
		/// <summary>
		/// Create a new Python script.
		/// </summary>
		[Q_SLOT()]
		public void NewPython()
		{
			Document document = new Document(window);
			window.DocArea.AddSubWindow(document);
			document.ShowMaximized();
		}
		
#endregion
	}
}
