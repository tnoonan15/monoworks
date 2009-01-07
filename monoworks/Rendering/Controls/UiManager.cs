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

using MonoWorks.Base;
using MonoWorks.Framework;

namespace MonoWorks.Rendering.Controls
{
    /// <summary>
    /// Provides a UI manager implementation for a viewport using Rendering.Controls.
    /// </summary>
    public class UiManager : UiManagerBase
    {

        public UiManager(ViewportController controller)
            : base(controller)
        {

        }


        protected override UiMode Mode
        {
            get { throw new NotImplementedException(); }
        }

        protected override void EndElement(System.Xml.XmlReader reader)
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

        protected override void CreateToolbar(System.Xml.XmlReader reader)
        {
            throw new NotImplementedException();
        }

        protected override void CreateToolbarItem(ActionAttribute action)
        {
            throw new NotImplementedException();
        }

        protected override void CreateMenu(System.Xml.XmlReader reader)
        {
            throw new NotImplementedException();
        }

        protected override void CreateMenuItem(ActionAttribute action)
        {
            throw new NotImplementedException();
        }

        protected override void CreateToolBox(System.Xml.XmlReader reader)
        {
            throw new NotImplementedException();
        }

        protected override void CreateToolShelf(System.Xml.XmlReader reader)
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

        protected override void CreateDockableSizer(System.Xml.XmlReader reader)
        {
            throw new NotImplementedException();
        }

        protected override void CreateDockableBook(System.Xml.XmlReader reader)
        {
            throw new NotImplementedException();
        }

        protected override void CreateDockable(System.Xml.XmlReader reader)
        {
            throw new NotImplementedException();
        }

        protected override void CreateDocumentArea(System.Xml.XmlReader reader)
        {
            throw new NotImplementedException();
        }
    }
}
