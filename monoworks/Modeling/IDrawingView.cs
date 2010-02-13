using System;
using System.Collections.Generic;


namespace MonoWorks.Modeling
{
	/// <summary>
	/// Interface for documents containing drawings.
	/// </summary>
	public interface IDrawingView
	{
		/// <summary>
		/// The drawing.
		/// </summary>
		Drawing Drawing { get; set; }

		/// <summary>
		/// Repaints the scene.
		/// </summary>
		void Repaint();

	}
}
