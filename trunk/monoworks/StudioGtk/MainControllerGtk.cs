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
using MonoWorks.Modeling;

namespace MonoWorks.StudioGtk
{
	
	/// <summary>
	/// Main controller class for the Studio.
	/// </summary>
	public class MainControllerGtk : StudioController
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		/// <param name="window"> </param>
		public MainControllerGtk(MainWindow window) : base()
		{
			ResourceManager.LoadAssembly("MonoWorks.Resources");
			
			uiManager = new UiManager(this, window);
			uiManager.LoadStream(ResourceHelper.GetStream("Studio.ui", "MonoWorks.Resources"));
			SetUiManager(uiManager);
		}
		
		/// <summary>
		/// The UI manager.
		/// </summary>
		private UiManager uiManager;




#region View Actions

		[Action("Render Preferences")]
		public void RenderPreferences()
		{
			Console.WriteLine("render prefs");	
		}

#endregion


		
#region File Loading
		


		public override void Save ()
		{
			throw new System.NotImplementedException();
		}

		public override void SaveAs ()
		{
			throw new System.NotImplementedException();
		}

		public override void Open ()
		{
			throw new System.NotImplementedException();
		}		
		
#endregion
		

	}
}
