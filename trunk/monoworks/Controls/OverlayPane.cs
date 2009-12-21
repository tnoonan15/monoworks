// OverlayPane.cs - MonoWorks Project
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

using Tao.OpenGl;

using MonoWorks.Base;
using MonoWorks.Rendering;
using MonoWorks.Rendering.Events;


namespace MonoWorks.Controls
{
	
	/// <summary>
	/// The overlay pane is a 2D pane that can be placed as an overlay over the scene.
	/// </summary>
	public class OverlayPane : Overlay, IPane
	{
		
		public OverlayPane() : base()
		{
			
		}
		
		public OverlayPane(Control2D control) : this()
		{
			this.Control = control;
		}
		
		
		
		/// <value>
		/// The size of the pane.
		/// </value>
		public Coord Size
		{
			get
			{
				if (Control != null)
					return Control.Size;
				else
					return new Coord();
			}
		}
		
		public double Width
		{
			get {return Size.X;}
		}
		
		public double Height
		{
			get {return Size.Y;}
		}
		
		/// <value>
		/// The position of the upper left corner of the pane.
		/// </value>
		public Coord Origin {get; set;}
		
		
		private Control2D control;
		
		public Control2D Control
		{
			get {return control;}
			set
			{
				if (control != null)
					control.Pane = null;
				if (value != null)
				{
					control = value;
					control.Pane = this;
				}
			}
		}
				
		
		
#region Interaction
		
		/// <summary>
		/// Gets a point in control-space corresponding to the screen-space point given.
		/// </summary>
		private Coord GetControlPoint(Coord screen)
		{
			var point = screen - Origin;
			point.Y = Height-point.Y;
			return point;
		}
		
		public override void OnButtonPress(MouseButtonEvent evt)
		{
			base.OnButtonPress (evt);
			
			if (Control != null)
			{
				var controlEvt = new MouseButtonEvent(GetControlPoint(evt.Pos), evt.Button, evt.Modifier, evt.Multiplicity);
				Control.OnButtonPress(controlEvt);
			}
		}

		public override void OnButtonRelease(MouseButtonEvent evt)
		{
			base.OnButtonRelease (evt);
			
			if (Control != null)
			{
				var controlEvt = new MouseButtonEvent(GetControlPoint(evt.Pos), evt.Button, evt.Modifier, evt.Multiplicity);
				Control.OnButtonRelease(controlEvt);
			}
		}

		public override void OnMouseMotion(MouseEvent evt)
		{
			base.OnMouseMotion (evt);
			
			if (Control != null)
			{
				var controlEvt = new MouseEvent(GetControlPoint(evt.Pos), evt.Modifier);
				Control.OnMouseMotion(controlEvt);
			}
		}

		public override void OnMouseWheel(MouseWheelEvent evt)
		{
			base.OnMouseWheel (evt);
			
			if (Control != null)
			{
				var controlEvt = new MouseWheelEvent(evt.Direction, evt.Modifier);
				Control.OnMouseWheel(controlEvt);
			}
		}
		
		protected override bool HitTest(Coord pos)
		{
			throw new System.NotImplementedException ();
		}

#endregion
		
		
#region Rendering
		
		/// <summary>
		/// This is set true if the pane was dirty last render cycle.
		/// </summary>
		private bool wasDirty = false;
		
		/// <summary>
		/// Handle to the OpenGL texture that the control will be rendered to.
		/// </summary>
		private uint texture = 0;
		
		public override void ComputeGeometry()
		{
			base.ComputeGeometry();
			
			if (Control == null)
				return;
						
			if (texture == 0)
				Gl.glGenTextures(1, out texture);
			
			wasDirty = true;
						
		}
		
		public override void RenderOverlay(Viewport viewport)
		{
			if (Control == null || !Control.IsVisible)
				return;
			
			base.RenderOverlay(viewport);
			
			if (Control.IsDirty)
				ComputeGeometry();
			
			// render the control to the texture
			if (wasDirty)
			{
				Gl.glBindTexture( Gl.GL_TEXTURE_RECTANGLE_ARB, texture );
				Control.RenderImage(viewport);
				Gl.glTexImage2D(Gl.GL_TEXTURE_RECTANGLE_ARB,
			                0, 
			                Gl.GL_RGBA, 
			                Control.IntWidth, 
			                Control.IntHeight, 
			                0, 
			                Gl.GL_BGRA, 
			                Gl.GL_UNSIGNED_BYTE, 
			                Control.ImageData);
				wasDirty = false;
			}
			
			
			// render the texture
//			viewport.Lighting.Disable();
			Gl.glEnable(Gl.GL_TEXTURE_RECTANGLE_ARB);
			Gl.glBindTexture( Gl.GL_TEXTURE_RECTANGLE_ARB, texture );
			Gl.glBegin(Gl.GL_QUADS);
			Gl.glColor3f(1f, 1f, 1f);
			Gl.glTexCoord2d(0.0,Control.Height);
			Origin.glVertex();
			Gl.glTexCoord2d(Control.Width, Control.Height);
			Gl.glVertex2d(Origin.X + Width, Origin.Y);
			Gl.glTexCoord2d(Control.Width,0.0);
			Gl.glVertex2d(Origin.X + Width, Origin.Y + Height);
			Gl.glTexCoord2d(0.0,0.0);
			Gl.glVertex2d(Origin.X, Origin.Y + Height);
			Gl.glEnd();
			Gl.glDisable(Gl.GL_TEXTURE_RECTANGLE_ARB);
//			viewport.Lighting.Enable();
		}

#endregion

		
	}
}