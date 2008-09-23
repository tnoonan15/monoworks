using System;
using System.Collections.Generic;
using System.Reflection;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Forms.Integration;
using System.Windows.Controls.Primitives;

using MonoWorks.Rendering;

namespace MonoWorks.GuiWpf
{
	public class TooledViewport : UserControl
	{
		public TooledViewport(ViewportUsage usage)
		{
			this.usage = usage;

			// create the main stack panel
			dockPanel = new DockPanel();
			Content = dockPanel;

			// create the toolbar
			CreateToolbar();

			// create the viewport
			viewport = new Viewport();

			WindowsFormsHost host = new WindowsFormsHost();
			host.Child = viewport;
			dockPanel.Children.Add(host);
			DockPanel.SetDock(host, Dock.Left | Dock.Right | Dock.Bottom);
		}


		private DockPanel dockPanel;


		#region The Viewport

		protected Viewport viewport;
		/// <summary>
		/// The viewport.
		/// </summary>
		public Viewport Viewport
		{
			get { return viewport; }
		}

		protected ViewportUsage usage;
		/// <summary>
		/// How the viewport is being used.
		/// </summary>
		public ViewportUsage Usage
		{
			get { return usage; }
			set { usage = value; }
		}

		protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
		{
			base.OnRenderSizeChanged(sizeInfo);

			viewport.ResizeGL();
		}

		#endregion


		#region Icons

		protected static Dictionary<string, Image> icons = null;

		/// <summary>
		/// Loads the icons from the resources.
		/// </summary>
		protected static void LoadIcons()
		{
			if (icons == null)
			{
				icons = new Dictionary<string, Image>();
				Assembly asm = Assembly.GetExecutingAssembly();
				foreach (string name in asm.GetManifestResourceNames())
				{
					if (name.EndsWith("png"))
					{
						string[] comps = name.Split('.');
						Image image = new Image();
						PngBitmapDecoder decoder = new PngBitmapDecoder(asm.GetManifestResourceStream(name), 
							BitmapCreateOptions.DelayCreation, BitmapCacheOption.Default);
						image.Source = decoder.Frames[0];
						icons[comps[comps.Length - 2]] = image;
					}
				}
			}
		}

		#endregion


		#region The Toolbar


		protected ToolBar toolbar;

		/// <summary>
		/// Create the toolbar.
		/// </summary>
		protected void CreateToolbar()
		{
			LoadIcons();


			toolbar = new ToolBar();
			//Button button;

			// add the direction buttons
			AddDirectionButton(ViewDirection.Standard);
			AddDirectionButton(ViewDirection.Front);
			if (usage == ViewportUsage.CAD)
				AddDirectionButton(ViewDirection.Back);
			AddDirectionButton(ViewDirection.Left);
			if (usage == ViewportUsage.CAD)
				AddDirectionButton(ViewDirection.Right);
			AddDirectionButton(ViewDirection.Top);
			if (usage == ViewportUsage.CAD)
				AddDirectionButton(ViewDirection.Bottom);
			toolbar.Items.Add(new Separator());

			// add teh perspective button
			ToggleButton button = new ToggleButton();
			button.Content = "Perspective";
			button.IsChecked = true;
			button.Click += OnToggleProjection;
			toolbar.Items.Add(button);

			toolbar.Height = 32;
			dockPanel.Children.Add(toolbar);
			DockPanel.SetDock(toolbar, Dock.Top);
			
		}

		private void AddDirectionButton(ViewDirection direction)
		{
			Button button = new Button();
			button.Content = icons[direction.ToString().ToLower() + "-view"];
			button.ToolTip = direction.ToString() + " view";
			button.Click += delegate(object sender, RoutedEventArgs args)
			{ OnChangeViewDirection(direction); };
			toolbar.Items.Add(button);
		}

		#endregion


		#region Actions

		/// <summary>
		/// Handles changing the view direction of the camera.
		/// </summary>
		/// <param name="direction"></param>
		protected void OnChangeViewDirection(ViewDirection direction)
		{
			Console.WriteLine("view direction changed to {0}", direction);
		}

		/// <summary>
		/// Toggles the projection of the viewport's camera.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		protected void OnToggleProjection(object sender, RoutedEventArgs args)
		{
			viewport.Camera.ToggleProjection();
			viewport.PaintGL();
		}

		#endregion

	}
}
