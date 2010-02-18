// WorkbenchController.cs - MonoWorks Project
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

using MonoWorks.Framework;
using MonoWorks.Workbench;

namespace MonoWorks.Workbench
{
	/// <summary>
	/// The base class for the workbench controllers.
	/// </summary>
	public abstract class WorkbenchController : AbstractController
	{

		private UiManagerBase uiManager;

		/// <summary>
		/// Set the ui manager.
		/// </summary>
		/// <param name="uiManager"></param>
		protected void SetUiManager(UiManagerBase uiManager)
		{
			this.uiManager = uiManager;
		}



#region Documents

		/// <summary>
		/// The document manager.
		/// </summary>
		protected DocumentManager<ISourceDocument> documentManager = new DocumentManager<ISourceDocument>();

#endregion


#region File Actions

		[ActionHandler]
		public void New()
		{
			ISourceDocument view = uiManager.CreateDocumentByName("BooDocument") as ISourceDocument;
			documentManager.Add(view);
		}

		[ActionHandler()]
		public abstract void Open();

		[ActionHandler()]
		public abstract void Save();

		[ActionHandler("Save As")]
		public abstract void SaveAs();

		[ActionHandler()]
		public void Close()
		{
			Console.WriteLine("close");
		}

		[ActionHandler()]
		public void Quit()
		{
			Console.WriteLine("quit");
		}


		#endregion


		#region Edit Actions

		[Action()]
		public void Undo()
		{
			//if (documentManager.Count > 0)
			//{
			//    Console.WriteLine("undo {0}", documentManager.Current.Title);
			//    documentManager.Current.Drawing.Undo();
			//    documentManager.Current.Repaint();
			//}
		}

		[Action()]
		public void Redo()
		{
			//if (documentManager.Count > 0)
			//{
			//    Console.WriteLine("redo {0}", documentManager.Current.Title);
			//    documentManager.Current.Drawing.Redo();
			//    documentManager.Current.Repaint();
			//}
		}

		[Action()]
		public void Cut()
		{
			Console.WriteLine("cut");
		}

		[Action()]
		public void Copy()
		{
			Console.WriteLine("copy");
		}

		[Action()]
		public void Paste()
		{
			Console.WriteLine("paste");
		}


		#endregion
	}
}
