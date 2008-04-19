// Feature.cs - MonoWorks Project
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
using glu = Tao.OpenGl.Glu;

using MonoWorks.Base;

namespace MonoWorks.Model
{
		
	
	/// <summary>
	/// Features are entities that map sketches into 3D surfaces.
	/// </summary>
	public class Feature : Entity
	{
		
		/// <summary>
		/// Default constructor.
		/// </summary>
		public Feature(Sketch sketch) : base()
		{
			this.Sketch = sketch;
			
			// initialize the display lists
			displayLists = 0;
		}
		
		
		/// <summary>
		/// Release resources.
		/// </summary>
		~Feature()
		{
			gl.glDeleteLists(displayLists, NumLists);
		}
	

#region Momentos
				
		/// <summary>
		/// Appends a momento to the momento list.
		/// </summary>
		protected override void AddMomento()
		{
			base.AddMomento();
			Momento momento = momentos[momentos.Count-1];
			momento["sketch"] = new Sketch();
		}
		
#endregion
	
		
#region Sketch

		public Sketch Sketch
		{
			get {return (Sketch)CurrentMomento["sketch"];}
			set {CurrentMomento["sketch"] = value;}
		}
	
#endregion
		

#region Display Lists
		
		/// <value>
		/// The display lists.
		/// </value>
		protected int displayLists;
		
		/// <value>
		/// The number of display lists.
		/// </value>
		public const int NumLists = 1;
		
#endregion
		
		
#region Normals

		/// <summary>
		/// Sets the OpenGL normal vector for a quadrilateral defined by the given corners.
		/// </summary>
		protected static void QuadNormal(Vector c1, Vector c2, Vector c3, Vector c4)
		{
			
		}
		
#endregion
		
		
		
#region Rendering

		/// <summary>
		/// Computes the feature's geometry.
		/// </summary>
		public override void ComputeGeometry ()
		{
			base.ComputeGeometry ();
			
			// ensure the display lists are empty
			if (gl.glIsList(displayLists)==0)
			{
				gl.glDeleteLists(displayLists, NumLists); // delete the lists
			}
			displayLists = gl.glGenLists(NumLists); // regenerate the display lists
		}

		
		
		/// <summary>
		/// Renders the feature, recomputing geometry if necessary.
		/// </summary>
		/// <param name="viewport"> A <see cref="IViewport"/> to render to. </param>
		 public override void Render (IViewport viewport)
		{
			base.Render(viewport);
			gl.glCallList(displayLists);
		}

		
#endregion
		
	}
	
}
