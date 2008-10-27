// Renderable.cs - MonoWorks Project
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
		/// Resets the bounds to their default value, if applicable.
		/// </summary>
		public virtual void ResetBounds()
		{
		}

		/// <summary>
		/// Called when the viewport changes size.
		/// </summary>
		/// <param name="viewport"> A <see cref="IViewport"/>. </param>
		public virtual void OnViewportResized(IViewport viewport)
		{			
		}

		/// <summary>
		/// Called when the viewport changes view direction.
		/// </summary>
		/// <param name="viewport"> A <see cref="IViewport"/>. </param>
		public virtual void OnViewDirectionChanged(IViewport viewport)
		{
		}


#region Rendering

		protected bool visible = true;
		/// <summary>
		/// Whether to render the renderable.
		/// </summary>
		/// <remarks> All geometry will still be computed.</remarks>
		public bool Visible
		{
			get { return visible; }
			set { visible = value; }
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

        /// <summary>
        /// Allows the renderable to directly handle a zoom event.
        /// </summary>
        /// <param name="viewport"> The <see cref="IViewport"/> on which the interaction was performed. </param>
        /// <param name="rubberBand"> The rubber band encompassing the zoom.</param>
        /// <returns> True to block the viewport from dealing with the interaction itself. </returns>
        public virtual bool HandleZoom(IViewport viewport, RubberBand rubberBand)
        {
            return false;
		}

#endregion


#region Hit Test and selection

		/// <summary>
		/// Performs a hit test with two vectors lying on a 3D line.
		/// </summary>
		/// <param name="v1"> A <see cref="Vector"/> on the hit line. </param>
		/// <param name="v2"> A <see cref="Vector"/> on the hit line. </param>
		/// <returns> True if the renderable was hit. </returns>
		public virtual bool HitTest(HitLine hitLine)
		{
			return bounds.HitTest(hitLine);
		}

		protected bool isSelected = false;
		/// <value>
		/// Whether the renderable is selected.
		/// </value>
		public bool IsSelected
		{
			get { return isSelected; }
			set { isSelected = value; }
		}

		/// <summary>
		/// Deselects this renderable and all of its children.
		/// </summary>
		public virtual void Deselect()
		{
			isSelected = false;
		}

		/// <summary>
		/// The string used to describe the selection.
		/// </summary>
		public virtual string SelectionDescription
		{
			get
			{
				return "";
			}
		}

#endregion

	}
	
}
