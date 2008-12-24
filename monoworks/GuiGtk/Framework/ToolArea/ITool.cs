// ITool.cs - Slate Mono Application Framework
//
//  Copyright (C) 2008 Andy Selvig
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

using MonoWorks.Framework;

namespace MonoWorks.GuiGtk.Framework.Tools
{
	/// <summary>
	/// Interface for Slate tools (widgets that can be docked to the edge of the window.
	/// </summary>
	public interface ITool
	{

		Gtk.Orientation Orientation { get; set; }

		ToolArea ToolArea { get; set; }

		bool ToolVisible {get; set;}

		ToolPosition LastPosition {get; set;}

	}
}
