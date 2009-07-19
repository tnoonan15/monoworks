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
	public enum AnchorLocation { N, E, S, W, NE, SE, SW, NW };
	
	/// <summary>
	/// A corner of the viewport, subset of AnchorLocation.
	/// </summary>
	public enum Corner {NE = AnchorLocation.NE,
						NW = AnchorLocation.NW,
						SE = AnchorLocation.SE,
						SW = AnchorLocation.SW};
	
	/// <summary>
	/// The anchor locations available for contexts, subset of AnchorLocation.
	/// </summary>
	public enum ContextLocation { N = AnchorLocation.N, 
								E = AnchorLocation.E, 
								S = AnchorLocation.S, 
								W = AnchorLocation.W};
}
