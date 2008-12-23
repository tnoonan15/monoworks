using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml;
using System.IO;
using System.Reflection;

using MonoWorks.Framework;

using AvalonDock;

namespace MonoWorks.GuiWpf.Framework
{
	/// <summary>
	/// The UI manager for WPF applications.
	/// </summary>
    public class UiManager : UiManagerBase
    {

		public UiManager(Controller controller) : base(controller)
		{
			this.controller = controller;

		}


		protected Controller controller;


		public override void Load(string fileName)
		{
			// wrap the base method and put any exceptions in a nice dialog
			try
			{
				base.Load(fileName);
			}
			catch (Exception ex)
			{
				string messageText = "Slate encountered an error when parsing the UI file. " + 
					"The contents of the error is listed below. Please revise the UI file to fix this error.\n\n" + 
					ex.Message;
				MessageBox.Show(messageText, "Slate UI Error", MessageBoxButton.OK);
				Application.Current.Shutdown();
			}
		}


		protected override void LoadCompleted()
		{
			base.LoadCompleted();

			// make the tools menu
			if (CreateToolsMenu)
			{
				// get the view menu or create one if it doesn't exist
				MenuItem viewMenu;
				if (menus.ContainsKey("View"))
					viewMenu = menus["View"];
				else // no view menu
				{
					viewMenu = controller.Window.CreateMenu("View");
					menus["View"] = viewMenu;					
				}

				// add the tools menu
				toolsMenu.Header = "Toolbars";
				viewMenu.Items.Add(toolsMenu);
				foreach (string name in toolbars.Keys)
				{
					MenuItem item = new MenuItem();
					item.Header = name;
					toolsMenu.Items.Add(item);
					ToolBar toolbar = toolbars[name];
					item.Click += delegate(object sender, RoutedEventArgs e)
					{
						if (toolbar.Visibility == Visibility.Visible)
							toolbar.Visibility = Visibility.Collapsed;
						else
							toolbar.Visibility = Visibility.Visible;
					};
				}
				foreach (string name in toolBoxes.Keys)
				{
					MenuItem item = new MenuItem();
					item.Header = name;
					toolsMenu.Items.Add(item);
					ToolBox toolBox = toolBoxes[name];
					item.Click += delegate(object sender, RoutedEventArgs e)
					{
						if (toolBox.Visibility == Visibility.Visible)
							toolBox.Visibility = Visibility.Collapsed;
						else
							toolBox.Visibility = Visibility.Visible;
					};
				}
			}
		}


#region Ui Elements

		protected Dictionary<string, ToolBar> toolbars = new Dictionary<string,ToolBar>();
		protected ToolBar currentToolbar = null;

		protected Dictionary<string, MenuItem> menus = new Dictionary<string,MenuItem>();
		protected MenuItem currentMenu = null;

		protected Dictionary<string, ToolBox> toolBoxes = new Dictionary<string,ToolBox>();
		protected ToolBox currentToolBox = null;

		protected Dictionary<string, ToolShelf> toolShelves = new Dictionary<string,ToolShelf>();
		protected ToolShelf currentToolShelf = null;


		protected override void EndElement(XmlReader reader)
		{
			currentToolbar = null;
			currentMenu = null;
            if (reader.Name == "ToolBox")
                currentToolBox = null;
			currentToolShelf = null;

			if (reader.Name == "DockableSizer")
			{
				// get the sizer's parent
				if (currentDockableSizer.Parent is ResizingPanel)
					currentDockableSizer = currentDockableSizer.Parent as ResizingPanel;
				else if (currentDockableSizer.Parent is DockingManager)
					currentDockableSizer = null;
				else
					throw new Exception("The current dockable sizer has an invalid parent type.");
			}

            if (reader.Name == "DockableBook")
            {
                // get the book's parent
                if (currentDockableBook.Parent is ResizingPanel)
                    currentDockableSizer = currentDockableBook.Parent as ResizingPanel;

                currentDockableBook = null;
                
            }

			if (reader.Name == "DockableArea")
			{
				currentDockableSizer = null;
				currentDockableBook = null;
			}
		}

#endregion


		protected override UiMode Mode
		{
			get
			{
				if (currentToolbar != null)
					return UiMode.Toolbar;
				if (currentMenu != null)
					return UiMode.Menu;
				if (currentToolBox != null)
				{
					if (currentToolShelf != null)
						return UiMode.ToolShelf;
					else
						return UiMode.ToolBox;
				}
				if (currentDockableSizer != null)
					return UiMode.DockableSizer;
				if (currentDockableBook != null)
					return UiMode.DockableBook;

				return UiMode.None;
			}
		}


		#region Toolbars, Menus, and Tools

		protected override void CreateToolbar(XmlReader reader)
		{
			string name = GetName(reader);
			ToolPosition position = GetToolPosition(reader);
			currentToolbar = controller.Window.CreateToolbar(position);
			toolbars[name] = currentToolbar;
		}


		protected override void CreateToolbarItem(ActionAttribute action)
		{
			Button button = new Button();
			if (action.IconName == null)
				button.Content = action.Name;
			else // the icon name is defined
				button.Content = ResourceManager.RenderIcon(action.IconName, 22);
			if (action.Tooltip != null)
				button.ToolTip = action.Tooltip;
			button.Click += delegate(object sender, RoutedEventArgs args)
			{
				action.MethodInfo.Invoke(abstractController, null);
			};
			currentToolbar.Items.Add(button);
		}


		protected override void CreateMenu(XmlReader reader)
		{
			string name = GetName(reader);
			currentMenu = controller.Window.CreateMenu(name);
			menus[name] = currentMenu;
		}


		protected override void CreateMenuItem(ActionAttribute action)
		{
			MenuItem item = new MenuItem();
			item.Header = action.Name;
			if (action.IconName != null)
				item.Icon = ResourceManager.RenderIcon(action.IconName, 16);
			if (action.Tooltip != null)
				item.ToolTip = action.Tooltip;
			item.Click += delegate(object sender, RoutedEventArgs args)
			{
				action.MethodInfo.Invoke(abstractController, null);
			};
			currentMenu.Items.Add(item);
		}

		/// <summary>
		/// The menu containing the tools.
		/// </summary>
		private MenuItem toolsMenu = null;

		protected override bool CreateToolsMenu
		{
			get {return toolsMenu != null;}
			set
			{
				if (value)
					toolsMenu = new MenuItem();
				else
					toolsMenu = null;
			}
		}


		protected override void CreateToolBox(XmlReader reader)
		{
			string name = GetName(reader);
			ToolPosition position = GetToolPosition(reader);
			currentToolBox = controller.Window.CreateToolBox(position, name);
			toolBoxes[name] = currentToolBox;
		}


		protected override void CreateToolShelf(XmlReader reader)
		{
			string name = GetName(reader);
			currentToolShelf = currentToolBox.AddShelf(name);
			toolShelves[name] = currentToolShelf;
		}


		protected override void CreateToolItem(ActionAttribute action)
		{
			currentToolShelf.AddAction(action, controller);
		}


		protected override void AddSeparator()
		{
			switch (Mode)
			{
				case UiMode.Menu: // add one to the menu
					currentMenu.Items.Add(new Separator());
					break;
				case UiMode.Toolbar: // add one to the toolbar
					currentToolbar.Items.Add(new Separator());
					break;
				default:
					throw new Exception(String.Format("{0} is an invalid mode to add a separator.", Mode));
			}
		}

		#endregion


		#region Dockables

		protected ResizingPanel currentDockableSizer = null;

		protected DockablePane currentDockableBook = null;

		/// <summary>
		/// A list of all dockables in the UI.
		/// </summary>
		protected Dictionary<string, DockableBase> dockables = new Dictionary<string, DockableBase>();

		/// <summary>
		/// Gets the dockable of the given name.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public DockableBase GetDockable(string name)
		{
			if (!dockables.ContainsKey(name))
				throw new Exception("There is not dockable called " + name);
			return dockables[name];
		}


		protected override void CreateDockableSizer(XmlReader reader)
		{
			// get the desired orientation
			string orientationString = reader.GetAttribute("orientation");
			Orientation orientation;
			if (orientationString == "Vertical")
				orientation = Orientation.Vertical;
			else if (orientationString == "Horizontal")
				orientation = Orientation.Horizontal;
			else 
				throw new Exception("All dockable sizers must declare a valid (Horizontal or Vertical) orientation attribute.");

			// create the sizer
			ResizingPanel sizer = new ResizingPanel();
			sizer.Orientation = orientation;

			if (currentDockableSizer != null)
			{
				currentDockableSizer.Children.Add(sizer);
			}
			else // add the sizer to the dockable area
			{
				controller.Window.DockManager.Content = sizer;
			}
			currentDockableSizer = sizer;
		}

		protected override void CreateDockableBook(XmlReader reader)
		{
            // create the book
            currentDockableBook = new DockablePane();

            if (currentDockableSizer != null)
            {
                currentDockableSizer.Children.Add(currentDockableBook);
                currentDockableSizer = null;
            }
            else // add the sizer to the dockable area
            {
                controller.Window.DockManager.Content = currentDockableBook;
            }

		}

		protected override void CreateDockable(XmlReader reader)
		{
			// get the name
			string name = GetName(reader);

			// get the type name
			string typeName = reader.GetAttribute("type");
			if (typeName == null)
				throw new Exception("All dockable tags must have a type attribute.");

			// lookup the type from the loaded assemblies
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			Type dockableType = null;
			foreach (Assembly assembly in assemblies) // search all loaded assemblies
			{
				Type[] types = assembly.GetExportedTypes();
				foreach (Type type in types) // search all exported types in this assembly
				{
					string[] typeNames = type.Name.Split('.');
					string typeName_ = typeNames[typeNames.Length - 1];
					if (typeName_ == typeName)
					{
						dockableType = type;
					}
				}
			}
			if (dockableType == null)
				throw new Exception(String.Format("Type {0} is not present in any loaded assembly.", typeName));

			// instantiate the dockable
			object dockableObject = Activator.CreateInstance(dockableType);
			if (!(dockableObject is DockableBase))
				throw new Exception("Dockable " + typeName + " is not a valid dockable.");
			DockableBase dockable = dockableObject as DockableBase;
			dockable.Title = name;
			dockables[name] = dockable;

            DockablePane pane;
			// create the pane
            if (currentDockableBook == null)
            {
                pane = new DockablePane();
                pane.Items.Add(dockable);

                // add the dockable to the current container
                if (currentDockableSizer != null)
                {
                    currentDockableSizer.Children.Add(pane);
                }
                else // add the sizer to the dockable area
                {
                    controller.Window.DockManager.Content = pane;
                }

            }
            else
            {
                pane = currentDockableBook;
                currentDockableBook.Items.Add(dockable);
            }

			// get the icon
			string iconName = reader.GetAttribute("icon");
			if (iconName != null)
			{
				// get the icon's full path
				//string originalSource = ResourceManager.ResourceDir.FullName + @"icons16\" + iconName + ".png";

				//// copy the icon to a temporary directory since it's already 
				//// being used by the resource manager and can't be opened again
				//string appDir = System.Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\Slate\";
				//string tempName = appDir + iconName + ".png";
				//File.Copy(originalSource, tempName);
				//dockable.IconSource = tempName;
			}


		}

		protected override void CreateDocumentArea(XmlReader reader)
		{
			DocumentPane documentArea = new DocumentPane();

			// make the document area prettier
			documentArea.Background = new LinearGradientBrush(Colors.Gray, Colors.LightBlue, new Point(0,0), new Point(400, 400));

			// add the document area to the current container
			if (currentDockableSizer != null)
				currentDockableSizer.Children.Add(documentArea);
			else // add the sizer to the dockable area
				controller.Window.DockManager.Content = documentArea;
			controller.Window.DocumentArea = documentArea;
		}

		#endregion
	}
}
