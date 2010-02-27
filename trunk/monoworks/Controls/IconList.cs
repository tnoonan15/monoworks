// 
//  IconList.cs - MonoWorks Project
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

using MonoWorks.Base;

namespace MonoWorks.Controls
{
	/// <summary>
	/// A dictionary mapping icon names to images.
	/// </summary>
	public class IconList : Dictionary<string, Image>, IMwxObject, IStringParsable
	{
		public IconList()
		{
		}
		
		
		/// <summary>
		/// Parses the icon list
		/// </summary>
		/// <param name="stringVal">
		/// A <see cref="System.String"/>
		/// </param>
		public void Parse(string stringVal)
		{
			
		}
		
		/// <summary>
		/// The name of the object.
		/// </summary>
		[MwxProperty]
		public virtual string Name {get;set;}
		
		/// <summary>
		/// The parent object to this one.
		/// </summary>
		public virtual IMwxObject Parent {get; set;}
		
		/// <summary>
		/// Adds an icon definition.
		/// </summary>
		public virtual void AddChild(IMwxObject child)
		{
			if (child is IconListEntry)
			{
				child.Parent = this;
				var entry = child as IconListEntry;
				if (entry.Name == null || entry.Name.Length == 0)
					throw new InvalidIconListEntryException("Icon must have a name");
				if (ContainsKey(entry.Name))
					throw new InvalidIconListEntryException("Icon named " + entry.Name + " already exists");
				this[entry.Name] = entry.Image;
			}
			else
				throw new Exception("Icon lists can only have children of type IconListEntry");
		}

		public IEnumerable<IMwxObject> GetMwxChildren()
		{
			var children = new List<IMwxObject>();
			foreach (var icon in Values)
				children.Add(icon);
			return children;
		}
		
	}
	
	/// <summary>
	/// Gets thrown when an invalid icon entry is added to an icon list.
	/// </summary>
	public class InvalidIconListEntryException : Exception
	{
		public InvalidIconListEntryException(string message)
			: base("Invalid Icon List Entry: " + message)
		{
			
		}
	}
	
	/// <summary>
	/// Temporary class that represents an IconList entry in a MWX file.
	/// </summary>
	public class IconListEntry : MwxDummyObject, IStringParsable
	{
		public IconListEntry()
		{
			Image = new Image();
		}
		
		public void Parse(string valString)
		{
			Image.Parse(valString);
		}
		
		public Image Image { get; set; }
		
	}
	
}

