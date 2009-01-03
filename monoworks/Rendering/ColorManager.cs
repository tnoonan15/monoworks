// ColorManager.cs - MonoWorks Project
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
using System.Collections.Generic;
using System.Xml;
using System.Reflection;
using System.IO;



namespace MonoWorks.Rendering
{
	
	/// <summary>
	/// The color manager handles a list of colors for a document.
	/// </summary>
	public class ColorManager
	{
		
#region Singleton
		
		protected static ColorManager instance = new ColorManager(); 
		/// <summary>
		/// The singleton instance.
		/// </summary>
		public static ColorManager Global
		{
			get {return instance;}
		}
		
#endregion
		
		/// <summary>
		/// Default constructor.
		/// Reads Resources/DefaultColors.xml for the default colors.
		/// </summary>
		public ColorManager()
		{
			colors = new Dictionary<string,Color>();
			Assembly asm = Assembly.GetExecutingAssembly();
			bool loaded = false;
			foreach (string resName in asm.GetManifestResourceNames())
			{
				if (resName.Contains("DefaultColors"))
				{
					Load(asm.GetManifestResourceStream(resName));
					loaded = true;
				}
			}
			if (!loaded)
				throw new Exception("ColorManager couldn't find a DefaultColors.xml resource.");
		}
		

		
#region The Colors
		
		protected Dictionary<string, Color> colors;
		
		/// <summary>
		/// Returns the color of the given name.
		/// </summary>
		/// <param name="name"> The color's name. </param>
		/// <returns> A <see cref="Color"/> with the given name. </returns>
		public Color GetColor(string name)
		{
			if (colors.ContainsKey(name))
				return colors[name];
			else
				throw new Exception(name + " is not a valid color");
		}
		
		/// <value>
		/// Gets a color by name.
		/// </value>
		public Color this[string name]
		{
			get {return GetColor(name);}
		}
		
		/// <value>
		/// A list of names of the colors.
		/// </value>
		public List<string> Names
		{
			get
			{
				return new List<string>(colors.Keys);
			}
		}
		
		/// <summary>
		/// Returns true if the manager has the color of the given name.
		/// </summary>
		/// <param name="name"> </param>
		public bool HasColor(string name)
		{
			return colors.ContainsKey(name);
		}

#endregion
		
		
#region File I/O
		
		/// <summary>
		/// Loads colors from the specified file stream.
		/// </summary>
		public virtual void Load(Stream stream)
		{
			XmlReader reader = new XmlTextReader(stream);
			
			while (!reader.EOF) // while there's still something left to read
			{
				reader.Read(); // read the next node
				
				// decide what to do based on the node type
				switch (reader.NodeType)
				{
				case XmlNodeType.Element:
					// decide what to do based on the element name
					switch (reader.Name)
					{
					case "Color":
						ReadColor(reader);
						break;
					}
					break;
				} // node type
			}
		}
		
		/// <summary>
		/// Reads a color from an XML reader at a color tag.
		/// </summary>
		/// <param name="reader"> A <see cref="XmlReader"/> at a color tag. </param>
		protected virtual void ReadColor(XmlReader reader)
		{
			string name = (string)reader.GetAttribute("name");
			if (name==null)
				throw new Exception("All colors must have name attributes.");
			
			// parse the value
			string valueString = (string)reader.GetAttribute("value");
			if (valueString==null)
				throw new Exception("All colors must have value attributes.");
			if (valueString.Length != 6)
				throw new Exception(valueString + " is an invalid color value");
			byte red = Convert.ToByte(valueString.Substring(0, 2), 16);
			byte green = Convert.ToByte(valueString.Substring(2, 2), 16);
			byte blue = Convert.ToByte(valueString.Substring(4, 2), 16);
			
			colors[name] = new Color(red, green, blue);
			colors[name].Name = name;
		}
		
		
#endregion
		
	}
}
