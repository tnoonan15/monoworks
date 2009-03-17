using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

using MonoWorks.GuiWpf;
using MonoWorks.Rendering;
using MonoWorks.Plotting;
using MonoWorks.Model;

namespace MonoWorks.DemoWpf
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();

			book = new TabControl();
			dockPanel.Children.Add(book);

			// create the model tab
			//DrawingFrame docFrame = new DrawingFrame();
			//docFrame.Height = Double.NaN;
			//docFrame.Width = Double.NaN;
			//docFrame.Drawing = new TestPart();
			//TabItem itemModel = new TabItem();
			//itemModel.Header = "Model";
			//itemModel.Content = docFrame;
			//book.Items.Add(itemModel);

			// create the basic 3D tab
			Pane3D pane3d = new Pane3D();
			TabItem item3d = new TabItem();
			item3d.Header = "Basic 3D";
			item3d.Content = pane3d;
			book.Items.Add(item3d);

			// create the basic 2D tab
			Pane2D pane2d = new Pane2D();
			TabItem item2d = new TabItem();
			item2d.Header = "Basic 2D";
			item2d.Content = pane2d;
			book.Items.Add(item2d);

			// create the controls tab
			PaneControls paneControls = new	PaneControls();
			TabItem itemControls = new TabItem();
			itemControls.Header = "Controls";
			itemControls.Content = paneControls;
			book.Items.Add(itemControls);

			book.SelectionChanged += OnPageChanged;
		}

		TabControl book;

		void OnPageChanged(object sender, SelectionChangedEventArgs e)
		{
			//((book.SelectedItem as TabItem).Content as PaneBase).OnUpdated();
			Console.WriteLine("page changed");
		}


	}
}
