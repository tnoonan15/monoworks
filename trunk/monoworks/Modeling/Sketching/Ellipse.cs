//   Ellipse.cs - MonoWorks Project
//
//    Copyright Andy Selvig 2008
//
//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU Lesser General Public License as published 
//    by the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU Lesser General Public License for more details.
//
//    You should have received a copy of the GNU Lesser General Public 
//    License along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using MonoWorks.Base;
using MonoWorks.Rendering;
using MonoWorks.Rendering.Events;

using gl = Tao.OpenGl.Gl;

namespace MonoWorks.Modeling.Sketching
{
	/// <summary>
	/// An ellipse, yo.
	/// </summary>
	public class Ellipse : BoxedSketchable
	{

		public Ellipse(Sketch sketch)
			: base(sketch)
		{
			wireframePoints = new Vector[4];
		}


#region Geometry

		/// <summary>
		/// Computes the ellipse geometry.
		/// </summary>
		/// <remarks>
		/// This is based on the psuedocode in http://en.wikipedia.org/wiki/Ellipse.
		/// </remarks>
		public override void ComputeGeometry()
		{
			base.ComputeGeometry();
			
			if (Anchor2 == null)
				return;

			// allocate the points
			int N = ModelingOptions.Global.CircleDivs;
			if (solidPoints.Length != N)
			{
				solidPoints = new Vector[N];
				directions = new Vector[N];
			}

			// get the major and minor axes
			Vector x = Sketch.Plane.LocalX;
			Vector y = Sketch.Plane.LocalY;
			Vector center = Center.ToVector();
			double a = x.Dot(Anchor2.ToVector() - center);
			double b = y.Dot(Anchor2.ToVector() - center);
			
			// compute the locations of the points
			var sinbeta = Tilt.Sin();
			var cosbeta = Tilt.Cos();
			Angle theta = new Angle();
			Angle dTheta = Angle.TwoPi / (N-1);
			bounds.Reset();
			for (int i = 0; i < N; i ++)
			{
				double dx = a * theta.Cos() * cosbeta - b * theta.Sin() * sinbeta;
				double dy = a * theta.Cos() * sinbeta + b * theta.Sin() * cosbeta;
				solidPoints[i] = center + x * dx + y * dy;
				bounds.Resize(solidPoints[i]);
				theta += dTheta;
			}

			// generate the directions
			for (int i = 0; i < N; i++)
			{
				if (i == 0)
					directions[i] = (solidPoints[i+1] - solidPoints[N - 1]).Normalize();
				else if (i == N - 1)
					directions[i] = (solidPoints[0] - solidPoints[i - 1]).Normalize();
				else
					directions[i] = (solidPoints[i + 1] - solidPoints[i - 1]).Normalize();
			}

			// generate the wireframe points
			wireframePoints[0] = center + x * a;
			wireframePoints[1] = center + y * b;
			wireframePoints[2] = center - x * a;
			wireframePoints[3] = center - y * b;
		}


		public override void DrawVertices()
		{
			if (IsDirty)
				ComputeGeometry();
			if (solidPoints.Length > 0)
			{
				solidPoints[0].glVertex();
				solidPoints[solidPoints.Length/2].glVertex();
			}
		}

#endregion


#region Hit Testing

		public override bool HitTest(HitLine hit)
		{
			if (Anchor2 == null)
				return false;
			
			// get the major and minor axes
			Vector x = Sketch.Plane.LocalX;
			Vector y = Sketch.Plane.LocalY;
			Vector center = Center.ToVector();
			double a = x.Dot(Anchor2.ToVector() - center);
			double b = y.Dot(Anchor2.ToVector() - center);

			// rotate the coordinate system to the tilt
			if (Tilt.Value != 0)
			{
				x = x.Rotate((Parent as Sketch).Plane.Normal, Tilt);
				y = y.Rotate((Parent as Sketch).Plane.Normal, Tilt);
			}

			// project the hit onto the plane's coordinate system
			lastHit = hit.GetIntersection((Parent as Sketch).Plane);
			double hitX = (lastHit - center).Dot(x);
			double hitY = (lastHit - center).Dot(y);

			// test for the hit
			double hitTol = 0.1;
			if (Math.Abs(1 - hitX * hitX / (a * a) - hitY * hitY / (b * b)) < hitTol)
				return true;

			return false;
		}

#endregion



	}
}
