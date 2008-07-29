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
			viewport.Camera.Position = new MonoWorks.Base.Vector(4, 0, 0);
			viewport.Camera.UpVector = new MonoWorks.Base.Vector(0, 0, 1);
			viewport.Camera.RecomputeUpVector();

			// connect the viewport resize event
			host.SizeChanged += new SizeChangedEventHandler(OnViewportSizeChanged);

			// make the array data set
			arrayData = new ArrayDataSet(1024,4);
			Random rand = new Random();
			for (int r = 0; r < arrayData.NumRows; r++)
			{
				arrayData[r, 0] = rand.NextDouble() * 2 * Math.PI;
				arrayData[r, 1] = rand.NextDouble() * Math.PI;
				arrayData[r, 2] = Math.Sin(arrayData[r, 0]) * Math.Cos(arrayData[r, 1]);
			}

			// add an axes box and plot
			AxesBox axes = new AxesBox();
			viewport.AddRenderable(axes);
			PointPlot plot1 = new PointPlot(axes);
			plot1.DataSet = arrayData;


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
