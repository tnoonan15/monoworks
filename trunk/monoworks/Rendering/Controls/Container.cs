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
using System.Collections.Generic;

using MonoWorks.Rendering.Events;

namespace MonoWorks.Rendering.Controls
{
	
	/// <summary>
	/// Base class for control container.
	/// </summary>
	public abstract class Container : Control
	{
		
		public Container() : base()
		{
		}


#region Children


		protected List<Control> children = new List<Control>();
		/// <value>
		/// The container's children.
		/// </value>
		public IEnumerable<Control> Children
		{
			get {return children;}
		}

		/// <value>
		/// Access the children by index.
		/// </value>
		public Control this[int index]
		{
			get {return GetChild(index);}
			set {SetChild(index, value);}
		}
		
		/// <summary>
		/// Appends a child control on to the end of the stack.
		/// </summary>
		/// <param name="child">
		/// A <see cref="Control"/>
		/// </param>
		public virtual void Add(Control child)
		{
			children.Add(child);
			child.Parent = this;
			//child.StyleClassName = StyleClassName;
			MakeDirty();
		}

		/// <summary>
		/// Get a child by index.
		/// </summary>
		public Control GetChild(int index)
		{
			if (index < 0 || index >= children.Count)
				throw new IndexOutOfRangeException("Invalid container child index: " + index.ToString());
			return children[index];
		}

		/// <summary>
		/// Set a child by index.
		/// </summary>
		/// <remarks>If index is equal to NumChildren, it will be appended to the end.</remarks>
		public void SetChild(int index, Control child)
		{
			if (index < 0 || index > children.Count)
				throw new IndexOutOfRangeException("Invalid container child index: " + index.ToString());
			if (index == children.Count)
				children.Add(child);
			else
				children[index] = child;
			MakeDirty();
		}

		/// <summary>
		/// Clears all children.
		/// </summary>
		public void Clear()
		{
			children.Clear();
			MakeDirty();
		}

#endregion


#region Mouse Interaction

		public override void OnButtonPress(MouseButtonEvent evt)
		{
			base.OnButtonPress(evt);
			
			foreach (Control child in children)
				child.OnButtonPress(evt);
		}

		public override void OnButtonRelease(MouseButtonEvent evt)
		{
			base.OnButtonRelease(evt);
			
			foreach (Control child in children)
				child.OnButtonRelease(evt);
		}

		public override void OnMouseMotion(MouseEvent evt)
		{
			base.OnMouseMotion(evt);
			
			foreach (Control child in children)
				child.OnMouseMotion(evt);
		}

#endregion


#region Rendering

		public override void RenderOverlay(Viewport viewport)
		{
			base.RenderOverlay(viewport);
			
			foreach (Control child in children)
				child.RenderOverlay(viewport);
		}

#endregion


	}
}
