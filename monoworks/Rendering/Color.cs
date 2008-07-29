//   Color.cs - MonoWorks Project
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

using gl = Tao.OpenGl.Gl;

namespace MonoWorks.Rendering
{
	
	/// <summary>
	/// The color class represents a single color.
	/// </summary>
	public class Color
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public Color()
		{
			rgba = new byte[]{0, 0, 0, 255};
		}
		
		/// <summary>
		/// Initialization constructor.
		/// </summary>
		/// <param name="red"> The red component. </param>
		/// <param name="green"> The green component. </param>
		/// <param name="blue"> The blue component. </param>
		public Color(byte red, byte green, byte blue)
		{
			rgba = new byte[]{red, green, blue, 255};
		}
		
		/// <summary>
		/// Initialization constructor.
		/// </summary>
		/// <param name="red"> The red component. </param>
		/// <param name="green"> The green component. </param>
		/// <param name="blue"> The blue component. </param>
		/// <param name="alpha"> The alpha component. </param>
		public Color(byte red, byte green, byte blue, byte alpha)
		{
			rgba = new byte[]{red, green, blue, alpha};
		}


		/// <summary>
		/// Float initialization constructor.
		/// </summary>
		/// <param name="red"> The red component. </param>
		/// <param name="green"> The green component. </param>
		/// <param name="blue"> The blue component. </param>
		/// <param name="alpha"> The alpha component. </param>
		public Color(float red, float green, float blue) : this(red, green, blue, 1F)
		{

		}



		/// <summary>
		/// Float initialization constructor with alpha.
		/// </summary>
		/// <param name="red"> The red component. </param>
		/// <param name="green"> The green component. </param>
		/// <param name="blue"> The blue component. </param>
		/// <param name="alpha"> The alpha component. </param>
		public Color(float red, float green, float blue, float alpha)
		{
			rgba = new byte[] { (byte)(red * 255f), (byte)(green * 255f), (byte)(blue * 255f), (byte)(alpha * 255f) };
		}
		
#region Components
		
		protected byte[] rgba;
		/// <value>
		/// The RGB components of the color.
		/// </value>
		public byte[] RGBA
		{
			get {return rgba;}
			set
			{
				if (value.Length==4)
					rgba = value;
				else
					throw new Exception("RGB vectors must have 4 components.");
			}
		}
		
		/// <value>
		/// Returns an array of floats representing the red, green, and blue components. 
		/// </value>
		public float[] RGBf
		{
			get {return new float[]{(float)rgba[0]/255f, (float)rgba[1]/255f, (float)rgba[2]/255f};} 
		}
		
		/// <value>
		/// Returns an array of floats representing the red, green, blue, and alpha components. 
		/// </value>
		public float[] RGBAf
		{
			get {return new float[]{(float)rgba[0]/255f, (float)rgba[1]/255f, (float)rgba[2]/255f, (float)rgba[3]/255f};} 
		}
		
		/// <summary>
		/// Returns true if the color is opaque.
		/// </summary>
		public bool IsOpaque()
		{
			if (rgba[3]<255)
				return false;
			return true;
		}

		/// <summary>
		/// Access a component of the color as a float.
		/// </summary>
		/// <param name="index"> 0-3 corresponding to rgba.</param>
		/// <returns> The given component.</returns>
		public float this[int index]
		{
			get
			{
				if (index < 0 || index > 3)
					throw new Exception("Color component must be between 0 and 3");
				return (float)rgba[index] / 255f;
			}
			set
			{
				if (index < 0 || index > 3)
					throw new Exception("Color component must be between 0 and 3");
				rgba[index] = (byte)(value * 255f);
			}
		}

		/// <summary>
		/// The red component.
		/// </summary>
		public float Redf
		{
			get { return this[0]; }
			set { this[0] = value; }
		}

		/// <summary>
		/// The green component.
		/// </summary>
		public float Greenf
		{
			get { return this[1]; }
			set { this[1] = value; }
		}

		/// <summary>
		/// The blue component.
		/// </summary>
		public float Bluef
		{
			get { return this[2]; }
			set { this[2] = value; }
		}

		/// <summary>
		/// The alpha component.
		/// </summary>
		public float Alphaf
		{
			get { return this[3]; }
			set { this[3] = value; }
		}
	
		
#endregion
		
		
#region OpenGL Commands
		
		/// <summary>
		/// Sets the color of the current OpenGL context.
		/// </summary>
		public void Setup()
		{
//			gl.glColor3bv(rgb); // this doesn't seem to work like it should
			gl.glColor4fv(RGBAf);
		}
		
#endregion
		
		
		
	}
}
