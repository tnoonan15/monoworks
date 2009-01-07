using System;
using System.Collections.Generic;

using System.Windows.Forms.Integration;

using MonoWorks.Rendering;
using System.Windows;

namespace MonoWorks.GuiWpf
{
	/// <summary>
	/// Wraps the SWF viewport into WPF.
	/// </summary>
	public class ViewportWrapper : WindowsFormsHost
	{

		public ViewportWrapper()
			: base()
		{
			viewport = new SwfViewport();
			Child = viewport;
		}


		private SwfViewport viewport;
		/// <summary>
		/// The underlying viewport.
		/// </summary>
		public IViewport Viewport
		{
			get { return viewport; }
		}

		protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
		{
			base.OnRenderSizeChanged(sizeInfo);

			viewport.ResizeGL();
		}

	}


}
