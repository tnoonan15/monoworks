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


namespace MonoWorks.Rendering
{
	
	/// <summary>
	/// The overlay pane is a 2D pane that can be placed as an overlay over the scene.
	/// </summary>
	public class OverlayPane : Overlay, IPane
	{
		
		public OverlayPane()
		{
			Origin = new Coord();
			OriginLocation = AnchorLocation.NW;
			Angle = new Angle();
		}
		
		public OverlayPane(Renderable2D control) : this()
		{
			Control = control;
		}
		
		
		
		/// <value>
		/// The size of the pane.
		/// </value>
		public Coord RenderSize
		{
			get
			{
				if (Control != null)
					return Control.RenderSize;
				return new Coord();
			}
		}
		
		public double RenderWidth
		{
			get { return RenderSize.X; }
			set { RenderSize.X = value; }
		}
		
		public double RenderHeight
		{
			get { return RenderSize.Y; }
			set { RenderSize.Y = value; }
		}
		
		/// <value>
		/// The position of the upper left corner of the pane.
		/// </value>
		public Coord Origin { get; set; }


		private AnchorLocation _originLocation;
		/// <summary>
		/// The location of the origin with respect to the pane.
		/// </summary>
		public AnchorLocation OriginLocation
		{
			get { return _originLocation; }
			set
			{
				_originLocation = value;
				MakeDirty();
			}
		}

		/// <summary>
		/// Gets the render origin of the pane (upper left corner).
		/// </summary>
		public Coord RenderOrigin
		{
			get
			{
				var coord = new Coord();
				switch (OriginLocation)
				{
				case AnchorLocation.NW:
					coord.X = Origin.X;
					coord.Y = Origin.Y;
					break;
				case AnchorLocation.NE:
					coord.X = Origin.X - RenderWidth;
					coord.Y = Origin.Y;
					break;
				case AnchorLocation.SW:
					coord.X = Origin.X;
					coord.Y = Origin.Y - RenderHeight;
					break;
				case AnchorLocation.SE:
					coord.X = Origin.X - RenderWidth;
					coord.Y = Origin.Y - RenderHeight;
					break;
				case AnchorLocation.None:
					coord.X = Origin.X - RenderWidth / 2.0;
					coord.Y = Origin.Y - RenderHeight / 2.0;
					break;
				default:
					throw new Exception("Don't know how to layout OverlayPane with OriginLocation " + OriginLocation.ToString());
				}
				return coord;
			}
		}

		/// <summary>
		/// Performs a hit test on the pane.
		/// </summary>
		public bool HitTest(MouseEvent evt)
		{
			var origin = RenderOrigin;
			return origin.X <= evt.Pos.X && origin.X + RenderWidth >= evt.Pos.X &&
				origin.Y <= evt.Pos.Y && origin.Y + RenderHeight >= evt.Pos.Y;
		}

		/// <summary>
		/// The angle to rotate the pane before rendering to the scene.
		/// </summary>
		public Angle Angle { get; set; }
		
		
		private Renderable2D control;
		
		public Renderable2D Control
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
		
		private Renderable2D _inFocus;
		/// <summary>
		/// Sets the control currently in focus.
		/// </summary>
		public Renderable2D InFocus
		{
			get {return _inFocus;}
			set {
				if (_inFocus == value)
					return;
				if (_inFocus != null)
					_inFocus.IsFocused = false;
				_inFocus = value;
				if (_inFocus != null && !_inFocus.IsFocused)
					_inFocus.IsFocused = true;
			}
		}
		
		/// <summary>
		/// Gets a point in control-space corresponding to the screen-space point given.
		/// </summary>
		private Coord GetControlPoint(Coord screen)
		{
			if (RenderSize == null)
				return new Coord();
			var point = screen - Origin;
			point.Y = RenderHeight-point.Y;
			return point;
		}
		
		public override void OnButtonPress(MouseButtonEvent evt)
		{
			base.OnButtonPress(evt);
			
			if (Control != null)
			{
				var controlEvt = evt.Copy();
				controlEvt.Pos = GetControlPoint(evt.Pos);
				Control.OnButtonPress(controlEvt);
				if (controlEvt.IsHandled)
					evt.Handle(controlEvt.LastHandler);
			}
		}

		public override void OnButtonRelease(MouseButtonEvent evt)
		{
			base.OnButtonRelease(evt);
			
			if (Control != null)
			{
				var controlEvt = evt.Copy();
				controlEvt.Pos = GetControlPoint(evt.Pos);
				Control.OnButtonRelease(controlEvt);
				if (controlEvt.IsHandled)
					evt.Handle(controlEvt.LastHandler);
			}
		}

		public override void OnMouseMotion(MouseEvent evt)
		{
			base.OnMouseMotion(evt);
			
			if (Control != null)
			{
				var controlEvt = evt.Copy();
				controlEvt.Pos = GetControlPoint(evt.Pos);
				if (evt.IsHandled)
					controlEvt.Handle(this);
				Control.OnMouseMotion(controlEvt);
				if (controlEvt.IsHandled)
					evt.Handle(controlEvt.LastHandler);
			}
		}

		public override void OnMouseWheel(MouseWheelEvent evt)
		{
			base.OnMouseWheel(evt);
			
			if (Control != null)
			{
				var controlEvt = new MouseWheelEvent(evt.Scene, evt.Direction, evt.Modifier);
				Control.OnMouseWheel(controlEvt);
				if (controlEvt.IsHandled)
					evt.Handle(controlEvt.LastHandler);
			}
		}
		
		public override void OnKeyPress(KeyEvent evt)
		{
			base.OnKeyPress(evt);
			
			if (Control == null)
				return;
			
			if (evt.SpecialKey == SpecialKey.Tab)
			{
				MakeDirty();
				if (evt.Modifier == InteractionModifier.Shift)
				{
					if (InFocus != null)
						InFocus = InFocus.GetPreviousFocus();
					else
						InFocus = Control.GetLastFocus();
				}
				else
				{
					if (InFocus != null)
						InFocus = InFocus.GetNextFocus();
					else
						InFocus = Control.GetFirstFocus();
				}
			}			
			else if (InFocus != null)
			{
				
				InFocus.OnKeyPress(evt);
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
		private bool wasDirty = true;
				
		public void QueueRender()
		{
			wasDirty = true;
		}
		
		/// <summary>
		/// Handle to the OpenGL texture that the control will be rendered to.
		/// </summary>
		private uint texture = 0;
		
		public override void ComputeGeometry()
		{
			base.ComputeGeometry();
			
			if (Control == null)
				return;
			
			if (Control.IsDirty)
				Control.ComputeGeometry();

			RenderWidth = Control.RenderWidth;
			RenderHeight = Control.RenderHeight;
			
			wasDirty = true;
						
		}
		
		public override void RenderOverlay(Scene scene)
		{
			if (Control == null || !Control.IsVisible)
				return;

			if (Control.IsDirty || wasDirty)
				ComputeGeometry();
			
			base.RenderOverlay(scene);
			
			// generate the texture
			if (texture == 0) {
				Gl.glGenTextures(1, out texture);
			}
			
			// render the control to the texture
			if (wasDirty)
			{
				Gl.glBindTexture( Gl.GL_TEXTURE_RECTANGLE_ARB, texture );
				Control.RenderImage(scene);
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
			
			// determine where to render the pane (based on origin and origin location)
			var origin = RenderOrigin;

			// apply rotation
			//if (Angle.Value != 0)
			//{
			//    //Gl.glPushMatrix();
			//    Gl.glRotated(Angle.Radians, 0, 0, 1);
			//}
			
			// render the texture
			scene.Lighting.Disable();
			Gl.glEnable(Gl.GL_TEXTURE_RECTANGLE_ARB);
			Gl.glBindTexture( Gl.GL_TEXTURE_RECTANGLE_ARB, texture );
			Gl.glBegin(Gl.GL_QUADS);
			Gl.glColor3f(1f, 1f, 1f);
			Gl.glTexCoord2d(0.0, Control.RenderHeight);
			Gl.glVertex2d(origin.X, origin.Y);
			Gl.glTexCoord2d(Control.RenderWidth, Control.RenderHeight);
			Gl.glVertex2d(origin.X + RenderWidth, origin.Y);
			Gl.glTexCoord2d(Control.RenderWidth,0.0);
			Gl.glVertex2d(origin.X + RenderWidth, origin.Y + RenderHeight);
			Gl.glTexCoord2d(0.0,0.0);
			Gl.glVertex2d(origin.X, origin.Y + RenderHeight);
			Gl.glEnd();
			Gl.glDisable(Gl.GL_TEXTURE_RECTANGLE_ARB);
			scene.Lighting.Enable();


			// reverse rotation
			//if (Angle.Value != 0)
			//{
			//    //Gl.glPopMatrix();
			//    Gl.glRotated(-Angle.Radians, 0, 0, 1);
			//}
		}
		
		#endregion

		
	}
}
