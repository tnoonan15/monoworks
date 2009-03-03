// LineSketcherState.cs - MonoWorks Project
//
//  Copyright (C) 2009 Andy Selvig
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
using System.Collections.Generic;

using MonoWorks.Base;
using MonoWorks.Rendering;
using MonoWorks.Rendering.Events;

namespace MonoWorks.Model
{
	/// <summary>
	/// Enumerates which part of the line is being edited.
	/// </summary>
	public enum LineSketcherState { None, AddVertex, Vertex, Segment };

	/// <summary>
	/// Sketcher for lines.
	/// </summary>
	public class LineSketcher : BaseSketcher<Line>
	{

		public LineSketcher(Sketch sketch, Line line)
			: base(sketch, line)
		{
			point = new Point();
			line.Points.Add(point);
		}

		/// <summary>
		/// The state of the sketching.
		/// </summary>
		private LineSketcherState state = LineSketcherState.AddVertex;

		/// <summary>
		/// The current point being edited for state = AddVertex | Vertex.
		/// </summary>
		private Point point = null;

		/// <summary>
		/// A point that is close to where the mouse is.
		/// </summary>
		private Point closePoint = null;

		public override void Apply()
		{
			if (state == LineSketcherState.AddVertex)
				Sketchable.Points.Remove(point);
		}


#region Mouse Interaction

		public override void OnButtonPress(MouseButtonEvent evt)
		{
			if (evt.Handled)
				return;

			base.OnButtonPress(evt);

			if (closePoint != null)
			{
				Sketchable.Points.Remove(point);
				Sketchable.IsClosed = true;
				point = null;
				closePoint = null;
				state = LineSketcherState.None;
			}
			else // there is no close point
			{
				if (state == LineSketcherState.AddVertex)
				{
					point = new Point();
					Sketchable.Points.Add(point);
					Vector intersect = evt.HitLine.GetIntersection(Sketch.Plane.Plane);
					point.SetPosition(intersect);
				}
			}
			Sketchable.MakeDirty();
		}


		public override void OnButtonRelease(MouseButtonEvent evt)
		{
			base.OnButtonRelease(evt);
			closePoint = null;
		}

		public override void OnMouseMotion(MouseEvent evt)
		{
			if (evt.Handled)
				return;

			base.OnMouseMotion(evt);

			if (state == LineSketcherState.AddVertex || state == LineSketcherState.Vertex)
			{
				Vector intersect = evt.HitLine.GetIntersection(Sketch.Plane.Plane);
				point.SetPosition(intersect);
				Sketchable.MakeDirty();
				
				// check if the first point is close
				if (point == Sketchable.Points.Last() && Sketchable.Points.Count > 2)
				{
					Coord firstCoord = evt.HitLine.Camera.WorldToScreen(Sketchable.Points.First().ToVector());
					Coord lastCoord = evt.HitLine.Camera.WorldToScreen(Sketchable.Points.Last().ToVector());
					if ((firstCoord - lastCoord).Magnitude < 6)
						closePoint = Sketchable.Points.First();
					else
						closePoint = null;
				}
			}
		}

#endregion


#region Rendering


		public override void RenderOpaque(Viewport viewport)
		{
			base.RenderOpaque(viewport);

			if (closePoint != null)
				HighlightPoint(viewport, closePoint);
		}
		

#endregion


	}
}
