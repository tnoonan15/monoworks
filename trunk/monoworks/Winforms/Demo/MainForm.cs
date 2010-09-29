using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using MonoWorks.Demo;
using MonoWorks.Controls;
using MonoWorks.Controls.Dock;

namespace Demo
{
	public partial class MainForm : Form
	{
		public MainForm()
		{
			InitializeComponent();

			// create the scene book
			var sceneSpace = new DockSpace(_viewportAdapter.Viewport);
			_viewportAdapter.Viewport.RootScene = sceneSpace;
			var book = new DockBook(_viewportAdapter.Viewport);
			sceneSpace.Root = book;

			// create the cards scene
			var cards = new CardScene(_viewportAdapter.Viewport);
			book.Add(cards);

			// create the controls scene
			var controls = new ControlsScene(_viewportAdapter.Viewport);
			book.Add(controls);

			// create the 2D plotting scene
			var plot2D = new Plot2dScene(_viewportAdapter.Viewport);
			book.Add(plot2D);

			// create the 3D plotting scene
			var plot3D = new Plot3dScene(_viewportAdapter.Viewport);
			book.Add(plot3D);

			_viewportAdapter.ResizeGL();
		}


	}
}
