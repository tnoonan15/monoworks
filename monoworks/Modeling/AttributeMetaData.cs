// AttributeMetaData.cs - MonoWorks Project
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
using System.Collections.Generic;
using System.Reflection;
using System.Xml;

using MonoWorks.Base;

namespace MonoWorks.Modeling
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


		public string NonCollectionTypeName
		{
			get
			{
				// look for a list
				if (IsList)
					return typeName.Substring(5, typeName.Length - 6);
				return typeName;
			}
		}

		/// <summary>
		/// Returns true if the attribute is a list type.
		/// </summary>
		public bool IsList
		{
			get { return typeName.StartsWith("List("); }
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

		/// <summary>
		/// Cache the types so we don't have to load them from the assemblies each time.
		/// </summary>
		private static Dictionary<string, Type> typeCache = new Dictionary<string, Type>();

		/// <value>
		/// The type of the attribute.
		/// </value>
		protected Type TheType
		{
			get
			{
				string type_ = NonCollectionTypeName;

				// get the type
				Type theType = null;
				if (type_.StartsWith("System"))
					theType = Type.GetType(type_, true);
				else
				{	
					// look in the type cache
					if (typeCache.TryGetValue(type_, out theType))
						return theType;

					// find the type in the currently loaded assemblies
					foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
					{
						if (type_.StartsWith(asm.GetName().Name))
							theType = asm.GetType(type_);
					}
					if (theType == null)
						throw new Exception("Could no find type " + typeName);
					typeCache[type_] = theType; // cache the type
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
				return theType.IsSubclassOf(typeof(MonoWorks.Modeling.Entity));
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
		/// Reads the meta data in from the specified XML reader.
		/// </summary>
		/// <param name="reader"> The XML reader. </param>
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
