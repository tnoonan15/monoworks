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

namespace MonoWorks.Model
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
			rgb = new byte[]{0, 0, 255};
		}
		
		/// <summary>
		/// Initialization constructor.
		/// </summary>
		/// <param name="red"> The red component. </param>
		/// <param name="green"> The green component. </param>
		/// <param name="blue"> The blue component. </param>
		public Color(byte red, byte green, byte blue)
		{
			rgb = new byte[]{red, green, blue};
		}
		
		
#region Components
		
		protected byte[] rgb;
		/// <value>
		/// The RGB components of the color.
		/// </value>
		public byte[] RGB
		{
			get {return rgb;}
			set
			{
				if (value.Length==3)
					rgb = value;
				else
					throw new Exception("RGB vectors must have 3 components.");
			}
		}
		
		/// <value>
		/// Returns an array of floats representing the red, green, and blue components. 
		/// </value>
		public float[] RGBf
		{
			get {return new float[]{(float)rgb[0]/255f, (float)rgb[1]/255f, (float)rgb[2]/255f};} 
		}
		
#endregion
		
		
#region OpenGL Commands
		
		/// <summary>
		/// Sets the color of the current OpenGL context.
		/// </summary>
		public void Setup()
		{
//			gl.glColor3bv(rgb); // this doesn't seem to work like it should
			gl.glColor3fv(RGBf);
		}
		
#endregion
		
		
		
	}
}
