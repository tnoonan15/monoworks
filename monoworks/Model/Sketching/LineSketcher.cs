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

using gl = Tao.OpenGl.Gl;

using MonoWorks.Base;
using MonoWorks.Rendering;
using MonoWorks.Framework;
using MonoWorks.Rendering.Events;

namespace MonoWorks.Model
{

	/// <summary>
	/// Sketcher for lines.
	/// </summary>
	public class LineSketcher : BaseSketcher<Line>
	{

		public LineSketcher(Sketch sketch, Line line)
			: base(sketch, line)
		{
			if (line.Points.Count == 0) // this is a new line
			{
				Point point = new Point();
				line.Points.Add(point);
				Select(point);
				addingVertex = true;
			}
		}

		/// <summary>
		/// A point that is close to where the mouse is.
		/// </summary>
		private Point closePoint = null;

		public override void Apply()
		{
			base.Apply();

			if (addingVertex)
				Sketchable.Points.Remove(selection[0]);
		}


#region Selection
		
		/// <summary>
		/// The selected points.
		/// </summary>
		private List<Point> selection = new List<Point>();

		/// <summary>
		/// Whether or not something is selected.
		/// </summary>
		private bool SomethingSelected
		{
			get { return selection.Count > 0; }
		}

		/// <summary>
		/// Clears the selection.
		/// </summary>
		private void ClearSelection()
		{
			selection.Clear();
		}

		/// <summary>
		/// Adds the point to the selection.
		/// </summary>
		private void Select(Point point)
		{
			selection.Add(point);
		}

		/// <summary>
		/// Appends the selection with hit points.
		/// </summary>
		private void AppendSelection(HitLine hit)
		{
			if (Sketchable.Points.Count == 0)
				return;

			// look for vertex hits
			bool hitSomething = false;
			foreach (var point in Sketchable.Points)
			{
				// project the point onto the screen
				Coord pointProj = hit.Camera.WorldToScreen(point.ToVector());
				if ((pointProj - hit.Screen).Magnitude <= Line.HitTol)
				{
					selection.Add(point);
					hitSomething = true;
				}
			}
			if (hitSomething) // don't look for edges if we found a vertex
				return;

			// look for edge hits
			for (int i = 0; i < Sketchable.Points.Count - 1; i++)
			{
				HitLine line = new HitLine()
				{
					Front = Sketchable.Points[i].ToVector(),
					Back = Sketchable.Points[i+1].ToVector()
				};
				if (line.ShortestDistance(hit) < Line.HitTol * hit.Camera.ViewportToWorldScaling)
				{
					selection.Add(Sketchable.Points[i]);
					selection.Add(Sketchable.Points[i+1]);
				}
			}
		}

#endregion


#region Mouse Interaction

		/// <summary>
		/// True if the next mouse click adds a vertex.
		/// </summary>
		private bool addingVertex = false;

		/// <summary>
		/// Whether or not the user is currently dragging something.
		/// </summary>
		private bool isDragging = false;

		public override void OnButtonPress(MouseButtonEvent evt)
		{
			if (evt.Handled)
				return;

			base.OnButtonPress(evt);

			if (closePoint != null) // there is a close point
			{
				Sketchable.Points.Remove(selection[0]);
				Sketchable.IsClosed = true;
				closePoint = null;
				ClearSelection();
				addingVertex = false;
				Apply();
			}
			else if (addingVertex) // add a vertex
			{
				ClearSelection();
				Point point = new Point();
				Select(point);
				Sketchable.Points.Add(point);
				Vector intersect = evt.HitLine.GetIntersection(Sketch.Plane.Plane);
				point.SetPosition(intersect);
				//ClearSelection();
			}
			else if (evt.Button == 1) // look for a hit
			{
				if (evt.Modifier == InteractionModifier.None)
					ClearSelection();
				AppendSelection(evt.HitLine);

				if (!SomethingSelected)
					Apply();
				else
					isDragging = true;
			}
			Sketchable.MakeDirty();
		}


		public override void OnButtonRelease(MouseButtonEvent evt)
		{
			base.OnButtonRelease(evt);
			closePoint = null;
			isDragging = false;
		}

		public override void OnMouseMotion(MouseEvent evt)
		{
			if (evt.Handled)
				return;

			base.OnMouseMotion(evt);

			if (addingVertex || (selection.Count == 1 && isDragging))
			{
				Vector intersect = evt.HitLine.GetIntersection(Sketch.Plane.Plane);
				if (ModelingOptions.Global.SnapToGrid)
					intersect = Sketch.Plane.SnapToGrid(intersect);
				selection[0].SetPosition(intersect);
				Sketchable.MakeDirty();

				
				// check if the first point is close
				if (selection[0] == Sketchable.Points.Last() && Sketchable.Points.Count > 2)
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
				HighlightPoint(viewport, closePoint, ColorManager.Global["Red"], 10);

			// highlight the selection
			Color selectedColor = ModelingOptions.Global.GetColor("sketchable", HitState.Selected);
			foreach (Point point in selection)
				HighlightPoint(viewport, point, selectedColor, 8);
			if (selection.Count > 1)
			{
				gl.glBegin(gl.GL_LINE_STRIP);
				foreach (Point point in selection)
					point.glVertex();
				gl.glEnd();
			}
		}
		

#endregion


	}
}
