// Control.cs - MonoWorks Project
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

namespace MonoWorks.Rendering.Controls
{
	
	/// <summary>
	/// Base class for all renderable controls.
	/// </summary>
	public abstract class Control : Overlay
	{
		
		public Control() : base()
		{
		}


#region Size and Position

		protected Coord position = new Coord();
		//// <value>
		/// The position of the lower left of the control.
		/// </value>
		public virtual Coord Position
		{
			get {return position;}
			set {position = value;}
		}

		protected Coord size = new Coord();
		/// <value>
		/// The rendering size of the control.
		/// </value>
		public virtual Coord Size
		{
			get {return size;}
			set {size = value;}
		}


#endregion


#region Rendering


		public override void ComputeGeometry()
		{
			base.ComputeGeometry();
		}

		public override void RenderOverlay(IViewport viewport)
		{
			base.RenderOverlay(viewport);

			
		}



#endregion


#region Hit Testing

		/// <summary>
		/// Performs the hit test on the rectangle defined by position and size.
		/// </summary>
		protected override bool HitTest(Coord pos)
		{
			return pos >= position && pos <= (position + size);
		}

		public override bool HoverTest(Coord pos)
		{
			Console.WriteLine("hover test at {0} for control at {1}", pos, position);
			if (HitTest(pos)) // hit
			{
				IsHovering = true;
				Console.WriteLine("hovering");
				return true;
			}
			else // not hit
			{
				IsHovering = false;
				return false;
			}
		}


#endregion


		#region Default Style

		private static ControlStyle defaultStyle = new ControlStyle();

		/// <value>
		/// Style that gets applied to all new controls.
		/// </value>
		public static ControlStyle DefaultStyle
		{
			get {return defaultStyle;}
		}

#endregion

		
		
	}
}
