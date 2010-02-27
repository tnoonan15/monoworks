// 
//  MwxWriter.cs - MonoWorks Project
//  
//  Author:
//       Andy Selvig <ajselvig@gmail.com>
// 
//  Copyright (c) 2010 Andy Selvig
// 
//  This library is free software; you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as
//  published by the Free Software Foundation; either version 2.1 of the
//  License, or (at your option) any later version.
// 
//  This library is distributed in the hope that it will be useful, but
//  WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
//  Lesser General Public License for more details.
// 
//  You should have received a copy of the GNU Lesser General Public
//  License along with this library; if not, write to the Free Software
//  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;

namespace MonoWorks.Base
{
	/// <summary>
	/// Writes a collection of Mwx objects to an XML stream.
	/// </summary>
	/// <remarks>
	/// Basic usage is:
	/// var writer = new MwxWriter();
	/// writer.Add(someObject);
	/// writer.Write(fileName);
	/// </remarks>
	public class MwxWriter
	{
		public MwxWriter()
		{
		}
		
		private List<IMwxObject> _objects = new List<IMwxObject>();
		
		/// <summary>
		/// Adds an object to the writer.
		/// </summary>
		public void Add(IMwxObject obj)
		{
			_objects.Add(obj);
		}
		
		/// <summary>
		/// Writes the objects to the given xml writer.
		/// </summary>
		public void Write(XmlWriter writer)
		{
			writer.WriteStartElement("mwx:Mwx");
			writer.WriteAttributeString("xmlns:mwx", MwxSource.MwxUri);
			
			foreach (var obj in _objects)
			{
				WriteObject(writer, obj);
			}
			writer.WriteEndElement();
		}
		
		/// <summary>
		/// Writes the objects to a file with the given name.
		/// </summary>
		public void Write(string fileName)
		{
			XmlTextWriter writer = new XmlTextWriter(fileName, System.Text.Encoding.ASCII);
			writer.Formatting = Formatting.Indented;
			Write(writer);
			writer.Close();
		}
		
		/// <summary>
		/// Gets the local mwx name for the given type.
		/// </summary>
		/// <example>
		/// GetLocalName(typeof(MonoWorks.Base.Vector)) return mwx:Base.Vector.
		/// </example>
		private static string GetLocalName(Type type)
		{
			var typeName = type.ToString();
			if (!typeName.StartsWith("MonoWorks"))
				throw new Exception(typeName + " is not a MonoWorks type. Can't handle non-MonoWorks types yet.");
			return typeName.Replace("MonoWorks.", "mwx:");
		}
		
		/// <summary>
		/// Writes the given object to the xml stream.
		/// </summary>
		private void WriteObject(XmlWriter writer, IMwxObject obj)
		{
			writer.WriteStartElement(GetLocalName(obj.GetType()));
			
			// write the properties
			foreach (var prop in obj.GetType().GetProperties()) {
				var mwxProps = prop.GetCustomAttributes<MwxPropertyAttribute>();
				if (mwxProps.Length > 0) {
					var mwxProp = mwxProps[0];
					mwxProp.PropertyInfo = prop;
					if (mwxProp.Name == null)
						mwxProp.Name = prop.Name;
					WriteProperty(writer, obj, mwxProp);
				}
			}
			
			// write the mwx children
			foreach (var child in obj.GetMwxChildren())
				WriteObject(writer, child);
			
			writer.WriteEndElement();
		}
		
		/// <summary>
		/// Writes the given property to the xml writer.
		/// </summary>
		private void WriteProperty(XmlWriter writer, IMwxObject obj, MwxPropertyAttribute prop)
		{
			var val = prop.PropertyInfo.GetValue(obj, new object[] {  });
			string valString = val.ToString();
			
//			if (prop.PropertyInfo.PropertyType == typeof(int))
//			{
//				valString = ((int)val);
//			}
			
			writer.WriteAttributeString(prop.Name, valString);
		}
		
	}
}

