// 
//  MainWindow.cs - MonoWorks Project
//  
//  Author:
//       Andy Selvig <ajselvig@gmail.com>
// 
//  Copyright (c) 2010 MonoWorks Project
// 
//  This library is free software; you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as
//  published by the Free Software Foundation; either version 2.1 of the
//  License, or (at your option) any later version.
// 
//  This library is distributed in the hope that it will be useful, but
//  WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
//  Lesser General Public License for more details.
// 
//  You should have received a copy of the GNU Lesser General Public
//  License along with this library; if not, write to the Free Software
//  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA

using System;

using Qyoto;

using MonoWorks.Controls;
using MonoWorks.Qt.Backend;
using MonoWorks.Demo;

namespace MonoWorks.Qt.Demo
{
	
	public class MainWindow : QMainWindow
	{
	
		/// <summary>
		/// Application entry point.
		/// </summary>
		public static int Main(string[] args)
		{
			new QApplication(args);
	        new MainWindow();
	        return QApplication.Exec();
		}
		
		
		public MainWindow()
		{
			SetWindowTitle("MonoWorks Demo");

			var adapter = new ViewportAdapter(this);
			this.SetCentralWidget(adapter);
			
			// create the scene space
			var sceneSpace = new SceneSpace(adapter.Viewport);
			adapter.Viewport.RootScene = sceneSpace;
			var book = new SceneBook(adapter.Viewport);
			sceneSpace.Root = book;
			
			// create the controls scene
			var controlsScene = new ControlsScene(adapter.Viewport);
			book.Add(controlsScene);
			
	        ToolTip = "This is the MonoWorks demo for Qt";
			Resize(800, 800);
			Show();
			
		}
		
		
	}
}

