// 
//  MwxDeepCopier.cs - MonoWorks Project
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


namespace MonoWorks.Base
{
	/// <summary>
	/// Performs a deep copy of a hierarchy of Mwx objects.
	/// </summary>
	public class MwxDeepCopier
	{
		public MwxDeepCopier()
		{
		}
		
		
		/// <summary>
		/// Recursively creates a deep copy of the object.
		/// </summary>
		public T DeepCopy<T>(T obj) where T : IMwxObject
		{
			T newObj = (T)Activator.CreateInstance(obj.GetType());
			
			// copy the properties
			var props = newObj.GetMwxProperties();
			foreach (var prop in props)
			{
				if (prop.Name == "Name")
					continue;
				var propObj = prop.PropertyInfo.GetValue(obj, new object[] {  });
				object propVal;
				if (propObj is IMwxObject)
				{
					propVal = DeepCopy(propObj as IMwxObject);
				}
				else if (propObj is IStringParsable)
				{
					propVal = Activator.CreateInstance(propObj.GetType());
					(propVal as IStringParsable).Parse(propObj.ToString());
				}
				else
				{
					propVal = propObj;
				}
				prop.PropertyInfo.SetValue(newObj, propVal, new object[] {  });
			}
			
			// copy the children
			var children = obj.GetMwxChildren();
			if (children != null)
			{
				foreach (var child in children)
				{
					IMwxObject newChild;
					if (child is IMwxObject)
					{
						newChild = DeepCopy(child);
					}
					else 
					{
						newChild = child;
					}
					newObj.AddChild(newChild);
					if (newChild.Parent == null)
						newChild.Parent = newObj;
				}
			}
			
			return newObj;
		}
		
		
		/// <summary>
		/// Non-generic version of DeepCopy<T>().
		/// </summary>
		public IMwxObject DeepCopy(IMwxObject obj)
		{
			return DeepCopy<IMwxObject>(obj);
		}
		
	}
}

