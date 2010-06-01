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
		public InvalidMwxElementException(string typeName, string details)
			: base(String.Format("Unable to resolve element {0} into a concrete object instance. {1}", typeName, details))
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
		/// Name of the current child property being parsed.
		/// </summary>
		private string _childPropertyName = null;

		/// <summary>
		/// Stores reference properties for resolution at the end of parsing.
		/// </summary>
		private List<ReferenceProperty> _refProps = new List<ReferenceProperty>();
		
		/// <summary>
		/// Keeps track of a count for each type name to use for automatic name generation.
		/// </summary>
		private Dictionary<string,int> _nameCounts = new Dictionary<string, int>();
		
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
					if (reader.LocalName == "Mwx")
						// ignore root element
						continue;
					if (reader.LocalName == "MwxProperty")
					{
						// parse as a child property
						_childPropertyName = reader.GetAttribute("Name");
						if (_childPropertyName == null)
							throw new Exception("MwxProperty elements need to have a Name attribute");
						reader.Read();
						while (reader.NodeType != XmlNodeType.Element)
							reader.Read();
					}
					else
						_childPropertyName = null;
					
					// create the object
					var obj = CreateMwxObject(reader);
					
					// get or generate its name
					if (obj.Name == null) {
						GenerateName(obj);
					}
					_objects[obj.Name] = obj;
					
					// add it to the current parent
					if (parent != null)
					{
						if (_childPropertyName == null) {
							parent.AddChild(obj);
							obj.Parent = parent;
						}
						else
							SetProperty(parent, _childPropertyName, obj);
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
			
			// resolve reference properties
			foreach (var prop in _refProps)
			{
				var obj = Get(prop.ReferenceName);
				prop.Resolve(obj);
			}
			_refProps.Clear();
			
			if (ParseCompleted != null)
				ParseCompleted(this, new EventArgs());
		}
				
		/// <summary>
		/// Gets the fully qualified class name associated with a mwx element.
		/// </summary>
		private string GetClassName(XmlReader reader)
		{
			return GetClassName(reader.NamespaceURI, reader.LocalName);
		}
		
		/// <summary>
		/// Gets the class name for a string containing the colon-separated namespace and type.
		/// </summary>
		private string GetClassName(XmlReader reader, string valString)
		{
			var comps = valString.Split(':');
			if (comps.Length != 2)
				throw new Exception("Types should be colon-separated namespace and type names, unlike " + valString);
			return GetClassName(reader.LookupNamespace(comps[0]), comps[1]);
		}
		
		/// <summary>
		/// Gets the class name for the given xml namespace and element type.
		/// </summary>
		private string GetClassName(string ns, string typeName)
		{
			if (ns.StartsWith(MwxUri)) // this is a MonoWorks class
			{
				// find out which assembly it belongs to
				var assemblies = AppDomain.CurrentDomain.GetAssemblies();
				ns = ns.Replace(MwxUri, "MonoWorks").Replace('/', '.');
				var comps = ns.Split('.');
				var asmName = "";
				foreach (var comp in comps) {
					asmName += comp;
					var match = from assembly in assemblies
								where assembly.GetName().Name == asmName
								select assembly;
					if (match.Count() > 0)
						break;
					asmName += ".";
				}
				if (asmName.EndsWith("."))
					throw new InvalidMwxElementException(typeName, "Could not find an assembly as a part of the namespace " + ns);
				return String.Format("{0}.{1},{2}", ns, typeName, asmName);
			}
			else
				throw new InvalidMwxElementException(typeName, "Haven't implemented non-MonoWorks classes in mwx yet.");
		}
		
		/// <summary>
		/// Creates an object based on the current mwx element.
		/// </summary>
		private IMwxObject CreateMwxObject(XmlReader reader)
		{
			// get the type
			var className = GetClassName(reader);
			Type type = null;
			var genericTypeName = reader.GetAttribute("GenericType");
			try
			{
				if (genericTypeName != null)
				{
					var genericType = Type.GetType(GetClassName(reader, genericTypeName), true);
					type = Type.GetType(String.Format("{0}`1[{1}]", className.Split(',')[0], genericType.FullName), true);
				}
				else 
				{
					// non-generic type
					type = Type.GetType(className, true);
				}
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
		/// Generates a unique name for obj.
		/// </summary>
		private void GenerateName(IMwxObject obj)
		{
			var typeName = obj.GetType().ToString();
			int count = 0;
			if (!_nameCounts.TryGetValue(typeName, out count)) {
				count = 0;
				_nameCounts[typeName] = count;
			}
			obj.Name = typeName + count.ToString();
			_nameCounts[typeName]++;
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
				{
					if (prop.Key == "GenericType")
						continue;
					else
						throw new Exception(String.Format("No property named {0} for type {1}", prop.Key, obj.GetType()));
				}
				var mwxProps = Attribute.GetCustomAttributes(propInfo, typeof(MwxPropertyAttribute), true);
				if (mwxProps.Length == 0)
					throw new Exception(String.Format("Property {0} for type {1} is not a MwxProperty", prop.Key, obj.GetType()));
				var mwxProp = mwxProps[0] as MwxPropertyAttribute;
				switch (mwxProp.Type)
				{
				case MwxPropertyType.Reference:
					// store the property to resolve it later
					_refProps.Add(new ReferenceProperty() {
						Property = propInfo,
						ReferenceName = prop.Value,
						Owner = obj
					});
					break;
				
				case MwxPropertyType.Attribute:
					propInfo.SetFromString(obj, prop.Value);
					break;
				}
			}
			
		}
		
		/// <summary>
		/// Sets the given property on the mwx object.
		/// </summary>
		private void SetProperty(IMwxObject obj, string name, object val)
		{
			var propInfo = obj.GetType().GetProperty(name);
			if (propInfo == null)
				throw new Exception("Type " + obj.GetType() + " does not have a property named " + name);
			propInfo.SetValue(obj, val, new object[] {  });
		}
		
		
	}
	
	
	
	
	/// <summary>
	/// Temporary storage for all the information needed to resolve a reference property.
	/// </summary>
	internal class ReferenceProperty
	{
		/// <summary>
		/// The property defining the reference.
		/// </summary>
		public PropertyInfo Property {
			get;
			set;
		}
		
		/// <summary>
		/// The name of the reference object.
		/// </summary>
		public string ReferenceName {
			get;
			set;
		}
		
		/// <summary>
		/// The object that the property value should be assigned to.
		/// </summary>
		public IMwxObject Owner {
			get;
			set;
		}
		
		/// <summary>
		/// Attempts to resolve the reference property with the given value.
		/// </summary>
		internal void Resolve(IMwxObject propVal)
		{
			Property.SetValue(Owner, propVal, new object[] {  });
		}
	}
	
}
