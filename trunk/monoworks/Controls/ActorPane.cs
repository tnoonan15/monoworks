// ActorPane.cs - MonoWorks Project
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

using MonoWorks.Base;
using MonoWorks.Rendering;
using MonoWorks.Rendering.Events;

using Tao.OpenGl;

namespace MonoWorks.Controls
{
	
	/// <summary>
	/// The ActorPane is an 3D plane that can be placed directly into the scene.
	/// </summary>
	public class ActorPane : Actor, IPane, IPlane
	{
		
		public ActorPane() : base()
		{
			Origin = new Vector();
			XAxis = new Vector(1, 0, 0);
			Normal = new Vector(0, 0, 1);
			Scaling = 1;
		}
				
		public ActorPane(Control2D control) : this()
		{
			Control = control;
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
		
		public Vector Origin {get; set;}
		
		public Vector Normal {get; set;}
		
		public Vector XAxis {get; set;}
		
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
		/// Gets a point in control-space corresponding to the hit line in 3D space.
		/// </summary>
		private Coord GetControlPoint(HitLine hitLine)
		{
			var intersection = hitLine.GetIntersection(this);
			var point = this.Project(intersection) / Scaling;
			point.Y = Height - point.Y;
			return point;
		}
		
		public override void OnButtonPress(MouseButtonEvent evt)
		{
			base.OnButtonPress(evt);
			
			if (Control != null)
			{
				var controlEvt = new MouseButtonEvent(GetControlPoint(evt.HitLine), evt.Button, evt.Modifier, evt.Multiplicity);
				Control.OnButtonPress(controlEvt);
				if (controlEvt.Handled)
					evt.Handle();
			}
		}

		public override void OnButtonRelease(MouseButtonEvent evt)
		{
			base.OnButtonRelease(evt);
			
			if (Control != null)
			{
				var controlEvt = new MouseButtonEvent(GetControlPoint(evt.HitLine), evt.Button, evt.Modifier, evt.Multiplicity);
				Control.OnButtonRelease(controlEvt);
				if (controlEvt.Handled)
					evt.Handle();
			}
		}

		public override void OnMouseMotion(MouseEvent evt)
		{
			base.OnMouseMotion(evt);
			
			if (Control != null)
			{
				var controlEvt = new MouseEvent(GetControlPoint(evt.HitLine), evt.Modifier);
				Control.OnMouseMotion(controlEvt);
				if (controlEvt.Handled)
					evt.Handle();
			}
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
		
		/// <summary>
		/// The scaling to go from viewport to world coordinates.
		/// </summary>
		public double Scaling { get; private set; }
		
		public override void ComputeGeometry()
		{
			base.ComputeGeometry();
			
			if (Control == null)
				return;
						
			if (texture == 0)
				Gl.glGenTextures(1, out texture);
			
			wasDirty = true;
			
		}
		
		public override void RenderTransparent(Viewport viewport)
		{
			if (Control == null)
				return;
			
			base.RenderTransparent(viewport);
			
			if (Control.IsDirty)
				ComputeGeometry();
			
			// render the control to the texture
			if (wasDirty)
			{
				Gl.glBindTexture(Gl.GL_TEXTURE_RECTANGLE_ARB, texture);
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
			
			// determine how big the control should be 
			Scaling = viewport.Camera.ViewportToWorldScaling;
			double width = Width * Scaling;
			double height = Height * Scaling;
						
			// render the texture
			viewport.Lighting.Disable();
			Gl.glEnable(Gl.GL_TEXTURE_RECTANGLE_ARB);
			Gl.glBindTexture( Gl.GL_TEXTURE_RECTANGLE_ARB, texture );
			Gl.glBegin(Gl.GL_QUADS);
			Gl.glColor3f(1f, 1f, 1f);
			Gl.glTexCoord2d(0.0,Control.Height);
			Origin.glVertex();
			Gl.glTexCoord2d(Control.Width, Control.Height);
			Gl.glVertex2d(Origin.X + width, Origin.Y);
			Gl.glTexCoord2d(Control.Width,0.0);
			Gl.glVertex2d(Origin.X + width, Origin.Y + height);
			Gl.glTexCoord2d(0.0,0.0);
			Gl.glVertex2d(Origin.X, Origin.Y + height);
			Gl.glEnd();
			Gl.glDisable(Gl.GL_TEXTURE_RECTANGLE_ARB);
			viewport.Lighting.Enable();
		}

#endregion
		
	}
}
