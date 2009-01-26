//   TestPart.cs - MonoWorks Project
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
	public class TestPart : Part
	{
		RefLine refLine;
		
		/// <summary>
		/// Default constructor.
		/// </summary>
		public TestPart() : base()
		{			
			// add the reference line
			refLine = new RefLine(new Point(0.0, 0.0, 0.5), new Vector(0.0, 0.0, 1.0));
			
			AddReference(refLine);

			CreateExtrusion();
			CreateRevolution();
		}
		
		
		/// <summary>
		/// Create the extrusion.
		/// </summary>
		protected void CreateExtrusion()
		{		
			
			// create the sketch
			Sketch extSketch = new Sketch(ZPlane);			
			AddSketch(extSketch);
			
			// add the line
			Point p1 = new Point(1.0, -1.0, 0.0);
			Point p2 = new Point(-1.0, -1.0, 0.0);
			Point p3 = new Point(-1.0, 1.0, 0.0);
			Line line1 = new Line(p1, p2);
			line1.Points.Add(p3);
			extSketch.AddChild(line1);
			
			// add the arc
			Arc arc1 = new Arc(p2, p3, new Vector(0.0, 0.0, 1.0), Angle.Pi()/-2.0);
			extSketch.AddChild(arc1);
			
			// add the extrusion
			Extrusion ext1 = new Extrusion(extSketch);
			ext1.Path = refLine;
			ext1.Travel = new Length(1.0);
			ext1.Snapshot();
			
			AddFeature(ext1);
		}
		
		
		/// <summary>
		/// Creates the revolution.
		/// </summary>
		protected void CreateRevolution()
		{
			// create the sketch
			Sketch revolutionSketch = new Sketch(XPlane);
			AddSketch(revolutionSketch);
			
			// add the line
			Point bottom = new Point(0.0, 1.0, 1.5);
			Point top = new Point(0.0, 1.0, 2.5);
			Point middle = new Point(0.0, 1.0, 2.0);
			Line line2 = new Line(bottom, top);
			line2.Points.Add(new Point(0.0, 1.25, 2.5));
			line2.Points.Add(new Point(0.0, 1.25, 2.0));
			line2.Points.Add(new Point(0.0, 1.5, 2.0));
			revolutionSketch.AddChild(line2);
			
			// add the arc			
			Arc arc2 = new Arc(middle, bottom, new Vector( 1.0, 0.0, 0.0), Angle.Pi()/2);
			revolutionSketch.AddChild(arc2);
			
			// create the revolution
			Revolution revolution1 = new Revolution(revolutionSketch);
			revolution1.Axis = refLine;
			revolution1.Travel = Angle.Pi()*-1;
			revolution1.Snapshot();
			AddFeature( revolution1);
		}
	}
}
