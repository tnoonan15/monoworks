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
		public Card() : this(new CardContents())
		{
			Control.UserSize = new Coord(300, 300);
		}
		
		/// <summary>
		/// Create the card and assign its contents.
		/// </summary>
		public Card(CardContents card) : base(card)
		{
			Padding = 100;
			Scaling = 1;
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
			card.Parent = this;
			MakeDirty();
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

		/// <summary>
		/// Finds the child at the given spatial position (if any).
		/// </summary>
		public Card FindByPosition(Coord coord)
		{
			var res = from card in _children
				where card.Origin.X <= coord.X && card.Origin.X + card.RenderWidth >= coord.X &&
					card.Origin.Y <= coord.Y && card.Origin.Y + card.RenderHeight >= coord.Y
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
		/// The index of each grid column, stored during ComputeGeometry().
		/// </summary>
		private int[] _xIndex;
		
		/// <summary>
		/// The center of each grid column, stored during ComputeGeometry().
		/// </summary>
		private double[] _xGrid;

		/// <summary>
		/// The index of each grid row, stored during ComputeGeometry().
		/// </summary>
		private int[] _yIndex;

		/// <summary>
		/// The center of each grid row, stored during ComputeGeometry().
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
					Console.WriteLine("origin: {0}, card origin: {1}, layer depth: {2}", Origin, card.Origin, book.LayerDepth);
					card.Origin.Z = Origin.Z - book.LayerDepth;
					if (card.Control.IsDirty)
						card.Control.ComputeGeometry();
				}
				
				bounds.Reset();
				bounds.Resize(0, 0, 0);
				
				// align the columns
				_xIndex = new int[max.X - min.X + 1];
				_xGrid = new double[max.X - min.X + 1];
				double x = 0;
				int i = 0;
				for (int col = min.X; col <= max.X; col++) 
				{
					_xIndex[i] = col;
					_xGrid[i] = x;
					
					// determine the width of the column
					var thisCol = FindByColumn(col);
					double width = 0;
					foreach (var card in thisCol) {
						if (card.Control.RenderWidth > width) {
							width = card.Control.RenderWidth;
						}
					}
					
					// set the x positions
					foreach (var card in thisCol) {
						card.Origin.X = x - width / 2;
					}
					
					x += width + Padding;
					i++;
				}
				
				// align the rows
				_yIndex = new int[max.Y - min.Y + 1];
				_yGrid = new double[max.Y - min.Y + 1];
				double y = 0;
				i = 0;
				for (int row = min.Y; row <= max.Y; row++)
				{
					_yIndex[i] = row;
					_yGrid[i] = y;
					
					// determine the height of the row
					var thisRow = FindByRow(row);
					double height = 0;
					foreach (var card in thisRow) {
						if (card.Control.RenderHeight > height) {
							height = card.Control.RenderHeight;
						}
					}
					
					// set the y positions
					foreach (var card in thisRow) {
						card.Origin.Y = y - height/2;
					}
					
					y += height + Padding;
					i++;
				}
				
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
		public void RoundToNearestGrid(Coord coord)
		{
			// compute the x coordinate
			coord.X = _xGrid.Round(coord.X);
			
			// compute the y coordinate
			coord.Y = _yGrid.Round(coord.Y);
		}
		
		/// <summary>
		/// Finds the grid coord for the given spatial coord.
		/// </summary>
		public IntCoord GetGridCoord(Coord coord)
		{
			var rounded = new Coord(_xGrid.Round(coord.X), _yGrid.Round(coord.Y));
			var gridCoord = new IntCoord();
			gridCoord.X = _xIndex[Array.IndexOf(_xGrid, rounded.X)];
			gridCoord.Y = _yIndex[Array.IndexOf(_yGrid, rounded.Y)];
			return gridCoord;
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
