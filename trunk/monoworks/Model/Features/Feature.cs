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
using MonoWorks.Rendering;
using MonoWorks.Model.Sketching;

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
			this.drawing = sketch.TheDrawing;
			this.Sketch = sketch;
			
			// Copy the working momento back to the first momento to take into account 
			// the sketch. This has to be done becuase the Entity constructor sets the first 
			// momento but we want the sketch to be included.
			momentos[0]["sketch"] = sketch;
			
			// initialize the display lists
			displayLists = 0;
		}
		
		
		/// <summary>
		/// Release resources.
		/// </summary>
		~Feature()
		{
			// causes memory access violationg on Windows, don't know why
			//if (gl.glIsList(displayLists) != 0)
			//{
			//    gl.glDeleteLists(displayLists, NumLists); // delete the lists
			//}
		}		


#region Momentos
								
		/// <value>
		/// The feature's sketch.
		/// </value>
		public Sketch Sketch
		{
			get {return (Sketch)this["sketch"];}
			set
			{
				// remove the old sketch if there was one
//				if (Sketch != null)
//					RemoveChild(Sketch);
				this["sketch"] = value;
				// assign the sketch as a child of this feature
				value.Parent.RemoveChild(value);
				AddChild(value);
			}
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
		protected const int NumLists = 2;
		
		/// <value>
		/// The offset of the solid list.
		/// </value>
		protected const int SolidListOffset = 1;
		
		/// <value>
		/// The offset of the wireframe list.
		/// </value>
		protected const int WireframeListOffset = 0;
		
		
#endregion
		
			
		
#region Rendering

		/// <summary>
		/// Computes the feature's geometry.
		/// </summary>
		public override void ComputeGeometry()
		{
			base.ComputeGeometry();

			if (displayLists == 0)
				displayLists = gl.glGenLists(NumLists); // generate the display lists

			ComputeWireframeGeometry();
			
			ComputeSolidGeometry();
		}
		
		/// <summary>
		/// Computes the wireframe geometry.
		/// </summary>
		public virtual void ComputeWireframeGeometry()
		{
		}	
		
		/// <summary>
		/// Computes the solid geometry.
		/// </summary>
		public virtual void ComputeSolidGeometry()
		{
		}		

		
		/// <summary>
		/// If the feature is transparent, renders it.
		/// </summary>
		/// <param name="viewport"> A <see cref="Viewport"/> to render to. </param>
		public override void RenderTransparent(Viewport viewport)
		{
			base.RenderTransparent(viewport);
		}
		
		/// <summary>
		/// Renders the feature, recomputing geometry if necessary.
		/// </summary>
		/// <param name="viewport"> A <see cref="Viewport"/> to render to. </param>
		 public override void RenderOpaque(Viewport viewport)
		{
			base.RenderOpaque(viewport);
			
			// render solid geometry
			if (viewport.RenderManager.SolidMode != SolidMode.None)
			{
				ColorManager.Global["Gray"].Setup();
				gl.glCallList(displayLists+SolidListOffset);
			}
			
			// render the wireframe
			if (viewport.RenderManager.ShowWireframe)
			{
				gl.glLineWidth( viewport.RenderManager.WireframeWidth);
				viewport.RenderManager.WireframeColor.Setup();
				gl.glCallList(displayLists+WireframeListOffset);
			}
		}

		
#endregion
		
	}
	
}
