// HitLine.cs - MonoWorks Project
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

using MonoWorks.Base;

namespace MonoWorks.Rendering
{
	/// <summary>
	/// Represents a line through the view area that is the unprojection of a point on the screen.
	/// </summary>
	public class HitLine
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public HitLine()
		{

		}


		protected Coord screen;
		/// <summary>
		/// The screen position.
		/// </summary>
		public Coord Screen
		{
			get { return screen; }
			set { screen = value; }
		}

		protected Vector front = new Vector();
		/// <summary>
		/// The intersection at the front of the view frustrum.
		/// </summary>
		public Vector Front
		{
			get { return front; }
			set { front = value; }
		}

		protected Vector back = new Vector();
		/// <summary>
		/// The intersection at the back of the view frustrum.
		/// </summary>
		public Vector Back
		{
			get { return back; }
			set { back = value; }
		}


		protected Camera camera;
		/// <summary>
		/// The camera used during the hit.
		/// </summary>
		public Camera Camera
		{
			get { return camera; }
			set { camera = value; }
		}


		/// <summary>
		/// Gets the intersection of the hitline with a plane.
		/// </summary>
		/// <param name="plane"></param>
		/// <returns></returns>
		/// <remarks>Uses the formula found in the Wikipedia article 
		/// (http://en.wikipedia.org/wiki/Line-plane_intersection).</remarks>
		public Vector GetIntersection(Plane plane)
		{
			double d = plane.Center.ToVector().Dot(plane.Normal);
			double t = (d - front.Dot(plane.Normal)) / (back - front).Dot(plane.Normal);
			return (back - front) * t + front;
		}

	}
}
