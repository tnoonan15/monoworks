//   Plane.cs - MonoWorks Project
//
//    Copyright Andy Selvig 2008
//
//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU Lesser General Public License as published 
//    by the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU Lesser General Public License for more details.
//
//    You should have received a copy of the GNU Lesser General Public 
//    License along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;

namespace MonoWorks.Base
{
	
	/// <summary>
	/// The Plane class represents an infinite 2D plane in 3D space.
	/// </summary>
	public class Plane
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public Plane()
		{
		}

		
#region Geometry
			
		protected Point center;
		/// <value>
		/// A point that is intersected by the plane.
		/// </value>
		public Point Center
		{
			get {return center;}
			set { center = value;}
		}
		
		protected Vector normal;
		/// <value>
		/// The normal vector of the plane.
		/// </value>
		public Vector Normal
		{
			get {return normal;}
			set { normal = value;}
		}		
		
#endregion
		
		
	}
}
