using System.Windows.Forms.Integration;

using MonoWorks.Rendering;
using System.Windows;

using MonoWorks.Winforms.Backend;

namespace MonoWorks.Wpf.Backend
{
	/// <summary>
	/// Wraps the SWF viewport into WPF.
	/// </summary>
	public class ViewportWrapper : WindowsFormsHost
	{

		public ViewportWrapper()
		{
			_adapter = new ViewportAdapter();
			Child = _adapter;
		}


		private readonly ViewportAdapter _adapter;
		/// <summary>
		/// The underlying viewport.
		/// </summary>
		public Viewport Viewport
		{
			get { return _adapter.Viewport; }
		}

		protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
		{
			base.OnRenderSizeChanged(sizeInfo);

			_adapter.ResizeGL();
		}

	}


}
