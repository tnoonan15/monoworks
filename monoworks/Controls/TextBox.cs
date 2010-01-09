// 
//  TextBox.cs
//  
//  Author:
//       Andy Selvig <ajselvig@gmail.com>
// 
//  Copyright (c) 2009 Andy Selvig
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

using Cairo;

using MonoWorks.Base;
using MonoWorks.Rendering;
using MonoWorks.Rendering.Events;

namespace MonoWorks.Controls
{

	/// <summary>
	/// A control that allows the user to enter text.
	/// </summary>
	public class TextBox : Label, IStringParsable
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public TextBox() : base()
		{
			UserSize = new Coord(100, 16);
			Padding = 6;
		}
		

		
		#region Mouse Interaction
		
		protected override void OnEnter(MouseEvent evt)
		{
			base.OnEnter(evt);
		}

		protected override void OnLeave(MouseEvent evt)
		{
			base.OnLeave(evt);
		}
		
		#endregion
		
		
		#region Text Entry
		
		/// <summary>
		/// Remakes the body based on the current Lines array.
		/// </summary>
		protected void SetBodyFromLines()
		{
			Body = Lines.Join(Label.LineBreak);
			Cursor.IsDirty = true;
		}
		
		public override void OnKeyPress(KeyEvent evt)
		{
			base.OnKeyPress(evt);
			
			var charVal = (char)evt.Value;
			if (Char.IsLetterOrDigit(charVal) || Char.IsPunctuation(charVal))
			{
				Insert(charVal);
			}
		}

		/// <summary>
		/// Inserts the given character at the current cursor position.
		/// </summary>
		public void Insert(Char c)
		{
			DeleteSelection();
			if (Cursor == null)
				Body = c.ToString();
			else if (Cursor.Row == 0 && Cursor.Column == 0)
			{
				Body = c.ToString() + Body;
				Cursor.IsDirty = true;
			}
			else if (Cursor.Row == Lines.Length - 1 && 
			         Cursor.Column == Lines[Lines.Length-1].Length - 1)
			{
				Body = Body + c.ToString();
				Cursor.IsDirty = true;
			}
			else // somewhere in the middle
			{
				Lines[Cursor.Row] = Lines[Cursor.Row].Insert(Cursor.Column, c.ToString());
				Cursor.Column++;
				SetBodyFromLines();
			}
			
			MakeDirty();
		}
		
		/// <summary>
		/// Deletes the currently selected text.
		/// </summary>
		public void DeleteSelection()
		{
			if (Anchor == null)	// nothing selected
				return;
			
			if (Anchor.Row == Cursor.Row) // single row selection
			{
				var firstColumn = Math.Min(Anchor.Column, Cursor.Column);
				var numColumns = Math.Abs(Anchor.Column - Cursor.Column);
				Lines[Cursor.Row] = Lines[Cursor.Row].Remove(firstColumn, numColumns);
				SetBodyFromLines();
				Cursor.Column = firstColumn;
			}
			else if (Anchor.Row < Cursor.Row)
			{
				throw new NotImplementedException();
			}
			else // Anchor.Row > Cursor.Row
			{
				throw new NotImplementedException();
			}
			
			Anchor = null;
		}
		
		#endregion
		
	}
}
