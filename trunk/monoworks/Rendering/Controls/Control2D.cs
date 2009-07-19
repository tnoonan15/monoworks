// Control2D.cs - MonoWorks Project
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

using Cairo;

using MonoWorks.Base;
using MonoWorks.Rendering;
using MonoWorks.Rendering.Events;



namespace MonoWorks.Rendering.Controls
{
	
	/// <summary>
	/// Base class for all renderable controls.
	/// </summary>
	public abstract class Control2D : Renderable
	{
		
		public Control2D() : base()
		{
			styleGroup = StyleGroup.Default;

			UserSize = false;
			ToolTip = "";
		}
		
		private Control2D parent = null;
		/// <value>
		/// The control's parent.
		/// </value>
		public Control2D Parent
		{
			get {return parent;}
			set
			{
				parent = value;
			}
		}

		
		private IPane pane = null;
		/// <value>
		/// The pane this control belongs to.
		/// </value>
		public IPane Pane
		{
			get
			{
				if (pane != null)
					return pane;
				else if (Parent != null)
					return Parent.Pane;
				else
					return null;
			}
			set
			{
				pane = value;
			}
		}
		
//		/// <value>
//		/// The pane this control belongs to.
//		/// </value>
//		public IPane Pane {get; set;}
		
		
		/// <summary>
		/// The control tooltip.
		/// </summary>
		public string ToolTip { get; set; }

		
		public override void MakeDirty ()
		{
			base.MakeDirty();
			
			if (Parent != null)
				parent.MakeDirty();
		}

		

#region Size and Position

		/// <value>
		/// The relative position of the lower left of the control.
		/// </value>
		/// <remarks>The absolute position if a control will be the combination 
		/// of all positions through the control hierarchy.</remarks>
		public Coord Position { get; set; }


		/// <summary>
		/// The absolute position that the control was last rendered at.
		/// </summary>
		/// <remarks>Use this for hit testing.</remarks>
		protected Coord LastPosition { get; private set; }

		/// <summary>
		/// This will be true if the control was made dirty after the last rendering cycle.
		/// </summary>
//		private bool wasDirty = true;

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
		
		/// <value>
		/// The integer width of the control.
		/// </value>
		public int IntWidth
		{
			get {return (int)Math.Ceiling(Width);}
		}
		
		/// <value>
		/// The integer height of the control.
		/// </value>
		public int IntHeight
		{
			get {return (int)Math.Ceiling(Height);}
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

//			wasDirty = true;

			if (!UserSize)
				size = MinSize;
//			Console.WriteLine("computing geometry for {0}, size: {1}, user size? {2}", this.GetType(), size, UserSize);
		}

		
		/// <summary>
		/// Renders the control to a Cairo context.
		/// </summary>
		/// <remarks>Calls Render() internally.</remarks>
		public void RenderCairo(RenderContext context)
		{
			if (IsDirty)
				ComputeGeometry();
			
//			Console.WriteLine("moving context for {0}: {1}", this.GetType(), Position);
			context.Cairo.RelMoveTo(Position.X, Position.Y);
			
			var point = context.Cairo.CurrentPoint;
			LastPosition = new Coord(point.X, point.Y);
//			Console.WriteLine("last position of {0}: {1}", this.GetType(), LastPosition);
			
			context.Decorator.Decorate(this);
			
			Render(context);
			
			context.Cairo.RelMoveTo(-Position.X, -Position.Y);
						
		}

		/// <summary>
		/// Performs the 2D rendering of the control.
		/// </summary>
		protected virtual void Render(RenderContext context)
		{
		}


#endregion
		
		
#region Image Data
		
		/// <summary>
		/// Renders the control to an internal image surface.
		/// </summary>
		public void RenderImage(Viewport viewport)
		{
			if (IsDirty)
				ComputeGeometry();
			
			// remake the image surface, if needed
			if (surface == null || surface.Width != IntWidth || surface.Height != IntHeight)
			{
				imageData = new byte[IntWidth * IntHeight * 4];
				surface = new ImageSurface(ref imageData, Format.ARGB32, IntWidth, IntHeight, 4 * IntWidth);
			}
			
			// render the control to the surface
			using (Context cr = new Context(surface))
			{
				cr.Operator = Operator.Source;
				cr.Color = new Cairo.Color(1, 1, 1, 0);
				cr.Paint();
				
				cr.Operator = Operator.Over;
				cr.Color = new Cairo.Color(0, 0, 1, 1);
				cr.MoveTo(0,0);
				RenderCairo(new RenderContext(cr, viewport.Decorator));
				
				surface.Flush();
			};
			
		}
		
		private ImageSurface surface;
		
		private byte[] imageData;
		/// <value>
		/// The image data.
		/// </value>
		public byte[] ImageData
		{
			get {return imageData;}
		}
		
#endregion


#region Hit Testing

		/// <summary>
		/// Performs the hit test on the rectangle defined by position and size.
		/// </summary>
		protected virtual bool HitTest(Coord pos)
		{
			return pos >= LastPosition && pos <= (LastPosition + size);
		}

#endregion

		
#region Interaction
		
		private bool isHoverable = false;
		/// <value>
		/// Whether the control responds to mouse motion over it by going into the hovering state.
		/// </value>
		public bool IsHoverable
		{
			get {return isHoverable;}
			set {isHoverable = value;}
		}
		
		public override void OnButtonPress(MouseButtonEvent evt)
		{
			
		}
		
		public override void OnButtonRelease(MouseButtonEvent evt)
		{
			
		}
		
		public override void OnMouseMotion(MouseEvent evt)
		{			
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
		/// Handles a mouse wheel event.
		/// </summary>
		public override void OnMouseWheel(MouseWheelEvent evt)
		{
			
		}

		/// <summary>
		/// This will get called whenever the mouse enters the region of the control.
		/// </summary>
		protected virtual void OnEnter(MouseEvent evt)
		{
//			if (ToolTip.Length > 0)
//				queueSetToolTip = true;
			MakeDirty();
		}

		/// <summary>
		/// This will get called whenever the mouse leaves the region of the control.
		/// </summary>
		protected virtual void OnLeave(MouseEvent evt)
		{
//			queueClearToolTip = true;
			MakeDirty();
		}

		/// <summary>
		/// Handles a keyboard event.
		/// </summary>
		public override void OnKeyPress(KeyEvent evt)
		{
			
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
		protected virtual void RenderBackground(RenderContext context)
		{
//			IFill bg = styleClass.GetBackground(hitState);
//			if (bg != null)
//				bg.DrawRectangle(new Coord(), size);
		}
		
		/// <summary>
		/// Renders the outline with the current style.
		/// </summary>
		protected virtual void RenderOutline(RenderContext context)
		{
//			Color fg = styleClass.GetForeground(hitState);
//			if (fg != null)
//			{
//				fg.Setup();
//				gl.glLineWidth(1f);
//				gl.glBegin(gl.GL_LINE_LOOP);
//				gl.glVertex2d(0, 0);
//				gl.glVertex2d(Width, 0);
//				gl.glVertex2d(Width, Height);
//				gl.glVertex2d(0, Height);
//				gl.glEnd();
//			}
		}

#endregion

		
		
	}
}
