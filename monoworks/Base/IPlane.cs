// IPlane.cs - MonoWorks Project
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

namespace MonoWorks.Base
{

	/// <summary>
	/// Interface for classes that act like Planes.
	/// </summary>
	/// <remarks>See MonoWorks.Base.Plane for a simple concrete implementation.</remarks>
	public interface IPlane
	{
		
		/// <value>
		/// A point that is intersected by the plane.
		/// </value>
		Vector Origin {get; set;}
		
		/// <value>
		/// The normal vector of the plane.
		/// </value>
		Vector Normal {get; set;}
		
		/// <value>
		/// The x axis in the plane's coordinate system.
		/// </value>
		Vector XAxis {get; set;}
	}
	
	
	/// <summary>
	/// Extension methods for IPlane.
	/// </summary>
	public static class IPlaneExtensions
	{
		/// <summary>
		/// Computes the y axis of the loca coordinate system.
		/// </summary>
		public static Vector YAxis(this IPlane plane)
		{
			return plane.Normal.Cross(plane.XAxis).Normalize();
		}
		
		
		/// <summary>
		/// Projects point onto the plane.
		/// </summary>
		public static Coord Project(this IPlane plane, Vector point)
		{
			var diff = point - plane.Origin; // point relative to the plane's origin
			var x = diff.Dot(plane.XAxis); // x projection
			var y = diff.Dot(plane.YAxis()); // y projection
			return new Coord(x, y);
		}
	}
}
