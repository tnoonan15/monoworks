using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

using System.Windows.Forms.Integration;

using MonoWorks.GuiWpf;
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
			viewport = new Viewport();
			WindowsFormsHost host = new WindowsFormsHost();
			host.Child = viewport;
			dockPanel.Children.Add(host);
			viewport.Camera.Center = new MonoWorks.Base.Vector(0, 0, 0);
			viewport.Camera.Position = new MonoWorks.Base.Vector(7, 0, 0);
			viewport.Camera.UpVector = new MonoWorks.Base.Vector(0, 0, 1);
			viewport.Camera.RecomputeUpVector();

			// connect the viewport resize event
			host.SizeChanged += new SizeChangedEventHandler(OnViewportSizeChanged);

			// add the test axes
			TestAxes axes = new TestAxes();
			viewport.AddRenderable(axes);

		}

		void OnViewportSizeChanged(object sender, SizeChangedEventArgs e)
		{
			viewport.ResizeGL();
		}

		protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
		{
			base.OnRenderSizeChanged(sizeInfo);

			viewport.ResizeGL();
		}

		protected Viewport viewport;

		protected ArrayDataSet arrayData;

	}
}
