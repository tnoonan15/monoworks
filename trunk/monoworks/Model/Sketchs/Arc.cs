// Arc.cs - MonoWorks Project
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
using MonoWorks.Base;

using gl = Tao.OpenGl.Gl;

namespace MonoWorks.Model
{
	
	
	/// <summary>
	/// The Arc class represents a circular arc with a 
	/// defined center, start point, and sweep angle.
	/// </summary>
	public class Arc : Sketchable
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public Arc() : base()
		{
		}
		
		/// <summary>
		/// Initialization constructor.
		/// </summary>
		/// <param name="center"> The center. </param>
		/// <param name="start"> The starting point. </param>
		/// <param name="sweep"> The sweep angle. </param>
		public Arc(Point center, Point start, Vector normal, Angle sweep) : this()
		{
			Center = center;
			Start = start;
			Normal = normal;
			Sweep = sweep;
		}
		
	
		
#region Attributes

		/// <value>
		/// The center.
		/// </value>
		public Point Center
		{
			get {return (Point)this["center"];}
			set
			{
				this["center"] = value;
			}
		}

		/// <value>
		/// The starting point.
		/// </value>
		public Point Start
		{
			get {return (Point)this["start"];}
			set
			{
				this["start"] = value;
			}
		}

		/// <value>
		/// The normal vector.
		/// </value>
		public Vector Normal
		{
			get {return (Vector)this["normal"];}
			set
			{
				this["normal"] = value;
			}
		}

		/// <value>
		/// The sweep angle.
		/// Rotations are applied using the right hand rule around the normal vector.
		/// </value>
		public Angle Sweep
		{
			get {return (Angle)this["sweep"];}
			set
			{
				this["sweep"] = value;
			}
		}
			
#endregion
		
		
#region Rendering
		
		/// <summary>
		/// Computes the raw points needed to draw the sketch.
		/// </summary>
		public override void ComputeGeometry()
		{
			base.ComputeGeometry();
			
			int N = 24; // temporary number of divisions
			Vector centerVec = Center.ToVector();
			Vector radius = (Start-Center).ToVector();
			Angle dSweep = Sweep / (double)N;
			solidPoints = new Vector[N+1];
			directions = new Vector[N+1];
			for (int i=0; i<=N; i++)
			{
				Vector thisPos = centerVec + radius.Rotate(Normal, dSweep*i);
				solidPoints[i] = thisPos;
				bounds.Resize(solidPoints[i]);
				
				// compute the direction
				directions[i] = (thisPos-centerVec).Cross(Normal).Normalize();
			}
			
			// make the wireframe points the first, middle, and last solid points
			wireframePoints = new Vector[3];
			wireframePoints[0] = solidPoints[0];
			wireframePoints[1] = solidPoints[solidPoints.Length/2];
			wireframePoints[2] = solidPoints[solidPoints.Length-1];
		}
		
#endregion
	}
}
