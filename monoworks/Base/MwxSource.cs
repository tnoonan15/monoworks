// 
//  MwxSource.cs
//  
//  Author:
//       Andy Selvig <ajselvig@gmail.com>
// 
//  Copyright (c) 2009 Andy Selvig
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
using System.Linq;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Reflection;

namespace MonoWorks.Base
{
	/// <summary>
	/// The exception that gets thrown when a MwxSource can't figure out how to instantiate an element.
	/// </summary>
	public class InvalidMwxElementException : Exception
	{
		public InvalidMwxElementException(XmlReader reader, string details)
			: base(String.Format("Unable to resolve element {0} into a concrete object instance. {1}", reader.Name, details))
		{			
		}
	}
	
	public class UnknownObjectException : Exception
	{
		public UnknownObjectException(string name) 
			: base(String.Format("There is no object named {0} in the mwx source.", name))
		{
			
		}
	}

	/// <summary>
	/// Parses a mwx file and provides access to the objs declared inside of it.
	/// </summary>
	public class MwxSource
	{
		/// <summary>
		/// URI of the mwx namespace.
		/// </summary>
		public const string MwxUri = "http://monoworksproject.org/mwx";

		public MwxSource()
		{
		}
		
		/// <summary>
		/// Parses a mwx file.
		/// </summary>
		public MwxSource(string fileName) : this()
		{
			var reader = new XmlTextReader(fileName);
			Parse(reader);
			reader.Close();
		}
		
		/// <summary>
		/// Parses a mwx stream.
		/// </summary>
		public MwxSource(Stream stream) : this()
		{
			var reader = new XmlTextReader(stream);
			Parse(reader);
			reader.Close();
		}
		
		
		private Dictionary<string, IMwxObject> _objects = new Dictionary<string, IMwxObject>();
		
		/// <summary>
		/// Gets an object by name.
		/// </summary>
		/// <remarks>This isn't type safe and you should generally use Get<T> instead.</remarks>
		public IMwxObject Get(string name)
		{
			IMwxObject obj = null;
			if (_objects.TryGetValue(name, out obj))
				return obj;
			throw new UnknownObjectException(name);
		}
		
		/// <summary>
		/// Gets an object by name and performs type checking.
		/// </summary>
		public T Get<T>(string name) where T : IMwxObject
		{
			IMwxObject obj = null;
			if (_objects.TryGetValue(name, out obj))
			{
				if (obj is T)
					return (T)obj;
				throw new Exception(String.Format("MwxBase {0} is of type {1}, not {2}", 
				                                  name, obj.GetType(), typeof(T)));
			}
			throw new UnknownObjectException(name);
		}
		
		
		/// <summary>
		/// Gets all objects of the given type.
		/// </summary>
		public IEnumerable<T> GetAll<T>() where T : IMwxObject
		{
			return from o in _objects.Values
				where o is T
				select (T)o;
		}
		
		/// <summary>
		/// Gets raised whenever something is parsed by the mwx source.
		/// </summary>
		public event EventHandler ParseCompleted;
		
		/// <summary>
		/// Parses the mwx in a stream.
		/// </summary>
		public void Parse(Stream stream)
		{
			var reader = new XmlTextReader(stream);
			Parse(reader);
			reader.Close();
		}
		
		/// <summary>
		/// Parses a mwx source through a xml reader.
		/// </summary>
		public void Parse(XmlReader reader)
		{
			IMwxObject parent = null;
			while (!reader.EOF)
			{
				reader.Read();
				if (reader.NodeType == XmlNodeType.Element)
				{
					var isEmpty = reader.IsEmptyElement;
					
					// create the obj
					if (reader.LocalName == "Mwx") // ignore root element
						continue;
					var obj = CreateMwxBase(reader);
					var name = obj.Name;
					if (name != null)
						_objects[name] = obj;
					
					// add it to the current parent
					if (parent != null)
					{
						parent.AddChild(obj);
					}
					
					// make this the current parent
					if (!isEmpty)
						parent = obj;
				}
				else if (reader.NodeType == XmlNodeType.EndElement && parent != null)
				{
					parent = parent.Parent;
				}
				else if (reader.NodeType == XmlNodeType.Text)
				{
					// parse the element value, if possible
					var val = reader.Value.Trim();
					if (val != null && val.Length > 0)
					{
						var stringParsable = parent as IStringParsable;
						if (stringParsable != null)
							stringParsable.Parse(val);
						else
							throw new Exception(String.Format("Can't parse mwx element value into an instance of {0} since it's not an IStringParsable.", parent.GetType()));
					}
				}
			}
			
			if (ParseCompleted != null)
				ParseCompleted(this, new EventArgs());
		}
				
		/// <summary>
		/// Gets the fully qualified class name associated with a mwx element.
		/// </summary>
		private string GetElementClassName(XmlReader reader)
		{
			if (reader.NamespaceURI.StartsWith(MwxUri)) // this is a MonoWorks class
			{
				var asm = reader.NamespaceURI.Replace(MwxUri, "MonoWorks").Replace('/', '.');
				return String.Format("{0}.{1},{0}", asm, reader.LocalName);
			}
			else
				throw new InvalidMwxElementException(reader, "Haven't implemented non-MonoWorks classes in mwx yet.");
		}
		
		/// <summary>
		/// Creates an object based on the current mwx element.
		/// </summary>
		private IMwxObject CreateMwxBase(XmlReader reader)
		{
			// get the type
			var className = GetElementClassName(reader);
			Type type = null;
			try
			{
				type = Type.GetType(className, true);
			}
			catch (Exception ex)
			{
				throw new InvalidMwxElementException(reader, ex.Message);
			}
			
			// verify it's an object
			if (!type.Implements(typeof(IMwxObject)))
				throw new InvalidMwxElementException(reader, "Type does not implement MonoWorks.Base.IMwxObject.");
			
			// instantiate the object
			var obj = Activator.CreateInstance(type) as IMwxObject;
			
			// populate the obj
			AssignProperties(obj, reader);
			
			return obj;
		}
		
		/// <summary>
		/// Populates the obj based on a mwx stream.
		/// </summary>
		private void AssignProperties(IMwxObject obj, XmlReader reader)
		{
			foreach (var prop in reader.GetProperties())
			{
				// handle Action assignment separately
				if (prop.Key == "Action")
				{
					if (!obj.GetType().Implements(typeof(IActionPopulatable)))
						throw new Exception(obj.GetType() + " does not implement IActionPopulatable");
					var action = Get<UiAction>(prop.Value);
					((IActionPopulatable)obj).Populate(action);
					return;
				}
				
				// handle it as a property
				var propInfo = obj.GetType().GetProperty(prop.Key);
				if (propInfo == null)
					throw new Exception(String.Format("No property named {0} for type {1}", prop.Key, obj.GetType()));
				if (propInfo.GetCustomAttributes<MwxPropertyAttribute>().Length == 0)
					throw new Exception(String.Format("Property {0} for type {1} is not a MwxProperty", prop.Key, obj.GetType()));
				propInfo.SetFromString(obj, prop.Value);
			}
			
		}
		
		
		
	}
}
