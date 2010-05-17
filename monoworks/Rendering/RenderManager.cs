//   RenderManager.cs - MonoWorks Project
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

using Tao.OpenGl;

namespace MonoWorks.Rendering
{
	
	/// <summary>
	/// The solid mode tells features how to draw their surfaces.
	/// </summary>
	public enum SolidMode {None, Flat, Smooth};
	
	/// <summary>
	/// The color mode determine what color/material to use for solid surfaces.
	/// </summary>
	public enum ColorMode {Cartoon, Realistic};
	
	
	/// <summary>
	/// The RenderManager class manages the rendering process for a viewport.
	/// It keeps track of display modes like wireframe/solid and cartoon/realistic.
	/// </summary>
	public class RenderManager
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public RenderManager()
		{
			SolidMode = SolidMode.Smooth;
			ColorMode = ColorMode.Cartoon;
			
			ShowWireframe = false;
			WireframeColor = ColorManager.Global["Black"];
			WireframeWidth = 1.5f;
			
			ReferenceColor = new Color(0, 128, 0, 32);
		}
		
		
		/// <summary>
		/// Initialized rendering.
		/// </summary>
		public void Initialize()
		{
			InitializeGL();
		}
		

		/// <summary>
		/// Initialize the OpenGL rendering.
		/// </summary>
		/// <remarks> This should only really be called once at the beginning.</remarks>
		protected void InitializeGL()
		{

			Gl.glClearColor(1.0f, 1.0f, 1.0f, 1.0f);
			Gl.glClearDepth(1.0f);

//			Gl.glEnable(Gl.GL_COLOR_MATERIAL);

			// depth testing
			Gl.glEnable(Gl.GL_DEPTH_TEST);
			Gl.glDepthFunc(Gl.GL_LEQUAL); // The Type Of Depth Test To Do
			//Gl.glDepthMask(Gl.GL_TRUE);


            // enable polygon offset so wireframes are displayed correctly
            Gl.glPolygonOffset(1f, 1f);


			// blending
			Gl.glEnable(Gl.GL_BLEND);
			Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA); // kinda works
			//Gl.glBlendFunc(Gl.GL_SRC_ALPHA_SATURATE, Gl.GL_ONE);
//			Gl.glBlendFunc(Gl.GL_ONE, Gl.GL_ONE);
			Gl.glShadeModel(Gl.GL_FLAT);
			Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL);
			Gl.glDisable(Gl.GL_CULL_FACE);

			// enable antialiasing
//			Gl.glHint(Gl.GL_LINE_SMOOTH_HINT, Gl.GL_NICEST);
//			Gl.glHint(Gl.GL_POLYGON_SMOOTH_HINT, Gl.GL_NICEST);

//			Gl.glFrontFace(Gl.GL_CW);

			// enable stipplinggl
//			Gl.glEnable(Gl.GL_LINE_STIPPLE);

			// Really Nice Perspective Calculations
			//Gl.glHint(Gl.GL_PERSPECTIVE_CORRECTION_HINT, Gl.GL_NICEST);
			
			// textures
			Gl.glShadeModel(Gl.GL_FLAT);
			Gl.glDisable(Gl.GL_TEXTURE_2D);
			Gl.glEnable(Gl.GL_TEXTURE_RECTANGLE_ARB);
			Gl.glTexEnvi(Gl.GL_TEXTURE_ENV, Gl.GL_TEXTURE_ENV_MODE, Gl.GL_MODULATE);
			Gl.glTexParameteri(Gl.GL_TEXTURE_RECTANGLE_ARB,
				 Gl.GL_TEXTURE_MIN_FILTER,
				 Gl.GL_NEAREST);
			Gl.glTexParameteri(Gl.GL_TEXTURE_RECTANGLE_ARB,
				 Gl.GL_TEXTURE_MAG_FILTER,
				 Gl.GL_LINEAR);
			
		}


		/// <summary>
		/// Clears the scene for the next rendering frame.
		/// </summary>
		/// <remarks> This should be done at the beginning of each frame.</remarks>
		public virtual void ClearScene()
		{
			Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
		}


		/// <summary>
		/// Sets up the current OpenGL context for rendering solids.
		/// </summary>
		public void BeginSolids()
		{
			switch (SolidMode)
			{
				case SolidMode.None:
					break;
				case SolidMode.Flat:
					Gl.glShadeModel(Gl.GL_FLAT);
					break;
				case SolidMode.Smooth:
					Gl.glShadeModel(Gl.GL_SMOOTH);
					break;
			}
			Gl.glEnable(Gl.GL_DEPTH_TEST);
			Gl.glEnable(Gl.GL_POLYGON_OFFSET_FILL);
		}

		/// <summary>
		/// Sets up OpenGL to render overlays instead of 3D content.
		/// </summary>
		public void BeginOverlays()
		{
			Gl.glDisable(Gl.GL_DEPTH_TEST);
			Gl.glShadeModel(Gl.GL_SMOOTH);
			Gl.glDisable(Gl.GL_POLYGON_OFFSET_FILL);
		}

		/// <summary>
		/// Enables line and polygon antialiasing.
		/// </summary>
		public void EnableAntialiasing()
		{
			Gl.glEnable(Gl.GL_LINE_SMOOTH);
			Gl.glEnable(Gl.GL_POLYGON_SMOOTH);
		}

		/// <summary>
		/// Disables line and polygon antialiasing.
		/// </summary>
		public void DisableAntialiasing()
		{
			Gl.glDisable(Gl.GL_LINE_SMOOTH);
			Gl.glDisable(Gl.GL_POLYGON_SMOOTH);
		}

		
#region Wireframe Display
		
		/// <value>
		/// Whether or not to render the wireframe.
		/// </value>
		public bool ShowWireframe { get; set; }
		
		/// <value>
		/// The wireframe color.
		/// </value>
		public Color WireframeColor { get; set; }
		
		/// <value>
		/// Wireframe width.
		/// </value>
		public float WireframeWidth { get; set; }
		
#endregion
		
		
#region Attrbutes
		
		/// <value>
		/// The color of reference items;
		/// </value>
		public Color ReferenceColor { get; set; }		
	

		/// <value>
		/// The solid rendering mode.
		/// </value>
		public SolidMode SolidMode { get; set; }


		/// <value>
		/// The feature's color mode.
		/// </value>
		public ColorMode ColorMode { get; set; }
		
#endregion
				
		
		
		
	}
}
