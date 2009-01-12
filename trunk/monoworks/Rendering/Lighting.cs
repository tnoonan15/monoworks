// Lighting.cs - MonoWorks Project
//
//  Copyright (C) 2008 Andy Selvig
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

using gl = Tao.OpenGl.Gl;

namespace MonoWorks.Rendering
{
	
	/// <summary>
	/// Handles lighting for a viewport.
	/// </summary>
	public class Lighting
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public Lighting()
		{
		}
		
		/// <summary>
		/// Initialize lighting in the OpenGL context.
		/// </summary>
		public void Initialize()
		{
			gl.glEnable(gl.GL_LIGHTING);
			gl.glEnable(gl.GL_LIGHT0);
			gl.glEnable(gl.GL_LIGHT1);
			
			float lightDist = 3.0f;
            float[] lightAmbient = new float[]{0.2f, 0.2f, 0.2f};
            float[] lightDiffuse = new float[]{0.5f, 0.5f, 0.5f};
            float[] lightSpecular = new float[]{0.5f, 0.5f, 0.5f};
            float[] lightPos0 = new float[]{lightDist, lightDist, -lightDist,2f};
            gl.glLightfv(gl.GL_LIGHT0, gl.GL_POSITION, lightPos0);
            gl.glLightfv(gl.GL_LIGHT0, gl.GL_AMBIENT, lightAmbient);
            gl.glLightfv(gl.GL_LIGHT0, gl.GL_DIFFUSE, lightDiffuse);
            gl.glLightfv(gl.GL_LIGHT0, gl.GL_SPECULAR, lightSpecular);

		}


		/// <summary>
		/// Enables lighting.
		/// </summary>
		public void Enable()
		{
			gl.glEnable(gl.GL_LIGHTING);
		}

		/// <summary>
		/// Disables lighting.
		/// </summary>
		public void Disable()
		{
			gl.glDisable(gl.GL_LIGHTING);
		}
		
		
	}
}
