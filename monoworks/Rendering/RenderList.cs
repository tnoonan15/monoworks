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

		#region Actors

		protected List<Actor> actors = new List<Actor>();
		/// <summary>
		/// Actors to render.
		/// </summary>
		public IEnumerable<Actor> Actors
		{
			get { return actors; }
		}

		/// <summary>
		/// Adds a actor to the rendering list.
		/// </summary>
		/// <param name="actor"> A <see cref="Renderable"/>. </param>
		public void AddActor(Actor actor)
		{
			if (!actors.Contains(actor))
				actors.Add(actor);
		}

		/// <summary>
		/// Removes a actor from the rendering list.
		/// </summary>
		/// <param name="actor"> A <see cref="Renderable"/>. </param>
		public void RemoveActor(Actor actor)
		{
			if (!actors.Contains(actor))
				throw new Exception("The actor is not a part of this viewport's rendering list.");
			actors.Remove(actor);
		}
		
		/// <value>
		/// The number of actors.
		/// </value>
		public int ActorCount
		{
			get {return actors.Count;}
		}

		/// <value>
		/// The bounds of all actors.
		/// </value>
		public Bounds Bounds
		{
			get
			{
				Bounds bounds = new Bounds();
				foreach (Actor actor in actors)
					bounds.Resize(actor.Bounds);
				return bounds;
			}
		}

		/// <summary>
		/// Reset the bounds of all actors.
		/// </summary>
		public void ResetBounds()
		{
			foreach (Actor actor in actors)
				actor.ResetBounds();
		}

		#endregion


		#region Overlays

		protected List<Overlay> overlays = new List<Overlay>();
		/// <summary>
		/// Overlays to render.
		/// </summary>
		/// <remarks>If the overlay list might be modified during iteration, use OverlayCopy instead.</remarks>
		public IEnumerable<Overlay> Overlays
		{
			get { return overlays; }
		}

		/// <summary>
		/// A copy of the overlay list.
		/// </summary>
		/// <remarks>Use this to iterate when the list might be modified.</remarks>
		public Overlay[] OverlayCopy
		{
			get
			{
				Overlay[] overlayCopy = new Overlay[OverlayCount];
				overlays.CopyTo(overlayCopy);
				return overlayCopy;
			}
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
			viewport.RenderManager.BeginSolids();
			viewport.Camera.Place(); // place the camera for 3D rendering

			foreach (Actor actor in actors)
			{
				if (actor.IsVisible)
					actor.RenderOpaque(viewport);
			}

			foreach (Actor actor in actors)
			{
				if (actor.IsVisible)
					actor.RenderTransparent(viewport);
			}

			viewport.Camera.PlaceOverlay(); // place the camera for overlay rendering
			foreach (Actor actor in actors)
			{
				if (actor.IsVisible)
					actor.RenderOverlay(viewport);
			}

			// render the overlays
			viewport.RenderManager.BeginOverlays();
			foreach (Overlay overlay in overlays)
			{
				if (overlay.IsVisible)
					overlay.RenderOverlay(viewport);
			}
		}

		#endregion


		/// <summary>
		/// The viewport should call this when it's resized.
		/// </summary>
		/// <param name="viewport"> </param>
		public void OnViewportResized(Viewport viewport)
		{
			foreach (Actor actor in Actors)
				actor.OnViewportResized(viewport);
			
			foreach (Overlay overlay in Overlays)
				overlay.OnViewportResized(viewport);
		}

	}
}
