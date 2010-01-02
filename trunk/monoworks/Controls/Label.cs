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
	/// Control containing just text.
	/// </summary>
	public class Label : Control2D, IStringParsable
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public Label() : base()
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
				_body = value;
				MakeDirty();
			}
		}
		
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
		private Coord extents = new Coord();
		
		/// <summary>
		/// A dummy surface used by Cairo to compute the text extents.
		/// </summary>
		private static Cairo.ImageSurface dummySurface = new Cairo.ImageSurface(Cairo.Format.ARGB32, 128, 128);

		/// <summary>
		/// The lines of the body separated out.
		/// </summary>
		private string[] _lines;
		
		public override void ComputeGeometry()
		{
			base.ComputeGeometry();
			
			if (Body == null)
				return;
			
			using (var cr = new Cairo.Context(dummySurface))
			{
				cr.SetFontSize(FontSize);
				extents = new Coord();
				_lines = Body.Split(new string[] { "\n" }, StringSplitOptions.None);
				foreach (var line in _lines)
				{
					var crExtents = cr.TextExtents(line);
					extents.X = Math.Max(crExtents.Width, extents.X);
					extents.Y += LineHeight;
				}
				extents.X += 2 * Padding;
				extents.Y += Padding;
			}
			
			MinSize = extents;
			ApplyUserSize();
		}


		protected override void Render(RenderContext context)
		{
			base.Render(context);

			if (Body != null)
			{
				context.Cairo.Save();
				context.Cairo.SetFontSize(FontSize);
				
				// draw the selection box
				context.Cairo.Color = context.Decorator.SelectionColor.Cairo;
				if (_anchor != null && _cursor != null)
				{
					var currentPos = context.Cairo.CurrentPoint;
					Console.WriteLine("drawing selection from {0} to {1}", _anchor, _cursor);
					var absPos = _anchor.Position + LastPosition + Padding;
					var selectSize = _cursor.Position - _anchor.Position;
					context.Cairo.Rectangle(absPos.PointD(), selectSize.X, LineHeight);
					context.Cairo.Fill();
					context.Cairo.MoveTo(currentPos);
				}
				
				// render the text
				context.Cairo.Color = new Cairo.Color(0, 0, 0);
				var point = context.Cairo.CurrentPoint;
				for (int i = 0; i < _lines.Length; i++)
				{
					context.Cairo.MoveTo(point.X + Padding, point.Y + (FontSize + Padding) * (i + 1));
					context.Cairo.ShowText(_lines[i]);
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
		private TextCursor _cursor;
		
		/// <summary>
		/// Specifies the point where a text selection operation began.
		/// </summary>
		private TextCursor _anchor;
		
		/// <summary>
		/// Determines the cursor point in the body corresponding to the given point.
		/// </summary>
		protected TextCursor HitCursor(Coord absPos)
		{
			var pos = absPos - LastPosition - Padding;
			var cursor = new TextCursor();
			cursor.Row = (int)Math.Max(Math.Min(
					Math.Floor(pos.Y / LineHeight), 
					_lines.Length - 1),
					0);
			var line = _lines[cursor.Row];
			
			// determine the column
			using (var cr = new Cairo.Context(dummySurface)) {
				cr.SetFontSize(FontSize);
				var extents = cr.TextExtents("m"); // used to ensure that leading and trailing spaces are counted correctly
				var mWidth = extents.Width;
				for (int c=0; c<line.Length; c++)
				{
					extents = cr.TextExtents("m" + line.Substring(0, c) + "m");
					var thisWidth = extents.Width - 2 * mWidth;
					if (thisWidth > pos.X)
					{
						cursor.Column = c;
						cursor.Position = new Coord(thisWidth, cursor.Row * LineHeight);
						break;
					}
				}
			}
			
			if (cursor.Position == null)
				cursor.Position = new Coord();
			
			Console.WriteLine("hit cursor: {0}", cursor);
			return cursor;
		}
		
		#endregion
		
		
		#region Mouse Interaction

		protected override void OnEnter(MouseEvent evt)
		{
			base.OnEnter(evt);
			
			evt.Viewport.SetCursor(CursorType.Beam);
		}

		protected override void OnLeave(MouseEvent evt)
		{
			base.OnLeave(evt);
			
			evt.Viewport.SetCursor(CursorType.Normal);
		}
				
		public override void OnButtonPress(MouseButtonEvent evt)
		{
			base.OnButtonPress(evt);
			
			if (!HitTest(evt.Pos))
				return;
			_cursor = HitCursor(evt.Pos);
			_anchor = _cursor;
		}

		public override void OnButtonRelease(MouseButtonEvent evt)
		{
			base.OnButtonRelease(evt);
			
			_cursor = null;
			_anchor = null;
			MakeDirty();
		}

		
		public override void OnMouseMotion(MouseEvent evt)
		{
			base.OnMouseMotion(evt);
			
			if (_anchor == null)
				return;
			_cursor = HitCursor(evt.Pos);
			MakeDirty();
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
		
		public override string ToString()
		{
			return string.Format("[{0}, {1}] Position={2}", Row, Column, Position);
		}

	}
	
}
