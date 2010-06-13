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

using gl=Tao.OpenGl.Gl;

using MonoWorks.Rendering;
using MonoWorks.Rendering.Interaction;
using MonoWorks.Rendering.Events;
using MonoWorks.Base;

namespace MonoWorks.Controls.Cards
{
	
	/// <summary>
	/// Specifies the interaction mode for a card interactor.
	/// </summary>
	public enum CardInteractionMode {
		Normal, // navigate between cards and levels
		Edit, // editing the contents of on card
		Move // moving a card from one location to another
	};

	/// <summary>
	/// Generic interactor for cards.
	/// </summary>
	/// <remarks>By using this class and specifying a card type, the interactor knows which cards to 
	/// create based on user interactions.</remarks>
	public class CardInteractor<CardType> : AbstractInteractor where CardType : AbstractCard, new()
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
			
			CreateContextMenus();
			
			_levelPlane = new Plane();
			_levelPlane.XAxis.X = 1;
			_levelPlane.Normal.Z = 1;
		}
		
		
		/// <summary>
		/// The card book that this interactor focuses on.
		/// </summary>
		public CardBook CardBook { get; set; }
		
		/// <summary>
		/// The card whos children we are currently viewing.
		/// </summary>
		public AbstractCard CurrentRoot { get; set; }
		
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
		
		/// <summary>
		/// The current interaction mode for the interactor.
		/// </summary>
		public CardInteractionMode Mode { get; protected set;}
		
		
		#region Mouse Interaction
		
		public override void OnButtonPress(MouseButtonEvent evt)
		{
			base.OnButtonPress(evt);
			
			evt.Handle(this);
			
			switch (Mode) {
			case CardInteractionMode.Normal:
			
				if (evt.Button == 1) {
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
						else if (CurrentRoot.Parent is AbstractCard) // try to go up on level
						{
							var oldRoot = CurrentRoot;
							CurrentRoot = CurrentRoot.Parent as AbstractCard;
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
				}
				break;
			
			case CardInteractionMode.Move:
				
				break;
			
			case CardInteractionMode.Edit:
				
				break;
			
			default:
				throw new Exception("Oops, someone forgot to update a switch statement!");
			}
		}
		
		public override void OnButtonRelease(MouseButtonEvent evt)
		{
			base.OnButtonRelease(evt);
			
			switch (Mode) {
			case CardInteractionMode.Normal:
				if (_mouseType == InteractionType.Pan && CardBook != null &&
					(evt.Pos - Anchor).MagnitudeSquared > 4)
				{
					// snap to the nearest grid location
					AnimateToNearest(evt.Scene.Camera);
				}
				else if (evt.Button == 3)
				{
					OnRightClick(evt);
				}
				
				_mouseType = InteractionType.None;
				break;
			
			case CardInteractionMode.Move:
				EndMoveCurrent(evt);
				break;
			
			case CardInteractionMode.Edit:
				
				break;
				
			default:
				throw new Exception("Oops, someone forgot to update a switch statement!");
			}
			
			
			evt.Handle(this);
		}


		public override void OnMouseMotion(MouseEvent evt)
		{
			switch (Mode)
			{
			case CardInteractionMode.Normal:
				if (_mouseType == InteractionType.Pan)
				{
					evt.Scene.Camera.Pan(evt.Pos - lastPos);
				}
				break;
			
			case CardInteractionMode.Move:
				OnMoveCurrent(evt);
				break;
			
			case CardInteractionMode.Edit:
				
				break;
			
			default:
				throw new Exception("Oops, someone forgot to update a switch statement!");
			}
			
			// see if there's an empty card location under the cursor
			if (Mode == CardInteractionMode.Normal || Mode == CardInteractionMode.Move)
			{
				var scenePos = ScenePos(evt);
				var grid = CurrentRoot.GetGridCoord(scenePos);
				var current = CurrentRoot.FindByGridCoord(grid);
				if (current == null)
				{
					CurrentRoot.RoundToNearestGrid(scenePos);
					_emptyCardPos = scenePos;
				}
				else
				{
					_emptyCardPos = null;
				}
			}
			
			evt.Handle(this);
			
			base.OnMouseMotion(evt);
		}
		
		public override void OnMouseWheel(MouseWheelEvent evt)
		{
			base.OnMouseWheel(evt);
			
			switch (Mode)
			{
			case CardInteractionMode.Normal:			
				if (evt.Direction == WheelDirection.Up)
					// zoom in
					AnimateToZoom(evt.Scene.Camera, Zoom + 0.5);
				else
					// zoom out
					AnimateToZoom(evt.Scene.Camera, Zoom - 0.5);
				break;
			
			case CardInteractionMode.Move:
				
				break;
			
			case CardInteractionMode.Edit:
				
				break;
				
			default:
				throw new Exception("Oops, someone forgot to update a switch statement!");
			}
			evt.Handle(this);
		}
		
		#endregion
		
		
		#region Rendering
		
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
		
		private Coord _emptyCardPos;
		
		public override void RenderOpaque(Scene scene)
		{
			base.RenderOpaque(scene);
			
			// render the outline of an empty card location under the cursor
			if (_emptyCardPos != null)
			{
				scene.Lighting.Disable();
				gl.glLineWidth(2);
				gl.glBegin(gl.GL_LINE_LOOP);
				gl.glColor3d(0, 1, 0);
				var pos = new Vector(_emptyCardPos.X - CurrentRoot.MaxChildSize.X / 2.0, 
									_emptyCardPos.Y - CurrentRoot.MaxChildSize.Y / 2.0,
									CurrentRoot.Origin.Z - CurrentRoot.CardBook.LayerDepth);
				pos.glVertex();
				pos.X += CurrentRoot.MaxChildSize.X;
				pos.glVertex();
				pos.Y += CurrentRoot.MaxChildSize.Y;
				pos.glVertex();
				pos.X -= CurrentRoot.MaxChildSize.X;
				pos.glVertex();
				pos.Y -= CurrentRoot.MaxChildSize.Y;
				pos.glVertex();
				gl.glEnd();
			}
		}
		
		#endregion
		

		#region Context Actions

		/// <summary>
		/// The user actions available through the context menus.
		/// </summary>
		public enum ContextAction { Edit, Move, Delete, Copy, Paste, New }

		public Dictionary<ContextAction, RingButton> ContextButtons { get; private set; }
		
		private void CreateContextMenus()
		{
			ContextButtons = new Dictionary<ContextAction, RingButton>();

			OccupiedContextMenu = new RingMenu();
			var editButton = new RingButton(new Image(ResourceHelper.GetStream("edit.png")));
			editButton.Clicked += EditCurrent;
			OccupiedContextMenu.Add(editButton);
			ContextButtons[ContextAction.Edit] = editButton;

			var moveButton = new RingButton(new Image(ResourceHelper.GetStream("transform-move.png")));
			moveButton.Clicked += BeginMoveCurrent;
			OccupiedContextMenu.Add(moveButton);
			ContextButtons[ContextAction.Move] = moveButton;

			var deleteButton = new RingButton(new Image(ResourceHelper.GetStream("edit-delete.png")));
			deleteButton.Clicked += DeleteCurrent;
			OccupiedContextMenu.Add(deleteButton);
			ContextButtons[ContextAction.Delete] = deleteButton;

			var copyButton = new RingButton(new Image(ResourceHelper.GetStream("edit-copy.png")));
			copyButton.Clicked += CopyCurrent;
			OccupiedContextMenu.Add(copyButton);
			ContextButtons[ContextAction.Copy] = copyButton;
			
			EmptyContextMenu = new RingMenu();
			var newButton = new RingButton(new Image(ResourceHelper.GetStream("document-new.png")));
			newButton.Clicked += NewCard;
			EmptyContextMenu.Add(newButton);
			ContextButtons[ContextAction.New] = newButton;
			
			var pasteButton = new RingButton(new Image(ResourceHelper.GetStream("edit-paste.png")));
			pasteButton.Clicked += Paste;
			EmptyContextMenu.Add(pasteButton);
			ContextButtons[ContextAction.Paste] = pasteButton;
			
		}
		
		/// <summary>
		/// The ring menu that appears when the user right clicks on a card.
		/// </summary>
		public RingMenu OccupiedContextMenu {get; private set;}
		
		/// <summary>
		/// The ring menu that appears when the user right clicks on an empty space.
		/// </summary>
		public RingMenu EmptyContextMenu { get; private set; }
		
		/// <summary>
		/// The screen location where the context menu was launched.
		/// </summary>
		protected Coord ContextCoord {get; private set;}
		
		/// <summary>
		/// The card where the last context menu was clicked.
		/// </summary>
		public AbstractCard ContextCard { get; private set; }
		
		protected virtual void OnRightClick(MouseButtonEvent evt)
		{
			if (CurrentRoot == null)
				return;
			ContextCoord = ScenePos(evt);
			ContextCard = CurrentRoot.FindByPosition(ContextCoord);
			Console.WriteLine("context menu at {0} on card {1}", 
				ContextCoord, ContextCard!=null ? ContextCard.Name : "null");
			if (ContextCard == null) 
				// no current card
				EmptyContextMenu.Show(evt);
			else
				// there is a current card
				OccupiedContextMenu.Show(evt);
		}
		
		/// <summary>
		/// Puts the current card in edit mode.
		/// </summary>
		public virtual void EditCurrent(object sender, EventArgs args)
		{
		
		}
		
		/// <summary>
		/// Deletes the current card.
		/// </summary>
		public virtual void DeleteCurrent(object sender, EventArgs args)
		{
			if (ContextCard == null)
				throw new Exception("There's nothing to delete!");
			CurrentRoot.Remove(ContextCard);
		}
		
		/// <summary>
		/// Copies the current card to the internal clipboard.
		/// </summary>
		public virtual void CopyCurrent(object sender, EventArgs args)
		{
			if (ContextCard == null)
				throw new Exception("There's nothing to delete!");
		}
		
		/// <summary>
		/// Pastes the internal clipboard into an empty space.
		/// </summary>
		public virtual void Paste(object sender, EventArgs args)
		{
		
		}
		
		/// <summary>
		/// Creates a new card at the current empty location.
		/// </summary>
		public virtual void NewCard(object sender, EventArgs args)
		{
			if (ContextCard != null)
				throw new Exception("Can't insert a card, there is already one at the current position.");
			var grid = CurrentRoot.GetGridCoord(ContextCoord);
			var card = new CardType() {GridCoord = grid};
			CurrentRoot.Add(card);
		}
		
		#endregion
		
		
		#region Moving Cards
		
		/// <summary>
		/// The card that is currently being moved.
		/// </summary>
		private AbstractCard _movingCard;

		/// <summary>
		/// The Z offset to apply while moving a card.
		/// </summary>
		private const double _moveZOffset = 50;

		/// <summary>
		/// Begins moving the current card.
		/// </summary>
		public virtual void BeginMoveCurrent(object sender, EventArgs args)
		{
			if (ContextCard == null)
				throw new Exception("There's nothing to move!");
			_movingCard = ContextCard;
			Mode = CardInteractionMode.Move;
			_movingCard.Origin.Z += _moveZOffset;
		}
		
		/// <summary>
		/// Handles the intermediate motion of a card.
		/// </summary>
		public void OnMoveCurrent(MouseEvent evt)
		{
			if (_movingCard == null)
				throw new Exception("There's nothing to move!");
			var scenePos = ScenePos(evt) - _movingCard.RenderSize/2.0;
			_movingCard.MoveTo(scenePos);
		}
		
		/// <summary>
		/// Stops moving the current card.
		/// </summary>
		public virtual void EndMoveCurrent(MouseEvent evt)
		{
			var newCoord = ScenePos(evt);
			var existingCard = CurrentRoot.FindByPosition(newCoord);
			var newGrid = CurrentRoot.GetGridCoord(newCoord);
			if (existingCard != null)
			{
				existingCard.GridCoord = _movingCard.GridCoord;
			}
			_movingCard.GridCoord = newGrid;
			_movingCard.Origin.Z -= _moveZOffset;
			CurrentRoot.MakeDirty();
			Mode = CardInteractionMode.Normal;
		}
		
		#endregion

			
		#region Camera Motion

		/// <summary>
		/// Store the animation options for each interaction type.
		/// </summary>
		private Dictionary<InteractionType, AnimationOptions> _animationOptions 
			= new Dictionary<InteractionType, AnimationOptions>();

		private IntCoord _currentGridCoord;
		
		/// <summary>
		/// The plane at the current level.
		/// </summary>
		private Plane _levelPlane;
		
		/// <summary>
		/// Gets the 2D coord for the current camera position in the scene's coordinates.
		/// </summary>
		protected Coord ScenePos(MouseEvent evt)
		{
			//			return new Coord(evt.Pos.X + evt.Scene.Camera.Position.X - evt.Scene.Width / 2.0, 
			//				evt.Scene.Height / 2.0 - evt.Pos.Y + evt.Scene.Camera.Position.Y);
			var intersect = evt.HitLine.GetIntersection(_levelPlane);
			return new Coord(intersect.X, intersect.Y);
		}
		
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
			_currentGridCoord = CurrentRoot.GetGridCoord(coord);
			
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
		public void MoveTo(Camera camera, AbstractCard card)
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
			_currentGridCoord = CurrentRoot.GetGridCoord(coord);
			
			// create the animation
			var center = camera.Center.Copy();
			center.X = coord.X;
			center.Y = coord.Y;
			var position = camera.Position.Copy();
			position.X = coord.X;
			position.Y = coord.Y;
			//camera.AnimationEnded += delegate { camera.Configure(); };
			camera.AnimateTo(center, position, camera.UpVector.Copy(), _animationOptions[InteractionType.Pan]);
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
			_levelPlane.Origin.Z = z;
			
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
			_levelPlane.Origin.Z = z;
			//camera.AnimationEnded += delegate { camera.Configure(); };
			camera.AnimateTo(center, position, camera.UpVector.Copy(), _animationOptions[InteractionType.Dolly]);
		}
		
		#endregion
		
		
	}
	
	
}
