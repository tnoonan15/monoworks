// 
//  CardPane.cs - MonoWorks Project
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
using System.Linq;

using MonoWorks.Controls;
using MonoWorks.Base;
using MonoWorks.Rendering;
using MonoWorks.Rendering.Events;

namespace MonoWorks.Controls.Cards
{
	
	public abstract class AbstractCard : ActorPane
	{
		
		public AbstractCard(Renderable2D control) : base(control)
		{
			Padding = 100;
			Scaling = 1;
		}

		/// <summary>
		/// The card book this card belongs to.
		/// </summary>
		public CardBook CardBook {
			get {
				if (this is CardBook)
					return this as CardBook;
				if (Parent is AbstractCard)
					return (Parent as AbstractCard).CardBook;
				return null;
			}
		}

		/// <summary>
		/// This card's coordinate in the grid system.
		/// </summary>
		[MwxProperty]
		public IntCoord GridCoord { get; set; }


		#region Children

		protected List<AbstractCard> _children = new List<AbstractCard>();

		/// <summary>
		/// The number of children the card has.
		/// </summary>
		public int NumChildren {
			get { return _children.Count; }
		}

		/// <summary>
		/// Adds a card as a child.
		/// </summary>
		public void Add(AbstractCard card)
		{
			_children.Add(card);
			card.Parent = this;
			MakeDirty();
		}

		/// <summary>
		/// Removes a card child.
		/// </summary>
		public void Remove(AbstractCard card)
		{
			_children.Remove(card);
			MakeDirty();
		}

		public override void AddChild(IMwxObject child)
		{
			if (child is AbstractCard)
				Add(child as AbstractCard);
			else if (child is Renderable2D)
				Control = child as Renderable2D;
			else
				throw new Exception(child.Name + " must be a Card or CardContents.");
		}

		/// <summary>
		/// Returns the card collection.
		/// </summary>
		public override IList<IMwxObject> GetMwxChildren()
		{
			var children = _children.Cast<IMwxObject>();
			children.Add(Control);
			return children;
		}

		/// <summary>
		/// Find all cards in the given row.
		/// </summary>
		public IEnumerable<AbstractCard> FindByRow(int row)
		{
			return from card in _children
				where card.GridCoord.Y == row
				select card;
		}

		/// <summary>
		/// Find all cards in the given column.
		/// </summary>
		public IEnumerable<AbstractCard> FindByColumn(int column)
		{
			return from card in _children
				where card.GridCoord.X == column
				select card;
		}

		/// <summary>
		/// Finds the child at the given grid coord (if any).
		/// </summary>
		public AbstractCard FindByGridCoord(IntCoord coord)
		{
			var res = from card in _children
				where card.GridCoord.X == coord.X && card.GridCoord.Y == coord.Y
				select card;
			if (res.Count() > 0)
				return res.First();
			return null;
		}

		/// <summary>
		/// Finds the child at the given spatial position (if any).
		/// </summary>
		public AbstractCard FindByPosition(Coord coord)
		{
			var grid = GetGridCoord(coord);
			return FindByGridCoord(grid);
		}

		private AbstractCard _focusedChild;
		/// <summary>
		/// The child that is currently in focus (if any).
		/// </summary>
		public AbstractCard FocusedChild {
			get { return _focusedChild; }
			set {
				if (value == null) {
					_focusedChild = null;
					return;
				}
				if (!_children.Contains(value))
					throw new Exception(String.Format("Card {0} does not belong to {1}", value.Name, Name));
				_focusedChild = value;
			}
		}
		
		#endregion
		

		#region Layout
		
		private bool _childrenVisible = false;
		/// <summary>
		/// If true, the card's children will be visible.
		/// </summary>
		[MwxProperty]
		public bool ChildrenVisible {
			get { return _childrenVisible; }
			set {
				_childrenVisible = value;
				MakeDirty();
			} 
		}

		/// <summary>
		/// The padding between cards.
		/// </summary>
		[MwxProperty]
		public double Padding { get; set; }
		
		/// <summary>
		/// The maximum size of a child card.
		/// </summary>
		public Coord MaxChildSize { get; private set; }
		
		/// <summary>
		/// The total spacing between the center of cards.
		/// </summary>
		public Coord Spacing { get; private set; }
		
		public override void ComputeGeometry()
		{
			base.ComputeGeometry();
			
			var book = CardBook;
			if (book == null)
				return;
			
			if (ChildrenVisible && _children.Count > 0)
			{
				// find out the largest card size to compute the spacing
				MaxChildSize = new Coord();
				foreach (var card in _children) {
					card.Origin.Z = Origin.Z - book.LayerDepth;
					if (card.Control.IsDirty)
						card.Control.ComputeGeometry();
					MaxChildSize.Max(card.Control.RenderSize);
				}
				Spacing = MaxChildSize + Padding;
				
				_bounds.Reset();
				_bounds.Resize(0, 0, 0);
				
				// layout the cards
				foreach (var card in _children) {
					var offset = Spacing * card.GridCoord - card.Control.RenderSize / 2.0;
					card.Origin.X = offset.X;
					card.Origin.Y = offset.Y;
					_bounds.Resize(card.Origin);
					card.ComputeGeometry();
				}
				
			}
			else // arrange children in a stack behind the parent
			{
				Vector offset = new Vector();
				double diff = Padding / 4.0;
				foreach (var card in _children) {
					offset.X += diff;
					offset.Y -= diff;
					offset.Z -= diff;
					card.Origin = Origin + offset;
					card.ComputeGeometry();
				}
			}
			
		}
		
		/// <summary>
		/// Moves the card and its children to the given 2D position.
		/// </summary>
		public void MoveTo(Coord pos)
		{
			Origin.X = pos.X;
			Origin.Y = pos.Y;
			
			if (!ChildrenVisible)
			{
				Vector offset = new Vector();
				double diff = Padding / 4.0;
				foreach (var card in _children) {
					offset.X += diff;
					offset.Y -= diff;
					offset.Z -= diff;
					card.Origin = Origin + offset;
				}
			}
		}
		
		
		/// <summary>
		/// Rounds to the nearest grid coord to the given spatial coordinate.
		/// </summary>
		public void RoundToNearestGrid(Coord coord)
		{
			coord.X = Math.Round(coord.X / Spacing.X) * Spacing.X;
			coord.Y = Math.Round(coord.Y / Spacing.Y) * Spacing.Y;
		}
		
		/// <summary>
		/// Finds the grid coord for the given spatial coord.
		/// </summary>
		public IntCoord GetGridCoord(Coord coord)
		{			
			return new IntCoord((int)Math.Round(coord.X / Spacing.X), (int)Math.Round(coord.Y / Spacing.Y));
		}

		#endregion
		
		
		#region Interaction
		
		public override void OnButtonPress(MouseButtonEvent evt)
		{
			base.OnButtonPress(evt);
			
			if (ChildrenVisible)
			{
				foreach (var child in _children)
					child.OnButtonPress(evt);
			}
		}
		
		public override void OnButtonRelease(MouseButtonEvent evt)
		{
			base.OnButtonRelease(evt);
			
			if (ChildrenVisible)
			{
				foreach (var child in _children)
					child.OnButtonRelease(evt);
			}
		}
		
		public override void OnMouseMotion(MouseEvent evt)
		{
			base.OnMouseMotion(evt);
			
			if (ChildrenVisible)
			{
				foreach (var child in _children)
					child.OnMouseMotion(evt);
			}
		}
		
		public override void OnMouseWheel(MouseWheelEvent evt)
		{
			base.OnMouseWheel(evt);
			
			if (ChildrenVisible)
			{
				foreach (var child in _children)
					child.OnMouseWheel(evt);
			}
			
		}
		
		#endregion
		
		
		#region Rendering

		public override void RenderOpaque(Scene scene)
		{
			base.RenderOpaque(scene);
			
			foreach (var card in _children)
				card.RenderOpaque(scene);
		}

		public override void RenderTransparent(Scene scene)
		{
			// render the children
			foreach (var card in _children)
			{
				card.RenderTransparent(scene);
				_bounds.Resize(card.Bounds);
			}

			// don't render ourselves if the children are visible, we won't be able to see them
			if (ChildrenVisible)
				return;

			base.RenderTransparent(scene);
		}
		
		#endregion
	}
	

	/// <summary>
	/// A card that is represented as a pane in the 3D world. It can contain CardContents and other cards.
	/// </summary>
	public class Card<ContentType> : AbstractCard where ContentType : Renderable2D, new()
	{
		/// <summary>
		/// Default constructor (automatically creates the content).
		/// </summary>
		public Card() : this(new ContentType())
		{
		}
		
		/// <summary>
		/// Create the card and assign its contents.
		/// </summary>
		public Card(ContentType contents) : base(contents)
		{
		}

		
	}
	
	
	
}
