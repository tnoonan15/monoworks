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
			adapter = new SwfViewportAdapter();
			Child = adapter;
		}


		private SwfViewportAdapter adapter;
		/// <summary>
		/// The underlying viewport.
		/// </summary>
		public Viewport Viewport
		{
			get { return adapter.Viewport; }
		}

		protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
		{
			base.OnRenderSizeChanged(sizeInfo);

			adapter.ResizeGL();
		}

	}


}
