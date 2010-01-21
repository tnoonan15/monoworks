// Container.cs - MonoWorks Project
//
//  Copyright (C) 2008 Andy Selvig
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
//
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA 

using System;
using System.Collections;
using System.Collections.Generic;

using MonoWorks.Rendering;
using MonoWorks.Rendering.Events;

namespace MonoWorks.Controls
{
	
	/// <summary>
	/// Base class for control container.
	/// </summary>
	public abstract class GenericContainer<T> : Control2D, IEnumerable<T> where T : Control2D
	{
		#region Children

		public override void AddChild(Renderable child)
		{
			if (child is T)
				AddChild(child as T);
			else
				throw new NotImplementedException(@"Children of Containers must be of type Control2D.");
		}


		private readonly List<T> _children = new List<T>();
		/// <value>
		/// The container's children.
		/// </value>
		public IEnumerable<T> Children
		{
			get {return _children;}
		}

		/// <value>
		/// A copy of the children.
		/// </value>
		/// <remarks>Only use this if there's a possibility the children 
		/// will be edited during iteration.</remarks>
		protected T[] ChildrenCopy
		{
			get
			{
				var copy = new T[_children.Count];
				_children.CopyTo(copy);
				return copy;
			}
		}
		
		/// <value>
		/// Access the children by index.
		/// </value>
		public T this[int index]
		{
			get {return GetChild(index);}
			set {SetChild(index, value);}
		}
		
		/// <summary>
		/// Appends a child control on to the end of the stack.
		/// </summary>
		public virtual void AddChild(T child)
		{
			_children.Add(child);
			child.ParentControl = this;
			MakeDirty();
		}

		/// <summary>
		/// Removes the given child from the children collection.
		/// </summary>
		public virtual void RemoveChild(T child)        {
			_children.Remove(child);
		}

		/// <summary>
		/// Get a child by index.
		/// </summary>
		public T GetChild(int index)
		{
			if (index < 0 || index >= _children.Count)
				throw new IndexOutOfRangeException("Invalid container child index: " + index);
			return _children[index];
		}

		/// <summary>
		/// Set a child by index.
		/// </summary>
		/// <remarks>If index is equal to NumChildren, it will be appended to the end.</remarks>
		public void SetChild(int index, T child)
		{
			if (index < 0 || index > _children.Count)
				throw new IndexOutOfRangeException("Invalid container child index: " + index);
			if (index == _children.Count)
				_children.Add(child);
			else
				_children[index] = child;
			child.ParentControl = this;
			MakeDirty();
		}

		/// <summary>
		/// Clears all children.
		/// </summary>
		public void Clear()
		{
			_children.Clear();
			MakeDirty();
		}
		
		/// <summary>
		/// The number of children the container has.
		/// </summary>
		public int NumChildren
		{
			get {return _children.Count;}
		}

		/// <summary>
		/// Returns the index of the given child in the children list.
		/// </summary>
		public int IndexOfChild(T child)
		{
			return _children.IndexOf(child);
		}

		/// <summary>
		/// True if the children collection contains the given child.
		/// </summary>
		public bool ContainsChild(T child)
		{
			return _children.Contains(child);
		}

		public IEnumerator<T> GetEnumerator()
		{
			return Children.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion
		
		
		#region Interaction
		
		public override void OnButtonPress(MouseButtonEvent evt)
		{
			base.OnButtonPress(evt);
			
			foreach (var child in ChildrenCopy)
				child.OnButtonPress(evt);
		}

		public override void OnButtonRelease(MouseButtonEvent evt)
		{
			base.OnButtonRelease(evt);
			
			foreach (var child in ChildrenCopy)
				child.OnButtonRelease(evt);
		}

		public override void OnMouseMotion(MouseEvent evt)
		{
			base.OnMouseMotion(evt);
			
			foreach (var child in ChildrenCopy)
				child.OnMouseMotion(evt);
		}

		#endregion

		
		#region Rendering

		protected override void Render(RenderContext context)
		{
			base.Render(context);
			
			foreach (var child in _children)
			{
				child.RenderCairo(context);
			}
		}
		
		#endregion


	}

	/// <summary>
	/// A non-generic container that can contain any type of control.
	/// </summary>
	public class Container : GenericContainer<Control2D>
	{
		
	}

}
