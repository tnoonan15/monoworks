using System;
using System.Collections.Generic;

using MonoWorks.Framework;

namespace MonoWorks.Model
{
	/// <summary>
	/// Base controller class for MonoWorks.
	/// </summary>
	public class MainController : AbstractController
	{

		public MainController()
			: base()
		{


		}


		private UiManagerBase uiManager;

		/// <summary>
		/// Set the ui manager.
		/// </summary>
		/// <param name="uiManager"></param>
		protected void SetUiManager(UiManagerBase uiManager)
		{
			this.uiManager = uiManager;
		}

		protected DocumentManager<IDrawingView> drawingManager = new DocumentManager<IDrawingView>();



#region Key Press Handling

		public override void OnKeyPress(int key)
		{
			base.OnKeyPress(key);

			uiManager.HandleKeyPress(key);
		}

#endregion


#region File Actions

		[Action("New Part")]
		public void NewPart()
		{
			IDrawingView view = uiManager.CreateDocumentByName("PartView") as IDrawingView;
			drawingManager.Add(view);
		}

		[Action("New Assembly")]
		public void NewAssembly()
		{
			IDrawingView view = uiManager.CreateDocumentByName("AssemblyView") as IDrawingView;
			drawingManager.Add(view);
		}

		[Action()]
		public void Open()
		{
			Console.WriteLine("open");
		}

		[Action()]
		public void Save()
		{
			Console.WriteLine("save");
		}

		[Action("Save As")]
		public void SaveAs()
		{
			Console.WriteLine("save as");
		}

		[Action()]
		public void Close()
		{
			Console.WriteLine("close");
		}

		[Action()]
		public void Quit()
		{
			Console.WriteLine("quit");
		}


#endregion


#region Edit Actions

		[Action()]
		public void Undo()
		{
			if (drawingManager.Count > 0)
			{
				Console.WriteLine("undo {0}", drawingManager.Current.Title);
				drawingManager.Current.Drawing.Undo();
				drawingManager.Current.Repaint();
			}
		}

		[Action()]
		public void Redo()
		{
			if (drawingManager.Count > 0)
			{
				Console.WriteLine("redo {0}", drawingManager.Current.Title);
				drawingManager.Current.Drawing.Redo();
				drawingManager.Current.Repaint();
			}
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
