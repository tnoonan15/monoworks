// Main.cs - MonoWorks Project
//
// Copyright Andy Selvig 2008
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA

using System;

using Qyoto;

using MonoWorks.Model;
using mwb = MonoWorks.Base;
using MonoWorks.Gui;

namespace MonoWorks.Viewer
{	
	/// <summary>
	/// Entrypoint for the MonoWorks Viewer.
	/// </summary>
	class Viewer
	{	
		public static int Main(String[] args)
		{
			
			
			/** create a dummy document **/
			Document document = new Document();
			
			
			// add the reference line
			RefLine refLine1 = new RefLine(new mwb.Point(0.0, 0.0, 0.0), new mwb.Vector(0.0, 1.0, 0.0));
			document.AddReference(refLine1);
			
			/* create the extrusion */
			Sketch extSketch = new Sketch();			
			
			// add the line
			mwb.Point p1 = new mwb.Point(-1.0, 0.0, -1.0);
			mwb.Point p2 = new mwb.Point(1.0, 0.0, -1.0);
			mwb.Point p3 = new mwb.Point(1.0, 0.0, 1.0);
			Line line1 = new Line(p1, p2);
			line1.Points.Add(p3);
			extSketch.AddChild(line1);
			
			// add the arc
			Arc arc1 = new Arc(p2, p3, new mwb.Vector(0.0, 1.0, 0.0), mwb.Angle.Pi()/-2.0);
			extSketch.AddChild(arc1);
			
			// add the extrusion
			Extrusion ext1 = new Extrusion(extSketch);
			ext1.Path = refLine1;
			ext1.Travel = new mwb.Length(1.0);
			
			document.AddFeature(ext1);
			
			
			/* create the sweep */
			Sketch sweepSketch = new Sketch();
			
			// add the line
			mwb.Point p4 = new mwb.Point(1.0, 1.5, 0.0);
			mwb.Point p5 = new mwb.Point(1.0, 2.5, 0.0);
			mwb.Point p6 = new mwb.Point(1.0, 2.0, 0.0);
			Line line2 = new Line(p4, p5);
			sweepSketch.AddChild(line2);
			
			// add the arc			
			Arc arc2 = new Arc(p6, p4, new mwb.Vector(0.0, 0.0, 1.0), mwb.Angle.Pi());
			sweepSketch.AddChild(arc2);
			
			// create the sweep
			Sweep sweep1 = new Sweep(sweepSketch);
			sweep1.Axis = refLine1;
			sweep1.Travel = mwb.Angle.Pi()*-1;
			document.AddFeature( sweep1);
			
		        QApplication app = new QApplication(args);
			Viewport window = new Viewport(document);   
			window.SetWindowTitle("MonoWorks Viewer");
			window.Size = new QSize(600,600);
		        window.Show();
		        return QApplication.Exec();
		    }
	}
	
}
