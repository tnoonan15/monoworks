// StudioController.cs - MonoWorks Project
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
using System.Xml;

using MonoWorks.Base;
using MonoWorks.Rendering;
using MonoWorks.Controls;

namespace MonoWorks.Modeling
{
	/// <summary>
	/// Controller for drawings.
	/// </summary>
	public class StudioController : AbstractController<StudioScene>
	{

		public StudioController(StudioScene scene)
			: base(scene)
		{
			Mwx.Parse(ResourceHelper.GetStream("Studio.mwx"));
						
			Scene.AddToGutter(Side.N, Mwx.Get<ToolBar>("FileToolbar"));
		}


//		protected DocumentManager<IDrawingView> drawingManager = new DocumentManager<IDrawingView>();



#region Key Press Handling

		public override void OnKeyPress(int key)
		{
			base.OnKeyPress(key);

//			uiManager.HandleKeyPress(key);
		}

#endregion


#region File Actions

		[ActionHandler("New Part")]
		public void NewPart()
		{
//			IDrawingView view = uiManager.CreateDocumentByName("PartView") as IDrawingView;
//			drawingManager.Add(view);
		}

		[ActionHandler("New Assembly")]
		public void NewAssembly()
		{
//			IDrawingView view = uiManager.CreateDocumentByName("AssemblyView") as IDrawingView;
//			drawingManager.Add(view);
		}

		[ActionHandler()]
		public void Open()
		{
			Console.WriteLine("open");
		}

		[ActionHandler()]
		public void Save()
		{
			Console.WriteLine("save");
		}

		[ActionHandler("Save As")]
		public void SaveAs()
		{
			Console.WriteLine("save as");
		}

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

		[ActionHandler()]
		public void Undo()
		{
//			if (drawingManager.Count > 0)
//			{
//				Console.WriteLine("undo {0}", drawingManager.Current.Title);
//				drawingManager.Current.Drawing.Undo();
//				drawingManager.Current.Repaint();
//			}
		}

		[ActionHandler()]
		public void Redo()
		{
//			if (drawingManager.Count > 0)
//			{
//				Console.WriteLine("redo {0}", drawingManager.Current.Title);
//				drawingManager.Current.Drawing.Redo();
//				drawingManager.Current.Repaint();
//			}
		}

		[ActionHandler()]
		public void Cut()
		{
			Console.WriteLine("cut");
		}

		[ActionHandler()]
		public void Copy()
		{
			Console.WriteLine("copy");
		}

		[ActionHandler()]
		public void Paste()
		{
			Console.WriteLine("paste");
		}


#endregion

	}
}
