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
		/// If true, the text box can hold multiple lines of text.
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
		/// The body of text inside the box.
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
		
		private static Cairo.ImageSurface dummySurface = new Cairo.ImageSurface(Cairo.Format.ARGB32, 128, 128);

		/// <summary>
		/// Returns the lines of the body separated out.
		/// </summary>
		private string[] Lines
		{
			get
			{
				return Body.Split(new string[]{"\n"}, StringSplitOptions.None);
			}
		}
		
		public override void ComputeGeometry()
		{
			base.ComputeGeometry();
			
			if (Body == null)
				return;
			
			using (var cr = new Cairo.Context(dummySurface))
			{
				cr.SetFontSize(FontSize);
				extents = new Coord();
				foreach (var line in Lines)
				{
					var crExtents = cr.TextExtents(line);
					extents.X = Math.Max(crExtents.Width, extents.X);
					extents.Y += FontSize + Padding;
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
				context.Cairo.Color = new Cairo.Color(0, 0, 0);
				
				var lines = Lines;
				var point = context.Cairo.CurrentPoint;
				for (int i = 0; i < lines.Length; i++)
				{
					context.Cairo.MoveTo(point.X + Padding, point.Y + (FontSize + Padding) * (i + 1));
					context.Cairo.ShowText(lines[i]);
				}
				context.Cairo.MoveTo(point);
				context.Cairo.Restore();
			}
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
		
		#endregion


		
	}
}
