using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using MonoWorks.Controls;
using MonoWorks.Demo;
using MonoWorks.Wpf.Backend;
using MonoWorks.Rendering;
using MonoWorks.Plotting;
using MonoWorks.Modeling;
using MonoWorks.Controls.Dock;

namespace MonoWorks.Wpf.Demo
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();

			// create the scene book
			var sceneSpace = new DockSpace(viewportWrapper.Viewport);
			viewportWrapper.Viewport.RootScene = sceneSpace;
			var book = new DockBook(viewportWrapper.Viewport);
			sceneSpace.Root = book;

			// create the controls scene
			var controls = new ControlsScene(viewportWrapper.Viewport);
			book.Add(controls);

			// create the 2D plotting scene
			var plot2D = new Plot2dScene(viewportWrapper.Viewport);
			book.Add(plot2D);

			// create the 3D plotting scene
			var plot3D = new Plot3dScene(viewportWrapper.Viewport);
			book.Add(plot3D);
		}


	}
}
