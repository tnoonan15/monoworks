// XmlExtensions.cs - MonoWorks Project
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
using System.Xml;

namespace MonoWorks.Base
{
	/// <summary>
	/// Exception for an invalid XML element name.
	/// </summary>
	public class InvalidElementExcepion : Exception
	{
		/// <summary>
		/// Construct the exception for an element of the given name.
		/// </summary>
		/// <param name="name"></param>
		public InvalidElementExcepion(string name) : base("Expecting element named " + name)
		{
		}
	}
	
	/// <summary>
	/// Exception for missing required attribute.
	/// </summary>
	public class MissingAttributeExcepion : Exception
	{
		/// <summary>
		/// Construct the exception for an attribute of the given name.
		/// </summary>
		public MissingAttributeExcepion(string elemName, string attrName) : 
			base("Expecting attribute named " + attrName + " in element " + elemName)
		{
		}
	}
	
	
	/// <summary>
	/// Extensions for XML reading and writing.
	/// </summary>
	public static class XmlExtensions
	{
		/// <summary>
		/// Validates that the reader is at an element with the given name.
		/// </summary>
		/// <param name="reader"> </param>
		/// <param name="name"> The element name. </param>
		/// <exception cref="InvalidElementExcepion"></exception>
		public static void ValidateElementName(this XmlReader reader, string name)
		{
			if (reader.Name != name)
				throw new InvalidElementExcepion(name);
		}
		
		/// <summary>
		/// Gets a required string attribute from a reader.
		/// </summary>
		/// <param name="reader"> </param>
		/// <param name="name"> The name of the attribute. </param>
		/// <returns> The value of the attribute. </returns>
		/// <exception cref="MissingAttributeExcepion"></exception>
		public static string GetRequiredString(this XmlReader reader, string name)
		{
			string attr = reader.GetAttribute(name);
			if (attr == null)
				throw new MissingAttributeExcepion(reader.Name, name);
			return attr;
		}
		
	}
}
