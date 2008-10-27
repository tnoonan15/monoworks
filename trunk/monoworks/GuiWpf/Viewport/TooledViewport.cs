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

			host = new WindowsFormsHost();
			host.Child = viewport;
			dockPanel.Children.Add(host);
			DockPanel.SetDock(host, Dock.Left | Dock.Right | Dock.Bottom);


			UpdateToolbar();

		}


		private DockPanel dockPanel;

		private WindowsFormsHost host;


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

			Console.WriteLine("tooled viewport resized to {0}, {1}", host.ActualWidth, host.ActualHeight);

			//viewport.Size= new System.Drawing.Size((int)host.ActualWidth, (int)host.ActualHeight);
			viewport.ResizeGL();
		}

#endregion


#region Icons

		protected static Dictionary<string, ImageSource> iconSources = null;

		/// <summary>
		/// Loads the icons from the resources.
		/// </summary>
		protected static void LoadIcons()
		{
			if (iconSources == null)
			{
				iconSources = new Dictionary<string, ImageSource>();
				Assembly asm = Assembly.GetExecutingAssembly();
				foreach (string name in asm.GetManifestResourceNames())
				{
					if (name.EndsWith("png"))
					{
						string[] comps = name.Split('.');
						PngBitmapDecoder decoder = new PngBitmapDecoder(asm.GetManifestResourceStream(name), 
							BitmapCreateOptions.DelayCreation, BitmapCacheOption.Default);
						iconSources[comps[comps.Length - 2]] = decoder.Frames[0];
					}
				}
			}
		}


		/// <summary>
		/// Renders an icon to an Image.
		/// </summary>
		/// <param name="iconName"> The name of the icon.</param>
		/// <returns></returns>
		public Image RenderIcon(string iconName)
		{
			if (!iconSources.ContainsKey(iconName))
				throw new Exception("There is no icon called " + iconName);
			Image image = new Image();
			image.Source = iconSources[iconName];
			return image;
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

			// add the perspective button
			projectionButton = new ToggleButton();
			projectionButton.Content = RenderIcon("perspective");
			projectionButton.ToolTip = "Toggle the projection (perspective or parallel)";
			projectionButton.Click += OnToggleProjection;
			toolbar.Items.Add(projectionButton);
			toolbar.Items.Add(new Separator());

			// add the 2D and 3D buttons
			RadioButton radio = new RadioButton();
			radio.Content = RenderIcon("2d");
			radio.ToolTip = "2D Interaction Mode";
			radio.Click += delegate(object sender, RoutedEventArgs args)
			{ OnSetInteractionMode(InteractionMode.Select2D); };
			toolbar.Items.Add(radio);
			interactionModeButtons[InteractionMode.Select2D] = radio;

			radio = new RadioButton();
			radio.Content = RenderIcon("3dSelect");
			radio.ToolTip = "3D Selection Mode";
			radio.Click += delegate(object sender, RoutedEventArgs args)
			{ OnSetInteractionMode(InteractionMode.Select3D); };
			toolbar.Items.Add(radio);
			interactionModeButtons[InteractionMode.Select3D] = radio;

			radio = new RadioButton();
			radio.Content = RenderIcon("3dInteract");
			radio.ToolTip = "3D Interaction Mode";
			radio.Click += delegate(object sender, RoutedEventArgs args)
			{ OnSetInteractionMode(InteractionMode.View3D); };
			toolbar.Items.Add(radio);
			interactionModeButtons[InteractionMode.View3D] = radio;
			toolbar.Items.Add(new Separator());

			// add the export button
			Button exportButton = new Button();
			exportButton.Content = RenderIcon("export");
			exportButton.ToolTip = "Export the viewport to a bitmap.";
			exportButton.Click += OnExport;
			toolbar.Items.Add(exportButton);


			toolbar.Height = 32;
			dockPanel.Children.Add(toolbar);
			DockPanel.SetDock(toolbar, Dock.Top);
		}

		private void AddDirectionButton(ViewDirection direction)
		{
			Button button = new Button();
			button.Content = RenderIcon(direction.ToString().ToLower() + "-view");
			button.ToolTip = direction.ToString() + " view";
			button.Click += delegate(object sender, RoutedEventArgs args)
			{ OnChangeViewDirection(direction); };
			toolbar.Items.Add(button);
		}


		ToggleButton projectionButton;

		Dictionary<InteractionMode, RadioButton> interactionModeButtons = new Dictionary<InteractionMode, RadioButton>();


		/// <summary>
		/// Prompts the user for a location and exports the viewport.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void OnExport(object sender, RoutedEventArgs e)
		{
			// Configure save file dialog box
			Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
			dlg.FileName = "export"; // Default file name
			dlg.DefaultExt = ".png"; // Default file extension
			dlg.Filter = "PNG Image File (.png)|*.png"; // Filter files by extension

			// Show save file dialog box
			Nullable<bool> result = dlg.ShowDialog();

			// Process save file dialog box results
			if (result == true)
			{
				// Save document
				Viewport.Export(dlg.FileName);
			}

		}


		/// <summary>
		/// True if the toolbar is being updated by an external source.
		/// </summary>
		protected bool externalUpdate = false;

		/// <summary>
		/// Updates the toolbar with the state of the viewport.
		/// </summary>
		public void UpdateToolbar()
		{
			externalUpdate = true;

			projectionButton.IsChecked = viewport.Camera.Projection == Projection.Perspective;

			foreach (InteractionMode mode in interactionModeButtons.Keys)
			{
				if (mode == viewport.InteractionState.Mode)
					interactionModeButtons[mode].IsChecked = true;
			}

			externalUpdate = false;
		}

#endregion


#region Actions

		/// <summary>
		/// Handles changing the view direction of the camera.
		/// </summary>
		/// <param name="direction"></param>
		protected void OnChangeViewDirection(ViewDirection direction)
		{
			if (!externalUpdate)
			{
				viewport.Camera.SetViewDirection(direction);
				viewport.PaintGL();
				UpdateToolbar();
			}
		}

		/// <summary>
		/// Toggles the projection of the viewport's camera.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		protected void OnToggleProjection(object sender, RoutedEventArgs args)
		{
			if (!externalUpdate)
			{
				viewport.Camera.ToggleProjection();
				viewport.PaintGL();
				UpdateToolbar();
			}
		}

		/// <summary>
		/// Sets the viewport interaction mode.
		/// </summary>
		/// <param name="mode"></param>
		protected void OnSetInteractionMode(InteractionMode mode)
		{
			if (!externalUpdate)
			{
				if (mode == InteractionMode.Select2D) // force to front parallel for 2D viewing
				{
					viewport.Camera.Projection = Projection.Parallel;
					viewport.Camera.SetViewDirection(ViewDirection.Front);
				}
				else if (viewport.InteractionState.Mode == InteractionMode.Select2D) // transitioning out of 2D
				{
					viewport.Camera.Projection = Projection.Perspective;
					viewport.Camera.SetViewDirection(ViewDirection.Standard);
				}

				viewport.InteractionState.Mode = mode;
				viewport.ResizeGL();
				viewport.PaintGL();
				UpdateToolbar();
			}
		}

#endregion

	}
}
