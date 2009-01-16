using System;
using System.Collections.Generic;

using MonoWorks.Framework;

namespace MonoWorks.Model
{
	/// <summary>
	/// Base controller class for MonoWorks.
	/// </summary>
	public class ModelController : AbstractController
	{

		public ModelController()
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


#region File Actions

		[Action("New Part")]
		public void NewPart()
		{
			uiManager.CreateDocumentByName("PartView");
		}

		[Action("New Assembly")]
		public void NewAssembly()
		{
			uiManager.CreateDocumentByName("AssemblyView");
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
			Console.WriteLine("undo");
		}

		[Action()]
		public void Redo()
		{
			Console.WriteLine("redo");
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
