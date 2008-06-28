// AbstractController.cs - MonoWorks Project
//
// Copyright (C) 2008 Andy Selvig
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA

using System;
using System.Xml;
using System.Collections.Generic;

using Qyoto;

namespace MonoWorks.Gui
{
	
	/// <summary>
	/// Base class for all window controllers.
	/// </summary>
	public abstract class AbstractController<WindowType> : QObject where WindowType : QMainWindow
	{
		
		/// <summary>
		/// Default constructor.
		/// </summary>
		/// <param name="window"> A <see cref="QMainWindow"/> to control. </param>
		public AbstractController(WindowType window)
		{
			this.window = window;
			
			actions = new Dictionary<string,QAction>();
			
			toolbars = new Dictionary<string,QToolBar>();
			currentToolbar = null;
			
			menus = new Dictionary<string,QMenu>();
			currentMenu = null;
			
			toolboxes = new Dictionary<string,Toolbox>();
			currentToolbox = null;
			
			toolshelves = new Dictionary<string,Toolshelf>();
			currentToolshelf = null;
		}
		
		
		protected WindowType window;
		/// <value>
		/// The window that is being controlled.
		/// </value>
		public WindowType Window
		{
			get {return window;}
			set {window = value;}
		}
		
		
#region Ui Elements
		
		protected Dictionary<string, QAction> actions;
		
		protected Dictionary<string, QToolBar> toolbars;		
		protected QToolBar currentToolbar;
		
		protected Dictionary<string, QMenu> menus; 		
		protected QMenu currentMenu;
		
		protected Dictionary<string, Toolbox> toolboxes;		
		protected Toolbox currentToolbox;
		
		protected Dictionary<string, Toolshelf> toolshelves;
		protected Toolshelf currentToolshelf;
		
#endregion
		
		
		
#region Loading
		
		/// <summary>
		/// Loads the default UI file.
		/// </summary>
		public virtual void Load()
		{
			string fileName;
			switch (typeof(WindowType).ToString())
			{
			case "MonoWorks.Studio.MainWindow":
				fileName = "DefaultUi.xml";
				break;
			case "MonoWorks.Editor.Window":
				fileName = "DefaultEditorUi.xml";
				break;
			default:
				throw new Exception("Invalid type for UiManager");
			}
			Load(ResourceManager.ResourceDirectory + fileName);
		}
		
		
		/// <summary>
		/// Loads the specified UI file.
		/// </summary>
		public virtual void Load(string fileName)
		{
			XmlReader reader = new XmlTextReader(fileName);
			
			while (!reader.EOF) // while there's still something left to read
			{
				reader.Read(); // read the next node
				
				// decide what to do based on the node type
				switch (reader.NodeType)
				{
				case XmlNodeType.Element:
					// decide what to do based on the element name
					switch (reader.Name)
					{
					case "Action":
						CreateAction(reader);
						break;
					case "Menu":
						CreateMenu(reader);
						break;
					case "MenuItem":
						CreateMenuItem(reader);
						break;
					case "Toolbar":
						CreateToolbar(reader);
						break;
					case "ToolItem":
						CreateToolItem(reader);
						break;
					case "Toolbox":
						CreateToolbox(reader);
						break;
					case "Toolshelf":
						CreateToolshelf(reader);
						break;
					case "Tool":
						CreateTool(reader);
						break;
					case "Separator":
						AddSeparator();
						break;
					}
					break;
				case XmlNodeType.EndElement:
					currentMenu = null;
					currentToolbar = null;
					currentToolshelf = null;
					if (reader.Name=="Toolbox")
					{
						currentToolbox.Show();
						currentToolbox = null;
					}
					break;
				}
			}
			
			reader.Close();
		}
		
		
		/// <summary>
		/// Creates a new action from the UI file stream.
		/// </summary>
		/// <param name="reader"> A <see cref="XmlReader"/> at an Action element. </param>
		protected virtual void CreateAction(XmlReader reader)
		{
			QAction action;
			string name = (string)reader.GetAttribute("name");
			string iconName = (string)reader.GetAttribute("icon");
			QIcon icon = ResourceManager.GetIcon(iconName); // get the icon
			if (icon == null) // there is no icon
				action = new QAction(name, Window);
			else // there is an icon
				action = new QAction(ResourceManager.GetIcon(iconName), name, Window);
			actions[name] = action;
			
			// assign the action attributes
			if (reader.GetAttribute("shortcut")!=null)
				action.Shortcut = (string)reader.GetAttribute("shortcut");
			if (reader.GetAttribute("statusTip") != null)
				action.StatusTip = (string)reader.GetAttribute("statusTip");
			
			// connect the slot
			QObject.Connect(action, QObject.SIGNAL("triggered()"), this, QObject.SLOT((string)reader.GetAttribute("slot")));
		}
	
		
#region Menus
		
		/// <summary>
		/// Creates a new menu from the UI file stream.
		/// </summary>
		/// <param name="reader"> A <see cref="XmlReader"/> at a Menu element. </param>
		protected virtual void CreateMenu(XmlReader reader)
		{
			string name = (string)reader.GetAttribute("name");
			QMenu menu = Window.MenuBar().AddMenu(name);
			currentMenu = menu;
			menus[name] = menu;
		}
		
		
		/// <summary>
		/// Creates a new menu item from the UI file stream.
		/// </summary>
		/// <param name="reader"> A <see cref="XmlReader"/> at a MenuItem element. </param>
		protected virtual void CreateMenuItem(XmlReader reader)
		{
			if (currentMenu == null)
				throw new Exception("All MenuItem elements must be inside a Menu element.");
			string actionName = (string)reader.GetAttribute("action");
			currentMenu.AddAction(actions[actionName]);
		}
		
#endregion
		
		
		
#region Toolbars		
		
		/// <summary>
		/// Creates a new toolbar from the UI file stream.
		/// </summary>
		/// <param name="reader"> A <see cref="XmlReader"/> at a toolbar element. </param>
		protected virtual void CreateToolbar(XmlReader reader)
		{
			string name = (string)reader.GetAttribute("name");
			QToolBar toolbar = Window.AddToolBar(name);
			currentToolbar = toolbar;
			toolbar.Size = new QSize(48, 48);
			toolbars[name] = toolbar;
		}
		
		
		/// <summary>
		/// Creates a new toolbar item from the UI file stream.
		/// </summary>
		/// <param name="reader"> A <see cref="XmlReader"/> at a ToolItem element. </param>
		protected virtual void CreateToolItem(XmlReader reader)
		{
			if (currentToolbar == null)
				throw new Exception("All ToolItem elements must be inside a Toolbar element.");
			string actionName = (string)reader.GetAttribute("action");
			currentToolbar.AddAction(actions[actionName]);
		}
		
#endregion
		
		

#region Toolboxes
		
		/// <summary>
		/// Creates a new toolbox from the UI file stream.
		/// </summary>
		/// <param name="reader"> A <see cref="XmlReader"/> at a toolbox element. </param>
		protected virtual void CreateToolbox(XmlReader reader)
		{
			string name = (string)reader.GetAttribute("name");
			QDockWidget dockWidget = new QDockWidget(name, Window);
			Window.AddDockWidget(QDockWidget.DockWidgetArea.RightDockWidgetArea, dockWidget);
			currentToolbox = new Toolbox(dockWidget);
			dockWidget.SetWidget(currentToolbox);

			toolboxes[name] = currentToolbox;
		}
		
		
		/// <summary>
		/// Creates a new toolshelf from the UI file stream.
		/// The toolshelf element must be declared inside a toolbox element.
		/// </summary>
		/// <param name="reader"> A <see cref="XmlReader"/> at a toolshelf element. </param>
		protected virtual void CreateToolshelf(XmlReader reader)
		{
			if (currentToolbox==null)
				throw new Exception("Toolshelfs must be declared inside toolboxes.");
			string name = (string)reader.GetAttribute("name");
			currentToolshelf = currentToolbox.AddShelf(name);
			toolshelves[name] = currentToolshelf;
		}		
		
		
		/// <summary>
		/// Creates a new toolshelf from the UI file stream.
		/// The toolshelf element must be declared inside a toolshelf element.
		/// </summary>
		/// <param name="reader"> A <see cref="XmlReader"/> at a tool element. </param>
		protected virtual void CreateTool(XmlReader reader)
		{
			if (currentToolshelf==null)
				throw new Exception("Tools must be declared inside toolshelves.");
			string actionName = (string)reader.GetAttribute("action");
			if (!actions.ContainsKey(actionName)) // ensure the action is valid
				throw new Exception("The UiManager does not contain an action called " + actionName);
			currentToolshelf.AddAction(actions[actionName]); 
		}
		
#endregion
		
		
		/// <summary>
		/// Adds a separator to the current menu or toolbar.
		/// </summary>
		protected virtual void AddSeparator()
		{
			if (currentMenu!=null) // if we're currently at a menu
				currentMenu.AddSeparator();
			else if (currentToolbar!=null) // if we're currently at a toolbar
				currentToolbar.AddSeparator();
			else // not at a menu or toolbar, separator is not valid
				throw new Exception("Separators must be added to Toolbar or Meny tags.");
		}
		
#endregion
		
		
	}
}
