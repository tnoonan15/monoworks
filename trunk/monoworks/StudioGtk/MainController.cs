// MainController.cs - MonoWorks Project
//
//  Copyright (C) 2008 Andy Selvig
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

using MonoWorks.Framework;
using MonoWorks.GuiGtk.Framework;

namespace MonoWorks.StudioGtk
{
	
	/// <summary>
	/// Main controller class for the Studio.
	/// </summary>
	public class MainController : Controller
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		/// <param name="window"> </param>
		public MainController(MainWindow window) : base(window)
		{
//			this.window = window;
			
			ResourceManager.Initialize("../../../Resources");
			
			uiManager = new UiManager(this);
			uiManager.LoadFile("../../../Resources/Scope.ui");
		}

		/// <summary>
		/// The main window.
		/// </summary>
//		private MainWindow window;
		
		/// <summary>
		/// The UI manager.
		/// </summary>
		private UiManager uiManager;



#region File Actions

		[Action("New Part")]
		public void NewPart()
		{
			uiManager.CreateDocumentByName("PartView");
		}

		[Action("New Assembly")]
		public void NewAssembly()
		{
			uiManager.CreateDocumentByName("AssemblyView");
		}

		[Action()]
		public void Open()
		{
			Console.WriteLine("open");	
		}

		[Action()]
		public void Save()
		{
			Console.WriteLine("save");	
		}

		[Action("Save As")]
		public void SaveAs()
		{
			Console.WriteLine("save as");	
		}

		[Action()]
		public void Close()
		{
			Console.WriteLine("close");	
		}

		[Action()]
		public void Quit()
		{
			Console.WriteLine("quit");	
		}


#endregion


#region Edit Actions

		[Action()]
		public void Undo()
		{
			Console.WriteLine("undo");	
		}

		[Action()]
		public void Redo()
		{
			Console.WriteLine("redo");	
		}

		[Action()]
		public void Cut()
		{
			Console.WriteLine("cut");	
		}

		[Action()]
		public void Copy()
		{
			Console.WriteLine("copy");	
		}

		[Action()]
		public void Paste()
		{
			Console.WriteLine("paste");	
		}


#endregion


#region View Actions

		[Action("Render Preferences")]
		public void RenderPreferences()
		{
			Console.WriteLine("render prefs");	
		}

#endregion


#region Sketch Actions

		[Action("New Sketch")]
		public void NewSketch()
		{
			Console.WriteLine("new sketch");	
		}

		[Action("Edit Sketch")]
		public void EditSketch()
		{
			Console.WriteLine("edit sketch");	
		}

		[Action()]
		public void Line()
		{
			Console.WriteLine("sketch line");	
		}

		[Action()]
		public void Arc()
		{
			Console.WriteLine("sketch arc");	
		}

		[Action()]
		public void Spline()
		{
			Console.WriteLine("sketch spline");	
		}		

#endregion


#region Feature Actions

		[Action()]
		public void Extrusion()
		{
			Console.WriteLine("extrusion");	
		}

		[Action()]
		public void Revolution()
		{
			Console.WriteLine("revolution");	
		}

		[Action()]
		public void Sweep()
		{
			Console.WriteLine("sweep");	
		}		

#endregion


#region Reference Actions

		[Action("Ref Point")]
		public void RefPoint()
		{
			Console.WriteLine("Ref point");	
		}

		[Action("Ref Line")]
		public void RefLine()
		{
			Console.WriteLine("ref line");	
		}

		[Action("Ref Plane")]
		public void RefPlane()
		{
			Console.WriteLine("Ref plane");	
		}

#endregion

		

	}
}
