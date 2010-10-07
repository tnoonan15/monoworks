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

using MonoWorks.Base;
using MonoWorks.Rendering;
using MonoWorks.Rendering.Events;

namespace MonoWorks.Controls
{
	
	/// <summary>
	/// Base class for control container.
	/// </summary>
	public abstract class GenericContainer<ControlType> : Control2D, IEnumerable<ControlType> where ControlType : Control2D
	{
		#region Children

		public override void AddChild(IMwxObject child)
		{
			if (child is ControlType)
				AddChild(child as ControlType);
			else
				throw new NotImplementedException(@"Children of Containers must be of type Control2D.");
		}


		private readonly List<ControlType> _children = new List<ControlType>();
		/// <value>
		/// The container's children.
		/// </value>
		public IList<ControlType> Children
		{
			get {return _children;}
		}

		/// <value>
		/// A copy of the children.
		/// </value>
		/// <remarks>Only use this if there's a possibility the children 
		/// will be edited during iteration.</remarks>
		protected ControlType[] ChildrenCopy
		{
			get
			{
				var copy = new ControlType[_children.Count];
				_children.CopyTo(copy);
				return copy;
			}
		}
		
		/// <value>
		/// Access the children by index.
		/// </value>
		public ControlType this[int index]
		{
			get {return GetChild(index);}
			set {SetChild(index, value);}
		}
		
		/// <summary>
		/// Appends a child control on to the end of the stack.
		/// </summary>
		public virtual void AddChild(ControlType child)
		{
			_children.Add(child);
			child.ParentControl = this;
			MakeDirty();
		}

		/// <summary>
		/// Inserts a child control at the given index of the stack.
		/// </summary>
		/// <remarks>Negative indices count from the back.</remarks>
		public virtual void InsertChild(ControlType child, int index)
		{
			if (index < 0)
				index = NumChildren + index;
			_children.Insert(index, child);
			child.ParentControl = this;
			MakeDirty();
		}

		/// <summary>
		/// Removes the given child from the children collection.
		/// </summary>
		public virtual void RemoveChild(ControlType child)        {
			_children.Remove(child);
		}

		/// <summary>
		/// Get a child by index.
		/// </summary>
		public ControlType GetChild(int index)
		{
			if (index < 0 || index >= _children.Count)
				throw new IndexOutOfRangeException("Invalid container child index: " + index);
			return _children[index];
		}

		/// <summary>
		/// Set a child by index.
		/// </summary>
		/// <remarks>If index is equal to NumChildren, it will be appended to the end.</remarks>
		public void SetChild(int index, ControlType child)
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
		public int IndexOfChild(ControlType child)
		{
			return _children.IndexOf(child);
		}

		/// <summary>
		/// True if the children collection contains the given child.
		/// </summary>
		public bool ContainsChild(ControlType child)
		{
			return _children.Contains(child);
		}

		public IEnumerator<ControlType> GetEnumerator()
		{
			return Children.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
		
		public override IList<IMwxObject> GetMwxChildren()
		{
			return _children.Cast<IMwxObject>();
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
		
		
		public override void ComputeGeometry()
		{
			base.ComputeGeometry();
			
			foreach (var child in _children)
			{
				if (child.IsDirty)
					child.ComputeGeometry();
			}
		}


		protected override void Render(RenderContext context)
		{
			base.Render(context);
			
			foreach (var child in _children)
			{
				child.RenderCairo(context);
			}
		}
		
		#endregion


		#region Focus
		
		public override Renderable2D GetNextFocus(Renderable2D child)
		{			
			var kid = child as ControlType;
			if (kid == null)
				throw new Exception("Child has the wrong type for this container.");
			if (!ContainsChild(kid))
				throw new Exception("child must be a child of this container.");
			
			var index = IndexOfChild(kid);
			if (index == NumChildren - 1) // the last child
			{
				if (ParentControl == null)
					return _children[0].GetFirstFocus();
				else
					return ParentControl.GetNextFocus(this);
			}
			return _children[index + 1].GetFirstFocus();
		}
		
		public override Renderable2D GetPreviousFocus(Renderable2D child)
		{
			var kid = child as ControlType;
			if (kid == null)
				throw new Exception("Child has the wrong type for this container.");
			if (!ContainsChild(kid))
				throw new Exception("child must be a child of this container.");
			
			var index = IndexOfChild(kid);
			if (index == 0) // the first child
			{
				if (ParentControl == null)
					return _children.Last<ControlType>().GetLastFocus();
				else
					return ParentControl.GetPreviousFocus(this);
			}
			return _children[index - 1].GetLastFocus();
		}

		public override Renderable2D GetFirstFocus()
		{
			if (NumChildren == 0)
			{
				if (ParentControl != null)
					return ParentControl.GetNextFocus(this);
				else
					return null;
			}
			return _children[0].GetFirstFocus();
		}
		
		public override Renderable2D GetLastFocus()
		{
			if (NumChildren == 0)
			{
				if (ParentControl != null)
					return ParentControl.GetPreviousFocus(this);
				else
					return null;
			}
			return _children.Last<ControlType>().GetLastFocus();
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
