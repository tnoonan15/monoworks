// Control.cs - MonoWorks Project
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

using MonoWorks.Base;
using MonoWorks.Rendering;
using MonoWorks.Rendering.Events;


using gl = Tao.OpenGl.Gl;
using glu = Tao.OpenGl.Glu;

namespace MonoWorks.Rendering.Controls
{
	
	/// <summary>
	/// Base class for all renderable controls.
	/// </summary>
	public abstract class Control : Overlay
	{
		
		public Control() : base()
		{
			styleGroup = StyleGroup.Default;

			UserSize = false;
			ToolTip = "";
		}
		
		private Control parent = null;
		/// <value>
		/// The control's parent.
		/// </value>
		public Control Parent
		{
			get {return parent;}
			set
			{
				parent = value;
			}
		}

		/// <summary>
		/// The control tooltip.
		/// </summary>
		public string ToolTip { get; set; }

		/// <summary>
		/// Queues the control to set its tooltip next time it's rendered.
		/// </summary>
		private bool queueSetToolTip = false;

		/// <summary>
		/// Queues the control to clear the tooltip next time it's rendered.
		/// </summary>
		private bool queueClearToolTip = false;


#region Size and Position

		/// <value>
		/// The relative position of the lower left of the control.
		/// </value>
		/// <remarks>The absolute position if a control will be the combination 
		/// of all positions through the control hierarchy.</remarks>
		public Coord Position { get; set; }

		/// <summary>
		/// Push the position transformation onto the transformation stack.
		/// </summary>
		protected void PushPosition()
		{
			gl.glTranslated(Position.X, Position.Y, 0);
			lastPushed.X = Position.X;
			lastPushed.Y = Position.Y;
		}

		/// <summary>
		/// The last pushed position.
		/// </summary>
		private Coord lastPushed = new Coord();

		/// <summary>
		/// Pop the position transformation from the rendering stack.
		/// </summary>
		protected void PopPosition()
		{
			gl.glTranslated(-lastPushed.X, -lastPushed.Y, 0);
		}

		/// <summary>
		/// The absolute position that the control was last rendered at.
		/// </summary>
		/// <remarks>Use this for hit testing.</remarks>
		protected Coord LastPosition { get; private set; }

		/// <summary>
		/// This will be true if the control was made dirty after the last rendering cycle.
		/// </summary>
		private bool wasDirty = true;

		protected Coord size = new Coord();
		/// <value>
		/// The rendering size of the control.
		/// </value>
		public Coord Size
		{
			get {return size;}
			set {size = value;}
		}

		/// <summary>
		/// The width of the control.
		/// </summary>
		public double Width
		{
			get {return size.X;}
			set {size.X = value;}
		}

		/// <summary>
		/// The height of the control.
		/// </summary>
		public double Height
		{
			get {return size.Y;}
			set {size.Y = value;}
		}

		/// <value>
		/// The minimum size that the control needs to render correctly.
		/// </value>
		public virtual Coord MinSize
		{
			get {return new Coord();}
		}

		/// <summary>
		/// The minimum width of the control.
		/// </summary>
		public virtual double MinWidth
		{
			get {return MinSize.X;}
		}

		/// <summary>
		/// The minimum height of the control.
		/// </summary>
		public virtual double MinHeight
		{
			get {return MinSize.Y;}
		}
		
		/// <value>
		/// Whether to use the currently set size or overwrite it
		/// with MinSize when the geometry is computed.
		/// </value>
		public bool UserSize {get; set;}
		

		protected double padding = 3;
		/// <summary>
		/// The padding to use on the interior of controls.
		/// </summary>
		public double Padding
		{
			get { return padding; }
			set { padding = value; }
		}

#endregion


#region Rendering


		/// <summary>
		/// Sets the size to MinSize if UserSize is false.
		/// </summary>
		public override void ComputeGeometry()
		{
			base.ComputeGeometry();

			styleClass = styleGroup.GetClass(styleClassName);

			wasDirty = true;

			if (!UserSize)
				size = MinSize;
//			Console.WriteLine("computing geometry for {0}, size: {1}, user size? {2}", this.GetType(), size, UserSize);
		}

		public override sealed void RenderOverlay(Viewport viewport)
		{
			base.RenderOverlay(viewport);

			PushPosition();

			Render(viewport);

			if (wasDirty)
			{
				// compute the last position
				double[] modelView = new double[16];
				gl.glGetDoublev(gl.GL_MODELVIEW_MATRIX, modelView);
				double screenX, screenY, screenZ;
				glu.gluProject(0, 0, 0,
					modelView, viewport.Camera.ProjectionMatrix, viewport.Camera.ViewportSize,
					out screenX, out screenY, out screenZ);
				LastPosition = new Coord(screenX, screenY);
				wasDirty = false;
			}

			PopPosition();

			if (queueSetToolTip)
			{
				viewport.ToolTip = ToolTip;
				queueSetToolTip = false;
			}
			if (queueClearToolTip)
			{
				viewport.ClearToolTip();
				queueClearToolTip = false;
			}
		}


		/// <summary>
		/// Actually renders the control.
		/// </summary>
		/// <remarks>This method should be overriden by subclasses, 
		/// not RenderOverlay, which handles positioning.</remarks>
		protected virtual void Render(Viewport viewport)
		{

		}


#endregion


#region Hit Testing

		/// <summary>
		/// Performs the hit test on the rectangle defined by position and size.
		/// </summary>
		protected override bool HitTest(Coord pos)
		{
			return pos >= LastPosition && pos <= (LastPosition + size);
		}

#endregion

		
#region Mouse Handling
		
		private bool isHoverable = false;
		/// <value>
		/// Whether the control responds to mouse motion over it by going into the hovering state.
		/// </value>
		public bool IsHoverable
		{
			get {return isHoverable;}
			set {isHoverable = value;}
		}
		
		public override void OnMouseMotion(MouseEvent evt)
		{
			base.OnMouseMotion(evt);
			
			if (isHoverable && !evt.Handled)
			{
				if (HitTest(evt.Pos)) // it's hovering now
				{
					if (!IsHovering) // it wasn't hovering already
						OnEnter(evt);
					IsHovering = true;
					evt.Handle();
				}
				else // it's not hovering now
				{
					if (IsHovering) // it was hovering before
						OnLeave(evt);
					IsHovering = false;
				}
			}
			else
				IsHovering = false;
				
		}

		/// <summary>
		/// This will get called whenever the mouse enters the region of the control.
		/// </summary>
		protected virtual void OnEnter(MouseEvent evt)
		{
			if (ToolTip.Length > 0)
				queueSetToolTip = true;
		}

		/// <summary>
		/// This will get called whenever the mouse enters the region of the control.
		/// </summary>
		protected virtual void OnLeave(MouseEvent evt)
		{
			queueClearToolTip = true;
		}



#endregion
		

#region Style

		protected StyleGroup styleGroup;
		/// <summary>
		/// The style group this control will use to look up its style class.
		/// </summary>
		public StyleGroup StyleGroup
		{
			get { return styleGroup; }
			set
			{
				styleGroup = value;
				MakeDirty();
			}
		}

		private string styleClassName = "default";
		/// <summary>
		/// Name of the style class to use.
		/// </summary>
		public string StyleClassName
		{
			get {return styleClassName;}
			set
			{
				styleClassName = value;
				MakeDirty();
			}
		}

		/// <summary>
		/// The current style class to use to render the control.
		/// </summary>
		/// <remarks>This should be cached by ComputeGeometry() so it 
		/// doesn't need to be looked up every render cycle.</remarks>
		protected StyleClass styleClass;
		
		/// <summary>
		/// Renders the background with the current style.
		/// </summary>
		protected virtual void RenderBackground()
		{
			IFill bg = styleClass.GetBackground(hitState);
			if (bg != null)
				bg.DrawRectangle(new Coord(), size);
		}
		
		/// <summary>
		/// Renders the outline with the current style.
		/// </summary>
		protected virtual void RenderOutline()
		{
			Color fg = styleClass.GetForeground(hitState);
			if (fg != null)
			{
				fg.Setup();
				gl.glLineWidth(1f);
				gl.glBegin(gl.GL_LINE_LOOP);
				gl.glVertex2d(0, 0);
				gl.glVertex2d(Width, 0);
				gl.glVertex2d(Width, Height);
				gl.glVertex2d(0, Height);
				gl.glEnd();
			}
		}

#endregion

		
		
	}
}
