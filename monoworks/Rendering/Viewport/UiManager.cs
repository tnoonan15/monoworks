// UiManager.cs - MonoWorks Project
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

using MonoWorks.Base;
using MonoWorks.Framework;
using MonoWorks.Rendering.Controls;

namespace MonoWorks.Rendering.Viewport
{
    /// <summary>
    /// Provides a UI manager implementation for a viewport using Rendering.Controls.
    /// </summary>
    public class UiManager : UiManagerBase
    {

        public UiManager(ViewportController controller)
            : base(controller)
        {	
			this.controller = controller;
        }

		protected ViewportController controller;

        protected override UiMode Mode
        {
			get
			{
				if (currentToolbar != null)
					return UiMode.Toolbar;
				return UiMode.None;
			}
        }

        protected override void EndElement(XmlReader reader)
        {
			
			
        }

        protected override bool CreateToolsMenu
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

		
#region Toolbars
		
		protected Dictionary<string, ToolBar> toolbars = new Dictionary<string, ToolBar>();
		
		protected ToolBar currentToolbar = null;
		
		/// <summary>
		/// Gets the toolbar of the given name.
		/// </summary>
		public ToolBar GetToolbar(string name)
		{
			if (!toolbars.ContainsKey(name))
				throw new Exception("There is no toolbar named " + name);
			return toolbars[name];
		}
		
        protected override void CreateToolbar(XmlReader reader)
        {
			string name = GetName(reader);
			currentToolbar = new ToolBar();
			toolbars[name] = currentToolbar;

			// try to get button style
			string styleString = reader.GetAttribute("buttonStyle");
			if (styleString != null)
				currentToolbar.ButtonStyle = (ButtonStyle)Enum.Parse(typeof(ButtonStyle), styleString);

        }

        protected override void CreateToolbarItem(ActionAttribute action)
        {
			// try to get the icon
			Image icon = null;
			Button button;
			if (action.IconName != null)
			{
				icon = new Image(ResourceHelper.GetStream(action.IconName + ".png"));
				button = new Button(action.Name, icon);
			}
			else
				button = new Button(action.Name);

			currentToolbar.Add(button);
			button.Clicked += delegate(object sender, EventArgs args)
			{
				action.MethodInfo.Invoke(controller, null);
			};
        }
		
#endregion


#region ContextLayer Creation

		/// <summary>
		/// Creates a context layer and puts all toolbars into it.
		/// </summary>
		/// <returns></returns>
		public ContextLayer CreateContextLayer()
		{
			ContextLayer layer = new ContextLayer();
			foreach (string name in toolbars.Keys)
				layer.AddToolbar(name, toolbars[name]);
			return layer;
		}


#endregion



		#region Not Used

		protected override void CreateMenu(XmlReader reader)
        {
            throw new NotImplementedException();
        }

        protected override void CreateMenuItem(ActionAttribute action)
        {
            throw new NotImplementedException();
        }

        protected override void CreateToolBox(XmlReader reader)
        {
            throw new NotImplementedException();
        }

        protected override void CreateToolShelf(XmlReader reader)
        {
            throw new NotImplementedException();
        }

        protected override void CreateToolItem(ActionAttribute action)
        {
            throw new NotImplementedException();
        }

        protected override void AddSeparator()
        {
            throw new NotImplementedException();
        }

        protected override void CreateDockableSizer(XmlReader reader)
        {
            throw new NotImplementedException();
        }

        protected override void CreateDockableBook(XmlReader reader)
        {
            throw new NotImplementedException();
        }

        protected override void CreateDockable(XmlReader reader)
        {
            throw new NotImplementedException();
        }

        protected override void CreateDocumentArea(XmlReader reader)
        {
            throw new NotImplementedException();
		}

#endregion

	}
}
