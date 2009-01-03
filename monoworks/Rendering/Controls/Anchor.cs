// Anchor.cs - MonoWorks Project
//
//  Copyright (C) 2009 Andy Selvig
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

namespace MonoWorks.Rendering.Controls
{
	/// <summary>
	/// Locations for an anchor.
	/// </summary>
	public enum AnchorLocation {N, NE, E, SE, S, SW, W, NW};
	
	/// <summary>
	/// Single control container that anchors its child to a particular side of the viewport.
	/// </summary>
	public class Anchor : Control
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		/// <param name="child"> The <see cref="Control"/> to anchor. </param>
		public Anchor(Control child)
		{
			this.child = child;
		}
		
		protected Control child;
		
		
		private AnchorLocation location = AnchorLocation.N;
		/// <value>
		/// The location on the edge of the viewport.
		/// </value>
		public AnchorLocation Location
		{
			get {return location;}
			set
			{
				location = value;
				MakeDirty();
			}
		}
		
		
		public override void RenderOverlay(IViewport viewport)
		{
			base.RenderOverlay(viewport);
			
			
		}


		
	}
}
