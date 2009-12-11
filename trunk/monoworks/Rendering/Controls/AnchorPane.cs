// AnchorPane.cs - MonoWorks Project
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

using MonoWorks.Base;

using MonoWorks.Rendering.Events;

namespace MonoWorks.Rendering.Controls
{
	
	/// <summary>
	/// Single control container that anchors its Control to a particular side of the viewport.
	/// </summary>
	public class AnchorPane : OverlayPane
	{
		public AnchorPane(AnchorLocation location)
			: this(null, location)
		{
		}

		public AnchorPane(Control2D Control) : this(Control, AnchorLocation.N)
		{
		}
		
		public AnchorPane(Control2D control, AnchorLocation location)
		{
			this.Control = control;
			Location = location;	
		}
		
		
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
		
		
		public override void OnViewportResized(Viewport viewport)
		{
			base.OnViewportResized(viewport);

			UpdatePosition(viewport);
		}
		
		public override void RenderOverlay(Viewport viewport)
		{
			// if we're dirty, we need to update the position, but this can't be done 
			// in ComputeGeometry() since we don't know the viewport
			if (IsDirty)
				UpdatePosition(viewport);
			
			base.RenderOverlay(viewport);
		}



		/// <summary>
		/// Updates the position of the anchor based on the viewport.
		/// </summary>
		private void UpdatePosition(Viewport viewport)
		{
			if (Control != null)
			{
				ComputeGeometry();
				Control.ComputeGeometry();
				switch (location)
				{
				case AnchorLocation.N:
					Origin = new Coord((viewport.WidthGL - Width) / 2.0, viewport.HeightGL - Height);
					break;
				case AnchorLocation.NE:
					Origin = new Coord(viewport.WidthGL - Width - 2, viewport.HeightGL - Height - 2);
					break;
				case AnchorLocation.E:
					Origin = new Coord(viewport.WidthGL - Width, (viewport.HeightGL - Height) / 2.0);
					break;
				case AnchorLocation.SE:
					Origin = new Coord(viewport.WidthGL - Width, 0);
					break;
				case AnchorLocation.S:
					Origin = new Coord((viewport.WidthGL - Width) / 2.0, 0);
					break;
				case AnchorLocation.SW:
					Origin = new Coord(0, 0);
					break;
				case AnchorLocation.W:
					Origin = new Coord(0, (viewport.HeightGL - Height) / 2.0);
					break;
				case AnchorLocation.NW:
					Origin = new Coord(0, viewport.HeightGL - Height);
					break;
				}
			}
		}
		
	}
}
