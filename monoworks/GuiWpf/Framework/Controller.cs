using System;
using System.Collections.Generic;
using System.Windows.Controls;

using MonoWorks.Framework;

namespace MonoWorks.GuiWpf.Framework
{
	public class Controller : AbstractController
	{
		/// <summary>
		/// Base class for WPF Slate controllers.
		/// </summary>
		public Controller(SlateWindow window) : base()
		{
			this.window = window;

		}


		protected SlateWindow window;
		/// <summary>
		/// The window associated with this controller.
		/// </summary>
		public SlateWindow Window
		{
			get { return window; }
		}


	}
}
