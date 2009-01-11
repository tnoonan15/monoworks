using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

//using System.Windows.Forms;
using System.Windows.Forms.Integration;

using MonoWorks.Model;
using MonoWorks.Model.ViewportControls;
using MonoWorks.Model.Interaction;
using MonoWorks.Rendering;

namespace MonoWorks.GuiWpf
{
	/// <summary>
	/// Interaction logic for DrawingFrame.xaml
	/// </summary>
	public partial class DrawingFrame : UserControl
	{
		public DrawingFrame()
		{
			InitializeComponent();

			Controller = new Controller(Viewport);
			Controller.SetUsage(ViewportUsage.CAD);

		}

		/// <summary>
		/// The viewport.
		/// </summary>
		public Viewport Viewport
		{
			get { return viewportWrapper.Viewport; }
		}

		/// <summary>
		/// The viewport controller.
		/// </summary>
		public Controller Controller { get; private set; }

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
					Viewport.RenderList.RemoveRenderable(drawing);
				drawing = value;
				Viewport.RenderList.AddRenderable(drawing);

				// add the drawing interactor
				DrawingInteractor interactor = new DrawingInteractor(Viewport, drawing);
				Viewport.PrimaryInteractor = interactor;

				// connect the entity manager with the tree view
				treeView.Drawing = drawing;

				drawing.EntityManager.SelectionChanged += delegate(Drawing drw)
				{
					Viewport.PaintGL();
				};

			}
		}


	}
}
