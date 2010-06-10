// ModalOverlay.cs - MonoWorks Project
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
using gl = Tao.OpenGl.Gl;

using MonoWorks.Base;

namespace MonoWorks.Rendering
{

	/// <summary>
	/// Overlays that can be displayed in a modal (blocking all other UI) form in the viewport.
	/// </summary>
	public abstract class ModalOverlay : Overlay
	{		
		
		/// <summary>
		/// This event gets raised when the modal overlay whishes to be closed.
		/// </summary>
		public event EventHandler Closed;
		
		/// <summary>
		/// Closes the modal overlay.
		/// </summary>
		public void Close()
		{
			if (Closed != null)
				Closed(this, new EventArgs());
		}

		/// <summary>
		/// If true, the rest of the scene will be grayed out when the overlay is shown.
		/// </summary>
		public bool	GrayScene { get; set; }
		
		protected override bool HitTest(Coord pos)
		{
			return true;
		}

		public override void RenderOverlay(Scene scene)
		{

			if (GrayScene)
			{
				// shade out the background
				scene.Lighting.Disable();					
				gl.glColor4d(0.85, 0.85, 0.85, 0.75);
				gl.glBegin(gl.GL_POLYGON);
				gl.glVertex2i(0, 0);
				gl.glVertex2d(scene.Width, 0);
				gl.glVertex2d(scene.Width, scene.Height);
				gl.glVertex2d(0, scene.Height);
				gl.glEnd();
			}
			base.RenderOverlay(scene);
		}
		
		
		/// <summary>
		/// Gets called by the scene when the modal overlay is shown in it.
		/// </summary>
		public virtual void OnShown(Scene scene)
		{			
		}

		
	}
}
