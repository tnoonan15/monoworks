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
using System.Reflection;
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
		
		private string typeName;
		/// <value>
		/// The attribute's type.
		/// </value>
		public string TypeName
		{
			get {return typeName;}
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
				

#region Instantiation
		
		/// <value>
		/// The type of the attribute.
		/// </value>
		protected Type TheType
		{
			get
			{
				// look for a list
				bool isList = typeName.StartsWith("List(");
				string type_ = typeName;
				if (isList)
				{
					type_ = typeName.Substring(5, typeName.Length-6);
				}
				
				// load the assembly
				string[] typeComps = type_.Split('.');
				string asmName = "";
				for (int i=0; i<typeComps.Length-1; i++)
				{
					asmName += typeComps[i] + ".";
				}
				
				// get the type
				Type theType = null;
				if (type_.StartsWith("System"))
					theType = Type.GetType(type_, true);
				else
				{
					Assembly asm = Assembly.Load(asmName.Substring(0, asmName.Length - 1));
					theType = asm.GetType(type_, true);
				}
				
				return theType;
			}
		}
		
		/// <value>
		/// True if the attribute is an entity.
		/// </value>
		public bool IsEntity
		{
			get
			{
				Type theType = TheType;
				return theType.IsSubclassOf(typeof(MonoWorks.Model.Entity));
			}
		}
		
		
		/// <summary>
		/// Tries to instantiate an object of this attribute's class.
		/// </summary>
		/// <returns> An instance of the attribute. </returns>
		/// <remarks>Throws an exception if it can't make the instance. </remarks>
		public object Instantiate()
		{
			bool isList = typeName.StartsWith("List(");
			Type theType = TheType; 
			object obj = null;
			if (theType == typeof(System.String))
				return "";
			if (isList)
			{
				Type genListType = Type.GetType("System.Collections.Generic.List`1");
				Type listType = genListType.MakeGenericType(theType);
				obj = Activator.CreateInstance(listType);
			}
			else // not a list
			{
				obj = Activator.CreateInstance(theType);
			}
			return obj;
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
			typeName = reader.GetAttribute("type");
			reader.Read();
			description = reader.Value.Trim();
		}

#endregion
		
	}
}
