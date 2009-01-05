using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

//using System.Windows.Forms;
using System.Windows.Forms.Integration;

using MonoWorks.Model;

namespace MonoWorks.GuiWpf
{
	/// <summary>
	/// Interaction logic for DocFrame.xaml
	/// </summary>
	public partial class DocFrame : UserControl
	{
		public DocFrame()
		{
			InitializeComponent();

			// create the viewport
			viewport = new Viewport();

			WindowsFormsHost host = new WindowsFormsHost();
			host.Child = viewport;
			viewportFrame.Content = host;
			

			// connect the viewport resize event
			host.SizeChanged += new SizeChangedEventHandler(OnViewportSizeChanged);
		}


		protected Viewport viewport;
		/// <summary>
		/// The viewport.
		/// </summary>
		public Viewport Viewport
		{
			get { return viewport; }
		}


		protected Drawing drawing = null;
		/// <summary>
		/// The drawing being displayed by the frame.
		/// </summary>
		public Drawing Drawing
		{
			get { return drawing; }
			set
			{
				if (drawing != null)
					viewport.RenderList.RemoveRenderable(drawing);
				drawing = value;
				viewport.RenderList.AddRenderable(drawing);
			}
		}





		protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
		{
			base.OnRenderSizeChanged(sizeInfo);

			viewport.ResizeGL();
		}

		void OnViewportSizeChanged(object sender, SizeChangedEventArgs e)
		{
			viewport.ResizeGL();
		}

	}
}
