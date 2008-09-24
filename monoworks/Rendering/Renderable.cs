// IRenderable.cs - MonoWorks Project
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

namespace MonoWorks.Rendering
{
	
	/// <summary>
	/// Base class for renderable objects.
	/// </summary>
	public abstract class Renderable
	{	
		
		/// <summary>
		/// True if the renderable is dirty and needs its geometry recomputed.
		/// </summary>
		protected bool dirty = true;
		
		/// <summary>
		/// Makes the renderable dirty.
		/// </summary>
		public virtual void MakeDirty()
		{
			dirty = true;
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
		/// Forces the renderable to compute its geometry.
		/// </summary>
		public virtual void ComputeGeometry()
		{
			dirty = false;
		}
		
		/// <summary>
		/// Renders the opaque portion of the renderable.
		/// </summary>
		/// <param name="viewport"> A <see cref="IViewport"/> to render to. </param>
		public virtual void RenderOpaque(IViewport viewport)
		{			
			if (dirty)
				ComputeGeometry();
		}
		
		/// <summary>
		/// Renders the transparent portion of the renderable, 
		/// </summary>
		/// <param name="viewport"> A <see cref="IViewport"/> to render to. </param>
		public virtual void RenderTransparent(IViewport viewport)
		{			
			if (dirty)
				ComputeGeometry();
		}
		
		
		/// <summary>
		/// Renders the overlay portion of the renderable, 
		/// </summary>
		/// <param name="viewport"> A <see cref="IViewport"/> to render to. </param>
		public virtual void RenderOverlay(IViewport viewport)
		{			
			if (dirty)
				ComputeGeometry();
		}
		
		
		/// <summary>
		/// Allows the renderable to directly handle a pan event.
		/// </summary>
		/// <param name="viewport"> The <see cref="IViewport"/> on which the interaction was performed. </param>
		/// <param name="dx"> The travel in the x screen dimension.</param>
		/// <param name="dy"> The travel in the y screen dimension.</param>
		/// <returns> True to block the viewport from dealing with the interaction itself. </returns>
		public virtual bool HandlePan(IViewport viewport, double dx, double dy)
		{
			return false;
		}		
		
		/// <summary>
		/// Allows the renderable to directly handle a dolly event.
		/// </summary>
		/// <param name="viewport"> The <see cref="IViewport"/> on which the interaction was performed. </param>
		/// <param name="factor"> The dolly factor.</param>
		/// <returns> True to block the viewport from dealing with the interaction itself. </returns>
		public virtual bool HandleDolly(IViewport viewport, double factor)
		{
			return false;
		}
		
		
	}
	
}
