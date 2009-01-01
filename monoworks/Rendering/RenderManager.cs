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

using gl = Tao.OpenGl.Gl;
using il=Tao.DevIl.Il;

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
//			this.colorManager = new ColorManager();
			
			solidMode = SolidMode.Flat;
			colorMode = ColorMode.Cartoon;
			
			showWireframe = false;
			wireframeColor = ColorManager.Global["Black"];
			wireframeWidth = 1.5f;
			
			referenceColor = new Color(0, 128, 0, 128);
		}
		
		
//		protected ColorManager colorManager;


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

			gl.glClearColor(1.0f, 1.0f, 1.0f, 0.0f);
			gl.glClearDepth(1.0f);

			gl.glEnable(gl.GL_AUTO_NORMAL);
			gl.glEnable(gl.GL_NORMALIZE);
			gl.glEnable(gl.GL_COLOR_MATERIAL);

			// depth testing
			gl.glEnable(gl.GL_DEPTH_TEST);
			gl.glDepthFunc(gl.GL_LEQUAL); // The Type Of Depth Test To Do

			// blending
			gl.glShadeModel(gl.GL_SMOOTH);						// Enables Smooth Shading
			gl.glEnable(gl.GL_BLEND);
			gl.glBlendFunc(gl.GL_SRC_ALPHA, gl.GL_ONE_MINUS_SRC_ALPHA);

			gl.glFrontFace(gl.GL_CW);

			// Really Nice Perspective Calculations
			gl.glHint(gl.GL_PERSPECTIVE_CORRECTION_HINT, gl.GL_NICEST);
		}


		/// <summary>
		/// Clears the scene for the next rendering frame.
		/// </summary>
		/// <remarks> This should be done at the beginning of each frame.</remarks>
		public virtual void ClearScene()
		{
			gl.glClear(gl.GL_COLOR_BUFFER_BIT | gl.GL_DEPTH_BUFFER_BIT);
		}

		
#region Wireframe Display
		
		protected bool showWireframe;
		/// <value>
		/// Whether or not to render the wireframe.
		/// </value>
		public bool ShowWireframe
		{
			get {return showWireframe;}
			set {showWireframe = value;}
		}
		
		
		protected Color wireframeColor;
		/// <value>
		/// The wireframe color.
		/// </value>
		public Color WireframeColor
		{
			get {return wireframeColor;}
			set {wireframeColor = value;}
		}
		
		protected float wireframeWidth;
		/// <value>
		/// Wireframe width.
		/// </value>
		public float WireframeWidth
		{
			get {return wireframeWidth;}
			set {wireframeWidth = value;}
		}
		
#endregion
		
		
#region Reference Items
		
		protected Color referenceColor;
		/// <value>
		/// The color of reference items;
		/// </value>
		public Color ReferenceColor
		{
			get {return referenceColor;}
			set {referenceColor = value;}
		}		
		
#endregion
		
		
#region Solid Mode

		protected SolidMode solidMode;
		/// <value>
		/// The solid rendering mode.
		/// </value>
		public SolidMode SolidMode
		{
			get {return solidMode;}
			set {solidMode = value;}
		}
		
		/// <summary>
		/// Sets up the current OpenGL context for rendering solids.
		/// </summary>
		public void SetupSolidMode()
		{
			switch (solidMode)
			{
			case SolidMode.None:
				break;
			case SolidMode.Flat:
				gl.glShadeModel(gl.GL_FLAT);
				break;
			case SolidMode.Smooth:
				gl.glShadeModel(gl.GL_SMOOTH);
				break;
			}
		}

#endregion
		

#region Color Mode
		
		protected ColorMode colorMode;
		/// <value>
		/// The feature's color mode.
		/// </value>
		public ColorMode ColorMode
		{
			get {return colorMode;}
			set {colorMode = value;}
		}		
		
#endregion
		
	}
}
