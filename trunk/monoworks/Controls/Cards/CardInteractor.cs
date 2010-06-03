// 
//  CardInteractor.cs - MonoWorks Project
//  
//  Author:
//       Andy Selvig <ajselvig@gmail.com>
// 
//  Copyright (c) 2010 Andy Selvig
// 
//  This library is free software; you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as
//  published by the Free Software Foundation; either version 2.1 of the
//  License, or (at your option) any later version.
// 
//  This library is distributed in the hope that it will be useful, but
//  WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
//  Lesser General Public License for more details.
// 
//  You should have received a copy of the GNU Lesser General Public
//  License along with this library; if not, write to the Free Software
//  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA

using System;
using System.Collections.Generic;

using MonoWorks.Rendering;
using MonoWorks.Rendering.Interaction;
using MonoWorks.Rendering.Events;
using MonoWorks.Base;

namespace MonoWorks.Controls.Cards
{

	/// <summary>
	/// Interactor for cards.
	/// </summary>
	public class CardInteractor : AbstractInteractor
	{

		public CardInteractor(Scene scene) : base(scene)
		{
			_mouseType = InteractionType.None;

			_animationOptions[InteractionType.Pan] = new AnimationOptions() {
				Duration = 2,
				EaseType = EaseType.Quadratic
			};
			_animationOptions[InteractionType.Dolly] = new AnimationOptions() {
				Duration = 1.5,
				EaseType = EaseType.Quadratic
			};
		}
		
		
		/// <summary>
		/// The card book that this interactor focuses on.
		/// </summary>
		public CardBook CardBook { get; set; }
		
		/// <summary>
		/// The card whos children we are currently viewing.
		/// </summary>
		public Card CurrentRoot { get; set; }
		
		private double _zoom = 1;
		/// <summary>
		/// The factor between screen coordinates and card coordinates.
		/// </summary>
		public double Zoom { 
			get {return _zoom;}
			set {
				_zoom = Math.Min(MaxZoom, Math.Max(MinZoom, value));
			}
		}

		/// <summary>
		/// The minimum allowed zoom level (most zoomed out).
		/// </summary>
		public const double MinZoom = 0.25;

		/// <summary>
		/// The maximum allowed zoom level (most zoomed in).
		/// </summary>
		public const double MaxZoom = 2.0;
		
		private InteractionType _mouseType;
		
		public override void OnButtonPress(MouseButtonEvent evt)
		{
			base.OnButtonPress(evt);
			
			evt.Handle(this);
			
			switch (evt.Button) {
			case 1:
				if (evt.Multiplicity == ClickMultiplicity.Double)
				{
					if (CurrentRoot.FocusedChild != null && CurrentRoot.FocusedChild.HitTest(evt.HitLine))
					{
						if (CurrentRoot.FocusedChild.NumChildren > 0) // go down one level
						{
							CurrentRoot = CurrentRoot.FocusedChild;
							CurrentRoot.ChildrenVisible = true;
							CurrentRoot.ComputeGeometry();
							MoveTo(evt.Scene.Camera, CurrentRoot.FocusedChild);
							AnimateToZoom(evt.Scene.Camera, Zoom);
						}
					}
					else if (CurrentRoot.Parent is Card) // try to go up on level
					{
						var oldRoot = CurrentRoot;
						CurrentRoot = CurrentRoot.Parent as Card;
						MoveTo(evt.Scene.Camera, CurrentRoot.FocusedChild);
						evt.Scene.Camera.AnimationEnded += delegate {
							oldRoot.ChildrenVisible = false;
							CurrentRoot.ComputeGeometry();
						};
						AnimateToZoom(evt.Scene.Camera, Zoom);
					}
				}
				else if (evt.Multiplicity == ClickMultiplicity.Single)
				{
					_mouseType = InteractionType.Pan;
				}
				break;
			}
		}
		
		public override void OnButtonRelease(MouseButtonEvent evt)
		{
			base.OnButtonRelease(evt);
			
			// snap to the nearest grid location
			if (_mouseType == InteractionType.Pan && CardBook != null &&
				(evt.Pos - Anchor).MagnitudeSquared > 4)
			{
				AnimateToNearest(evt.Scene.Camera);
			}
			
			_mouseType = InteractionType.None;
			
			evt.Handle(this);
		}


		public override void OnMouseMotion(MouseEvent evt)
		{
			
			if (_mouseType == InteractionType.Pan)
			{
				evt.Scene.Camera.Pan(evt.Pos - lastPos);
			}
			
			evt.Handle(this);
			
			base.OnMouseMotion(evt);
		}
		
		public override void OnMouseWheel(MouseWheelEvent evt)
		{
			base.OnMouseWheel(evt);
			
			if (evt.Direction == WheelDirection.Up)
				// zoom in
				AnimateToZoom(evt.Scene.Camera, Zoom + 0.5);
			else
				// zoom out
				AnimateToZoom(evt.Scene.Camera, Zoom - 0.5);
			evt.Handle(this);
		}
		
		
		public override void OnSceneResized(Scene scene)
		{
			base.OnSceneResized(scene);
			
			MoveToZoom(scene.Camera, Zoom);
		}
		

		private bool _isInitialized = false;
		
		public override void RenderOverlay(Scene scene)
		{
			base.RenderOverlay(scene);
			
			if (!_isInitialized) {
				_isInitialized = true;
				InitCamera(scene.Camera);
			}
		}


			
		#region Camera Motion

		/// <summary>
		/// Store the animation options for each interaction type.
		/// </summary>
		private Dictionary<InteractionType, AnimationOptions> _animationOptions 
			= new Dictionary<InteractionType, AnimationOptions>();

		/// <summary>
		/// Sets the animation options to use for the given type of interaction.
		/// </summary>
		public void SetAnimationOptions(InteractionType type, AnimationOptions opts)
		{
			_animationOptions[type] = opts;
		}
		
		/// <summary>
		/// Initializes the camera for interacting with cards.
		/// </summary>
		public void InitCamera(Camera camera)
		{
			MoveToZoom(camera, 1);
			MoveToNearest(camera);
			camera.UpVector = new Vector(0, 1, 0);
		}
				
		/// <summary>
		/// Instantly moves the camera to the nearest card.
		/// </summary>
		public void MoveToNearest(Camera camera)
		{
			// get the nearest grid point
			if (CurrentRoot == null)
				CurrentRoot = CardBook;
			var coord = new Coord(camera.Position.X, camera.Position.Y);
			CurrentRoot.RoundToNearestGrid(coord);
			CurrentRoot.FocusedChild = CurrentRoot.FindByPosition(coord);
			
			// move the camera
			camera.Center.X = coord.X;
			camera.Center.Y = coord.Y;
			camera.Position.X = coord.X;
			camera.Position.Y = coord.Y;
		}

		/// <summary>
		/// Moves to look at the given card.
		/// </summary>
		/// <remarks>If the card is null, moves to the nearest grid point.</remarks>
		public void MoveTo(Camera camera, Card card)
		{
			if (card == null)
				MoveToNearest(camera);
			else
			{
				var x = card.Origin.X + card.RenderWidth / 2.0;
				var y = card.Origin.Y + card.RenderHeight / 2.0;
				camera.Center.X = x;
				camera.Center.Y = y;
				camera.Position.X = x;
				camera.Position.Y = y;
			}
		}

		/// <summary>
		/// Animates the camera to the nearest card.
		/// </summary>
		public void AnimateToNearest(Camera camera)
		{
			// get the nearest grid point
			if (CurrentRoot == null)
				CurrentRoot = CardBook;
			var coord = new Coord(camera.Position.X, camera.Position.Y);
			CurrentRoot.RoundToNearestGrid(coord);
			CurrentRoot.FocusedChild = CurrentRoot.FindByPosition(coord);
			
			// create the animation
			var center = camera.Center.Copy();
			center.X = coord.X;
			center.Y = coord.Y;
			var position = camera.Position.Copy();
			position.X = coord.X;
			position.Y = coord.Y;
			camera.AnimateTo(center, position, camera.UpVector, _animationOptions[InteractionType.Pan]);
		}
		
		/// <summary>
		/// Instantly moves the camera to the given zoom level.
		/// </summary>
		public void MoveToZoom(Camera camera, double zoom)
		{
			Zoom = zoom;
			
			// determine how far back the camera needs to be for the zoom level
			var offset = camera.ViewportHeight / (camera.FoV / 2.0).Tan() / zoom / 2;
			
			// get the z position of the current level
			if (CurrentRoot == null)
				CurrentRoot = CardBook;
			var z = CurrentRoot.Origin.Z - CardBook.LayerDepth;
			
			// move the camera
			camera.Position.Z = z + offset;
			camera.Center = camera.Position.Copy();
			camera.Center.Z = z;
			
			camera.Configure();
		}

		/// <summary>
		/// Animates the camera to the given zoom level.
		/// </summary>
		public void AnimateToZoom(Camera camera, double zoom)
		{
			//if (Zoom == zoom)
			//    return;
			Zoom = zoom;
			
			// determine how far back the camera needs to be for the zoom level
			var offset = camera.ViewportHeight / (camera.FoV / 2.0).Tan() / Zoom / 2;
			
			// get the z position of the current level
			if (CurrentRoot == null)
				CurrentRoot = CardBook;
			var z = CurrentRoot.Origin.Z - CardBook.LayerDepth;
			
			// create the animation
			var position = camera.Position.Copy();
			position.Z = z + offset;
			var center = position.Copy();
			center.Z = z;
			camera.AnimateTo(center, position, camera.UpVector, _animationOptions[InteractionType.Dolly]);
		}
		
		#endregion
		
		
	}
}
