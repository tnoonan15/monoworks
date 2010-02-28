// 
//  MwxList.cs - MonoWorks Project
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

namespace MonoWorks.Base
{
	/// <summary>
	/// Wraps the standard generic list and provides hooks for collection monitoring and mwx support.
	/// </summary>
	public class MwxList<T> : IList<T>, IMwxObject where T : IMwxObject
	{
		public MwxList()
		{
			_internal = new List<T>();
		}
		
		/// <summary>
		/// Constructor providing an initial capacity.
		/// </summary>
		public MwxList(int capacity)
		{
			_internal = new List<T>(capacity);
		}
		
		private List<T> _internal;
		
		/// <summary>
		/// Finds the index of item in the list.
		/// </summary>
		public int IndexOf(T item)
		{
			return _internal.IndexOf(item);
		}
		
		/// <summary>
		/// Inserts item at the given index.
		/// </summary>
		public void Insert(int index, T item)
		{
			_internal.Insert(index, item);
		}
		
		/// <summary>
		/// Removes the value at the given index.
		/// </summary>
		public void RemoveAt(int index)
		{
			_internal.RemoveAt(index);
		}
		
		/// <summary>
		/// Bracket-style element access.
		/// </summary>
		public T this[int index] {
			get {
				return _internal[index];
			}
			set {
				_internal[index] = value;
			}
		}
		
		/// <summary>
		/// Gets an enumerator to traverse the list.
		/// </summary>
		public IEnumerator<T> GetEnumerator()
		{
			return _internal.GetEnumerator();
		}
		
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _internal.GetEnumerator();
		}
		
		/// <summary>
		/// Adds item to the end of the list.
		/// </summary>
		public void Add(T item)
		{
			_internal.Add(item);
		}
		
		/// <summary>
		/// Cleas all items from the list.
		/// </summary>
		public void Clear()
		{
			_internal.Clear();
		}
		
		/// <summary>
		/// True if the list contains item.
		/// </summary>
		public bool Contains(T item)
		{
			return _internal.Contains(item);
		}
		
		/// <summary>
		/// Copies an array to the list starting at the given index.
		/// </summary>
		public void CopyTo(T[] array, int arrayIndex)
		{
			_internal.CopyTo(array, arrayIndex);
		}
		
		/// <summary>
		/// Removes the given item.
		/// </summary>
		public bool Remove(T item)
		{
			return _internal.Remove(item);
		}
		
		/// <summary>
		/// Attempts to add a mwx object to the list.
		/// </summary>
		/// <param name="child"> Must be type T. </param>
		public void AddChild(IMwxObject child)
		{
			if (child is T)
				_internal.Add((T)child);
			else
				throw new Exception(String.Format("Can't convert type {0} to {1}", child.GetType(), typeof(T)));
		}
		
		public IEnumerable<IMwxObject> GetMwxChildren()
		{
			var list = new List<IMwxObject>();
			foreach (var item in _internal)
				list.Add(item);
			return list;
		}
		
		/// <summary>
		/// The name of the list (a simple one will be automatically generated).
		/// </summary>
		public string Name {get; set;}
		
		/// <summary>
		/// The parent of the list in the mwx tree.
		/// </summary>
		public IMwxObject Parent {get; set;	}
		
		/// <summary>
		/// The number of elements in the list.
		/// </summary>
		public int Count {
			get {
				return _internal.Count;
			}
		}
		
		/// <summary>
		/// Whether or not the list can be written to.
		/// </summary>
		public bool IsReadOnly {
			get {
				return false;
			}
		}
		
		
		
	}
}

