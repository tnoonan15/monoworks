// DocumentBase.cs - Slate Mono Application Framework
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


namespace MonoWorks.GtkBackend.Framework.Dock
{
	
	/// <summary>
	/// Base class for document content.
	/// </summary>
	public class DocumentBase : DockableBase, IDocument
	{
		
		public DocumentBase() : base()
		{
		}
		

		/// <value>
		/// The document title.
		/// </value>
		public string Title
		{
			get {return Name;}
			set {Name = value;}
		}
		
		/// <value>
		/// Whether the document is the current one.
		/// </value>
		public bool IsCurrent
		{
			get
			{
				return true; // HACK: this will break with more than one document!
			}
		}
		
		
	}
}
