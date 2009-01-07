// StyleGroup.cs - MonoWorks Project
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
using System.IO;
using System.Reflection;
using System.Xml;

using MonoWorks.Base;

namespace MonoWorks.Rendering.Controls
{
	
	/// <summary>
	/// Contains rendering style information for controls. 
	/// </summary>
	/// <remarks>It's basically a collection of StyleClasses.
	/// The caller asks for the class by name. This object 
	/// either gives them the corresponding class or the default one.
	/// </remarks>
	public class StyleGroup
	{
		
		public StyleGroup()
		{
			DefaultClass = new StyleClass();
		}

		/// <summary>
		/// Maps control type to style class.
		/// </summary>
		protected Dictionary<string, StyleClass> classes = new Dictionary<string, StyleClass>();

		/// <summary>
		/// The default class for this style group.
		/// </summary>
		public StyleClass DefaultClass
		{
			get { return classes["default"]; }
			set { classes["default"] = value; }
		}

		/// <summary>
		/// Adds a style class for the given class name.
		/// </summary>
		/// <param name="className"></param>
		/// <param name="styleClass"></param>
		public void AddClass(string className, StyleClass styleClass)
		{
			classes[className] = styleClass;
		}


		/// <summary>
		/// Gets the style class associated with the given class name.
		/// </summary>
		/// <remarks>Returns the default class if there isn't one associated with this name.</remarks>
		public StyleClass GetClass(string className)
		{
			if (classes.ContainsKey(className))
				return classes[className];
			else
				return DefaultClass;
		}

		
#region Default Style Group
		
		/// <summary>
		/// Loads the default style group from the assembly.
		/// </summary>
		/// <returns> </returns>
		protected static StyleGroup LoadDefault()
		{
			Assembly asm = Assembly.GetExecutingAssembly();
			Stream stream = asm.GetManifestResourceStream("DefaultStyles.xml");
			XmlReader reader = new XmlTextReader(stream);
			return FromXml(reader);
		}
		
		
		protected static StyleGroup defaultGroup = LoadDefault();
		/// <value>
		/// The default style group.
		/// </value>
		public static StyleGroup Default
		{
			get {return defaultGroup;}
		}
		
#endregion
		

#region Style Group

		/// <summary>
		/// Creates a style group from an XML reader at a StyleGroup element.
		/// </summary>
		/// <param name="reader"> </param>
		/// <returns> </returns>
		public static StyleGroup FromXml(XmlReader reader)
		{
			StyleGroup group = new StyleGroup();
			group.LoadXml(reader);
			return group;
		}

		/// <summary>
		/// Reads a style group in from an XML stream.
		/// </summary>
		/// <param name="reader"></param>
		public void LoadXml(XmlReader reader)
		{
			reader.Read();
			reader.ValidateElementName("StyleGroup");

			while (!reader.EOF)
			{
				reader.Read();
				if (reader.NodeType == XmlNodeType.Element)
				{
					if (reader.Name == "StyleClass") // add a new style class
					{
						AddClass(reader.GetRequiredString("name"), StyleClass.FromXml(reader));
					}
					else // invalid element
						throw new Exception("StyleGroup elements should only contain StyleClass elements.");
				}
			}
		}


#endregion
		

	}
}
