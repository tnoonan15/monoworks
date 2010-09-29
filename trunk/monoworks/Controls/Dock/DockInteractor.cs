using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MonoWorks.Rendering;
using MonoWorks.Rendering.Interaction;

namespace MonoWorks.Controls.Dock
{
	/// <summary>
	/// Interactor for the DockSpace.
	/// </summary>
	/// <remarks>Probably not useful for much else.</remarks>
	public class DockInteractor : AbstractInteractor
	{

		public override void OnButtonPress(Rendering.Events.MouseButtonEvent evt)
		{
			base.OnButtonPress(evt);
		}

		public override void OnButtonRelease(Rendering.Events.MouseButtonEvent evt)
		{
			base.OnButtonRelease(evt);
		}

		public override void OnMouseMotion(Rendering.Events.MouseEvent evt)
		{
			base.OnMouseMotion(evt);
		}

	}
}
