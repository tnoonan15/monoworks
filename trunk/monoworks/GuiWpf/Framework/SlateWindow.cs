using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Media;

using AvalonDock;

using MonoWorks.Framework;
using MonoWorks.Rendering.Interaction;

namespace MonoWorks.GuiWpf.Framework
{
	/// <summary>
	/// Base class for a Slate main window.
	/// </summary>
	public class SlateWindow : Window
	{
		public SlateWindow() : base()
		{
			// the dock panel that stores the toolbars and content
			DockPanel dockPanel = new DockPanel();
			base.Content = dockPanel;
            dockPanel.HorizontalAlignment = HorizontalAlignment.Stretch;
            dockPanel.VerticalAlignment = VerticalAlignment.Stretch;

			// create the menu bar
			mainMenu = new Menu();
			dockPanel.Children.Add(mainMenu);
			DockPanel.SetDock(mainMenu, Dock.Top);

			// the toolbar areas
			ToolBarTray tray;
			// top
			tray = new ToolBarTray();
			dockPanel.Children.Add(tray);
			DockPanel.SetDock(tray, Dock.Top);
			toolTrays[ToolPosition.Top] = tray;
			// bottom
			tray = new ToolBarTray();
			dockPanel.Children.Add(tray);
			DockPanel.SetDock(tray, Dock.Bottom);
			toolTrays[ToolPosition.Bottom] = tray;
			// left
			tray = new ToolBarTray();
			tray.Orientation = Orientation.Vertical;
			dockPanel.Children.Add(tray);
			DockPanel.SetDock(tray, Dock.Left);
			toolTrays[ToolPosition.Left] = tray;
			// right
			tray = new ToolBarTray();
			tray.Orientation = Orientation.Vertical;
			dockPanel.Children.Add(tray);
			DockPanel.SetDock(tray, Dock.Right);
			toolTrays[ToolPosition.Right] = tray;

            // create the dock manager
            dockManager = new DockingManager();
            dockPanel.Children.Add(dockManager);
            DockPanel.SetDock(dockManager, Dock.Top);
			dockManager.Background = Brushes.White;

            // create the document pane
			//documentPane = new DocumentPane();
			//dockManager.Content = documentPane;

		}

		#region Menus

		/// <summary>
		/// The main menu.
		/// </summary>
		protected Menu mainMenu;

		/// <summary>
		/// Creates a menu with the given name.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public MenuItem CreateMenu(string name)
		{
			MenuItem menu = new MenuItem();
			menu.Header = name;
			mainMenu.Items.Add(menu);
			return menu;
		}

		#endregion


		#region Tools

		/// <summary>
		/// The tool areas.
		/// </summary>
		protected Dictionary<ToolPosition, ToolBarTray> toolTrays = new Dictionary<ToolPosition, ToolBarTray>();
		
		/// <summary>
		/// Creates a toolbar at the given position.
		/// </summary>
		/// <param name="position"> The toolbar position.</param>
		/// <returns> The new toolbar.</returns>
		public ToolBar CreateToolbar(ToolPosition position)
		{
			ToolBar toolbar = new ToolBar();
			toolTrays[position].ToolBars.Add(toolbar);
			return toolbar;
		}

		/// <summary>
		/// Creates a toolbox at the given position.
		/// </summary>
		/// <param name="position"> The toolbox position.</param>
		/// <param name="name">The name of the toolbox.</param>
		/// <returns> The new toolbox.</returns>
		public ToolBox CreateToolBox(ToolPosition position, string name)
		{
			ToolBox toolBox = new ToolBox(name);
			toolTrays[position].ToolBars.Add(toolBox);
			return toolBox;
		}

		#endregion


		#region Catching Keyboard Events

		/// <summary>
		/// Called when a key is pressed in the window.
		/// </summary>
		public event KeyHandler KeyPressed;

		protected override void OnKeyDown(System.Windows.Input.KeyEventArgs e)
		{
			base.OnKeyDown(e);

			if (e.Handled)
				return;

			int key = (int)e.Key + 53; // offset in WPF key mapping

			if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
				key += (int)InteractionModifier.Shift;
			if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
				key += (int)InteractionModifier.Control;
			if (Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt))
				key += (int)InteractionModifier.Alt;

			if (KeyPressed != null)
				KeyPressed(key);			
		}


		#endregion



		#region Documents


		protected DockingManager dockManager;
		/// <summary>
		/// The dock manager.
		/// </summary>
		public DockingManager DockManager
		{
			get { return dockManager; }
		}


		protected DocumentPane documentArea;
		/// <summary>
		/// The document area.
		/// </summary>
		public DocumentPane DocumentArea
		{
			get { return documentArea; }
			set { documentArea = value; }
		}

		/// <summary>
		/// Document counters to keep track of how many documents of each type have been created.
		/// </summary>
		protected Dictionary<DocumentType, int> documentCounters = new Dictionary<DocumentType, int>();

		/// <summary>
		/// Add a document.
		/// </summary>
		/// <param name="documentType"> Type of the document to create.</param>
		public virtual void AddDocument(DocumentType documentType)
        {
			if (documentArea == null)
				throw new Exception("Cannot add documents if the DocumentArea was never added.");

			// create the instance
			object docObject = Activator.CreateInstance(documentType.Type);

			// ensure the type is valid
			if (!(docObject is DocumentBase))
				throw new Exception(documentType.ToString() + " is not a valid document type.");
			DocumentBase document = (DocumentBase)docObject;

			// get the counter for this type
			if (!documentCounters.ContainsKey(documentType))
				documentCounters[documentType] = 0;
			documentCounters[documentType]++;

			// add the document to the document pane
			document.Title = documentType.DisplayName + documentCounters[documentType].ToString();
			documentArea.Items.Add(document);
        }

		/// <summary>
		/// Adds a document that has already been created.
		/// </summary>
		/// <param name="document"></param>
		public virtual void AddDocument(DocumentBase document)
		{
			documentArea.Items.Add(document);
		}

        #endregion

    }
}
