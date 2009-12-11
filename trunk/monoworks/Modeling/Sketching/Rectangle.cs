// Rectangle.cs - MonoWorks Project
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

using gl = Tao.OpenGl.Gl;

namespace MonoWorks.Modeling.Sketching
{
	/// <summary>
	/// A sketchable that represents a rectangle.
	/// </summary>
	/// <remarks>Rectangles are defined by two anchor points at opposite corners.</remarks>
	public class Rectangle : BoxedSketchable
	{
		public Rectangle(Sketch sketch) : base(sketch)
		{
			solidPoints = new Vector[5]; // needs to come back on itself
			directions = new Vector[5];
			wireframePoints = new Vector[4]; // just the corners
		}


#region Geometry
		
		public override void ComputeGeometry()
		{
			base.ComputeGeometry();

			// generate the solid points
			Vector x = Sketch.Plane.LocalX;
			Vector y = Sketch.Plane.LocalY;
			solidPoints[0] = Anchor1.ToVector();
			if (Anchor2 == null)
			{
				solidPoints[1] = solidPoints[2] = solidPoints[3] = solidPoints[0];
			}
			else
			{
				solidPoints[2] = Anchor2.ToVector();
				Vector diff = solidPoints[2] - solidPoints[0];
				double dx = x.Dot(diff);
				solidPoints[1] = solidPoints[0] + x * dx;
				double dy = y.Dot(diff);
				solidPoints[3] = solidPoints[0] + y * dy;
			}
			solidPoints[4] = solidPoints[0];

			// update the bounds
			bounds.Reset();
			foreach (var vector in solidPoints)
				bounds.Resize(vector);

			// copy over the wireframe points
			for (int i = 0; i < wireframePoints.Length; i++)
				wireframePoints[i] = solidPoints[i];

			// generate the directions
			directions[0] = (x + y).Invert().Normalize();
			directions[1] = (x - y).Normalize();
			directions[2] = (x + y).Normalize();
			directions[3] = (y - x).Normalize();
			directions[4] = directions[0];
		}

#endregion


#region Hit Testing

		public override bool HitTest(HitLine hit)
		{
			if (Anchor2 == null)
				return false;

			for (int i = 0; i < solidPoints.Length - 1; i++)
			{
				HitLine line = new HitLine()
				{
					Front = solidPoints[i],
					Back = solidPoints[i + 1],
					Camera = hit.Camera
				};
				if (line.ShortestDistance(hit) < HitTol * hit.Camera.ViewportToWorldScaling)
				{
					lastHit = hit.GetIntersection((Parent as Sketch).Plane);
					return true;
				}
			}
			return false;
		}

#endregion

	}
}
