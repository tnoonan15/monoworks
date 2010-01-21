// Label.cs - MonoWorks Project
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

using MonoWorks.Base;
using MonoWorks.Rendering;
using MonoWorks.Rendering.Events;

namespace MonoWorks.Controls
{
	/// <summary>
	/// Event for some text changing.
	/// </summary>
	public class TextChangedEvent : ValueChangedEvent<string>
	{
		public TextChangedEvent(string oldVal, string newVal)
			: base(oldVal, newVal)
		{
			
		}
	}
	
	/// <summary>
	/// Delegate for handling text changed events.
	/// </summary>
	public delegate void TextChangedHandler(object sender, TextChangedEvent evt);
	
	/// <summary>
	/// Control containing just text.
	/// </summary>
	public class Label : Control2D, IStringParsable
	{
		/// <summary>
		/// Character used to delineate lines in labels and its subclasses.
		/// </summary>
		public const char LineBreak = '\n';
		
		/// <summary>
		/// Default constructor.
		/// </summary>
		public Label()
		{
			Body = "";
			FontSize = 12;
			IsHoverable = true;
		}
		
		/// <summary>
		/// Initialization constructor.
		/// </summary>
		/// <param name="text"> The text to display. </param>
		public Label(string text) : this()
		{
			Body = text;
		}

		private double _fontSize;
		/// <summary>
		/// The font size (in pixels).
		/// </summary>
		[MwxProperty]
		public double FontSize {
			get { return _fontSize; }
			set {
				_fontSize = value;
				MakeDirty();
			}
		}

		private bool _isMultiLine;
		/// <summary>
		/// If true, the control can hold multiple lines of text.
		/// </summary>
		[MwxProperty]
		public bool IsMultiLine {
			get { return _isMultiLine; }
			set {
				_isMultiLine = value;
				MakeDirty();
			}
		}

		private string _body;
		/// <summary>
		/// The body of text inside the control.
		/// </summary>
		[MwxProperty]
		public string Body {
			get { return _body; }
			set {
				var oldVal = _body;
				_body = value;
				MakeDirty();
				if (BodyChanged != null)
					BodyChanged(this, new TextChangedEvent(oldVal, value));
			}
		}
		
		public event TextChangedHandler BodyChanged;
		
		/// <summary>
		/// The total height of each line (based on font size and padding).
		/// </summary>
		public double LineHeight
		{
			get { return FontSize + Padding;}
		}

		/// <summary>
		/// Assigns the text.
		/// </summary>
		public void Parse(string valString)
		{
			Body = valString;
		}
		
		
		#region Rendering

		/// <summary>
		/// The extents of the text.
		/// </summary>
		private Coord _extents = new Coord();
		
		/// <summary>
		/// A dummy surface used by Cairo to compute the text extents.
		/// </summary>
		private static readonly Cairo.ImageSurface DummySurface = new Cairo.ImageSurface(Cairo.Format.ARGB32, 128, 128);

		/// <summary>
		/// The lines of the body separated out.
		/// </summary>
		protected string[] Lines;

		/// <summary>
		/// The current line (where the cursor is).
		/// </summary>
		public string CurrentLine
		{
			get { 
				if (Cursor != null)
					return Lines[Cursor.Row]; 
				return Body;
			}
			protected set {
				Lines[Cursor.Row] = value;
			}
		}
		
		/// <summary>
		/// The number of lines in the body (separated by Label.NewLine).
		/// </summary>
		public int NumLines
		{
			get { return Lines.Length;}
		}
		
		public override void ComputeGeometry()
		{
			base.ComputeGeometry();
			
			if (Body == null)
				return;
			
			using (var cr = new Cairo.Context(DummySurface))
			{
				cr.SetFontSize(FontSize);
				_extents = new Coord();
				Lines = Body.Split(LineBreak);
				for (int i = 0; i < Lines.Length; i++)
				{
					var crExtents = cr.TextExtents(Lines[i]);
					_extents.X = Math.Max(crExtents.Width, _extents.X);
					_extents.Y += LineHeight;
				}
				_extents.X += 2 * Padding;
				_extents.Y += Padding;
			}
			
			MinSize = _extents;
			ApplyUserSize();
			
			// make sure the cursors are up to date
			if (Cursor != null && Cursor.IsDirty)
				UpdateCursorPosition(Cursor);
			if (Anchor != null && Anchor.IsDirty)
				UpdateCursorPosition(Anchor);
		}


		protected override void Render(RenderContext context)
		{
			base.Render(context);

			if (Body != null)
			{
				context.Cairo.Save();
				context.Cairo.SetFontSize(FontSize);
				
				if (Cursor != null)
				{
					var currentPos = context.Cairo.CurrentPoint;
					var absPos = Cursor.Position + LastPosition + Padding;
					
					// draw the cursor
					context.Cairo.Color = new Cairo.Color(0, 0, 0);
					context.Cairo.LineWidth = 2;
					context.Cairo.MoveTo(absPos.X - 2, absPos.Y - 1); // I don't know why we need to subtract 
					context.Cairo.RelLineTo(0, LineHeight);
					context.Cairo.Stroke();
					
					// draw the selection box
					if (Anchor != null)
					{
						context.Cairo.Color = context.Decorator.SelectionColor.Cairo;
						var selectSize = Anchor.Position - Cursor.Position;
						context.Cairo.Rectangle(absPos.X - 2, absPos.Y - 1, selectSize.X, LineHeight);
						context.Cairo.Fill();
					}
					context.Cairo.MoveTo(currentPos);
				}
								
				// render the text
				context.Cairo.Color = context.Decorator.GetColor(ColorType.Text, HitState.None).Cairo;
				var point = context.Cairo.CurrentPoint;
				for (int i = 0; i < Lines.Length; i++)
				{
					context.Cairo.MoveTo(point.X + Padding, point.Y + (FontSize + Padding) * (i + 1));
					context.Cairo.ShowText(Lines[i]);
				}
				context.Cairo.MoveTo(point);
				context.Cairo.Restore();
			}
		}

		#endregion		
		
		
		#region Selection

		/// <summary>
		/// Specifies the position of the cursor in the Body.
		/// </summary>
		protected TextCursor Cursor;
		
		/// <summary>
		/// Specifies the point where a text selection operation began.
		/// </summary>
		protected TextCursor Anchor;
		
		/// <summary>
		/// Determines the cursor point in the body corresponding to the given point.
		/// </summary>
		protected TextCursor HitCursor(Coord absPos)
		{
			var pos = absPos - LastPosition - Padding;
			var cursor = new TextCursor();
			cursor.Row = (int)Math.Max(Math.Min(
					Math.Floor(pos.Y / LineHeight), 
					Lines.Length - 1),
					0);
			cursor.Position = new Coord(0, cursor.Row * LineHeight);
			var line = CurrentLine;
			
			// determine the column
			double x = 0;
			using (var cr = new Cairo.Context(DummySurface))
			{
				cr.SetFontSize(FontSize);
				var extents = cr.TextExtents("m"); // used to ensure that leading and trailing spaces are counted correctly
				var mWidth = extents.Width;
				for (int c=0; c<line.Length+1; c++)
				{
					extents = cr.TextExtents("m" + line.Substring(0, c) + "m");
					x = extents.Width - 2 * mWidth;
					if (x > pos.X)
					{
						cursor.Column = c;
						break;
					}
				}
			}	
			cursor.Position.X = x;
			cursor.IsDirty = false;
			return cursor;
		}
		
		/// <summary>
		/// Updates the position of the cursor based on its row and column.
		/// </summary>
		protected void UpdateCursorPosition(TextCursor cursor)
		{
			if (cursor.Position ==  null)
				cursor.Position = new Coord();
			cursor.Position.Y = cursor.Row * LineHeight;
			
			var line = CurrentLine;
			using (var cr = new Cairo.Context(DummySurface))
			{
				cr.SetFontSize(FontSize);
				var extents = cr.TextExtents("m");
				// used to ensure that leading and trailing spaces are counted correctly
				var mWidth = extents.Width;
				extents = cr.TextExtents("m" + line.Substring(0, cursor.Column) + "m");
				cursor.Position.X = extents.Width - 2 * mWidth;
			}
			cursor.IsDirty = false;
		}
		
		/// <summary>
		/// Selects all the tet in the body.
		/// </summary>
		public void SelectAll()
		{
			Anchor = new TextCursor() {
				Row = 0,
				Column = 0,
				IsDirty = true
			};
			Cursor = new TextCursor() {
				Row = NumLines - 1,
				Column = Lines[Lines.Length - 1].Length,
				IsDirty = true
			};
		}
		
		#endregion
		
		
		#region Mouse Interaction
		
		/// <summary>
		/// Whether or not the user is dragging to select text.
		/// </summary>
		private bool _isDragging = false;

		protected override void OnEnter(MouseEvent evt)
		{
			base.OnEnter(evt);
			
			evt.Scene.SetCursor(CursorType.Beam);
		}

		protected override void OnLeave(MouseEvent evt)
		{
			base.OnLeave(evt);
			
			evt.Scene.SetCursor(CursorType.Normal);
		}
				
		public override void OnButtonPress(MouseButtonEvent evt)
		{
			base.OnButtonPress(evt);
			
			if (!HitTest(evt.Pos))
				return;
			
			if (evt.Multiplicity == ClickMultiplicity.Double)
			{
				SelectAll();
				return;
			}
			
			Cursor = HitCursor(evt.Pos);
			Anchor = Cursor;
			
			IsFocused = true;
			_isDragging = true;
			
			evt.Handle();
			
			
			MakeDirty();
		}

		public override void OnButtonRelease(MouseButtonEvent evt)
		{
			base.OnButtonRelease(evt);
			
			_isDragging = false;
		}

		
		public override void OnMouseMotion(MouseEvent evt)
		{
			base.OnMouseMotion(evt);
			
			if (Anchor == null || !_isDragging)
				return;
			Cursor = HitCursor(evt.Pos);
			MakeDirty();
		}
		
		protected override void OnHitStateChanged (HitState oldVal)
		{
			base.OnHitStateChanged(oldVal);
			
			if (oldVal.IsFocused() && !IsFocused) // lost focus
			{
				Cursor = null;
				Anchor = null;
				_isDragging = false;
				MakeDirty();
			}
		}
		
		#endregion


		
	}
	
	
	/// <summary>
	/// Stores information about a position inside a body of text.
	/// </summary>
	public class TextCursor
	{
		public int Row { get; set; }
		
		public int Column { get; set; }
		
		public Coord Position { get; set; }
		
		public bool IsDirty { get; set; }
		
		/// <summary>
		/// Creates a new, dirty cursor with the same row and column.
		/// </summary>
		public TextCursor Copy()
		{
			return new TextCursor() {
				Row = Row,
				Column = Column,
				Position = Position.Copy(),
				IsDirty = true
			};
		}
		
		public override string ToString()
		{
			return string.Format("[{0}, {1}] Position={2}", Row, Column, Position);
		}

		
	}
	
}
