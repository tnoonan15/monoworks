// 
//  IMwxObject.cs - MonoWorks Project
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
using System.Xml;
using System.Reflection;

namespace MonoWorks.Base
{
	/// <summary>
	/// Base class for objects that can be serialized into MWX files.
	/// </summary>
	public interface IMwxObject
	{
		/// <summary>
		/// The name of the object.
		/// </summary>
		[MwxProperty]
		string Name {get; set;}
		
		/// <summary>
		/// The parent object to this one.
		/// </summary>
		IMwxObject Parent {get; set;}
		
		/// <summary>
		/// Adds a child to this object.
		/// </summary>
		void AddChild(IMwxObject child);
		
		/// <summary>
		/// Gets all Mwx children that need to be persisted.
		/// </summary>
		IEnumerable<IMwxObject> GetMwxChildren();
	}
	
	
	/// <summary>
	/// Provides a naive default implementation for an MWX object.
	/// </summary>
	public class MwxDummyObject : IMwxObject
	{
		/// <exception cref="NotImplementedException"></exception>
		public void AddChild(IMwxObject child)
		{
			throw new System.NotImplementedException();
		}
		
		[MwxProperty]
		public string Name {get; set;}
				
		public IMwxObject Parent { get; set; }
		
		public IEnumerable<IMwxObject> GetMwxChildren()
		{
			return new List<IMwxObject>();
		}
	}
	
}

