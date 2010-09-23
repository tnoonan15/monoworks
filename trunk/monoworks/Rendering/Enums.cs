// Enums.cs - MonoWorks Project
//
//  Copyright (C) 2009 Andy Selvig
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

namespace MonoWorks.Rendering
{
	
	/// <summary>
	/// Locations for an anchor.
	/// </summary>
	[Flags]
	public enum AnchorLocation {None = 0, Center = 0, N = 1, E = 2, S = 4, W = 8, NE = 32, SE = 64, SW = 128, NW = 256 };
	
	/// <summary>
	/// The locations in a corner of a rectangle, subset of AnchorLocation.
	/// </summary>
	[Flags]
	public enum Corner {None = AnchorLocation.None,
						NE = AnchorLocation.NE,
						NW = AnchorLocation.NW,
						SE = AnchorLocation.SE,
						SW = AnchorLocation.SW};
	
	/// <summary>
	/// The locations along the sides of a rectangle, subset of AnchorLocation.
	/// </summary>
	[Flags]
	public enum Side { N = AnchorLocation.N, 
						E = AnchorLocation.E, 
						S = AnchorLocation.S, 
						W = AnchorLocation.W};

}
