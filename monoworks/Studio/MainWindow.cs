// MainWindow.cs - MonoWorks Project
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
	/// Main window for the MonoWorks Studio.
	/// </summary>
	public class MainWindow : QMainWindow
	{
		
		/// <summary>
		/// Default constructor.
		/// </summary>
		public MainWindow() : base()
		{
			// initialize the global resource manager
			ResourceManager.Initialize();
			
			// create the controller
			controller = new MainController(this);
			controller.Load();
			
			// add the document area
			docArea = new QMdiArea(this);
			this.SetCentralWidget(docArea);
			
			this.WindowIcon = ResourceManager.GetIcon("MonoWorksLogo");
		}
		
		

#region UI 
		
		protected MainController controller;

		protected QMdiArea docArea;
		/// <value>
		/// The doc area.
		/// </value>
		public QMdiArea DocArea
		{
			get {return docArea;}
		}
		
#endregion
		

#region Document Windows
	
		// The current document window.
		public DocWindow CurrentDocWindow
		{
			get {return (DocWindow)docArea.CurrentSubWindow();}
		}
		
		/// <value>
		/// The current document.
		/// </value>
		public Document CurrentDocument
		{
			get {return CurrentDocWindow.Document;}
		}
		
#endregion
		
	
		
	}
	
}
