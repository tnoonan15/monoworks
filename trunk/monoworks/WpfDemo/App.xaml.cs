using System;
using System.Collections.Generic;
using System.Windows;

using MonoWorks.WpfBackend.Framework;

namespace DemoWpf
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{

		public App()
			: base()
		{

			ResourceManager.Initialize();

		}

	}
}
