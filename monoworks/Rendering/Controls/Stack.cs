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

namespace MonoWorks.Rendering.Controls
{

	/// <summary>
	/// Container that stacks it children together.
	/// </summary>
	public class Stack : Container
	{
		/// <summary>
		/// Default (horizontal) constructor.
		/// </summary>
		public Stack() : this(Orientation.Horizontal)
		{
		}

		/// <summary>
		/// Constructor with orientation initialization.
		/// </summary>
		public Stack(Orientation orientation) : base()
		{
			this.orientation = orientation;
		}


		private Orientation orientation;
		//// <value>
		/// The orientation of the stack.
		/// </value>
		public Orientation Orientation
		{
			get {return Orientation;}
			set
			{
				orientation = value;
				MakeDirty();
			}
		}
		

#region Rendering


		public override void ComputeGeometry()
		{
			base.ComputeGeometry();
			
			// compute the size
			size = new Coord();
			double span = 0;
			foreach (Control child in Children)
			{
				child.ComputeGeometry();
				Coord size_ = child.MinSize;
				if (orientation == Orientation.Horizontal)
				{
					child.Position = position + new Coord(span, 0);
					span += size_.X;
					size.Y = Math.Max(size.Y, size_.Y);
				}
				else // vertical
				{
					span -= size_.Y;
					child.Position = position + new Coord(0, span);
					size.X = Math.Max(size.X, size_.X);
				}
			}
			
			// assign the size
			if (orientation == Orientation.Horizontal)
				size.X = span;
			else 
				size.Y = span;
			
			// assign the children size
			foreach (Control child in Children)
			{
				child.UserSize = true;
				if (orientation == Orientation.Horizontal)
					child.Height = size.Y;
				else
					child.Width = size.X;
			}
		}


		

#endregion
		
	}
}
