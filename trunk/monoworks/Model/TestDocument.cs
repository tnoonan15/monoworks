//   TestDocument.cs - MonoWorks Project
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

using MonoWorks.Base;

namespace MonoWorks.Model
{
	
	/// <summary>
	/// The test document shows off all of the features of the MonoWorks model framework.
	/// </summary>
	public class TestDocument : Document
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public TestDocument() : base()
		{			
			
			// add the reference line
			RefLine refLine1 = new RefLine(new Point(0.0, 0.0, 0.0), new Vector(0.0, 1.0, 0.0));
			AddReference(refLine1);
			
			/* create the extrusion */
			Sketch extSketch = new Sketch();			
			
			// add the line
			Point p1 = new Point(-1.0, 0.0, -1.0);
			Point p2 = new Point(1.0, 0.0, -1.0);
			Point p3 = new Point(1.0, 0.0, 1.0);
			Line line1 = new Line(p1, p2);
			line1.Points.Add(p3);
			extSketch.AddChild(line1);
			
			// add the arc
			Arc arc1 = new Arc(p2, p3, new Vector(0.0, 1.0, 0.0), Angle.Pi()/-2.0);
			extSketch.AddChild(arc1);
			
			// add the extrusion
			Extrusion ext1 = new Extrusion(extSketch);
			ext1.Path = refLine1;
			ext1.Travel = new Length(1.0);
			
			AddFeature(ext1);
			
			
			/* create the revolution */
			Sketch revolutionSketch = new Sketch();
			
			// add the line
			Point p4 = new Point(1.0, 1.5, 0.0);
			Point p5 = new Point(1.0, 2.5, 0.0);
			Point p6 = new Point(1.0, 2.0, 0.0);
			Line line2 = new Line(p4, p5);
			revolutionSketch.AddChild(line2);
			
			// add the arc			
			Arc arc2 = new Arc(p6, p4, new Vector(0.0, 0.0, 1.0), Angle.Pi());
			revolutionSketch.AddChild(arc2);
			
			// create the revolution
			Revolution revolution1 = new Revolution(revolutionSketch);
			revolution1.Axis = refLine1;
			revolution1.Travel = Angle.Pi()*-1;
			AddFeature( revolution1);
		}
	}
}
