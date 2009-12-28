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
using System.Collections.Generic;

using MonoWorks.Base;
using MonoWorks.Rendering;

namespace MonoWorks.Controls
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
		/// <value>
		/// The orientation of the stack.
		/// </value>
		[MwxProperty]
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
			RenderSize = new Coord();
			double span = 0;
			Control2D[] children_ = new Control2D[children.Count];
			children.CopyTo(children_);
			foreach (Control2D child in children_)
			{
				child.ComputeGeometry();
				Coord size_ = child.RenderSize;
				span += padding;
				if (orientation == Orientation.Horizontal)
				{
					child.Origin = new Coord(span, padding);
					span += size_.X;
					RenderSize.Y = Math.Max(RenderSize.Y, size_.Y);
				}
				else // vertical
				{
					child.Origin = new Coord(padding, span);
					span += size_.Y;
					RenderSize.X = Math.Max(RenderSize.X, size_.X);
				}
				span += padding;
			}
			
			// assign the size
			if (orientation == Orientation.Horizontal)
				RenderSize.X = span;
			else 
				RenderSize.Y = span;
			
			// assign the children size
			foreach (Control2D child in Children)
			{
				if (orientation == Orientation.Horizontal)
					child.RenderHeight = RenderSize.Y;
				else
					child.RenderWidth = RenderSize.X;
			}
			
			// add padding to the size
			if (orientation == Orientation.Horizontal)
				RenderSize.Y += 2*padding;
			else
				RenderSize.X += 2*padding;
			
		}		

		#endregion
		
		
	}
}
