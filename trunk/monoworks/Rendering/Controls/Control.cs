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


using gl=Tao.OpenGl.Gl;

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
		}
		
		private Control parent = null;
		//// <value>
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

		public override void MakeDirty()
		{
			base.MakeDirty();
			
			//if (parent != null)
				//parent.MakeDirty();
		}


#region Size and Position

		protected Coord position = new Coord();
		//// <value>
		/// The position of the lower left of the control.
		/// </value>
		public virtual Coord Position
		{
			get {return position;}
			set {position = value;}
		}

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
		

		protected double padding = 4;
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
			
			if (!UserSize)
				size = MinSize;
//			Console.WriteLine("computing geometry for {0}, size: {1}, user size? {2}", this.GetType(), size, UserSize);
		}

		public override void RenderOverlay(Viewport viewport)
		{
			base.RenderOverlay(viewport);

			
		}



#endregion


#region Hit Testing

		/// <summary>
		/// Performs the hit test on the rectangle defined by position and size.
		/// </summary>
		protected override bool HitTest(Coord pos)
		{
			return pos >= position && pos <= (position + size);
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
			
			if (isHoverable && !evt.Handled && HitTest(evt.Pos) && !IsSelected)
			{
				IsHovering = true;
				evt.Handle();
			}
			else
				IsHovering = false;
				
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
				bg.DrawRectangle(position, size);
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
				position.glVertex();
				(position + new Coord(Width,0)).glVertex();
				(position + size).glVertex();
				(position + new Coord(0,Height)).glVertex();
				gl.glEnd();
			}
		}

#endregion

		
		
	}
}
