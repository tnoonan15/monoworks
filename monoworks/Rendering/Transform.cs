// Transform.cs - MonoWorks Project
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
	/// This class represents a transformation (scaling and offset) in 3D space.
	/// </summary>
	public class Transform
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public Transform()
		{
			Offset = new Vector();
			Scaling = new Vector();
		}


		#region Attributes

		protected Vector Offset;
		/// <summary>
		/// The offset (applied before the scaling).
		/// </summary>
		public Vector Offset { get; set; }

		/// <summary>
		/// The scaling (applied after the offset).
		/// </summary>
		public Vector Scaling {get; set;}

		#endregion


		/// <summary>
		/// Computes the transform to go from bounds1 to bounds2.
		/// </summary>
		/// <param name="bounds1"></param>
		/// <param name="bounds2"></param>
		public void Compute(Bounds bounds1, Bounds bounds2)
		{
			Scaling = bounds2.Size / bounds1.Size;
			Offset = (bounds2.Minima - bounds1.Minima*Scaling);
		}


		/// <summary>
		/// Applies the transform to the vector.
		/// </summary>
		/// <param name="vector"></param>
		/// <returns> The resulting vector.</returns>
		/// <remarks> This is basically scaling*(vector+offset). </remarks>
		public Vector Apply(Vector vector)
		{
			return Scaling * vector + Offset;
		}

		/// <summary>
		/// Applies the inverse of the tranform to a vector.
		/// </summary>
		/// <param name="vector"> A <see cref="Vector"/>. </param>
		/// <returns> The tranformed <see cref="Vector"/>. </returns>
		public Vector InverseApply(Vector vector)
		{
			return (vector - Offset) / Scaling;
		}

		/// <summary>
		/// Applies the transformation to x, y, z.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="z"></param>
		public void Apply(ref double x, ref double y, ref double z)
		{
			x = Scaling[0] * x + Offset[0];
			y = Scaling[1] * y + Offset[1];
			z = Scaling[2] * z + Offset[2];
		}

		/// <summary>
		/// Applies the inverse transformation to x, y, z.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="z"></param>
		public void InverseApply(ref double x, ref double y, ref double z)
		{
			x = (x - Offset[0]) / Scaling[0];
			y = (y - Offset[1]) / Scaling[1];
			z = (z - Offset[2]) / Scaling[2];
		}

		/// <summary>
		/// Applies the transformation to a single dimension.
		/// </summary>
		/// <param name="val"> The value to transform.</param>
		/// <param name="dim"> The dimension.</param>
		/// <returns> The transformed value. </returns>
		public double Apply(double val, int dim)
		{
			if (dim < 0 || dim > 2)
				throw new Exception("Dimension is invalid.");

			return Scaling[dim] * val + Offset[dim];
		}

	}
}
