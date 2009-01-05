using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

using System.Windows.Forms;
using System.Windows.Forms.Integration;

using MonoWorks.Model;
using MonoWorks.GuiWpf;

namespace MonoWorks.ViewerWpf
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();

            TestPart drawing = new TestPart();
            docFrame.Drawing = drawing;
			
		}

		
	}
}
