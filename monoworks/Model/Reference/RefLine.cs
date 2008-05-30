// RefLine.cs - MonoWorks Project
//
// Copyright (C) 2008 Andy Selvig
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

namespace MonoWorks.Model
{	
	
	/// <summary>
	/// Refline represents an infinite 2D line in 3D space.
	/// The line is defined by a "center" point and a direction vector.
	/// Any point u on the line can be parameterized by a value t such that:
	///     u = center + t*direction
	/// </summary>
	public class RefLine : Reference
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public RefLine() : base()
		{
		}
		
		/// <summary>
		/// Initialization constructor.
		/// </summary>
		/// <param name="center"> The "center" of the line. </param>
		/// <param name="direction"> The direction. </param>
		public RefLine(Point center, Vector direction) : this()
		{
			Center = center;
			Direction = direction;
		}
		

		/// <value>
		/// Name of the type.
		/// </value>
		public override string TypeName
		{
			get {return "refline";}
		}

		
#region Attributes
						
		/// <summary>
		/// Appends a momento to the momento list.
		/// </summary>
		protected override void AddMomento()
		{
			base.AddMomento();
			Momento momento = momentos[momentos.Count-1];
			momento["center"] = new Point();
			momento["direction"] = new Vector();
		}
				
		
		/// <value>
		/// A point that is intersected by the line.
		/// </value>
		public Point Center
		{
			get {return (Point)CurrentMomento["center"];}
			set { CurrentMomento["center"] = value;}
		}
		
		
		/// <value>
		/// The direction of the line.
		/// </value>
		public Vector Direction
		{
			get {return (Vector)CurrentMomento["direction"];}
			set { CurrentMomento["direction"] = value;}
		}
		
		
#endregion
		
		
#region Geometry

		protected Vector start;
		
		protected Vector stop;
		
#endregion
		
		
#region Rendering

		/// <summary>
		/// Computes the line's geometry.
		/// </summary>
		public override void ComputeGeometry()
		{
			base.ComputeGeometry();
			
			double t=4;
			start = Center.ToVector() - Direction * t;
			bounds.Resize(start);
			stop = Center.ToVector() + Direction * t;
			bounds.Resize(stop);
		}

		
		/// <summary>
		/// Renders the line to the viewport.
		/// </summary>
		/// <param name="viewport"> A <see cref="IViewport"/> to render to. </param>
		public override void Render (IViewport viewport)
		{
			base.Render(viewport);
						
			// render the points
			gl.glBegin(gl.GL_LINE);
			gl.glVertex3d(start[0], start[1], start[2]);
			gl.glVertex3d(stop[0], stop[1], stop[2]); 
			gl.glEnd();
		}

		
#endregion
		
		
	}
}
