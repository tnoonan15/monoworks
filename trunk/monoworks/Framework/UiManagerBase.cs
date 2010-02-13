// UiManagerBase.cs - MonoWorks Project
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
using System.IO;

namespace MonoWorks.Framework
{
	/// <summary>
	/// Possible positions for tools.
	/// </summary>
	public enum ToolPosition { Top, Left, Right, Bottom };

	/// <summary>
	/// The modes for the UI manager.
	/// </summary>
	public enum UiMode { None, Actions, Action, Menu, Tools, Toolbar, ToolBox, ToolShelf, DockableArea, DockableSizer, DockableBook};



	/// <summary>
	/// Base class for UI managers.
	/// </summary>
	/// <remarks>
	/// The UiManager loads toolbars and menus from a UI (XML) file and places them in the window.
	/// It can then write the modified configuration back to a file.
	/// </remarks>
	public abstract class UiManagerBase
	{
		/// <summary>
		/// Initialization constructor.
		/// </summary>
		public UiManagerBase(AbstractController controller)
		{
			abstractController = controller;
		}

		protected AbstractController abstractController;

		/// <summary>
		/// The assembly that's currently using the UiManager.
		/// </summary>
		protected Assembly CallingAssembly = null;


		/// <summary>
		/// Loads a ui file from a stream.
		/// </summary>
		/// <param name="stream"></param>
		public void LoadStream(Stream stream)
		{
			CallingAssembly = Assembly.GetCallingAssembly();

			XmlReader reader = new XmlTextReader(stream);
			Load(reader);
		}

		/// <summary>
		/// Loads a ui file from a file.
		/// </summary>
		/// <param name="fileName"></param>
		public void LoadFile(string fileName)
		{
			CallingAssembly = Assembly.GetCallingAssembly();

			XmlReader reader = new XmlTextReader(fileName);
			Load(reader);
		}

		/// <summary>
		/// Loads the ui from a ui file stream.
		/// </summary>
		/// <remarks>Use convenience methods LoadFile() and LoadResource() 
		/// to load the ui directly from a file or embedded resource.</remarks>
		protected virtual void Load(XmlReader reader)
		{

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
					case "Tools":
						InitializeTools(reader);
						break;
					case "Toolbar":
						CreateToolbar(reader);
						break;
					case "ToolBox":
						CreateToolBox(reader);
						break;
					case "ToolShelf":
						CreateToolShelf(reader);
						break;
					case "Item":
						CreateItem(reader);
						break;
					case "Separator":
						AddSeparator();
						break;
					case "DocumentType":
						AddDocumentType(reader);
						break;
					case "DocumentArea":
						CreateDocumentArea(reader);
						break;
					case "DockableSizer":
						CreateDockableSizer(reader);
						break;
					case "DockableBook":
						CreateDockableBook(reader);
						break;
					case "Dockable":
						CreateDockable(reader);
						break;
					}
					break;
				case XmlNodeType.EndElement:
					EndElement(reader);
					break;
				}
			}

			reader.Close();
			
			LoadCompleted();
		}
		
		/// <summary>
		/// Called when the loading is complete.
		/// </summary>
		protected virtual void LoadCompleted()
		{
			
		}

        /// <summary>
        /// The current mode.
        /// </summary>
		protected abstract UiMode Mode
		{
			get;
		}

		/// <summary>
		/// Signals the end of the current element.
		/// </summary>
		/// <remarks> The subclasses should clear their state so that Mode is None.</remarks>
        protected abstract void EndElement(XmlReader reader);

		/// <summary>
		/// Gets the name attribute of the current element in the reader.
		/// </summary>
		/// <param name="reader"> An XML reader</param>
		/// <returns> The name.</returns>
		/// <remarks> An exception will be thrown if the name can't be found.</remarks>
		protected string GetName(XmlReader reader)
		{
			string name = reader.GetAttribute("name");
			if (name == null)
				throw new Exception("All UI elements should have a name attribute.");
			return name;
		}


        #region Actions

		/// <summary>
		/// The actions that have been registered.
		/// </summary>
		protected Dictionary<string, ActionAttribute> actions = new Dictionary<string,ActionAttribute>();

		/// <summary>
		/// Creates a new action from the UI file stream.
		/// </summary>
		/// <param name="reader"> A <see cref="XmlReader"/> at an Action element. </param>
		protected virtual void CreateAction(XmlReader reader)
		{
			string name = GetName(reader);
			ActionAttribute action = null;
			if (abstractController.HasMethod(name))
			{
				action = abstractController.GetAction(name);
			}
			else
			{
				Console.WriteLine("The controller does not have an action called {0}", name);
				action = new ActionAttribute();
			}


			// assign the icon name
			string iconName = reader.GetAttribute("icon");
			if (iconName != null)
				action.IconName = iconName;

			// assign the tooltip
			string tooltip = reader.GetAttribute("tooltip");
			if (tooltip != null)
				action.Tooltip = tooltip;

			// assign the tooltip
			string togglableString = reader.GetAttribute("togglable");
			if (togglableString != null)
				action.IsTogglable = Boolean.Parse(togglableString);

			// assign the shortcut
			action.Shortcut = reader.GetAttribute("shortcut");
			if (action.Shortcut != null)
			{
				int key = ParseShortcut(action.Shortcut);
				actionKeyMap[key] = action;
			}

			actions[name] = action;
		}

        #endregion


		#region Keyboard Handling

		/// <summary>
		/// Maps key combinations to actions.
		/// </summary>
		protected Dictionary<int, ActionAttribute> actionKeyMap = new Dictionary<int, ActionAttribute>();

		/// <summary>
		/// Parses the string representation of a shortcut into a unique integer.
		/// </summary>
		/// <param name="shortcut"></param>
		/// <returns></returns>
		protected int ParseShortcut(string shortcut)
		{
			string[] comps = shortcut.Split('+');
			int key = 0;
			foreach (string comp in comps)
			{
				string compLower = comp.ToLower();
				switch (compLower)
				{
				case "ctrl":
				case "control":
					key += (int)InteractionModifier.Control;
					break;
				case "shift":
					key += (int)InteractionModifier.Alt;
					break;
				case "alt":
					key += (int)InteractionModifier.Shift;
					break;
				default:
					if (compLower.Length == 1) // a proper character
						key += (int)compLower.ToCharArray()[0];
					else
						throw new Exception("Invalid shortcut: " + comp);
					break;
				}
			}
			return key;
		}

		/// <summary>
		/// Attempts to handle the key press with its assigned action, if any.
		/// </summary>
		/// <param name="key">A bitwise OR of the key value and interaction modifiers.</param>
		/// <returns>True if the key press was handled.</returns>
		public bool HandleKeyPress(int key)
		{
			ActionAttribute action;
			if (actionKeyMap.TryGetValue(key, out action))
			{
				action.MethodInfo.Invoke(abstractController, null);
				return true;
			}
			return false;
		}

		#endregion


		#region Tools

		/// <summary>
		/// Whether or not to create a tools submenu in the view menu that shows all tools.
		/// </summary>
		protected abstract bool CreateToolsMenu {get; set;}

		/// <summary>
		/// Initialize the tools from the Tools element.
		/// </summary>
		/// <param name="reader"> </param>
		protected void InitializeTools(XmlReader reader)
		{
			string menuString = reader.GetAttribute("createMenu");
			if (menuString != null)
				CreateToolsMenu = Boolean.Parse(menuString);
		}
		
        /// <summary>
		/// Gets the action for the item and creates the appropriate item based on the mode.
		/// </summary>
		/// <param name="reader"></param>
		protected void CreateItem(XmlReader reader)
		{
			// get the action
			string actionName = reader.GetAttribute("action");
			if (actionName == null)
				throw new Exception("Item tags must have an action attribute.");
			if (!actions.ContainsKey(actionName))
				throw new Exception(String.Format("Action {0} has not been declared in the UI file.", actionName));
			ActionAttribute action = actions[actionName];

			switch (Mode)
			{
				case UiMode.Toolbar:
					CreateToolbarItem(action);
					break;
				case UiMode.Menu:
					CreateMenuItem(action);
					break;
				case UiMode.ToolShelf:
					CreateToolItem(action);
					break;
				default:
					throw new Exception("All items must be in a Toolbar, Menu, or ToolShelf tag.");
			}
		}

		/// <summary>
		/// Creates a new toolbar from the UI file stream.
		/// </summary>
		/// <param name="reader"> A <see cref="XmlReader"/> at a toolbar element. </param>
		protected abstract void CreateToolbar(XmlReader reader);
		
		/// <summary>
		/// Creates a toolbar button from the action.
		/// </summary>
		/// <param name="action"></param>
		protected abstract void CreateToolbarItem(ActionAttribute action);

		/// <summary>
		/// Creates a new menu from the UI file stream.
		/// </summary>
		/// <param name="reader"> A <see cref="XmlReader"/> at a menu element. </param>
		protected abstract void CreateMenu(XmlReader reader);

		/// <summary>
		/// Creates a menu item from the action.
		/// </summary>
		/// <param name="action"></param>
		protected abstract void CreateMenuItem(ActionAttribute action);

		/// <summary>
		/// Creates a new toolbox from the UI file stream.
		/// </summary>
		/// <param name="reader"> A <see cref="XmlReader"/> at a toolbox element. </param>
		protected abstract void CreateToolBox(XmlReader reader);

		/// <summary>
		/// Creates a new toolshelf from the UI file stream.
		/// The toolshelf element must be declared inside a toolbox element.
		/// </summary>
		/// <param name="reader"> A <see cref="XmlReader"/> at a toolshelf element. </param>
		protected abstract  void CreateToolShelf(XmlReader reader);

		/// <summary>
		/// Adds an action to the current toolshelf.
		/// The toolshelf element must be declared inside a toolshelf element.
		/// </summary>
		protected abstract void CreateToolItem(ActionAttribute action);

		/// <summary>
		/// Adds a separator to the current menu or toolbar.
		/// </summary>
		protected abstract void AddSeparator();

		/// <summary>
		/// Gets the tool position based on the position attribute of the current element.
		/// </summary>
		/// <param name="reader"> An XML reader.</param>
		/// <returns> The tool position.</returns>
		/// <remarks> An exception willl be raised if the attribute is not present or invalid.</remarks>
		protected ToolPosition GetToolPosition(XmlReader reader)
		{
			
			// get the toolbar position
			string posName = reader.GetAttribute("position");
			if (posName == null)
				throw new Exception("All toolbars must have a position attribute.");
			ToolPosition position;
			switch (posName)
			{
				case "Top":
					position = ToolPosition.Top;
					break;
				case "Bottom":
					position = ToolPosition.Bottom;
					break;
				case "Left":
					position = ToolPosition.Left;
					break;
				case "Right":
					position = ToolPosition.Right;
					break;
				default:
					throw new Exception(String.Format("The tool position {0} is invalid.", posName));
			}
			return position;
		}

        #endregion


        #region Document Types

        /// <summary>
        /// The types that can be instantiated as documents.
        /// </summary>
		protected Dictionary<string, DocumentType> documentTypes = new Dictionary<string, DocumentType>();

        /// <summary>
        /// Adds a document type.
        /// </summary>
        /// <param name="reader"></param>
        protected virtual void AddDocumentType(XmlReader reader)
        {
			string name = GetName(reader);
			string baseNamespace = reader.GetAttribute("baseNamespace");
			DocumentType documentType = new DocumentType(name, baseNamespace);

			// add the display name
			string displayName = reader.GetAttribute("displayName");
			if (displayName == null) // use the type name
				documentType.DisplayName = name;
			else
				documentType.DisplayName = displayName;

			// get the icon name
			string iconName = reader.GetAttribute("icon");
			if (iconName != null)
				documentType.IconName = iconName;

			documentTypes[name] = documentType;
        }

		/// <summary>
		/// Gets the document type for the given type name.
		/// </summary>
		/// <param name="typeName"> The name of the document type (declared in a DocumentType tag in the UI file).</param>
		/// <returns></returns>
		public DocumentType GetDocumentType(string typeName)
		{
			if (!documentTypes.ContainsKey(typeName))
				throw new Exception("The UI manager doesn't a document type definition for " + typeName);
			return documentTypes[typeName];
		}

		/// <summary>
		/// Document counters to keep track of how many documents of each type have been created.
		/// </summary>
		protected Dictionary<DocumentType, int> documentCounters = new Dictionary<DocumentType, int>();

		/// <summary>
		/// Creates a document of the given type.
		/// </summary>
		public virtual IDocument CreateDocument(DocumentType documentType)
		{
			// get the counter for this type
			if (!documentCounters.ContainsKey(documentType))
				documentCounters[documentType] = 0;
			documentCounters[documentType]++;

			return null;
		}

		/// <summary>
		/// Creates a document with the given type name.
		/// </summary>
		public IDocument CreateDocumentByName(string typeName)
		{
			return CreateDocument(GetDocumentType(typeName));
		}
		
        #endregion


		#region Dockables

		/// <summary>
		/// Create a dockable sizer.
		/// </summary>
		/// <param name="reader"></param>
		protected abstract void CreateDockableSizer(XmlReader reader);

		/// <summary>
		/// Create a dockable book.
		/// </summary>
		/// <param name="reader"></param>
		protected abstract void CreateDockableBook(XmlReader reader);

		/// <summary>
		/// Create a dockable.
		/// </summary>
		/// <param name="reader"></param>
		protected abstract void CreateDockable(XmlReader reader);

		/// <summary>
		/// Create the document area.
		/// </summary>
		/// <param name="reader"></param>
		protected abstract void CreateDocumentArea(XmlReader reader);

		#endregion

	}


}
