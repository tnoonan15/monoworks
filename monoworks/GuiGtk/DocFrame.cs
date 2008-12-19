// DocFrame.cs - MonoWorks Project
//
//    Copyright Andy Selvig 2008
//
//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU Lesser General Public License as published 
//    by the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU Lesser General Public License for more details.
//
//    You should have received a copy of the GNU Lesser General Public 
//    License along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;

using MonoWorks.Model;

namespace MonoWorks.GuiGtk
{
	
	/// <summary>
	/// The frame containing a document and it's tree.
	/// </summary>
	public class DocFrame : Gtk.HPaned
	{
		
		public DocFrame() : base()
		{
			// create the tree
			treeView = new TreeView();
			Add1(treeView);
			
			// create the viewport			
			viewport = new Viewport();
			Add2(viewport);
		}
		
		
		protected Document document = null;
		/// <value>
		/// The document associated with this frame.
		/// </value>
		public Document Document
		{
			get {return document;}
			set
			{
				if (document != null)
					viewport.RenderList.RemoveRenderable(document);
				document = value;
				viewport.RenderList.AddRenderable(document);
//				treeModel.Document = document;
			}
		}

		protected Viewport viewport;
		/// <value>
		/// The viewport.
		/// </value>
		public Viewport Viewport
		{
			get {return viewport;}
		}
		
		
		protected TreeView treeView;
		/// <value>
		/// The tree view widget.
		/// </value>
		public TreeView Treeview
		{
			get {return treeView;}
			set {treeView = value;}
		}
	}
}
