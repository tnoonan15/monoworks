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
			Scaling = 1/200.0;
		}
		
		/// <summary>
		/// Create the card and assign its contents.
		/// </summary>
		public Card(CardContents card) : base(card)
		{
		}
		
		private List<Card> _children = new List<Card>();

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
			if (child is CardContents)
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

		
		#region Rendering

		public override void RenderOpaque(Scene scene)
		{
			base.RenderOpaque(scene);
			
			foreach (var card in _children)
				card.RenderOpaque(scene);
		}

		public override void RenderTransparent(Scene scene)
		{
			base.RenderTransparent(scene);
			
			foreach (var card in _children)
				card.RenderTransparent(scene);
		}
		
		#endregion
		
	}
}
