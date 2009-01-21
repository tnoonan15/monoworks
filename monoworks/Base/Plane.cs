// Plane.cs - MonoWorks Project
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



#region String Conversion


		public override string ToString()
		{
			return Center.ToString() + Normal.ToString(); ;
		}


#endregion


	}
}
