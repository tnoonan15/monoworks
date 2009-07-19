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
using System.Xml;
using System.Xml.Serialization;

using gl = Tao.OpenGl.Gl;

using MonoWorks.Base;

namespace MonoWorks.Rendering
{
	
	
	public class InvalidColorStringException : Exception
	{
		public InvalidColorStringException(string cString) : base("Color string " + cString + " is invalid." + 
@"Valid color strings must have 3, 4, 6, or 8 characters. All characters must be valid hex (0-9, a-f).")
		{
		}
	}
	
	
	/// <summary>
	/// The color class represents a single color.
	/// </summary>
	public class Color : IFill
	{
			
		/// <value>
		/// The color's name.
		/// </value>
		[XmlAttribute]
		public string Name { get; set; }
		
		
		public object Clone ()
		{
			Color color = new Color();
			color.rgba = rgba;
			color.Name = Name;
			return color;
		}


		public override string ToString()
		{
			if (Name == null) // not named
				return string.Format("{0:x2}{1:x2}{2:x2}{3:x2}", rgba[0], rgba[1], rgba[2], rgba[3]);
			else // named
				return Name;
		}
		
		
#region Constructors		
		
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
		public Color(byte red, byte green, byte blue) : this(red, green, blue, 255)
		{
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
		
#endregion
		
		
#region HSV Conversions
		
		/// <summary>
		/// Creates a color from the given hsv values.
		/// </summary>
		/// <param name="h"> The hue component. </param>
		/// <param name="s"> The saturation component. </param>
		/// <param name="v"> The value component. </param>
		/// <returns> A <see cref="Color"/>. </returns>
		public static Color FromHsv(float h, float s, float v)
		{
			Color color = new Color();
			color.SetHsv(h, s, v);
			return color;
		}
		
		/// <summary>
		/// Sets the HSV values (performs automatic conversion to RGB).
		/// </summary>
		/// <param name="h"> The hue component. </param>
		/// <param name="s"> The saturation component. </param>
		/// <param name="v"> The value component. </param>
		public void SetHsv(float h, float s, float v)
		{
			int hi = (int)Math.Floor(h/60f) % 6;
			float f = h/60f - (float)Math.Floor(h/60f);
			float p = v * (1-s);
			float q = v * (1-f*s);
			float t = v * (1 - (1 - f) * s);
			
			switch (hi)
			{
			case 0:
				RGBf = new float[]{v,t,p};
				break;
			case 1:
				RGBf = new float[]{q,v,p};
				break;
			case 2:
				RGBf = new float[]{p,v,t};
				break;
			case 3:
				RGBf = new float[]{p,q,v};
				break;
			case 4:
				RGBf = new float[]{t,p,v};
				break;
			case 5:
				RGBf = new float[]{v,p,q};
				break;
			}
		}
		
#endregion
		
		
#region Components
		
		protected byte[] rgba;
		/// <value>
		/// The RGB components of the color.
		/// </value>
		[XmlAttribute]
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
			set
			{
				if (value.Length != 3)
					throw new Exception("RGB vectors must have 3 components.");
				for (int i=0; i<value.Length; i++)
					this[i] = value[i];				
			}
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
		public bool IsOpaque
		{
			get
			{
				if (rgba[3] < 255)
					return false;
				return true;
			}
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


#region Other Operations

		/// <summary>
		/// Returns the "inverse" color.
		/// </summary>
		public Color Inverse
		{
			get { return new Color(Greenf, Bluef, Redf); }
		}

		/// <summary>
		/// Interpolates the two colors linearly.
		/// </summary>
		/// <param name="c1"></param>
		/// <param name="c2"></param>
		/// <returns></returns>
		public static Color Interp(Color c1, Color c2)
		{
			return new Color((c1.Redf + c2.Redf) / 2, (c1.Greenf + c2.Greenf) / 2, (c1.Bluef + c2.Bluef) / 2);
		}
		
		/// <value>
		/// Gets the color as a Cairo color.
		/// </value>
		public Cairo.Color Cairo
		{
			get {return new Cairo.Color(Redf, Greenf, Bluef, Alphaf);}
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

		/// <summary>
		/// Draws a solid rectangle at the given position with the given size.
		/// </summary>
		public void DrawRectangle(Coord pos, Coord size)
		{
			Setup();
			gl.glBegin(gl.GL_QUADS);
			gl.glVertex2d(pos.X, pos.Y);
			gl.glVertex2d(pos.X + size.X, pos.Y);
			gl.glVertex2d(pos.X + size.X, pos.Y + size.Y);
			gl.glVertex2d(pos.X, pos.Y + size.Y);
			gl.glEnd();
		}
		
#endregion
		
		
#region XML Loading
		
		/// <summary>
		/// Creates a color from cString.
		/// </summary>
		/// <param name="cString"> A string with rgb or rgba components 
		/// defined in one or two character hex (i.e. 08f, a452f0, or f008). </param>
		/// <returns> </returns>
		public static Color FromString(string cString)
		{
			byte red = 0;
			byte green = 0;
			byte blue = 0;
			byte alpha = 255;
			
			// check for valid string length
			switch (cString.Length)
			{
			case 3:
			case 4:
				red = Convert.ToByte(cString.Substring(0, 1) + "F", 16);
				green = Convert.ToByte(cString.Substring(1, 1) + "F", 16);
				blue = Convert.ToByte(cString.Substring(2, 1) + "F", 16);
				if (cString.Length == 4) // there's an alpha component					
					alpha = Convert.ToByte(cString.Substring(3, 1) + "F", 16);
				break;
				
			case 6:
			case 8:
				red = Convert.ToByte(cString.Substring(0, 2), 16);
				green = Convert.ToByte(cString.Substring(2, 2), 16);
				blue = Convert.ToByte(cString.Substring(4, 2), 16);
				if (cString.Length == 8) // there's an alpha component					
					alpha = Convert.ToByte(cString.Substring(6, 2), 16);
				break;
				
			default:
				throw new InvalidColorStringException(cString);
			}
			
			return new Color(red, green, blue, alpha);
		}
		
		
		/// <summary>
		/// Loads a color from an XML Color element.
		/// </summary>
		/// <param name="reader"> </param>
		/// <returns> </returns>
		/// <remarks>The element can contain a name attribute specifying the name
		/// of the default color to use, or a valid rgb attribute with the components.</remarks>
		public static Color FromXml(XmlReader reader)
		{
			// try to find global color
			string name = reader.GetAttribute("name");
			if (name != null && ColorManager.Global.HasColor(name))
				return ColorManager.Global[name];				
			
			// try to read as hex values
			return FromString(reader.GetRequiredString("value"));
		}
		
#endregion


	}
}
