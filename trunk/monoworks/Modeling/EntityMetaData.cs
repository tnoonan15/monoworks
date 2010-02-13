//    EntityMetaData.cs - MonoWorks Project
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
using System.IO;
using System.Xml;

using MonoWorks.Base;

namespace MonoWorks.Modeling
{
	
	/// <summary>
	/// Singleton class that stores meta data about entities.
	/// </summary>
	public class EntityMetaData
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public EntityMetaData(EntityMetaData parent)
		{
			this.parent = parent;
			children = new Dictionary<string,EntityMetaData>();
			attributes = new Dictionary<string,AttributeMetaData>();  
		}
		
		protected EntityMetaData parent;		
		
		/// <summary>
		/// The the top-level (Entity) instance.
		/// </summary>
		public static EntityMetaData TopLevel = CreateFromStream(ResourceHelper.GetStream("EntityMetaData.xml"));
		
		
		private string name;
		/// <value>
		/// The attribute's name.
		/// </value>
		public string Name
		{
			get {return name;}
		}
		
		
#region Children
		
		protected Dictionary<string, EntityMetaData> children;
		
		/// <summary>
		/// Gets the entity of a given name by recursively climbing the meta data tree.
		/// </summary>
		/// <param name="entityName"> The entity's name. </param>
		/// <returns> The <see cref="EntityMetaData"/> representing the entity. </returns>
		public EntityMetaData GetEntity(string entityName)
		{
			if (children.ContainsKey(entityName))
				return children[entityName];
			else
			{
				EntityMetaData data = null;
				foreach (EntityMetaData child in children.Values)
				{
					data = child.GetEntity(entityName);
					if (data != null)
						return data;
				}
			}
			return null;
		}
		
#endregion
		
		
		
#region Attributes

		protected Dictionary<string, AttributeMetaData> attributes;
		
		/// <value>
		/// Returns all attributes in a list.
		/// </value>
		public List<AttributeMetaData> AttributeList
		{
			get
			{
				List<AttributeMetaData> data = new List<AttributeMetaData>();
				if (parent != null)
					data.AddRange(parent.AttributeList);
				foreach (string key in attributes.Keys)
					data.Add(attributes[key]);
				return data;
			}
		}
		
		/// <summary>
		/// Wether the entity contains an attribute of the given name.
		/// </summary>
		/// <param name="name"> The name of the attribute. </param>
		public bool ContainsAttribute(string name)
		{
			foreach (AttributeMetaData attribute in AttributeList)
			{
				if (attribute.Name == name)
					return true;
			}
			return false;
		}

		/// <summary>
		/// Returns the attribute meta data for the given attribute name.
		/// </summary>
		/// <param name="name"></param>
		/// <returns>The attribute meta data, or null if the entity does not 
		/// have an attribute with the given name.
		/// There is no exception thrown since the caller should choose how
		/// to deal with this situation.</returns>
		public AttributeMetaData GetAttribute(string name)
		{
			foreach (AttributeMetaData attribute in AttributeList)
			{
				if (attribute.Name == name)
					return attribute;
			}
			return null;
		}
		
#endregion
		

#region File I/O
		
		/// <summary>
		/// Creates an instance from an XML file.
		/// </summary>
		/// <param name="fileName"> The file name. </param>
		/// <returns> A new <see cref="EntityMetaData"/>. </returns>
		public static EntityMetaData CreateFromFile(string fileName)
		{
			EntityMetaData data  = new EntityMetaData(null);
			data.FromFile(fileName);
			return data;
		}

		/// <summary>
		/// Creates an instance from an XML stream.
		/// </summary>
		/// <param name="stream"> The stream. </param>
		/// <returns> A new <see cref="EntityMetaData"/>. </returns>
		public static EntityMetaData CreateFromStream(Stream stream)
		{
			EntityMetaData data = new EntityMetaData(null);
			XmlReader reader = new XmlTextReader(stream);
			reader.Read();
			data.FromXML(reader);
			return data;
		}
		
		/// <summary>
		/// Loads the meta data from an XML file.
		/// </summary>
		/// <param name="fileName"> The file name. </param>
		public void FromFile(string fileName)
		{			
			XmlTextReader reader = new XmlTextReader(fileName);
			reader.Read();
			FromXML(reader);
			reader.Close();
		}
		
		/// <summary>
		/// Reads the meta data in from the specified XML reader.
		/// </summary>
		public void FromXML(XmlReader reader)
		{
			name = reader.GetAttribute("name");
			if (name == null)
				throw new Exception("All entity tags must have a name attribute.");
			
			while (!reader.EOF)
			{
				reader.Read();
				if (reader.NodeType == XmlNodeType.Element && reader.Name=="Attribute")
				{
					AttributeMetaData attribute = new AttributeMetaData();
					attribute.FromXML(reader);
					attributes[attribute.Name] = attribute;
				}
				else if (reader.NodeType == XmlNodeType.Element && reader.Name=="Entity")
				{
					EntityMetaData child = new EntityMetaData(this);
					child.FromXML(reader);
					children[child.Name] = child;
				}
				else if (reader.NodeType == XmlNodeType.EndElement && reader.Name=="Entity")
					break;
			}
		}

#endregion
		
	}
}
