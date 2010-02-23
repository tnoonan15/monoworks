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
using MonoWorks.Modeling;

namespace MonoWorks.Studio
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


		

#region File Actions

		[ActionHandler("New Part")]
		public void NewPart(object sender, EventArgs args)
		{
			var scene = Scene.AddDrawing(new TestPart());
			scene.Closing += OnSceneClosing;
		}

		/// <summary>
		/// The user is trying to close one of the scenes. 
		/// </summary>
		private void OnSceneClosing(object sender, SceneClosingEvent evt)
		{
			if (evt.Scene is DrawingScene) // don't know what to do for other scenes
			{
				var drawing = (evt.Scene as DrawingScene).Drawing;
				if (drawing.IsModified) // unsaved changes
				{
					evt.Cancel();
					var mb = MessageBox.Show(Scene, MessageBoxIcon.Info, 
						String.Format("Are you sure you want to close {0} without saving it?", drawing.Name),
						MessageBoxResponse.Cancel | MessageBoxResponse.CloseWithoutSaving);
					mb.Closed += delegate {
						if (mb.CurrentResponse == MessageBoxResponse.CloseWithoutSaving)
							evt.Scene.ForceClose();
					};
				}
			}
		}

		[ActionHandler("New Assembly")]
		public void NewAssembly(object sender, EventArgs args)
		{
			var scene = Scene.AddDrawing(new Assembly());
			scene.Closing += OnSceneClosing;
		}

		[ActionHandler()]
		public void Open(object sender, EventArgs args)
		{
			var def = new FileDialogDef() {
				Type = FileDialogType.Open,
				Title = "Select drawing to open"
			};
			def.Extensions.Add("mwp");
			def.Extensions.Add("mwa");
			if (Scene.Viewport.FileDialog(def))
				Console.WriteLine("open");
		}

		[ActionHandler()]
		public void Save(object sender, EventArgs args)
		{
			var current = Scene.GetCurrent();
			Console.WriteLine("save " + current.Name);
		}

		[ActionHandler("Save As")]
		public void SaveAs(object sender, EventArgs args)
		{
			SaveAs(Scene.GetCurrent());
		}
		
		/// <summary>
		/// Performs the "save as" operation on the given scene. 
		/// </summary>
		public void SaveAs(Scene scene)
		{
			var def = new FileDialogDef() {
				Type = FileDialogType.SaveAs,
				Title = "Select file name for drawing"
			};
			def.Extensions.Add("mwp");
			def.Extensions.Add("mwa");
			if (Scene.Viewport.FileDialog(def))
				Console.WriteLine("save as " + def.FileName);
		}

		[ActionHandler("Save All")]
		public void SaveAll(object sender, EventArgs args)
		{
			Console.WriteLine("save all");
		}

		[ActionHandler()]
		public void Close(object sender, EventArgs args)
		{
			Console.WriteLine("close");
		}

		[ActionHandler()]
		public void Quit(object sender, EventArgs args)
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
