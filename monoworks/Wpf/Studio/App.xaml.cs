using System;
using System.Collections.Generic;
using System.Configuration;
using System.Windows;
using MonoWorks.WpfStudio;

namespace MonoWorks.StudioWpf
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		public App()
		{
			MainWindow window = new MainWindow();
			window.Show();
		}
	}
}
