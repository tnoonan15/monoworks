// ArcSketcher.cs - MonoWorks Project
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

namespace MonoWorks.Modeling.Sketching
{
	/// <summary>
	/// The type of point that is being added to an arc.
	/// </summary>
	internal enum ArcAddMode { None, Center, Start, Stop };

	/// <summary>
	/// Sketcher for arcs.
	/// </summary>
	public class ArcSketcher : BaseSketcher<Arc>
	{

		public ArcSketcher(Arc arc)
			: base(arc)
		{
			if (arc.Center == null)
			{
				addMode = ArcAddMode.Center;
				currentPoint = new Point();
				arc.Center = currentPoint;
			}
			else
				addMode = ArcAddMode.None;
		}

		private Point currentPoint = null;


#region Mouse Interaction

		private ArcAddMode addMode;

		public override void OnButtonPress(MouseButtonEvent evt)
		{
			if (evt.Handled)
				return;

			base.OnButtonPress(evt);

			if (currentPoint != null)
			{
				Vector intersect = Sketch.Plane.GetIntersection(evt.HitLine);
				currentPoint.SetPosition(intersect);

				if (addMode == ArcAddMode.Center)
				{
					currentPoint = new Point();
					currentPoint.SetPosition(intersect);
					Sketchable.Start = currentPoint;
					addMode = ArcAddMode.Start;
				}
				else if (addMode == ArcAddMode.Start)
				{
					currentPoint = new Point();
					currentPoint.SetPosition(intersect);
					addMode = ArcAddMode.Stop;
				}
				else if (addMode == ArcAddMode.Stop)
				{
					UpdateRadius(evt.HitLine.Camera);
					currentPoint = null;
					addMode = ArcAddMode.None;

				}
				else // not adding anything
				{
					currentPoint = null;
					Sketchable.MakeDirty();
				}
			}
			else // no current point
			{
				if (Sketchable.Center == null)
					return;

				HitLine hit = evt.HitLine;

				// test for hitting the center
				Coord center = hit.Camera.WorldToScreen(Sketchable.Center.ToVector());
				if ((hit.Screen - center).Magnitude < Arc.HitTol)
				{
					currentPoint = Sketchable.Center;
					return;
				}

				if (Sketchable.Start == null)
					return;

				// test for hitting the start
				Coord start = hit.Camera.WorldToScreen(Sketchable.Start.ToVector());
				if ((hit.Screen - start).Magnitude < Arc.HitTol)
				{
					currentPoint = Sketchable.Start;
					return;
				}

				// test for hitting the stop
				Vector stopVec = Sketchable.SolidPoints[Sketchable.SolidPoints.Length - 1];
				Coord stop = hit.Camera.WorldToScreen(stopVec);
				if ((hit.Screen - stop).Magnitude < Arc.HitTol)
				{
					currentPoint = new Point();
					currentPoint.SetPosition(stopVec);
					return;
				}

				if (!HitTest(evt.HitLine))
					Apply();
			}
		}


		public override void OnButtonRelease(MouseButtonEvent evt)
		{
			base.OnButtonRelease(evt);

			if (currentPoint != null && addMode == ArcAddMode.None)
			{
				currentPoint = null;
				Sketchable.MakeDirty();
				evt.Handle();
			}
		}

		public override void OnMouseMotion(MouseEvent evt)
		{
			base.OnMouseMotion(evt);

			if (currentPoint != null)
			{
				Vector intersect = Sketch.Plane.GetIntersection(evt.HitLine);
				currentPoint.SetPosition(intersect);
				evt.Handle();

				if (currentPoint != Sketchable.Center && currentPoint != Sketchable.Start)
					UpdateRadius(evt.HitLine.Camera);
				Sketchable.MakeDirty();
			}
		}

		/// <summary>
		/// Updates the radius based on the currentPoint being the stop.
		/// </summary>
		private void UpdateRadius(Camera camera)
		{
			Coord center = camera.WorldToScreen(Sketchable.Center.ToVector());
			Coord start = camera.WorldToScreen(Sketchable.Start.ToVector());
			Coord stop = camera.WorldToScreen(currentPoint.ToVector());
			Coord v1 = start - center;
			Coord v2 = stop - center;
			Angle angle = v1.AngleTo(v2);
			if (v2.Magnitude > v1.Magnitude) // invert angle when the current point is outside the radius
			{
				if (angle.Value > 0)
					angle = angle - Angle.Pi() * 2;
				else
					angle = Angle.Pi() * 2 + angle;
			}
			Sketchable.Sweep = angle;
		}

#endregion


#region Rendering
		
		public override void RenderOpaque(Viewport viewport)
		{
			base.RenderOpaque(viewport);
			
			Color highlightColor = ModelingOptions.Global.GetColor("sketchable", HitState.Hovering);

			if (Sketchable.Center != null)
			{
				HighlightPoint(viewport, Sketchable.Center, highlightColor, 6);
			}

			//if (currentPoint != null)
			//    HighlightPoint(viewport, currentPoint, highlightColor, 6);

		}

#endregion

	}
}
