//    AttributeMetaData.cs - MonoWorks Project
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
using System.Collections.Generic;
using System.Xml;

using MonoWorks.Base;

namespace MonoWorks.Model
{
	
	/// <summary>
	/// Store meta data for an entity's attribute.
	/// </summary>
	public class AttributeMetaData
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public AttributeMetaData()
		{
		}
		
		
#region The Data		
		
		private string name;
		/// <value>
		/// The attribute's name.
		/// </value>
		public string Name
		{
			get {return name;}
		}
		
		private string type;
		/// <value>
		/// The attribute's type.
		/// </value>
		public string Type
		{
			get {return type;}
		}
		
		private string description;
		/// <value>
		/// The attribute's description.
		/// </value>
		public string Description
		{
			get {return description;}
		}		
		
#endregion
		

#region File I/O
		
		/// <summary>
		/// Reads the meta data in from the specified XML file.
		/// </summary>
		/// <param name="fileName"> The file name. </param>
		public void FromXML(XmlReader reader)
		{
			name = reader.GetAttribute("name");
			type = reader.GetAttribute("type");
			reader.Read();
			description = reader.Value.Trim();
		}

#endregion
		
	}
}
