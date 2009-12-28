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
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Reflection;

using MonoWorks.Base;

namespace MonoWorks.Rendering
{
	/// <summary>
	/// The exception that gets thrown when a MwxSource can't figure out how to instantiate an element.
	/// </summary>
	public class InvalidMwxElementException : Exception
	{
		public InvalidMwxElementException(XmlReader reader, string details)
			: base(String.Format("Unable to resolve element {0} into a concrete renderable instance. {1}", reader.Name, details))
		{			
		}
	}
	
	public class UnknownRenderableException : Exception
	{
		public UnknownRenderableException(string name) 
			: base(String.Format("There is no renderable named {0} in the mwx source.", name))
		{
			
		}
	}

	/// <summary>
	/// Parses a mwx file and provides access to the renderables declared inside of it.
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
		
		
		private Dictionary<string, Renderable> _renderables = new Dictionary<string, Renderable>();
		
		/// <summary>
		/// Gets a renderable by name.
		/// </summary>
		/// <remarks>This isn't type safe and you should generally use GetRenderable<T> instead.</remarks>
		public Renderable GetRenderable(string name)
		{
			Renderable ren = null;
			if (_renderables.TryGetValue(name, out ren))
				return ren;
			throw new UnknownRenderableException(name);
		}
		
		/// <summary>
		/// Gets a renderable by name and performs type checking.
		/// </summary>
		public T GetRenderable<T>(string name) where T : Renderable
		{
			Renderable ren = null;
			if (_renderables.TryGetValue(name, out ren))
			{
				if (ren is T)
					return ren as T;
				throw new Exception(String.Format("Renderable {0} is of type {1}, not {2}", 
				                                  name, ren.GetType(), typeof(T)));
			}
			throw new UnknownRenderableException(name);
		}
		
		/// <summary>
		/// Parses a mwx source through a xml reader.
		/// </summary>
		private void Parse(XmlReader reader)
		{
			Renderable parent = null;
			while (!reader.EOF)
			{
				reader.Read();
				if (reader.NodeType == XmlNodeType.Element)
				{
					var isEmpty = reader.IsEmptyElement;
					
					// create the renderable
					if (reader.LocalName == "Ui")
						continue;
					var renderable = CreateRenderable(reader);
					var name = renderable.Name;
					if (name != null)
						_renderables[name] = renderable;
					
					// add it to the current parent
					if (parent != null)
					{
						parent.AddChild(renderable);
					}
					
					// make this the current parent
					if (!isEmpty)
						parent = renderable;
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
		/// Creates a renderable based on the current mwx element.
		/// </summary>
		private Renderable CreateRenderable(XmlReader reader)
		{
			// get the type
			var className = GetElementClassName(reader);
			Console.WriteLine ("parsing " + className);
			Type type = null;
			try
			{
				type = Type.GetType(className, true);
			}
			catch (Exception ex)
			{
				throw new InvalidMwxElementException(reader, ex.Message);
			}
			
			// verify it's a renderable
			if (!type.IsSubclassOf(typeof(Renderable)))
				throw new InvalidMwxElementException(reader, "Type is not a subclass of MonoWorks.Rendering.Renderable.");
			
			// instantiate the renderable
			var renderable = Activator.CreateInstance(type) as Renderable;
			
			// populate the renderable
			renderable.FromMwx(reader);
			
			return renderable;
		}
		
		
	}
}
