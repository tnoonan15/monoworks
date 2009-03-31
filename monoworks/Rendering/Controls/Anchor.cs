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

using MonoWorks.Base;

using MonoWorks.Rendering.Events;

namespace MonoWorks.Rendering.Controls
{
	/// <summary>
	/// Locations for an anchor.
	/// </summary>
	public enum AnchorLocation { N, E, S, W, NE, SE, SW, NW };
	
	/// <summary>
	/// Single control container that anchors its child to a particular side of the viewport.
	/// </summary>
	public class Anchor : Bin
	{
		public Anchor(AnchorLocation location)
			: base()
		{
			Location = location;
		}

		public Anchor(Control child) : base(child)
		{
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



		protected override void Render(Viewport viewport)
		{
			// adjust position according to location
			if (child != null && (IsDirty || child.IsDirty)) // need to check this before calling parent since they will make us clean
			{
				UpdatePosition(viewport);
			}

			base.Render(viewport);

		}


		/// <summary>
		/// Updates the position of the anchor based on the viewport.
		/// </summary>
		private void UpdatePosition(Viewport viewport)
		{
			if (child != null)
			{
				child.ComputeGeometry();
				size = child.Size;
				//				Console.WriteLine("anchor size: {0}, viewport size: {1} x {2}", size, viewport.WidthGL, viewport.HeightGL);
				switch (location)
				{
				case AnchorLocation.N:
					Position = new Coord((viewport.WidthGL - Width) / 2.0, viewport.HeightGL - Height);
					break;
				case AnchorLocation.NE:
					Position = new Coord(viewport.WidthGL - Width - 2, viewport.HeightGL - Height - 2);
					break;
				case AnchorLocation.E:
					Position = new Coord(viewport.WidthGL - Width, (viewport.HeightGL - Height) / 2.0);
					break;
				case AnchorLocation.SE:
					Position = new Coord(viewport.WidthGL - Width, 0);
					break;
				case AnchorLocation.S:
					Position = new Coord((viewport.WidthGL - Width) / 2.0, 0);
					break;
				case AnchorLocation.SW:
					Position = new Coord(0, 0);
					break;
				case AnchorLocation.W:
					Position = new Coord(0, (viewport.HeightGL - Height) / 2.0);
					break;
				case AnchorLocation.NW:
					Position = new Coord(0, viewport.HeightGL - Height);
					break;
				}
			}
		}

		
	}
}
