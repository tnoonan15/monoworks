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
		/// Interactor used for Edit mode.
		/// </summary>
		public SingleActorInteractor<CardType> EditInteractor { get; set; }
		
		/// <summary>
		/// The card that is currently being edited.
		/// </summary>
		public CardType EditingCard { get; set; }
		
		
		#region Mouse Interaction
		
		public override void OnButtonPress(MouseButtonEvent evt)
		{
			base.OnButtonPress(evt);
			
			evt.Handle(this);			
			
			if (EditInteractor != null && EditingCard != null) // edit
			{				
				if (evt.Button == 1 && evt.Multiplicity == ClickMultiplicity.Double)
					EndEdit();
				else
					EditInteractor.OnButtonPress(evt);
			}
			else if (_movingCard != null) // moving
			{
				
			}
			else // normal
			{
				if (evt.Button == 1) {
					if (evt.Multiplicity == ClickMultiplicity.Double)
					{
						BeginEdit(evt);
					}
					else if (evt.Multiplicity == ClickMultiplicity.Single)
					{
						_mouseType = InteractionType.Pan;
					}
				}
			}
			
		}
		
		public override void OnButtonRelease(MouseButtonEvent evt)
		{
			base.OnButtonRelease(evt);
			
			if (EditInteractor != null && EditingCard != null) // edit
			{
				EditInteractor.OnButtonRelease(evt);
			}
			else if (_movingCard != null) // moving
			{
				EndMoveCurrent(evt);
			}
			else // normal
			{
				if (_mouseType == InteractionType.Pan && CardBook != null &&
					(evt.Pos - Anchor).MagnitudeSquared > 4)
				{
					// snap to the nearest grid location
					AnimateToNearest();
				}
				else if (evt.Button == 3)
				{
					OnRightClick(evt);
				}
				
				_mouseType = InteractionType.None;
			}
			
			evt.Handle(this);
		}


		public override void OnMouseMotion(MouseEvent evt)
		{
			var highlightEmpties = false;
			if (EditingCard != null && EditInteractor != null)
			{
				EditInteractor.OnMouseMotion(evt);
			}
			else if (_movingCard != null) // moving
			{
				OnMoveCurrent(evt);
				highlightEmpties = true;
			}
			else // normal
			{
				if (_mouseType == InteractionType.Pan)
				{
					Camera.Pan(evt.Pos - lastPos);
				}
				highlightEmpties = true;
			}
			
			// see if there's an empty card location under the cursor
			if (highlightEmpties)
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
			
			if (EditingCard != null && EditInteractor != null)
			{
				EditInteractor.OnMouseWheel(evt);
			}
			else if (_movingCard != null) // moving
			{
			}
			else // normal
			{
				if (evt.Direction == WheelDirection.Up)
					// zoom in
					AnimateToZoom(Zoom + 0.5);
				else
					// zoom out
					AnimateToZoom(Zoom - 0.5);
			}
			
			evt.Handle(this);
		}
		
		#endregion
		
		
		#region Rendering
		
		public override void OnSceneResized(Scene scene)
		{
			base.OnSceneResized(scene);
			
			MoveToZoom(Zoom);
		}		

		private bool _isInitialized = false;
		
		public override void RenderOverlay(Scene scene)
		{
			base.RenderOverlay(scene);
			
			if (!_isInitialized) {
				_isInitialized = true;
				InitCamera(scene.Camera);
			}
		
			if (EditingCard != null && EditInteractor != null)
				EditInteractor.RenderOverlay(scene);
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
			
		
			if (EditingCard != null && EditInteractor != null)
				EditInteractor.RenderOpaque(scene);
		}
		
		
		public override void RenderTransparent(Scene scene)
		{
			base.RenderTransparent(scene);
			
			// gray out the other cards if there's one being edited
			if (EditingCard != null && EditInteractor != null)
			{
				scene.Lighting.Disable();
				gl.glColor4d(0.5, 0.5, 0.5, 0.5);
				gl.glBegin(gl.GL_QUADS);
				var pos = EditingCard.Origin.Copy();
				pos.Z = CurrentRoot.Origin.Z - CardBook.LayerDepth;
				
				pos.Y -= scene.Height;
				pos.X -= scene.Width;
				pos.glVertex();
				pos.Y += 2 * scene.Height;
				pos.glVertex();
				pos.X += 2 * scene.Width;
				pos.glVertex();
				pos.Y -= 2 * scene.Height;
				pos.glVertex();
				pos.X -= 2 * scene.Width;
				pos.glVertex();
				
				gl.glEnd();
				
				EditInteractor.RenderTransparent(scene);				
			}
		}
		
		#endregion
		

		#region Context Actions

		/// <summary>
		/// The user actions available through the context menus.
		/// </summary>
		public enum ContextAction { Edit, Move, Delete, Copy, Paste, New, GoIn, GoOut }

		public Dictionary<ContextAction, RingButton> ContextButtons { get; private set; }
		
		private void CreateContextMenus()
		{
			ContextButtons = new Dictionary<ContextAction, RingButton>();

			OccupiedContextMenu = new RingMenu();
//			var editButton = new RingButton(new Image(ResourceHelper.GetStream("edit.png")));
//			editButton.Clicked += EditCurrent;
//			OccupiedContextMenu.Add(editButton);
//			ContextButtons[ContextAction.Edit] = editButton;

			var moveButton = new RingButton(new Image(ResourceHelper.GetStream("transform-move.png")));
			moveButton.Clicked += BeginMoveCurrent;
			OccupiedContextMenu.Add(moveButton);
			ContextButtons[ContextAction.Move] = moveButton;

			var goInButton = new RingButton(new Image(ResourceHelper.GetStream("go-up.png")));
			goInButton.Clicked += GoInCurrent;
			OccupiedContextMenu.Add(goInButton);
			ContextButtons[ContextAction.GoIn] = goInButton;

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

			var goOutButton = new RingButton(new Image(ResourceHelper.GetStream("go-down.png")));
			goOutButton.Clicked += GoOut;
			EmptyContextMenu.Add(goOutButton);
			ContextButtons[ContextAction.GoOut] = goOutButton;
			
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
			if (ContextCard == null)
			{
				EmptyContextMenu.Show(evt);
			}
			else
			{
				ContextButtons[ContextAction.GoIn].IsEnabled = ContextCard.NumChildren > 0;
				OccupiedContextMenu.Show(evt);
			}
		}
		
		/// <summary>
		/// Makes the current card the root, showing its children.
		/// </summary>
		public virtual void GoInCurrent(object sender, EventArgs args)
		{
			if (ContextCard == null)
				throw new Exception("There's nothing to go into!");
			CurrentRoot = ContextCard;
			CurrentRoot.ChildrenVisible = true;
			CurrentRoot.ComputeGeometry();
			MoveTo(CurrentRoot.FocusedChild);
			AnimateToZoom(Zoom);
		}
		
		/// <summary>
		/// Moves the current root up one level.
		/// </summary>
		public virtual void GoOut(object sender, EventArgs args)
		{
			var oldRoot = CurrentRoot;
			CurrentRoot = CurrentRoot.Parent as AbstractCard;
			MoveTo(CurrentRoot.FocusedChild);
			Camera.AnimationEnded += delegate {
				oldRoot.ChildrenVisible = false;
				CurrentRoot.ComputeGeometry();
			};
			AnimateToZoom(Zoom);
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
		
		
		#region Editing Cards
			
		/// <summary>
		/// Begins editing the card at the given location.
		/// </summary>
		public virtual void BeginEdit(MouseEvent evt)
		{
			var pos = ScenePos(evt);
			var card = CurrentRoot.FindByPosition(pos);
			if (card == null)
				throw new Exception("No card to edit!");
			EditingCard = card as CardType;
			EditingCard.Origin.Z += 1;
			if (EditInteractor != null)
				EditInteractor.Actor = EditingCard;
		}
		
		/// <summary>
		/// Stops any current editing operation.
		/// </summary>
		public virtual void EndEdit()
		{
			EditingCard.Origin.Z -= 1;
			EditingCard = null;
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
			_movingCard = null;
		}
		
		#endregion
		
		
		#region Copying Cards
		
		/// <summary>
		/// The card that will copied during the next paste operation.
		/// </summary>
		public CardType Clipboard { get; private set; }
		
		/// <summary>
		/// Used to perform deep copies for copy/paste functionality.
		/// </summary>
		private MwxDeepCopier _deepCopier = new MwxDeepCopier();
		
		/// <summary>
		/// Copies the current card to the internal clipboard.
		/// </summary>
		public virtual void CopyCurrent(object sender, EventArgs args)
		{
			if (ContextCard == null)
				throw new Exception("There's nothing to delete!");
			Clipboard = ContextCard as CardType;
		}
		
		/// <summary>
		/// Pastes the internal clipboard into an empty space.
		/// </summary>
		public virtual void Paste(object sender, EventArgs args)
		{
			if (ContextCard != null)
				throw new Exception("Can't insert a card, there is already one at the current position.");
			var grid = CurrentRoot.GetGridCoord(ContextCoord);
			var card = _deepCopier.DeepCopy<CardType>(Clipboard);
			card.GridCoord = grid;
			CurrentRoot.Add(card);
		}		
		
		#endregion

			
		#region Camera Motion

		/// <summary>
		/// Store the animation options for each interaction type.
		/// </summary>
		private Dictionary<InteractionType, AnimationOptions> _animationOptions 
			= new Dictionary<InteractionType, AnimationOptions>();
		
		/// <summary>
		/// The plane at the current level.
		/// </summary>
		private Plane _levelPlane;
		
		/// <summary>
		/// Gets the 2D coord for the current camera position in the scene's coordinates.
		/// </summary>
		protected Coord ScenePos(MouseEvent evt)
		{
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
			MoveToZoom(1);
			MoveToNearest();
			camera.UpVector = new Vector(0, 1, 0);
		}
				
		/// <summary>
		/// Instantly moves the camera to the nearest card.
		/// </summary>
		public void MoveToNearest()
		{
			// get the nearest grid point
			if (CurrentRoot == null)
				CurrentRoot = CardBook;
			var coord = new Coord(Camera.Position.X, Camera.Position.Y);
			CurrentRoot.RoundToNearestGrid(coord);
			CurrentRoot.FocusedChild = CurrentRoot.FindByPosition(coord);
			
			// move the camera
			Camera.Center.X = coord.X;
			Camera.Center.Y = coord.Y;
			Camera.Position.X = coord.X;
			Camera.Position.Y = coord.Y;
		}

		/// <summary>
		/// Moves to look at the given card.
		/// </summary>
		/// <remarks>If the card is null, moves to the nearest grid point.</remarks>
		public void MoveTo(AbstractCard card)
		{
			if (card == null)
				MoveToNearest();
			else
			{
				var x = card.Origin.X + card.RenderWidth / 2.0;
				var y = card.Origin.Y + card.RenderHeight / 2.0;
				Camera.Center.X = x;
				Camera.Center.Y = y;
				Camera.Position.X = x;
				Camera.Position.Y = y;
			}
		}

		/// <summary>
		/// Animates the camera to the nearest card.
		/// </summary>
		public void AnimateToNearest()
		{
			// get the nearest grid point
			if (CurrentRoot == null)
				CurrentRoot = CardBook;
			var coord = new Coord(Camera.Position.X, Camera.Position.Y);
			CurrentRoot.RoundToNearestGrid(coord);
			CurrentRoot.FocusedChild = CurrentRoot.FindByPosition(coord);
			
			// create the animation
			var center = Camera.Center.Copy();
			center.X = coord.X;
			center.Y = coord.Y;
			var position = Camera.Position.Copy();
			position.X = coord.X;
			position.Y = coord.Y;
			Camera.AnimateTo(center, position, Camera.UpVector.Copy(), _animationOptions[InteractionType.Pan]);
		}
		
		/// <summary>
		/// Instantly moves the camera to the given zoom level.
		/// </summary>
		public void MoveToZoom(double zoom)
		{
			Zoom = zoom;
			
			// determine how far back the camera needs to be for the zoom level
			var offset = Camera.ViewportHeight / (Camera.FoV / 2.0).Tan() / zoom / 2;
			
			// get the z position of the current level
			if (CurrentRoot == null)
				CurrentRoot = CardBook;
			var z = CurrentRoot.Origin.Z - CardBook.LayerDepth;
			
			// move the camera
			Camera.Position.Z = z + offset;
			Camera.Center = Camera.Position.Copy();
			Camera.Center.Z = z;
			_levelPlane.Origin.Z = z;
			
			Camera.Configure();
		}

		/// <summary>
		/// Animates the camera to the given zoom level.
		/// </summary>
		public void AnimateToZoom(double zoom)
		{
			Zoom = zoom;
			
			// determine how far back the camera needs to be for the zoom level
			var offset = Camera.ViewportHeight / (Camera.FoV / 2.0).Tan() / Zoom / 2;
			
			// get the z position of the current level
			if (CurrentRoot == null)
				CurrentRoot = CardBook;
			var z = CurrentRoot.Origin.Z - CardBook.LayerDepth;
			
			// create the animation
			var position = Camera.Position.Copy();
			position.Z = z + offset;
			var center = position.Copy();
			center.Z = z;
			_levelPlane.Origin.Z = z;
			Camera.AnimateTo(center, position, Camera.UpVector.Copy(), _animationOptions[InteractionType.Dolly]);
		}
		
		#endregion
		
		
	}
	
	
}
