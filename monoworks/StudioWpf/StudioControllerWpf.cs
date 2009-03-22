// StudioController.cs - MonoWorks Project
//
//  Copyright (C) 2009 Andy Selvig
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
using System.Windows;

using MonoWorks.Model;
using MonoWorks.GuiWpf.Framework;

namespace MonoWorks.StudioWpf
{
	/// <summary>
	/// Main controller for the MonoWorks Studio.
	/// </summary>
	public class StudioControllerWpf : StudioController
	{

		public StudioControllerWpf(MainWindow window)
			: base()
		{
			this.window = window;
			window.KeyPressed += OnKeyPress;

			ResourceManager.LoadAssembly("MonoWorks.Resources");

			uiManager = new UiManager(this, window);
			uiManager.LoadStream(Framework.ResourceHelper.GetStream("Studio.ui", "MonoWorks.Resources"));
			SetUiManager(uiManager);
		}


		private MainWindow window;

		private UiManager uiManager;



		public override void Open()
		{
			Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
			dlg.DefaultExt = ".mwp"; // Default file extension
			dlg.Filter = "Part (.mwp)|*.mwp"; // Filter files by extension

			// Show open file dialog box
			bool? result = dlg.ShowDialog();

			// Process open file dialog box results
			if (result == true)
			{
				try
				{
					Drawing.FromFile(dlg.FileName);
				}
				catch (Exception ex)
				{
					MessageBox.Show("There was an error loading the drawing: \n\n" + ex.Message, "Load Error", MessageBoxButton.OK);
				}
			}

		}


		public override void Save()
		{
			Drawing drawing = drawingManager.Current.Drawing;
			if (drawing == null)
				return;
			else if (drawing.FileName == null)
				SaveAs();
			else
				drawing.Save();
		}


		public override void SaveAs()
		{
			Drawing drawing = drawingManager.Current.Drawing;
			if (drawing == null)
				return;

			Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
			if (drawing.FileName != null)
				dlg.FileName = drawing.FileName;
			dlg.DefaultExt = ".mwp"; // Default file extension
			dlg.Filter = "Part (.mwp)|*.mwp"; // Filter files by extension

			// Show open file dialog box
			bool? result = dlg.ShowDialog();

			// Process savefile dialog box results
			if (result == true)
			{
				try
				{
					drawing.SaveAs(dlg.FileName);
				}
				catch (Exception ex)
				{
					MessageBox.Show("There was an error saving the drawing: \n\n" + ex.Message, "Load Error", MessageBoxButton.OK);
				}
			}
		}


	}
}
