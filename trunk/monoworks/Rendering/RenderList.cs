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
using System.Linq;
using System.Collections.Generic;

namespace MonoWorks.Rendering
{
	/// <summary>
	/// Holds the list of renderables to render.
	/// </summary>
	public class RenderList
	{

		#region Actors

		private readonly List<Actor> _actors = new List<Actor>();
		/// <summary>
		/// Actors to render.
		/// </summary>
		public IEnumerable<Actor> Actors
		{
			get { return _actors; }
		}

		/// <summary>
		/// A copy of the actor list.
		/// </summary>
		/// <remarks>Use this to iterate when the list might be modified.</remarks>
		public Actor[] ActorCopy
		{
			get
			{
				var copy = new Actor[ActorCount];
				_actors.CopyTo(copy);
				return copy;
			}
		}

		/// <summary>
		/// Adds a actor to the rendering list.
		/// </summary>
		/// <param name="actor"> A <see cref="Renderable"/>. </param>
		public void AddActor(Actor actor)
		{
			if (!_actors.Contains(actor))
				_actors.Add(actor);
		}

		/// <summary>
		/// Removes a actor from the rendering list.
		/// </summary>
		/// <param name="actor"> A <see cref="Renderable"/>. </param>
		public void RemoveActor(Actor actor)
		{
			if (!_actors.Contains(actor))
				throw new Exception("The actor is not a part of this scene's rendering list.");
			_actors.Remove(actor);
		}
		
		/// <value>
		/// The number of actors.
		/// </value>
		public int ActorCount
		{
			get {return _actors.Count;}
		}

		/// <value>
		/// The bounds of all actors.
		/// </value>
		public Bounds Bounds
		{
			get
			{
				var bounds = new Bounds();
				foreach (Actor actor in _actors)
					bounds.Resize(actor.Bounds);
				return bounds;
			}
		}

		/// <summary>
		/// Reset the bounds of all actors.
		/// </summary>
		public void ResetBounds()
		{
			foreach (Actor actor in _actors)
				actor.ResetBounds();
		}

		#endregion


		#region Overlays

		private readonly List<Overlay> _overlays = new List<Overlay>();
		/// <summary>
		/// Overlays to render.
		/// </summary>
		/// <remarks>If the overlay list might be modified during iteration, use OverlayCopy instead.</remarks>
		public IEnumerable<Overlay> Overlays
		{
			get { return _overlays; }
		}

		/// <summary>
		/// A copy of the overlay list.
		/// </summary>
		/// <remarks>Use this to iterate when the list might be modified.</remarks>
		public Overlay[] OverlayCopy
		{
			get
			{
				var overlayCopy = new Overlay[OverlayCount];
				_overlays.CopyTo(overlayCopy);
				return overlayCopy;
			}
		}

		/// <summary>
		/// Add an overlay to the scene.
		/// </summary>
		public void AddOverlay(Overlay overlay)
		{
			_overlays.Add(overlay);
		}

		/// <summary>
		/// Remove an overlay from the scene.
		/// </summary>
		public void RemoveOverlay(Overlay overlay)
		{
			_overlays.Remove(overlay);
		}
		
		/// <value>
		/// The number of overlays.
		/// </value>
		public int OverlayCount
		{
			get {return _overlays.Count;}
		}

		#endregion

		
		#region Modal Overlays
		
		private readonly Stack<ModalOverlay> _modals = new Stack<ModalOverlay>();
		/// <summary>
		/// Modal overlays to render.
		/// </summary>
		public IEnumerable<ModalOverlay> Modals
		{
			get { return _modals; }
		}
		
		
		public ModalOverlay[] ModalsCopy
		{
			get
			{
				var copy = new ModalOverlay[ModalCount];
				_modals.CopyTo(copy, 0);
				return copy;
			}
		}
		
		/// <summary>
		/// The top of the modal overlay stack. 
		/// </summary>
		public ModalOverlay TopModal
		{
			get {return _modals.Peek();}
		}

		/// <summary>
		/// Add a modal overlay to the scene.
		/// </summary>
		public void PushModal(ModalOverlay overlay)
		{
			_modals.Push(overlay);
		}

		/// <summary>
		/// Removes a modal overlay from the scene.
		/// </summary>
		public void PopModal(ModalOverlay overlay)
		{
			while (_modals.Count > 0)
			{
				var popped = _modals.Pop();
				if (popped == overlay)
					break;
				if (_modals.Count == 0)
					throw new Exception("Error popping modal. The render list doesn't contain the given modal overlay");
			}
		}
		
		/// <value>
		/// The number of modal overlays.
		/// </value>
		public int ModalCount
		{
			get {return _modals.Count;}
		}
		
		
		
		#endregion
		

		#region Rendering

		public void Render(Scene scene)
		{
			scene.RenderManager.BeginSolids();
			scene.Camera.Place(); // place the camera for 3D rendering

			foreach (var actor in _actors)
			{
				if (actor.IsVisible)
					actor.RenderOpaque(scene);
			}

			foreach (var actor in _actors)
			{
				if (actor.IsVisible)
					actor.RenderTransparent(scene);
			}

			scene.Camera.PlaceOverlay(); // place the camera for overlay rendering
			foreach (var actor in _actors)
			{
				if (actor.IsVisible)
					actor.RenderOverlay(scene);
			}

			// render the overlays
			scene.RenderManager.BeginOverlays();
			foreach (var overlay in _overlays)
			{
				if (overlay.IsVisible)
					overlay.RenderOverlay(scene);
			}
			
			// render the modal overlays
			foreach (var modal in _modals.Reverse())
			{
				if (modal.IsVisible)
					modal.RenderOverlay(scene);
			}
			
		}

		#endregion


		/// <summary>
		/// The scene should call this when it's resized.
		/// </summary>
		public void OnSceneResized(Scene scene)
		{
			foreach (var actor in Actors)
				actor.OnSceneResized(scene);
			
			foreach (var overlay in Overlays)
				overlay.OnSceneResized(scene);
			
			foreach (var modal in Modals)
				modal.OnSceneResized(scene);
		}

	}
}
