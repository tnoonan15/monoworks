// Matrix.cs - MonoWorks Project
//
// Copyright (C) 2008 Andy Selvig
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
	/// The Matrix class represents a 3x3 matrix.
	/// </summary>
	public class Matrix
	{

		/// <summary>
		/// Default constructor.
		/// </summary>
		public Matrix()
		{
			val = new double[3,3];
		}
		
		/// <summary>
		/// Initialization constructor.
		/// </summary>
		public Matrix(double a11, double a12, double a13,
		              double a21, double a22, double a23, 
		              double a31, double a32, double a33)
		{
			val = new double[3,3]{{a11, a21, a32}, {a21, a22, a23}, {a31, a32, a33}};
		}
		

#region Value
		
		private double[,] val;
		
		/// <summary>
		/// Access individual values.
		/// </summary>
		public double this[int i, int j]
		{
			get {return val[i,j];}
			set {val[i,j] = value;}
		}
		
#endregion
		
		
#region Operator Overloading
		
		/// <summary>
		/// Performs matrix multiplication on a vector.
		/// </summary>
		/// <param name="m"> The <see cref="Matrix"/>. </param>
		/// <param name="v"> The <see cref="Vector"/>. </param>
		/// <returns> The resulting <see cref="Vector"/>. </returns>
		public static Vector operator*(Matrix m, Vector v)
		{
			Vector ret = new Vector();
			for (int i=0; i<3; i++)
			{
				double row = 0;
				for (int j=0; j<3; j++)
				{
					row += (m[i,j] * v[j]);					
				}
				ret[i] = row;
			}
			return ret;
		}
		
#endregion
		
	}
}
