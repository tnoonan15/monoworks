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

namespace MonoWorks.Controls.Cards
{
	/// <summary>
	/// A card that is represented as a pane in the 3D world. It can contain CardContents and other cards.
	/// </summary>
	public class Card : ActorPane
	{
		public Card()
		{
			Scaling = 1;
			Padding = 100;
		}
		
		/// <summary>
		/// Create the card and assign its contents.
		/// </summary>
		public Card(CardContents card) : base(card)
		{
			Padding = 100;
		}
		
		/// <summary>
		/// The card book this card belongs to.
		/// </summary>
		public CardBook CardBook
		{
			get {
				if (this is CardBook)
					return this as CardBook;
				if (Parent is Card)
					return (Parent as Card).CardBook;
				return null;
			}
		}
		
		/// <summary>
		/// This card's coordinate in the grid system.
		/// </summary>
		[MwxProperty]
		public IntCoord GridCoord { get; set; }
		
		
		#region Children
		
		protected List<Card> _children = new List<Card>();

		/// <summary>
		/// The number of children the card has.
		/// </summary>
		public int NumChildren
		{
			get { return _children.Count; }
		}

		/// <summary>
		/// Adds a card as a child.
		/// </summary>
		public void Add(Card card)
		{
			_children.Add(card);
		}

		/// <summary>
		/// Removes a card child.
		/// </summary>
		public void Remove(Card card)
		{
			_children.Remove(card);
		}

		public override void AddChild(IMwxObject child)
		{
			if (child is Card)
				Add(child as Card);
			else if (child is CardContents)
				Control = child as CardContents;
			else
				throw new Exception(child.Name + " must be a Card or CardContents.");
		}

		/// <summary>
		/// Returns the card collection.
		/// </summary>
		public override IEnumerable<IMwxObject> GetMwxChildren()
		{
			return _children as IEnumerable<IMwxObject>;
		}

		/// <summary>
		/// Find all cards in the given row.
		/// </summary>
		public IEnumerable<Card> FindByRow(int row)
		{
			return from card in _children
				where card.GridCoord.Y == row
				select card;
		}

		/// <summary>
		/// Find all cards in the given column.
		/// </summary>
		public IEnumerable<Card> FindByColumn(int column)
		{
			return from card in _children
				where card.GridCoord.X == column
				select card;
		}

		/// <summary>
		/// Finds the child at the given grid coord (if any).
		/// </summary>
		public Card FindByGridCoord(IntCoord coord)
		{
			var res = from card in _children
					where card.GridCoord.X == coord.X && card.GridCoord.Y == coord.Y
					select card;
			if (res.Count() > 0)
				return res.First();
			return null;
		}

		private Card _focusedChild;
		/// <summary>
		/// The child that is currently in focus (if any).
		/// </summary>
		public Card FocusedChild {
			get { return _focusedChild; }
			set	{
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
		/// The start of each grid column, stored during ComputeGeometry().
		/// </summary>
		private double[] _xGrid;

		/// <summary>
		/// The start of each grid row, stored during ComputeGeometry().
		/// </summary>
		private double[] _yGrid;

		public override void ComputeGeometry()
		{
			base.ComputeGeometry();
			
			var book = CardBook;
			if (book == null)
				return;
			
			
			if (ChildrenVisible && _children.Count > 0)
			{
				// compute grid min and max grid coordinates and card geometry
				IntCoord min = null;
				IntCoord max = null;
				foreach (var card in _children) {
					if (min == null) {
						min = card.GridCoord.Copy();
						max = card.GridCoord.Copy();
					}
					else {
						min.Min(card.GridCoord);
						max.Max(card.GridCoord);
					}
					card.Origin.Z = Origin.Z - book.LayerDepth;
					if (card.Control.IsDirty)
						card.Control.ComputeGeometry();
				}
				
				bounds.Reset();
				bounds.Resize(0, 0, 0);
				
				// align the columns
				_xGrid = new double[max.X - min.X + 2];
				_xGrid[0] = 0;
				double x = 0;
				for (int col = min.X; col <= max.X; col++) 
				{
					// determine the width of the column and set the x positions
					var thisCol = FindByColumn(col);
					double width = 0;
					foreach (var card in thisCol) {
						if (card.Control.RenderWidth > width) {
							width = card.Control.RenderWidth;
						}
						card.Origin.X = x;
					}
					x += width + Padding;
					_xGrid[col - min.X + 1] = x;
				}
				
				// align the rows
				_yGrid = new double[max.Y - min.Y + 2];
				_yGrid[0] = 0;
				double y = 0;
				for (int row = min.Y; row <= max.Y; row++)
				{
					// determine the height of the row and set the y positions
					var thisRow = FindByRow(row);
					double height = 0;
					foreach (var card in thisRow) {
						if (card.Control.RenderHeight > height) {
							height = card.Control.RenderHeight;
						}
						card.Origin.Y = y - height;
					}
					y -= height + Padding;
					_yGrid[row - min.Y + 1] = y;
				}
				Array.Reverse(_yGrid);
				
				foreach (var card in _children) {
					card.ComputeGeometry();
				}					
				
				bounds.Resize(x, y, 0);
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
		/// Rounds to the nearest grid coord to the given spatial coordinate.
		/// </summary>
		/// <returns>The card at that location, if any.</returns>
		/// <remarks>The return value is also set as FocusedChild.</remarks>
		public Card RoundToNearestGrid(Coord coord)
		{
			IntCoord gridCoord = new IntCoord();

			// compute the x coordinate
			for (int i = 0; i < _xGrid.Length; i++)
			{
				if (_xGrid[i] >= coord.X) {
					if (i == 0)
						coord.X = (_xGrid[0] + _xGrid[1]) / 2.0;
					else
						coord.X = (_xGrid[i - 1] + _xGrid[i]) / 2.0;
					gridCoord.X = i - 1;
					break;
				}
			}
			if (coord.X > _xGrid.Last())
			{
				coord.X = (_xGrid[_xGrid.Length - 2] + _xGrid.Last()) / 2.0;
				gridCoord.X = _xGrid.Length - 1;
			}
			
			// compute the y coordinate
			for (int i = 0; i < _yGrid.Length; i++)
			{
				if (_yGrid[i] > coord.Y) {
					if (i == 0)
						coord.Y = (_yGrid[0] + _yGrid[1]) / 2.0;
					else
						coord.Y = (_yGrid[i - 1] + _yGrid[i]) / 2.0;
					gridCoord.Y = _yGrid.Length - i - 1;
					break;
				}
			}
			if (coord.Y > _yGrid.Last()) {
				coord.Y = (_yGrid[_yGrid.Length - 2] + _yGrid.Last()) / 2.0;
				gridCoord.Y = 0;
			}

			_focusedChild = FindByGridCoord(gridCoord);
			return _focusedChild;
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
				bounds.Resize(card.Bounds);
			}

			// don't render ourselves if the children are visible, we won't be able to see them
			if (ChildrenVisible)
				return;

			base.RenderTransparent(scene);
		}
		
		#endregion
		
	}
}
