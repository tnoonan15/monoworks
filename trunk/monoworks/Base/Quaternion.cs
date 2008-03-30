// Quaternion.cs - MonoWorks Project
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
	/// A Quaternion is a magical 4D vector that does all sorts of awesome stuff.
	/// </summary>
	public class Quaternion
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public Quaternion()
		{
			vector = new Vector();
			scalar = 0.0;
		}
		
		/// <summary>
		/// Initialization constructor.
		/// </summary>
		/// <param name="x"> The i component. </param>
		/// <param name="y"> The j component. </param>
		/// <param name="z"> The k component. </param>
		/// <param name="t"> The scalar component. </param>
		public Quaternion(double t, double x, double y, double z)
		{
			scalar = t;
			vector = new Vector( x, y, z);
		}
		
		/// <summary>
		/// Initialization constructor.
		/// </summary>
		/// <param name="t">The scalar component. </param>
		/// <param name="v"> The vector component.  </param>
		public Quaternion(double t, Vector v)
		{
			scalar = t;
			vector = v;
		}
		

#region Value
		
		private double scalar;
		/// <summary>
		/// Access the scalar component.
		/// </summary>
		public double Scalar
		{
			get {return scalar;}
			set {scalar = value;}
		}
		
		private Vector vector;
		/// <value>
		/// Access the vector component.
		/// </value>
		public Vector Vector
		{
			get {return vector;}
			set {vector = value;}
		}
		
				
#endregion


#region Math
		
		/// <summary>
		/// Computes the transpose (conjugate) of the quaternion.
		/// </summary>
		public Quaternion T()
		{
			return new Quaternion(scalar, vector*-1);
		}
		
#endregion
		
		
		
#region Operator Overloading
		
		/// <summary>
		/// Addition operator.
		/// </summary>
		/// <param name="q1"> The lhs operand. </param>
		/// <param name="q2"> The rhs operand. </param>
		/// <returns> A new <see cref="Quaternion"/> that is the sum of q1 and q2. </returns>
		public static Quaternion operator+(Quaternion q1, Quaternion q2)
		{
			return new Quaternion(q1.Scalar + q2.Scalar, q1.Vector + q2.Vector); 
		}
		
		/// <summary>
		/// Subtraction operator.
		/// </summary>
		/// <param name="q1"> The lhs operand. </param>
		/// <param name="q2"> The rhs operand. </param>
		/// <returns> A new <see cref="Quaternion"/> that is the difference of q1 and q2. </returns>
		public static Quaternion operator-(Quaternion q1, Quaternion q2)
		{
			return new Quaternion(q1.Scalar - q2.Scalar, q1.Vector - q2.Vector); 
		}
		
		/// <summary>
		/// Multiplication operator.
		/// </summary>
		/// <param name="q1"> The lhs operand. </param>
		/// <param name="q2"> The rhs operand. </param>
		/// <returns> A new <see cref="Quaternion"/> that is the product of q1 and q2. </returns>
		public static Quaternion operator*(Quaternion q1, Quaternion q2)
		{
			Quaternion ret = new Quaternion();
			ret.Scalar = q1.Scalar * q2.Scalar - q1.Vector.Dot(q2.Vector);
			ret.Vector = q2.Vector * q1.Scalar + q1.Vector * q2.Scalar + q1.Vector.Cross(q2.Vector);
			return ret;
		}
		
#endregion
		
		
	}
}
