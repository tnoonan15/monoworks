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

			if (state == LineSketcherState.AddVertex)
			{
				point = new Point();
				Sketchable.Points.Add(point);
				Vector intersect = evt.HitLine.GetIntersection(Sketch.Plane.Plane);
				point.SetPosition(intersect);
			}
		}


		public override void OnButtonRelease(MouseButtonEvent evt)
		{
			base.OnButtonRelease(evt);
		}

		public override void OnMouseMotion(MouseEvent evt)
		{
			if (evt.Handled)
				return;

			base.OnMouseMotion(evt);

			if (state == LineSketcherState.AddVertex)
			{
				Vector intersect = evt.HitLine.GetIntersection(Sketch.Plane.Plane);
				point.SetPosition(intersect);
				Sketchable.MakeDirty();
			}
		}

#endregion


#region Rendering


		

#endregion


	}
}
