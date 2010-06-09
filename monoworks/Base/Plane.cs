// 
//  Plane.cs - MonoWorks Project
//  
//  Author:
//       Andy Selvig <ajselvig@gmail.com>
// 
//  Copyright (c) 2010 Andy Selvig
// 
//  This library is free software; you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as
//  published by the Free Software Foundation; either version 2.1 of the
//  License, or (at your option) any later version.
// 
//  This library is distributed in the hope that it will be useful, but
//  WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
//  Lesser General Public License for more details.
// 
//  You should have received a copy of the GNU Lesser General Public
//  License along with this library; if not, write to the Free Software
//  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA

using System;

namespace MonoWorks.Base
{
	/// <summary>
	/// Simple implementation of an IPlane. Represents a plane in 3D space.
	/// </summary>
	public class Plane : IPlane
	{

		public Plane()
		{
			Origin = new Vector();
			Normal = new Vector();
			XAxis = new Vector();
		}
		
		/// <summary>
		/// The origin of the plane's coordinate system.
		/// </summary>
		public Vector Origin { get; set;}
		
		/// <summary>
		/// The normal vector for the plane.
		/// </summary>
		public Vector Normal { get; set; }
		
		/// <summary>
		/// The direction of the X axis for the plane's coordinate system.
		/// </summary>
		public Vector XAxis { get; set; }
	}
}

