// AbstractSketcher.cs - MonoWorks Project
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

using gl = Tao.OpenGl.Gl;

using MonoWorks.Base;
using MonoWorks.Rendering;
using MonoWorks.Rendering.Events;


namespace MonoWorks.Model
{
	/// <summary>
	/// Abstract base class for skecthers (classes that handle the user interface of sketching).
	/// </summary>
	public abstract class AbstractSketcher : Actor
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		/// <param name="sketch"></param>
		public AbstractSketcher(Sketch sketch)
		{
			Sketch = sketch;
		}

		/// <summary>
		/// The sketch being sketched on.
		/// </summary>
		public Sketch Sketch { get; private set; }

		/// <summary>
		/// Delegate for the SketchApplied event.
		/// </summary>
		public delegate void SketchAppliedHandler();

		/// <summary>
		/// Gets called when the sketch is applied.
		/// </summary>
		public event SketchAppliedHandler SketchApplied;

		/// <summary>
		/// Apply the current sketching operation.
		/// </summary>
		public virtual void Apply()
		{
			if (SketchApplied != null)
				SketchApplied();
		}


		public virtual void OnKeyPress(KeyEvent evt)
		{
		}



#region Rendering Helpers

		/// <summary>
		/// Highlights the given point.
		/// </summary>
		protected void HighlightPoint(Viewport viewport, Point point, Color color, float size)
		{
			viewport.RenderManager.Lighting.Disable();
			gl.glPointSize(size);
			color.Setup();
			gl.glBegin(gl.GL_POINTS);
			point.glVertex();
			gl.glEnd();
			viewport.RenderManager.Lighting.Enable();
		}

#endregion


	}
}
