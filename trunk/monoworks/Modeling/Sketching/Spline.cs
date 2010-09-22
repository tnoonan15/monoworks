// Spline.cs - MonoWorks Project
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

using Tao.OpenGl;

using MonoWorks.Base;
using MonoWorks.Rendering;
using MonoWorks.Rendering.Events;

namespace MonoWorks.Modeling.Sketching
{
	public class Spline : Line
	{

		public Spline(Sketch sketch)
			: base(sketch)
		{

		}
		
		

#region Hit Testing

		public override bool HitTest(HitLine hit)
		{
			return base.HitTest(hit);
		}

#endregion
		
		
#region Rendering
		
		public override void ComputeGeometry()
		{
			base.ComputeGeometry();
			
			int pathDivs = 24; // the number of divisions in each path segemnt
			var points = Points; // reduce calls to the attribute system
			int numPoints = Points.Count;
			int numFullPaths = (int)Math.Truncate((double)(numPoints-2) / 2); // the number of full paths
			int remainder = numPoints - numFullPaths*2; // number of points after the full paths
			if (remainder < 1)
				numFullPaths--;
			int count = 0; // current point count
			
			// allocate the solid points
			if (remainder == 0)
				solidPoints = new Vector[numFullPaths*pathDivs];
			else if (remainder == 1)
				solidPoints = new Vector[numFullPaths*pathDivs + 1];
			else if (remainder == 2)
				solidPoints = new Vector[numFullPaths*pathDivs + 2];
			else // 3 remainder
				solidPoints = new Vector[(numFullPaths+1)*pathDivs + 1];
			solidPoints[0] = points[0].ToVector();
			count = 1;
			Console.WriteLine("points: {0}, full paths: {1}, remainder: {2}, solid points: {3}", 
			                  numPoints, numFullPaths, remainder, solidPoints.Length);
			
			// compute the geometry for the full paths
			for (int p=0; p<numFullPaths; p++)
			{
				Vector p0 = points[p*2 + 0].ToVector();
				Vector p1 = points[p*2 + 1].ToVector();
				Vector p3 = points[p*2 + 2].ToVector();
				Vector p4 = points[p*2 + 3].ToVector();
				Vector p2 = p3*2 - p4;
				for (int i=0; i<pathDivs; i++)
				{
					double t = (double)i / (double)(pathDivs - 1);
					double one_t = 1 - t;
					solidPoints[count] = p0*one_t*one_t*one_t + p1*3*one_t*one_t*t + p2*3*one_t*t*t + p3*t*t*t;
					count++;
				}
			}
			
			// compute geometry for the remainder
			if (remainder == 0)
			{
				// do nothing
			}
			else if (remainder == 1)
			{
				// do nothing
			}
			else if (remainder == 2)
			{
				solidPoints[count] = points[numPoints-1].ToVector();
			}
			else // 3 remaining points
			{
				Vector p0 = points[numPoints-3].ToVector();
				Vector p1 = points[numPoints-2].ToVector();
				Vector p2 = points[numPoints-1].ToVector();
				for (int i=0; i<pathDivs; i++)
				{
					double t = (double)i / (double)(pathDivs - 1);
					double one_t = 1 - t;
					Console.WriteLine("point {0}, i={1}", count, i);
					solidPoints[count] = p0*one_t*one_t + p1*2*one_t*t + p2*t*t;
					count++;
				}
			}
			
			// assign the wireframe courts
			wireframePoints = new Vector[]{solidPoints[0], solidPoints[solidPoints.Length-1]};
			
			// compute directions and resize bounds
			_bounds.Reset();
			directions = new Vector[solidPoints.Length];
			for (int i=0; i<solidPoints.Length; i++)
			{				
				// compute the direction
				if (i < solidPoints.Length - 1)
					directions[i] = (solidPoints[i] - solidPoints[i + 1]).Normalize();
				else if (numPoints > 1) // only compute direction if there's more than one point
					directions[i] = (solidPoints[i] - solidPoints[i - 1]).Normalize();
				else
					directions[i] = new Vector();
				
				_bounds.Resize(solidPoints[i]);
			}
		}

				
#endregion
		
		
	}

}
