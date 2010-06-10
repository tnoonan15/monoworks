// SceneInfoOverlay.cs - MonoWorks Project
//
//  Copyright (C) 2010 Andy Selvig
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
//
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA 

using System;
using System.Collections.Generic;
using System.Diagnostics;

using MonoWorks.Base;
using MonoWorks.Rendering;

namespace MonoWorks.Controls
{
	/// <summary>
	/// An overlay that sits in the corner and displays information about the scene.
	/// </summary>
	public class SceneInfoOverlay : AnchorPane
	{
		public SceneInfoOverlay(Scene scene)
			: base(AnchorLocation.SE)
		{
			_label = new Label();
			Control = _label;

			_fpsAverager = new RunningAverager(50);
			_stopwatch = new Stopwatch();
		}

		private Label _label;

		private RunningAverager _fpsAverager;

		private Stopwatch _stopwatch;

		/// <summary>
		/// The scene for which the info is being displayed.
		/// </summary>
		public Scene Scene { get; set; }


		public override void RenderOverlay(Scene scene)
		{
			// compute the current frame rate
			if (_stopwatch.IsRunning)
			{
				var fps = 1.0 / _stopwatch.Elapsed.TotalSeconds;
				_stopwatch.Reset();
				_fpsAverager.Add(fps);
			}
			else
			{
				_stopwatch.Start();
			}

			_label.Body = String.Format("{0:###.#} fps", _fpsAverager.Compute());
			OnSceneResized(scene);

			base.RenderOverlay(scene);
		}

	}
}
