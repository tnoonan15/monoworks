// Stack.cs - MonoWorks Project
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
using MonoWorks.Base;
using MonoWorks.Rendering;

namespace MonoWorks.Controls
{

	/// <summary>
	/// A generic container that stacks its children together.
	/// </summary>
	public class GenericStack<T> : GenericContainer<T>, IOrientable where T : Control2D
	{
		/// <summary>
		/// Default (horizontal) constructor.
		/// </summary>
		public GenericStack() : this(Orientation.Horizontal)
		{
		}

		/// <summary>
		/// Constructor with orientation initialization.
		/// </summary>
		public GenericStack(Orientation orientation)
		{
			_orientation = orientation;
		}


		private Orientation _orientation;
		/// <value>
		/// The orientation of the stack.
		/// </value>
		[MwxProperty]
		public Orientation Orientation
		{
			get { return _orientation; }
			set
			{
				_orientation = value;
				MakeDirty();
			}
		}
		
		private bool _forceFill;
		/// <summary>
		/// If true, the stack forces all children to be the same 
		/// width for Vertical and height for Horizontal.
		/// </summary>
		[MwxProperty]
		public bool ForceFill 
		{
			get { return _forceFill; }
			set {
				_forceFill = value;
				MakeDirty();
			}
		}
		

		#region Rendering

		public override void ComputeGeometry()
		{
			base.ComputeGeometry();

			// compute the size
			RenderSize = new Coord();
			double span = 0;
			foreach (var child in ChildrenCopy)
			{
				Coord size_ = child.RenderSize;
				span += Padding;
				if (_orientation == Orientation.Horizontal)
				{
					child.Origin = new Coord(span, Padding);
					span += size_.X;
					RenderSize.Y = Math.Max(RenderSize.Y, size_.Y);
				}
				else // vertical
				{
					child.Origin = new Coord(Padding, span);
					span += size_.Y;
					RenderSize.X = Math.Max(RenderSize.X, size_.X);
				}
				span += Padding;
			}
			
			// assign the size
			if (_orientation == Orientation.Horizontal)
				RenderSize.X = span;
			else 
				RenderSize.Y = span;
			
			// force the children to fill their area
			if (ForceFill)
			{
				foreach (Control2D child in Children)
				{
					if (_orientation == Orientation.Horizontal)
						child.RenderHeight = RenderSize.Y;
					else
						child.RenderWidth = RenderSize.X;
				}
			}
			
			// add padding to the size
			if (_orientation == Orientation.Horizontal)
				RenderSize.Y += 2 * Padding;
			else
				RenderSize.X += 2 * Padding;
			
		}		

		#endregion
		
		
	}


	/// <summary>
	/// A non-generic stack that can contain any type of control.
	/// </summary>
	public class Stack : GenericStack<Control2D>
	{
		public Stack()
		{
			
		}

		public Stack(Orientation orientation) : base(orientation)
		{
			
		}
	}

}
