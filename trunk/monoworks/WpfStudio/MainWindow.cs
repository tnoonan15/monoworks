// MainWindow.cs - MonoWorks Project
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


using System.Windows;
using System.Windows.Controls;

using MonoWorks.WpfBackend;
using MonoWorks.Studio;

namespace MonoWorks.WpfStudio
{
	/// <summary>
	/// The main window for the WPF port of MonoWorks Studio.
	/// </summary>
	public class MainWindow : Window
	{

		public MainWindow()
			: base()
		{
			Title = "MonoWorks Studio";

			_wrapper = new ViewportWrapper();
			Content = _wrapper;

			_scene = new StudioScene(_wrapper.Viewport);
			_wrapper.Viewport.RootScene = _scene;

		}

		private ViewportWrapper _wrapper;

		private StudioScene _scene;
	}
}


