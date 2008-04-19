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
	

#region Momentos
				
		/// <summary>
		/// Appends a momento to the momento list.
		/// </summary>
		protected override void AddMomento()
		{
			base.AddMomento();
			Momento momento = momentos[momentos.Count-1];
			momento["center"] = new Point();
			momento["start"] = new Point();
			momento["normal"] = new Vector();
			momento["sweep"] = new Angle();
		}
		
#endregion
		
		
#region Attributes

		/// <value>
		/// The center.
		/// </value>
		public Point Center
		{
			get {return (Point)CurrentMomento["center"];}
			set
			{
				CurrentMomento["center"] = value;
				ComputeGeometry();
			}
		}

		/// <value>
		/// The starting point.
		/// </value>
		public Point Start
		{
			get {return (Point)CurrentMomento["start"];}
			set
			{
				CurrentMomento["start"] = value;
				ComputeGeometry();
			}
		}

		/// <value>
		/// The normal vector.
		/// </value>
		public Vector Normal
		{
			get {return (Vector)CurrentMomento["normal"];}
			set
			{
				CurrentMomento["normal"] = value;
				ComputeGeometry();
			}
		}

		/// <value>
		/// The sweep angle.
		/// Rotations are applied using the right hand rule around the normal vector.
		/// </value>
		public Angle Sweep
		{
			get {return (Angle)CurrentMomento["sweep"];}
			set
			{
				CurrentMomento["sweep"] = value;
				ComputeGeometry();
			}
		}
			
#endregion
		
		
#region Rendering
		
		/// <summary>
		/// Computes the raw points needed to draw the sketch.
		/// </summary>
		public override void ComputeGeometry()
		{
			int N = 24; // temporary number of divisions
			Vector centerVec = Center.ToVector();
			Vector radius = (Start-Center).ToVector();
			Angle dSweep = Sweep / (double)N;
			rawPoints = new Vector[N+1];
			for (int i=0; i<=N; i++)
			{
				Vector thisPos = centerVec + radius.Rotate(Normal, dSweep*i);
				rawPoints[i] = thisPos;
				bounds.Resize(rawPoints[i]);
			}
		}
		
#endregion
	}
}
