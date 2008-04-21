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

namespace MonoWorks.Model
{
	
	/// <summary>
	/// The solid mode tells features how to draw their surfaces.
	/// </summary>
	public enum SolidMode {None, Flat, Smooth};
	
	
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
			showWireframe = false;
			solidMode = SolidMode.Flat;
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
		
		
		
	}
}
