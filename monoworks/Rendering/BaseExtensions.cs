// BaseExtensions.cs - MonoWorks Project
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
using System.Collections.Generic;

using gl = Tao.OpenGl.Gl;

using MonoWorks.Base;

namespace MonoWorks.Rendering
{
	/// <summary>
	/// Extension methods for objects in the MonoWorks.Base namespace.
	/// </summary>
	public static class BaseExtensions
	{
		/// <summary>
		/// Makes an OpenGL vertex at the coordinate.
		/// </summary>
		/// <param name="coord"> </param>
		public static void glVertex(this Coord coord)
		{
			gl.glVertex2d(coord.X, coord.Y);
		}

		/// <summary>
		/// Makes an OpenGL vertex at the vector.
		/// </summary>
		/// <param name="vec"> </param>
		public static void glVertex(this Vector vec)
		{
			gl.glVertex3d(vec.X, vec.Y, vec.Z);
		}

		/// <summary>
		/// Makes an OpenGL normal with the vector.
		/// </summary>
		/// <param name="vec"> </param>
		public static void glNormal(this Vector vec)
		{
			gl.glNormal3d(vec.X, vec.Y, vec.Z);
		}

		/// <summary>
		/// Makes an OpenGL vertex at the point.
		/// </summary>
		/// <param name="point"></param>
		public static void glVertex(this Point point)
		{
			gl.glVertex3d(point[0].Value, point[1].Value, point[2].Value);
		}


	}
}
