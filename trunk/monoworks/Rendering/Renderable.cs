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

using MonoWorks.Rendering.Events;

namespace MonoWorks.Rendering
{

	/// <summary>
	/// Renderable states with respect to mouse interaction.
	/// </summary>
	public enum HitState {None, Hovering, Selected};

	
	/// <summary>
	/// Base class for renderable objects.
	/// </summary>
	public abstract class Renderable : IMouseHandler
	{

		public Renderable()
		{
			IsVisible = true;
		}

		protected bool dirty = true;
		/// <summary>
		/// True if the renderable is dirty and needs its geometry recomputed.
		/// </summary>
		public bool IsDirty
		{
			get { return dirty; }
		}

		/// <summary>
		/// Makes the renderable dirty.
		/// </summary>
		public virtual void MakeDirty()
		{
			dirty = true;
		}

		/// <summary>
		/// Called when the viewport changes size.
		/// </summary>
		/// <param name="viewport"> A <see cref="Viewport"/>. </param>
		public virtual void OnViewportResized(Viewport viewport)
		{			
		}

		/// <summary>
		/// Called when the viewport changes view direction.
		/// </summary>
		/// <param name="viewport"> A <see cref="Viewport"/>. </param>
		public virtual void OnViewDirectionChanged(Viewport viewport)
		{
		}


#region Rendering

		/// <summary>
		/// Whether to render the renderable.
		/// </summary>
		public bool IsVisible { get; set; }


		/// <summary>
		/// Forces the renderable to compute its geometry.
		/// </summary>
		public virtual void ComputeGeometry()
		{
			dirty = false;
		}
		
		
		/// <summary>
		/// Renders the overlay portion of the renderable, 
		/// </summary>
		/// <param name="viewport"> A <see cref="Viewport"/> to render to. </param>
		public virtual void RenderOverlay(Viewport viewport)
		{			
			if (dirty)
				ComputeGeometry();
		}
		
		
		/// <summary>
		/// Allows the renderable to directly handle a pan event.
		/// </summary>
		/// <param name="viewport"> The <see cref="Viewport"/> on which the interaction was performed. </param>
		/// <param name="dx"> The travel in the x screen dimension.</param>
		/// <param name="dy"> The travel in the y screen dimension.</param>
		/// <returns> True to block the viewport from dealing with the interaction itself. </returns>
		public virtual bool HandlePan(Viewport viewport, double dx, double dy)
		{
			return false;
		}		
		
		/// <summary>
		/// Allows the renderable to directly handle a dolly event.
		/// </summary>
		/// <param name="viewport"> The <see cref="Viewport"/> on which the interaction was performed. </param>
		/// <param name="factor"> The dolly factor.</param>
		/// <returns> True to block the viewport from dealing with the interaction itself. </returns>
		public virtual bool HandleDolly(Viewport viewport, double factor)
		{
			return false;
		}

        /// <summary>
        /// Allows the renderable to directly handle a zoom event.
        /// </summary>
        /// <param name="viewport"> The <see cref="Viewport"/> on which the interaction was performed. </param>
        /// <param name="rubberBand"> The rubber band encompassing the zoom.</param>
        /// <returns> True to block the viewport from dealing with the interaction itself. </returns>
        public virtual bool HandleZoom(Viewport viewport, RubberBand rubberBand)
        {
            return false;
		}

#endregion


#region Hit Test and selection


		protected HitState hitState = HitState.None;
		
		/// <value>
		/// Whether the renderable is selected.
		/// </value>
		public bool IsSelected
		{
			get { return hitState == HitState.Selected; }
			set
			{
//				if (value != IsSelected)
//					MakeDirty();
				if (value)
					hitState = HitState.Selected;
				else
					hitState = HitState.None;
			}
		}
		
		/// <summary>
		/// Sets IsSelected to true.
		/// </summary>
		public virtual void Select()
		{
			IsSelected = true;
		}
		
		/// <summary>
		/// Sets IsSelected to false.
		/// </summary>
		public virtual void Deselect()
		{
			IsSelected = false;
		}

		/// <value>
		/// Whether the cursor is hovering over the renderable.
		/// </value>
		public virtual bool IsHovering
		{
			get {return hitState == HitState.Hovering;}
			set
			{
//				if (value != IsHovering)
//					MakeDirty();
				if (value && hitState != HitState.Selected)
					hitState = HitState.Hovering;
				else if (hitState == HitState.Hovering)
					hitState = HitState.None;
			}
		}

		/// <summary>
		/// Makes the selected state opposite of what it was before.
		/// </summary>
		public void ToggleSelection()
		{
			IsSelected = !IsSelected;
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
		
		
#region Mouse Handling
				
		public virtual void OnButtonPress(MouseButtonEvent evt) {}
		
		public virtual void OnButtonRelease(MouseButtonEvent evt) {}

		public virtual void OnMouseMotion(MouseEvent evt) { }

		public virtual void OnMouseWheel(MouseWheelEvent evt) { }
				
#endregion

	}
	
}
