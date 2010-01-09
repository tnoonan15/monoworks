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

using MonoWorks.Framework;
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
			
			MakeDirty();
			
			// look for special keys
			switch (evt.SpecialKey)
			{
			case SpecialKey.Right:
				CursorRight();
				return;
			case SpecialKey.Left:
				CursorLeft();
				return;
			case SpecialKey.Backspace:
				Backspace(evt.Modifier);
				return;
			}
			
			var charVal = (char)evt.Value;
			if (Char.IsLetterOrDigit(charVal) || Char.IsPunctuation(charVal))
			{
				Insert(charVal.ToString());
			}
		}

		/// <summary>
		/// Inserts the given string at the current cursor position.
		/// </summary>
		public void Insert(string val)
		{
			DeleteSelection();
			if (Cursor == null)
			{
				Body = val;
				Cursor.Column = val.Length;
			}
			else if (Cursor.Row == 0 && Cursor.Column == 0)
			{
				Body = val + Body;
				Cursor.Column = val.Length;
				Cursor.IsDirty = true;
			}
			else if (Cursor.Row == Lines.Length - 1 && 
			         Cursor.Column == Lines[Lines.Length - 1].Length - 1)
			{
				Body = Body + val;
				Cursor.IsDirty = true;
			}
			else // somewhere in the middle
			{
				Lines[Cursor.Row] = Lines[Cursor.Row].Insert(Cursor.Column, val);
				Cursor.Column++;
				SetBodyFromLines();
			}
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
		
		/// <summary>
		/// Deletes one or more characters to the left depending on the modifier.
		/// </summary>
		public void Backspace(InteractionModifier modifier)
		{
			if (Cursor == null)
				return;
			
			if (Anchor != null)
			{
				DeleteSelection();
				return;
			}
			
			if (modifier == InteractionModifier.None)
			{
				if (Cursor.Column == 0)
				{
					if (Cursor.Row > 0) // there's a line break to delete
					{
						throw new NotImplementedException();
					}
				}
				else
				{
					Lines[Cursor.Row] = Lines[Cursor.Row].Remove(Cursor.Column-1, 1);
					SetBodyFromLines();
					Cursor.Column--;
				}
			}
			else // another modifier
			{
				throw new NotImplementedException();
			}
		}
		
		/// <summary>
		/// Moves the cursor one character to the left.
		/// </summary>
		public void CursorLeft()
		{
			if (Anchor != null)
				Anchor = null;
			
			// make sure there is a cursor
			if (Cursor == null)
			{
				Cursor = new TextCursor() {
					Column = 0,
					Row = 0,
					IsDirty = true
				};
			}
			else
			{
				if (Cursor.Column > 0)
				{
					Cursor.Column--;
					Cursor.IsDirty = true;
				}
				else if (Cursor.Row > 0)
				{
					Cursor.Row--;
					Cursor.Column = Lines[Cursor.Row].Length - 1;
					Cursor.IsDirty = true;
				}
			}
		}
		
		/// <summary>
		/// Moves the cursor one character to the right.
		/// </summary>
		public void CursorRight()
		{
			if (Anchor != null)
				Anchor = null;
			
			// make sure there is a cursor
			if (Cursor == null)
			{
				Cursor = new TextCursor() {
					Column = 0,
					Row = 0,
					IsDirty = true
				};
			}
			else
			{
				if (Cursor.Column < Lines[Cursor.Row].Length )
				{
					Cursor.Column++;
					Cursor.IsDirty = true;
				}
				else if (Cursor.Row < Lines.Length - 1)
				{
					Cursor.Row++;
					Cursor.Column = 0;
					Cursor.IsDirty = true;
				}
			}
		}
		
		#endregion
		
	}
}
