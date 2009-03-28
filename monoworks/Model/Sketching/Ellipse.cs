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

namespace MonoWorks.Model.Sketching
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


			var sinbeta = Tilt.Sin();
			var cosbeta = Tilt.Cos();
			Angle theta = new Angle();
			Angle dTheta = Angle.TwoPi / N;
			for (int i = 0; i < N; i ++)
			{
				theta += dTheta;
				double dx = a * theta.Cos() * cosbeta - b * theta.Sin() * sinbeta;
				double dy = a * theta.Cos() * sinbeta + b * theta.Sin() * cosbeta;
				solidPoints[i] = center + x * dx + y * dy;
			}


			// generate the directions
		}


		public override void DrawVertices()
		{
			if (IsDirty)
				ComputeGeometry();
			if (solidPoints.Length > 0)
			{
				solidPoints[0].glVertex();
				solidPoints[solidPoints.Length - 1].glVertex();
			}
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
					lastHit = hit.GetIntersection((Parent as Sketch).Plane.Plane);
					return true;
				}
			}
			return false;
		}

#endregion



	}
}
