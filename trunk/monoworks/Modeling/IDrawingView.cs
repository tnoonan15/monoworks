using System;
using System.Collections.Generic;

using MonoWorks.Framework;

namespace MonoWorks.Modeling
{
	/// <summary>
	/// Interface for documents containing drawings.
	/// </summary>
	public interface IDrawingView : IDocument
	{
		/// <summary>
		/// The drawing.
		/// </summary>
		Drawing Drawing { get; set; }

		/// <summary>
		/// Repaints the viewport.
		/// </summary>
		void Repaint();

	}
}
