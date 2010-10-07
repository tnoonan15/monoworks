// SceneExtensions.cs - MonoWorks Project
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
using System.Linq;
using System.Text;

using MonoWorks.Base;
using MonoWorks.Rendering;

namespace MonoWorks.Controls
{
	/// <summary>
	/// Extension methods relating to scenes.
	/// </summary>
	public static class SceneExtensions
	{

		/// <summary>
		/// Pane to write textual labels to.
		/// </summary>
		private static Label _tooltipLabel;

		static SceneExtensions()
		{
			_tooltipLabel = new Label() {
				BackgroundColor = Color.LightGray
			};
		}

		/// <summary>
		/// Sets the tooltip to a flat string.
		/// </summary>
		public static void SetToolTip(this Scene scene, string content, bool followCursor)
		{
			_tooltipLabel.Body = content;
			scene.SetToolTip(_tooltipLabel, followCursor);
		}
	}
}
