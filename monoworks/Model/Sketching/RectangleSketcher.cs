// RectangleSketcher.cs - MonoWorks Project
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

namespace MonoWorks.Model.Sketching
{
	/// <summary>
	/// Sketcher for rectangles.
	/// </summary>
	public class RectangleSketcher : BaseSketcher<Rectangle>
	{

		public RectangleSketcher(Rectangle sketchable)
			: base(sketchable)
		{
			if (sketchable.Anchor2 == null) // it's a new rectangle
			{
				isDragging = true;
				dragPoint = sketchable.Anchor1;
			}
		}

		private Point dragPoint = null;


#region Mouse Interaction

		public override void OnButtonPress(MouseButtonEvent evt)
		{
			if (evt.Handled)
				return;

			base.OnButtonPress(evt);

			if (isDragging)
			{
				Vector intersect = Sketch.Plane.GetIntersection(evt.HitLine);
				dragPoint.SetPosition(intersect);

				if (dragPoint == Sketchable.Anchor1)
				{
					Sketchable.Anchor2 = Sketchable.Anchor1.Clone() as Point;
					dragPoint = Sketchable.Anchor2;
				}
				else // dragging anchor2
				{
					isDragging = false;
					Apply();
				}
				Sketchable.MakeDirty();
				evt.Handle();
			}
			else // not dragging anything, look for hit
			{
				// test for first anchor
				Coord vecProj = evt.HitLine.Camera.WorldToScreen(Sketchable.Anchor1.ToVector());
				if ((vecProj - evt.Pos).Magnitude <= Rectangle.HitTol)
				{
					dragPoint = Sketchable.Anchor1;
					return;
				}
				// test for second anchor
				vecProj = evt.HitLine.Camera.WorldToScreen(Sketchable.Anchor2.ToVector());
				if ((vecProj - evt.Pos).Magnitude <= Rectangle.HitTol)
				{
					dragPoint = Sketchable.Anchor2;
					return;
				}
				// test for corner 1
				vecProj = evt.HitLine.Camera.WorldToScreen(Sketchable.SolidPoints[1]);
				if ((vecProj - evt.Pos).Magnitude <= Rectangle.HitTol)
				{
					Sketchable.InvertAnchors();
					dragPoint = Sketchable.Anchor1;
					return;
				}
				// test for corner 3
				vecProj = evt.HitLine.Camera.WorldToScreen(Sketchable.SolidPoints[3]);
				if ((vecProj - evt.Pos).Magnitude <= Rectangle.HitTol)
				{
					Sketchable.InvertAnchors();
					dragPoint = Sketchable.Anchor2;
					return;
				}
				
				// if we didn't hit anything, apply the sketching
				if (!Sketchable.HitTest(evt.HitLine))
					Apply();
			}
		}

		public override void OnButtonRelease(MouseButtonEvent evt)
		{
			base.OnButtonRelease(evt);

			if (!isDragging && dragPoint != null) // moving a corner
				dragPoint = null;
		}

		public override void OnMouseMotion(MouseEvent evt)
		{
			if (evt.Handled)
				return;

			base.OnMouseMotion(evt);

			if (dragPoint != null)
			{
				Vector intersect = Sketch.Plane.GetIntersection(evt.HitLine);
				dragPoint.SetPosition(intersect);
				Sketchable.MakeDirty();
				evt.Handle();
			}
		}

#endregion


#region Rendering

		public override void RenderOpaque(Viewport viewport)
		{
			base.RenderOpaque(viewport);

			if (dragPoint != null)
			{
				Color highlightColor = ModelingOptions.Global.GetColor("sketchable", HitState.Hovering);
				HighlightPoint(viewport, dragPoint, highlightColor, 8);
			}
		}

#endregion

	}
}
