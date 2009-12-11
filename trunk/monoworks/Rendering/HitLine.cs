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

		/// <summary>
		/// The screen position.
		/// </summary>
		public Coord Screen { get; set; }

		/// <summary>
		/// The intersection at the Front of the view frustrum.
		/// </summary>
		public Vector Front { get; set; }

		/// <summary>
		/// The intersection at the Back of the view frustrum.
		/// </summary>
		public Vector Back { get; set; }

		/// <summary>
		/// The direction of the hit line.
		/// </summary>
		public Vector Direction
		{
			get { return Back - Front; }
		}


		public override string ToString()
		{
			return String.Format("HitLine: {0} to {1}", Front, Back);
		}

		/// <summary>
		/// The camera used during the hit.
		/// </summary>
		public Camera Camera { get; set; }


		/// <summary>
		/// Gets the intersection of the hitline with a plane.
		/// </summary>
		/// <param name="plane"></param>
		/// <returns></returns>
		/// <remarks>Uses the formula found in the Wikipedia article 
		/// (http://en.wikipedia.org/wiki/Line-plane_intersection).</remarks>
		public Vector GetIntersection(IPlane plane)
		{
			double d = plane.Origin.Dot(plane.Normal);
			double t = (d - Front.Dot(plane.Normal)) / (Back - Front).Dot(plane.Normal);
			return (Back - Front) * t + Front;
		}


		/// <summary>
		/// Computes the shortest distance between two lines.
		/// </summary>
		/// <remarks>Uses the forumlae from 
		/// http://pacificcoast.net/~cazelais/261/distance.pdf</remarks>
		public double ShortestDistance(HitLine other)
		{
			Vector n = Direction.Cross(other.Direction);
			if (n.Magnitude == 0)
				return 0;
			return Math.Abs((Front - other.Front).Dot(n) / n.Magnitude);
		}

	}
}
