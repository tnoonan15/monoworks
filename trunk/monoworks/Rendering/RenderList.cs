// RenderList.cs - MonoWorks Project
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

namespace MonoWorks.Rendering
{
	/// <summary>
	/// Holds the list of renderables to render.
	/// </summary>
	public class RenderList
	{
		#region Renderable Registry

		protected List<Renderable3D> renderables = new List<Renderable3D>();
		/// <summary>
		/// 3D Renderables to render.
		/// </summary>
		public IEnumerable<Renderable3D> Renderables
		{
			get { return renderables; }
		}

		/// <summary>
		/// Adds a renderable to the rendering list.
		/// </summary>
		/// <param name="renderable"> A <see cref="Renderable"/>. </param>
		public void AddRenderable(Renderable3D renderable)
		{
			if (!renderables.Contains(renderable))
				renderables.Add(renderable);
		}

		/// <summary>
		/// Removes a renderable from the rendering list.
		/// </summary>
		/// <param name="renderable"> A <see cref="Renderable"/>. </param>
		public void RemoveRenderable(Renderable3D renderable)
		{
			if (!renderables.Contains(renderable))
				throw new Exception("The renderable is not a part of this viewport's rendering list.");
			renderables.Remove(renderable);
		}
		
		/// <value>
		/// The number of renderables.
		/// </value>
		public int RenderableCount
		{
			get {return renderables.Count;}
		}

		/// <value>
		/// The bounds of all renderables.
		/// </value>
		public Bounds Bounds
		{
			get
			{
				Bounds bounds = new Bounds();
				foreach (Renderable3D renderable in renderables)
					bounds.Resize(renderable.Bounds);
				return bounds;
			}
		}

		/// <summary>
		/// Reset the bounds of all renderables.
		/// </summary>
		public void ResetBounds()
		{
			foreach (Renderable3D renderable in renderables)
				renderable.ResetBounds();
		}

		#endregion


		#region Overlays

		protected List<Overlay> overlays = new List<Overlay>();
		/// <summary>
		/// Overlays to render.
		/// </summary>
		public IEnumerable<Overlay> Overlays
		{
			get { return overlays; }
		}

		/// <summary>
		/// Add an overlay to the viewport.
		/// </summary>
		public void AddOverlay(Overlay overlay)
		{
			overlays.Add(overlay);
		}

		/// <summary>
		/// Remove an overlay from the viewport.
		/// </summary>
		public void RemoveOverlay(Overlay overlay)
		{
			overlays.Remove(overlay);
		}
		
		/// <value>
		/// The number of overlays.
		/// </value>
		public int OverlayCount
		{
			get {return overlays.Count;}
		}

		#endregion


		#region Rendering

		public void Render(Viewport viewport)
		{
			viewport.Camera.Place(); // place the camera for 3D rendering

			viewport.RenderManager.SetupSolidMode();
			
			foreach (Renderable3D renderable in renderables)
				renderable.RenderOpaque(viewport);

			foreach (Renderable3D renderable in renderables)
				renderable.RenderTransparent(viewport);

			viewport.Camera.PlaceOverlay(); // place the camera for overlay rendering
			foreach (Renderable3D renderable in renderables)
				renderable.RenderOverlay(viewport);

			// render the overlays
			viewport.RenderManager.BeginOverlays();
			foreach (Overlay overlay in overlays)
				overlay.RenderOverlay(viewport);
			viewport.RenderManager.EndOverlays();
		}

		#endregion


		/// <summary>
		/// The viewport should call this when it's resized.
		/// </summary>
		/// <param name="viewport"> </param>
		public void OnViewportResized(Viewport viewport)
		{
			foreach (Renderable3D renderable in Renderables)
				renderable.OnViewportResized(viewport);
			
			foreach (Overlay overlay in Overlays)
				overlay.OnViewportResized(viewport);
		}

	}
}
