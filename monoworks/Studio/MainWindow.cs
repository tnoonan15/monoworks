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
			
			// load the UI
			uiManager = new UiManager(this);
			uiManager.Load();
			
			// add the document area
			docArea = new QMdiArea(this);
			this.SetCentralWidget(docArea);
			
		}
		
		

#region UI 

		/// <summary>
		/// User interface manager.
		/// </summary>
		protected UiManager uiManager;

		protected QMdiArea docArea;
		
#endregion
		

#region Document Windows
				
		
#endregion
		
		
#region File I/O
		
		/// <summary>
		/// Create a new file.
		/// </summary>
		[Q_SLOT("NewFile()")]
		public void NewFile()
		{
			DocWindow docWindow = new DocWindow(this);
			docArea.AddSubWindow(docWindow);
			docWindow.ShowMaximized();
		}
		
		/// <summary>
		/// Open an existing.
		/// </summary>
		[Q_SLOT("OpenFile()")]
		public void OpenFile()
		{
			Console.WriteLine("Open FIle");
		}
		
		/// <summary>
		/// Save the current file.
		/// </summary>
		[Q_SLOT("SaveFile()")]
		public void SaveFile()
		{
			Console.WriteLine("Save FIle");
		}
		
		/// <summary>
		/// Save the current file with a different name.
		/// </summary>
		[Q_SLOT("SaveFileAs()")]
		public void SaveFileAs()
		{
			Console.WriteLine("Save File As");
		}
		
		/// <summary>
		/// Close the current file.
		/// </summary>
		[Q_SLOT("CloseFile()")]
		public void CloseFile()
		{
			Console.WriteLine("Close File");
		}
		
		/// <summary>
		/// Quits the application.
		/// </summary>
		[Q_SLOT("Quit()")]
		public void Quit()
		{
			Console.WriteLine("Quit");
			QApplication.Quit();
		}
		
#endregion
		
		
#region Edit
		
		/// <summary>
		/// Undo the last action.
		/// </summary>
		[Q_SLOT("Undo()")]
		public void Undo()
		{
			Console.WriteLine("Undo");
		}
		
		/// <summary>
		/// Redo the last undone action.
		/// </summary>
		[Q_SLOT("Redo()")]
		public void Redo()
		{
			Console.WriteLine("Redo");
		}
		
		/// <summary>
		/// Copy the current selection to the clipboard.
		/// </summary>
		[Q_SLOT("EditCopy()")]
		public void EditCopy()
		{
			Console.WriteLine("Copy");
		}
		
		/// <summary>
		/// Cut the current selection to the clipboard.
		/// </summary>
		[Q_SLOT("EditCut()")]
		public void EditCut()
		{
			Console.WriteLine("Cut");
		}
		
		/// <summary>
		/// Pastes the clipboard.
		/// </summary>
		[Q_SLOT("EditPaste()")]
		public void EditPaste()
		{
			Console.WriteLine("Paste");
		}
		
#endregion
		
		
#region Sketches
				
		/// <summary>
		/// Create a new sketch.
		/// </summary>
		[Q_SLOT("NewSketch()")]
		public void NewSketch()
		{
			Console.WriteLine("New Sketch");
		}
				
		/// <summary>
		/// Edit the current sketch.
		/// </summary>
		[Q_SLOT("EditSketch()")]
		public void EditSketch()
		{
			Console.WriteLine("Edit Sketch");
		}
				
		/// <summary>
		/// Add a line to the current sketch.
		/// </summary>
		[Q_SLOT("SketchLine()")]
		public void SketchLine()
		{
			Console.WriteLine("Sketch Line");
		}
				
		/// <summary>
		/// Add an arc to the current sketch.
		/// </summary>
		[Q_SLOT("SketchArc()")]
		public void SketchArc()
		{
			Console.WriteLine("Sketch Arc");
		}
				
		/// <summary>
		/// Add a spline to the current sketch.
		/// </summary>
		[Q_SLOT("SketchSpline()")]
		public void SketchSpline()
		{
			Console.WriteLine("Sketch Spline");
		}
		
#endregion
		
		
#region Features
						
		/// <summary>
		/// Add an extrusion.
		/// </summary>
		[Q_SLOT("AddExtrusion()")]
		public void AddExtrusion()
		{
			Console.WriteLine("Add Extrusion");
		}
				
		/// <summary>
		/// Add a revolution.
		/// </summary>
		[Q_SLOT("AddRevolution()")]
		public void AddRevolution()
		{
			Console.WriteLine("Add Revolutioni");
		}
				
		/// <summary>
		/// Add a sweep.
		/// </summary>
		[Q_SLOT("AddSweep()")]
		public void AddSweep()
		{
			Console.WriteLine("Add Sweep");
		}		
		
#endregion
		
	}
	
}
