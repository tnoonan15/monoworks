using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

using MonoWorks.GuiWpf;
using MonoWorks.Rendering;
using MonoWorks.Plotting;

namespace MonoWorks.PlottingDemoWpf
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();

			// add the viewport
			tooledViewport = new TooledViewport(ViewportUsage.Plotting);
			viewport = tooledViewport.Viewport;
			dockPanel.Children.Add(tooledViewport);

			viewport.Camera.Center = new MonoWorks.Base.Vector(0, 0, 0);
			viewport.Camera.Position = new MonoWorks.Base.Vector(7, 0, 0);
			viewport.Camera.UpVector = new MonoWorks.Base.Vector(0, 0, 1);
			viewport.Camera.RecomputeUpVector();

			// add the test axes
			TestAxes axes = new TestAxes();
			viewport.AddRenderable(axes);

		}


		protected TooledViewport tooledViewport;

		protected Viewport viewport;

		protected ArrayDataSet arrayData;

	}
}
