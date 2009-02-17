using System;
using System.Collections.Generic;

using MonoWorks.Rendering.Events;

namespace MonoWorks.Rendering
{
	/// <summary>
	/// Interface for object that handle keyboard events.
	/// </summary>
	public interface IKeyHandler
	{

		void OnKeyPress(KeyEvent evt);

	}
}
