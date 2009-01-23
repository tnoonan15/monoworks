// UiManager.cs - Slate Mono Application Framework
//
//  Copyright (C) 2008 Andy Selvig
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
//
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA 

using System;
using System.Collections.Generic;
using System.Xml;
using System.Reflection;

using MonoWorks.Framework;
using MonoWorks.GuiGtk.Framework.Dock;
using MonoWorks.GuiGtk.Framework.Tools;

namespace MonoWorks.GuiGtk.Framework
{
	
	
	/// <summary>
	/// UI Manager for Gtk Slate applications.
	/// </summary>
	public class UiManager : UiManagerBase
	{
		
		public UiManager(AbstractController controller, SlateWindow window) : base(controller)
		{
			this.controller = controller;
			this.window = window;
			
			dockManager = window.DockManager;
		}
		
		/// <summary>
		/// The controller.
		/// </summary>
		private AbstractController controller;
		
		/// <value>
		/// The window.
		/// </value>
		private SlateWindow window;

		/// <value>
		/// String for generating a Gtk UiManager.
		/// </value>
		protected string uiString = "<ui><menubar name='menubar'>";
		
		/// <value>
		/// Whether or not a menu is currently being parsed.
		/// </value>
		private bool parsingMenu = false;
		
				
		/// <value>
		/// The current UI mode.
		/// </value>
		protected override UiMode Mode
		{
			get
			{
				if (currentToolbar != null)
					return UiMode.Toolbar;
				if (parsingMenu)
					return UiMode.Menu;

				if (currentToolBox != null)
				{
					if (currentToolShelf != null)
						return UiMode.ToolShelf;
					else
						return UiMode.ToolBox;
				}
				
				if (currentStackNode != null)
					return UiMode.DockableSizer;
//				if (currentDockableBook != null)
//					return UiMode.DockableBook;
				
				
				return UiMode.None;
			}
		}
		
		
		protected override void EndElement(XmlReader reader)
		{
			switch (reader.Name)
			{
			case "Menus": // end of the menu bar
				uiString += "</menubar>";
				break;
			case "Menu":
				parsingMenu = false;
				uiString += "</menu>";
				break;
			case "DockableSizer":
				GetParentNode(currentStackNode);
				break;
			case "DockableBook":
				GetParentNode(currentBookNode);
				break;
			}
			
			currentToolbar = null;
		}

		
		protected override void LoadCompleted()
		{
			base.LoadCompleted();

			dockManager.RootNode.Refresh();
			
			uiString += "</ui>";
			
			// create the Gtk ui manager
			Gtk.UIManager gtkUiManager = new Gtk.UIManager();
			gtkUiManager.InsertActionGroup(actionGroup, 0);
			gtkUiManager.AddUiFromString(uiString);
			window.AddAccelGroup(gtkUiManager.AccelGroup);

			Gtk.MenuBar menuBar = (Gtk.MenuBar)gtkUiManager.GetWidget("/menubar");
			window.AddMenuBar(menuBar);

			// add the tools menu item
			if (CreateToolsMenu)
			{
				// construct the tools menu
				foreach (string name in toolbars.Keys)
				{
					Gtk.CheckMenuItem item_ = new Gtk.CheckMenuItem(name);
					Toolbar toolbar = toolbars[name];
					item_.Active = toolbar.Visible;
					item_.Activated += delegate(object sender, EventArgs e) {
						toolbar.ToolVisible = !toolbar.ToolVisible;
					};
					toolsMenu.Append(item_);
				}
				foreach (string name in toolBoxes.Keys)
				{
					Gtk.CheckMenuItem item_ = new Gtk.CheckMenuItem(name);
					ToolBox toolBox = toolBoxes[name];
					item_.Active = toolBox.Visible;
					item_.Activated += delegate(object sender, EventArgs e) {
						toolBox.ToolVisible = !toolBox.ToolVisible;
					};
					toolsMenu.Append(item_);
				}

				// add the tools menu to the view menu
				Gtk.Menu viewMenu = (gtkUiManager.GetWidget("/menubar/ViewMenu") as Gtk.ImageMenuItem).Submenu as Gtk.Menu;
				Gtk.MenuItem item = new Gtk.MenuItem("Toolbars");
				viewMenu.Append(item);
				item.Submenu = toolsMenu;
				item.ShowAll();
			}
		}

		
#region Actions
		
		/// <summary>
		/// The action group for menu actions.
		/// </summary>
		protected Gtk.ActionGroup actionGroup = new Gtk.ActionGroup("Slate");

		protected override void CreateAction(XmlReader reader)
		{
			base.CreateAction(reader);
			
			string name = GetName(reader);

			Gtk.Action action = new Gtk.Action(name, actions[name].Name, actions[name].Tooltip, actions[name].IconName);
			if (actions[name].MethodInfo != null) // there may not be a method associated with the action
			{
				action.Activated += delegate(object sender, EventArgs args)
				{
					actions[name].MethodInfo.Invoke(controller, null);
				};
			}
			actionGroup.Add(action, actions[name].GtkAccelerator);
		
		}


#endregion
		
		
#region Menus
		
		protected override void CreateMenu(XmlReader reader)
		{			
			parsingMenu = true;
			
			// create the menu action
			string name = GetName(reader);
			Gtk.Action action = new Gtk.Action(name, name);
			actionGroup.Add(action);
			
			uiString += String.Format("<menu action='{0}' name='{1}'>", name, name + "Menu");
		}
		
		
		protected override void CreateMenuItem(ActionAttribute action)
		{			
			uiString += String.Format("<menuitem action='{0}'/>", action.Name);
		}


		private Gtk.Menu toolsMenu = null;
		
		protected override bool CreateToolsMenu
		{
			get { return toolsMenu != null;}
			set
			{
				if (value)
					toolsMenu = new Gtk.Menu();
				else
					toolbars = null;
			}
		}

		

#endregion
		
		
#region Tools
		
		/// <summary>
		/// The current toolbar.
		/// </summary>
		Toolbar currentToolbar = null;
				
		/// <summary>
		/// The toolbars.
		/// </summary>
		protected Dictionary<string, Toolbar> toolbars = new Dictionary<string,Toolbar>();
		
		/// <summary>
		/// The current tool box.
		/// </summary>
		protected ToolBox currentToolBox = null;
		
		/// <summary>
		/// The tool boxes.
		/// </summary>
		protected Dictionary<string, ToolBox> toolBoxes = new Dictionary<string,ToolBox>();
		
		/// <summary>
		/// The current tool shelf.
		/// </summary>
		protected ToolShelf currentToolShelf = null;
		
		/// <summary>
		/// The tool shelves.
		/// </summary>
		protected Dictionary<string, ToolShelf> toolShelves = new Dictionary<string,ToolShelf>();
		
		protected override void CreateToolbar(XmlReader reader)
		{
			string name = GetName(reader);
			ToolPosition position = GetToolPosition(reader);
			currentToolbar = window.CreateToolbar(position);
			toolbars[name] = currentToolbar;
		}
		
		protected override void CreateToolbarItem(ActionAttribute action)
		{
			Gdk.Pixbuf pixbuf = currentToolbar.RenderIcon(action.IconName, Gtk.IconSize.LargeToolbar, "");
			Gtk.Image image = new Gtk.Image(pixbuf);
			Gtk.ToolButton button = new Gtk.ToolButton(image, action.Name);
			currentToolbar.Add(button);
			button.Clicked += delegate(object sender, EventArgs args)
			{
				action.MethodInfo.Invoke(controller, null);
			};
			button.TooltipText = action.Tooltip;
		}
		
		protected override void CreateToolBox(XmlReader reader)
		{
			currentToolBox = window.CreateToolBox(GetToolPosition(reader), GetName(reader));
			toolBoxes[GetName(reader)] = currentToolBox;
		}

		protected override void CreateToolShelf(XmlReader reader)
		{
			currentToolShelf = currentToolBox.CreateShelf(GetName(reader));
			
		}
		
		protected override void CreateToolItem (ActionAttribute action)
		{
			Gdk.Pixbuf pixbuf = currentToolShelf.RenderIcon(action.IconName, Gtk.IconSize.Dnd, "");
			Gtk.Image image = new Gtk.Image(pixbuf);
			Gtk.ToolButton button = new Gtk.ToolButton(image, action.Name);
			button.Clicked += delegate(object sender, EventArgs args)
			{
				action.MethodInfo.Invoke(controller, null);
			};
			button.TooltipText = action.Tooltip;
			currentToolShelf.AddButton(button);
		}
		
		
#endregion		
		
		protected override void AddSeparator ()
		{
			switch (Mode)
			{
			case UiMode.Menu:
				uiString += "<separator/>";
				break;
			case UiMode.Toolbar:
				currentToolbar.Add(new Gtk.SeparatorToolItem());
				break;
			default:
				throw new Exception("Can't add separator in the current UI mode.");
			}
		}

		
#region Docking
		
		/// <summary>
		/// The docking manager for the window associated with this controller.
		/// </summary>
		/// <param name="reader"> </param>
		protected DockManager dockManager;
		
		/// <value>
		/// The current node in the docking framework.
		/// </value>
		protected Node CurrentNode
		{
			get
			{				
				if (currentStackNode != null)
					return currentStackNode;
				else if (currentBookNode != null)
					return currentBookNode;
				else
					return dockManager.RootNode;
			}
		}
		
		/// <summary>
		/// Gets the parent of node and stores it as the current one.
		/// </summary>
		/// <param name="node"> A <see cref="Node"/>. </param>
		protected void GetParentNode(Node node)
		{			
			Node parent = node.Parent;
			
			if (node is PanedNode)
				currentStackNode = null;
			else if (node is BookNode)
				currentBookNode = null;
			
			if (parent is PanedNode)
				currentStackNode = parent as PanedNode;
			else if (parent is BookNode)
				currentBookNode = parent as BookNode;
		}
		
		BookNode currentBookNode = null;
		
		protected override void CreateDockableBook(XmlReader reader)
		{
			BookNode bookNode = new BookNode();
			
			CurrentNode.Add(bookNode);
			
			currentStackNode = null;
			currentBookNode = bookNode;
		}	
		
		PanedNode currentStackNode = null;
		
		protected override void CreateDockableSizer(XmlReader reader)
		{			
			// get the orientation
			Gtk.Orientation orientation;
			string orientationName = reader.GetAttribute("orientation");
			if (orientationName == null)
				throw new Exception("All DockableSizer tags must have an orientation attribute.");
			else if (orientationName == "Horizontal")
				orientation = Gtk.Orientation.Horizontal;
			else if (orientationName == "Vertical")
				orientation = Gtk.Orientation.Vertical;
			else
				throw new Exception("The orientation attribute must be either Horizontal or Vertical.");
			
			PanedNode stackNode = new PanedNode(orientation);
			
			CurrentNode.Add(stackNode);
			
			currentBookNode = null;
			currentStackNode = stackNode;
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
			DockableBase dockableWidget = dockableObject as DockableBase;

			Dockable dockable = dockManager.CreateDockable(dockableWidget, name);
			DockableNode node = new DockableNode(dockable);
			
			// set the preferred size of the dockable
			string heightString = reader.GetAttribute("height");
			if (heightString != null)
				dockableWidget.HeightRequest = Int32.Parse(heightString);
			string widthString = reader.GetAttribute("width");
			if (widthString != null)
				dockableWidget.WidthRequest = Int32.Parse(widthString);
			
			
			// create the dockable container
			Node currentNode = CurrentNode;
			currentNode.Add(node);
			dockManager.RegisterDockable(dockable, node);
		}
		
		protected override void CreateDocumentArea(XmlReader reader)
		{
			DockArea documentArea = new DockArea(true);
//			documentArea.SetSizeRequest(800,800);
			dockManager.DocumentArea = documentArea;
			CurrentNode.Add(dockManager.DocumentAreaNode);
		}
		

		
#endregion


#region Documents

		public override IDocument CreateDocument(DocumentType documentType)
		{
			base.CreateDocument(documentType);
			
			if (dockManager.DocumentArea == null)
				throw new Exception("Cannot add documents if the DocumentArea was never added.");

			// create the instance
			object docObject = Activator.CreateInstance(documentType.Type);

			// ensure the type is valid
			if (!(docObject is DocumentBase))
				throw new Exception(documentType.ToString() + " is not a valid document type.");
			DocumentBase document = docObject as Dock.DocumentBase;
			document.IconName = documentType.IconName;

			// add the document to the document pane
			document.Name = documentType.DisplayName + documentCounters[documentType].ToString();
			dockManager.AddDocument(document);
			
			return document;
		}




#endregion





		
		
		
	}
}
