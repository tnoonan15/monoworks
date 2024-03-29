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
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Text.RegularExpressions;

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
			writer.WriteStartDocument();
			
			writer.WriteStartElement("mwx:Mwx");
			writer.WriteAttributeString("xmlns:mwx", MwxSource.MwxUri);
			foreach (var kv in _namespaceAbbrevs)
			{
				writer.WriteAttributeString("xmlns:" + kv.Value, MwxSource.MwxUri + "/" + kv.Key);
			}
			
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
		/// Maps assembly names to namespace abbreviations.
		/// </summary>
		private static Dictionary<string,string> _namespaceAbbrevs = new Dictionary<string, string>() {
			{"Base", "mwb"},
			{"Rendering", "mwr"},
			{"Controls", "mwc"},
			{"Modeling", "mwm"}
		};
		
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
			
			// replace namespace designation
			foreach (var kv in _namespaceAbbrevs)
			{
				var asm = "MonoWorks." + kv.Key + ".";
				if (typeName.Contains(asm))
					return typeName.Replace(asm, kv.Value + ":");
			}
			return typeName.Replace("MonoWorks.", "mwx:");
		}
		
		/// <summary>
		/// Writes the given object to the xml stream.
		/// </summary>
		private void WriteObject(XmlWriter writer, IMwxObject obj)
		{
			
			// replace generic declarations
			var typeName = GetLocalName(obj.GetType());
			if (typeName.Contains("`"))
			{
				var genericType = typeName.Split('[').Last();
				genericType = genericType.Substring(0, genericType.Length-1);
				typeName = typeName.Split('`')[0];
				writer.WriteStartElement(typeName);
				writer.WriteAttributeString("GenericType", genericType);
			}
			else // non-generic
			{
				writer.WriteStartElement(typeName);
			}
				
			
			// retrieve the mwx properties
			var mwxProps = obj.GetMwxProperties();
			
			// write the attribute properties
			var attrProps = from prop in mwxProps
				where prop.Type == MwxPropertyType.Attribute
				select prop;
			foreach (var prop in attrProps)
			{
				var val = prop.PropertyInfo.GetValue(obj, new object[] {  });
				var valString = "";
				if (val != null)
					valString = val.ToString();
				writer.WriteAttributeString(prop.Name, valString);
			}
			
			// write the reference properties
			var refProps = from prop in mwxProps
				where prop.Type == MwxPropertyType.Reference
				select prop;
			foreach (var prop in refProps) {
				var val = prop.PropertyInfo.GetValue(obj, new object[] {  });
				if (val is IMwxObject)
					writer.WriteAttributeString(prop.Name, (val as IMwxObject).Name);
				else
					throw new NotImplementedException("Don't know how to reference non-mwx objects like " + prop.PropertyInfo.PropertyType);
			}
			
			// write the child properties
			var excludeChildren = new List<IMwxObject>();
			var childProps = from prop in mwxProps
				where prop.Type == MwxPropertyType.Child
				select prop;
			foreach (var prop in childProps) {
				var val = prop.PropertyInfo.GetValue(obj, new object[] {  });
				if (val is IMwxObject)
				{
					writer.WriteStartElement("MwxProperty");
					writer.WriteAttributeString("Name", prop.Name);
					WriteObject(writer, val as IMwxObject);
					writer.WriteEndElement();
					excludeChildren.Add(val as IMwxObject);
				}
				else
					throw new NotImplementedException("Child properties need to implement IMwxObject, unlike " + prop.PropertyInfo.PropertyType);
			}
			
			// write the mwx children
			foreach (var child in obj.GetMwxChildren())
			{
				if (child is IMwxObject)
				{
					var mwxChild = child as IMwxObject;
					if (excludeChildren.Contains(mwxChild))
						continue;
					WriteObject(writer, child);
				}
				else
					throw new NotImplementedException("Can't write children that don't implement IMwxObject, like " + child);
			}
			
			writer.WriteEndElement();
		}
		
	}
}

