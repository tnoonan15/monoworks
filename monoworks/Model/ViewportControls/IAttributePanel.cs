using System;
using System.Collections.Generic;

using MonoWorks.Model;

namespace MonoWorks.Model.ViewportControls
{

	public delegate void AttributePanelHandler(IAttributePanel panel);

	public interface IAttributePanel
	{
		/// <summary>
		/// Show the panel with the given entity.
		/// </summary>
		void Show(Controller controller, Entity entity);

		/// <summary>
		/// Hide the panel.
		/// </summary>
		void Hide();

	}
}
