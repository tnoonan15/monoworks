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
using System.Reflection;
using System.Xml;

using MonoWorks.Base;

namespace MonoWorks.Modeling
{
	
	/// <summary>
	/// Singleton class that stores meta data about entities.
	/// </summary>
	public class EntityMetaData
	{	
		
		public EntityMetaData(Type type)
		{
			_children = new Dictionary<string, EntityMetaData>();
			_attributes = new Dictionary<string, MwxPropertyAttribute>();
			foreach (var prop in type.GetProperties())
			{
				var mwxProps = prop.GetCustomAttributes<MwxPropertyAttribute>();
				if (mwxProps.Length > 0) 
				{
					var mwxProp = mwxProps[0];
					mwxProp.PropertyInfo = prop;
					if (mwxProp.Name == null)
						mwxProp.Name = prop.Name;
					_attributes[mwxProp.Name] = mwxProp;
				}
			}
		}
				
				
		#region Children
		
		protected Dictionary<string, EntityMetaData> _children;
		
		/// <summary>
		/// Gets the entity of a given name by recursively climbing the meta data tree.
		/// </summary>
		/// <param name="entityName"> The entity's name. </param>
		/// <returns> The <see cref="EntityMetaData"/> representing the entity. </returns>
		public EntityMetaData GetEntity(string entityName)
		{
			if (_children.ContainsKey(entityName))
				return _children[entityName];
			else
			{
				EntityMetaData data = null;
				foreach (EntityMetaData child in _children.Values)
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

		protected Dictionary<string, MwxPropertyAttribute> _attributes;
		
		/// <value>
		/// Returns all attributes in a list.
		/// </value>
		public IEnumerable<MwxPropertyAttribute> Attributes
		{
			get { return _attributes.Values; }
		}
		
		/// <summary>
		/// Wether the entity contains an attribute of the given name.
		/// </summary>
		/// <param name="name"> The name of the attribute. </param>
		public bool ContainsAttribute(string name)
		{
			foreach (var attribute in Attributes)
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
		public MwxPropertyAttribute GetAttribute(string name)
		{
			foreach (var attribute in Attributes)
			{
				if (attribute.Name == name)
					return attribute;
			}
			return null;
		}
		
		#endregion
				
	
		
	}
}
