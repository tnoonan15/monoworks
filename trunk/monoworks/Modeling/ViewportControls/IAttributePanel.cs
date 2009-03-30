using System;
using System.Collections.Generic;

using MonoWorks.Modeling;

namespace MonoWorks.Modeling.ViewportControls
{

	public delegate void AttributePanelHandler(IAttributePanel panel);

	public interface IAttributePanel
	{
		/// <summary>
		/// Show the panel with the given entity.
		/// </summary>
		void Show(DrawingController controller, Entity entity);

		/// <summary>
		/// Hide the panel.
		/// </summary>
		void Hide();

	}
}
