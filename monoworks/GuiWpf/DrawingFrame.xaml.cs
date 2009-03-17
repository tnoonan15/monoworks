using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

//using System.Windows.Forms;
using System.Windows.Forms.Integration;

using MonoWorks.Model;
using MonoWorks.Model.ViewportControls;
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

			MonoWorks.GuiWpf.Framework.ResourceManager.LoadAssembly("MonoWorks.Resources");

			Controller = new DrawingController(Viewport, attributePanel);

			attributePanel.Hidden += Controller.OnAttributePanelHidden;

			Viewport.Camera.SetViewDirection(ViewDirection.Standard);
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
		public DrawingController Controller { get; private set; }

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
					Viewport.RenderList.RemoveActor(drawing);
				drawing = value;
				Viewport.RenderList.AddActor(drawing);

				// add the drawing interactor
				DrawingInteractor interactor = new DrawingInteractor(Viewport, drawing);
				Viewport.PrimaryInteractor = interactor;

				// connect the entity manager with the tree view
				treeView.Drawing = drawing;

				drawing.EntityManager.SelectionChanged += new EntityManager.SelectionChangedHandler( Controller.OnSelectionChanged);
				drawing.EntityManager.RaiseSelectionChanged();
			}
		}

	}
}
