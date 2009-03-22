// SourceDocument.cs - MonoWorks Project
//
//  Copyright (C) 2009 Andy Selvig
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
using System.Collections.Generic;
using System.Windows.Forms.Integration;

using ScintillaNet;

using MonoWorks.GuiWpf.Framework;
using MonoWorks.Workbench;

namespace MonoWorks.WorkbenchWpf
{

	/// <summary>
	/// Possible types of source files.
	/// </summary>
	public enum SourceType { Boo, CSharp };


	/// <summary>
	/// Base class for source code documents.
	/// </summary>
	public abstract class SourceDocument : DocumentBase, ISourceDocument
	{

		public SourceDocument(SourceType type)
		{
			host = new WindowsFormsHost();
			Content = host;

			editor = new Scintilla();
			host.Child = editor;
		}

		/// <summary>
		/// The type of source code contained in the document.
		/// </summary>
		public SourceType SourceType { get; private set; }

		/// <summary>
		/// Windows forms integration host.
		/// </summary>
		private WindowsFormsHost host;

		protected Scintilla editor;

	}
}
