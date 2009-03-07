// Actor.cs - MonoWorks Project
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

namespace MonoWorks.Rendering
{
	
	/// <summary>
	/// Renderable that can renders itself in 3D.
	/// </summary>
	public class Actor : Renderable
	{
		
		public Actor() : base()
		{
		}

		protected Bounds bounds = new Bounds();
		/// <summary>
		/// The bounding box of the renderable.
		/// Should be updated by ComputeGeometry().
		/// </summary>
		public Bounds Bounds
		{
			get {return bounds;}
		}

		/// <summary>
		/// Resets the bounds to their default value, if applicable.
		/// </summary>
		public virtual void ResetBounds()
		{
		}
		
		/// <summary>
		/// Renders the opaque portion of the renderable.
		/// </summary>
		/// <param name="viewport"> A <see cref="Viewport"/> to render to. </param>
		public virtual void RenderOpaque(Viewport viewport)
		{			
			if (IsDirty)
				ComputeGeometry();
		}
		
		/// <summary>
		/// Renders the transparent portion of the renderable, 
		/// </summary>
		/// <param name="viewport"> A <see cref="Viewport"/> to render to. </param>
		public virtual void RenderTransparent(Viewport viewport)
		{			
			if (IsDirty)
				ComputeGeometry();
		}
		
		/// <summary>
		/// Performs a hit test with two vectors lying on a 3D line.
		/// </summary>
		/// <returns> True if the renderable was hit. </returns>
		public virtual bool HitTest(HitLine hitLine)
		{
			return bounds.HitTest(hitLine, out lastHit);
		}

		protected Vector lastHit = null;
		/// <summary>
		/// The position of the last hit during a hit test.
		/// </summary>
		/// <remarks>NOTE: This could very often be null.</remarks>
		public Vector LastHit
		{
			get { return lastHit; }
		}

	}
}
