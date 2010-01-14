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
using System.Runtime.InteropServices;



namespace MonoWorks.Controls
{

	/// <summary>
	/// An event that's used for a value changing.
	/// </summary>
	public class ValueChangedEvent<T> : EventArgs
	{
		public ValueChangedEvent(T oldVal, T newVal)
		{
			OldValue = oldVal;
			NewValue = newVal;
		}
		
		/// <summary>
		/// The old value.
		/// </summary>
		public T OldValue { get; private set; }
		
		/// <summary>
		/// The new value.
		/// </summary>
		public T NewValue { get; private set; }
	}
	
	/// <summary>
	/// Base class for all renderable controls.
	/// </summary>
	public abstract class Control2D : Renderable
	{
		
		public Control2D() : base()
		{
			ToolTip = "";
			
			Origin = new Coord();
		}
		
		private Control2D _parent;
		/// <value>
		/// The control's parent.
		/// </value>
		public Control2D ParentControl
		{
			get {return _parent;}
			set {
				_parent = value;
			}
		}
		
		
		public override Renderable Parent
		{
			get {
				return ParentControl;
			}
			set {
				ParentControl = value as Control2D;
			}
		}


		
		private IPane _pane = null;
		/// <value>
		/// The pane this control belongs to.
		/// </value>
		public IPane Pane
		{
			get
			{
				if (_pane != null)
					return _pane;
				else if (ParentControl != null)
					return ParentControl.Pane;
				else
					return null;
			}
			set
			{
				_pane = value;
			}
		}
		
		
		/// <summary>
		/// The control tooltip.
		/// </summary>
		public string ToolTip { get; set; }

		
		public override void MakeDirty ()
		{
			base.MakeDirty();
			
			if (ParentControl != null)
				ParentControl.MakeDirty();
		}

		

		#region Size and Position

		/// <value>
		/// The relative position of the lower left of the control.
		/// </value>
		/// <remarks>The absolute position if a control will be the combination 
		/// of all positions through the control hierarchy.</remarks>
		public Coord Origin { get; set; }


		/// <summary>
		/// The absolute position that the control was last rendered at.
		/// </summary>
		/// <remarks>Use this for hit testing.</remarks>
		protected Coord LastPosition { get; private set; }

		/// <summary>
		/// This will be true if the control was made dirty after the last rendering cycle.
		/// </summary>
//		private bool wasDirty = true;

		/// <value>
		/// The last rendered size of the control.
		/// </value>
		public Coord RenderSize { get; protected set; }

		/// <summary>
		/// The width of the control as it was last rendered.
		/// </summary>
		public double RenderWidth
		{
			get {return RenderSize.X;}
			set {RenderSize.X = value;}
		}

		/// <summary>
		/// The height of the control as it was last rendered.
		/// </summary>
		public double RenderHeight
		{
			get {return RenderSize.Y;}
			set {RenderSize.Y = value;}
		}

		/// <value>
		/// The minimum size that the control needs to render correctly.
		/// </value>
		public Coord MinSize { get; protected set; }

		/// <summary>
		/// The minimum width of the control.
		/// </summary>
		public double MinWidth
		{
			get {return MinSize.X;}
		}

		/// <summary>
		/// The minimum height of the control.
		/// </summary>
		public double MinHeight
		{
			get {return MinSize.Y;}
		}
		
		/// <value>
		/// If not null, the layout system will attempt to use this size over the MinSize when computing RenderSize.
		/// </value>
		public Coord UserSize {get; set;}
		
		/// <summary>
		/// Tries to apply the user size to the render size, otherwise uses the min size.
		/// </summary>
		protected void ApplyUserSize()
		{
			if (UserSize != null)
				RenderSize = Coord.Max(MinSize, UserSize);
			else
				RenderSize = MinSize.Copy();
		}

		protected double padding = 4;
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
			get {return (int)Math.Ceiling(RenderWidth);}
		}
		
		/// <value>
		/// The integer height of the control.
		/// </value>
		public int IntHeight
		{
			get {return (int)Math.Ceiling(RenderHeight);}
		}

		#endregion

		
		#region Rendering

		/// <summary>
		/// Sets the size to MinSize if UserSize is false.
		/// </summary>
		public override void ComputeGeometry()
		{
			base.ComputeGeometry();
		}

		
		/// <summary>
		/// Renders the control to a Cairo context.
		/// </summary>
		/// <remarks>Calls Render() internally.</remarks>
		public void RenderCairo(RenderContext context)
		{
			if (IsDirty)
				ComputeGeometry();
			context.Cairo.RelMoveTo(Origin.X, Origin.Y);
			
			var point = context.Cairo.CurrentPoint;
			LastPosition = new Coord(point.X, point.Y);
			
			context.Decorator.Decorate(this);
			
			Render(context);
			
			context.Cairo.RelMoveTo(-Origin.X, -Origin.Y);
						
		}

		/// <summary>
		/// Performs the 2D rendering of the control.
		/// </summary>
		protected virtual void Render(RenderContext context)
		{
		}

		#endregion
				
				
		#region Image Data

		private GCHandle _gch;
		
		/// <summary>
		/// Renders the control to an internal image surface.
		/// </summary>
		public void RenderImage(Viewport viewport)
		{
			if (IsDirty)
				ComputeGeometry();

			// nothing to see here, folks. move along
			if (IntWidth == 0 || IntHeight == 0)
				return;
			
			// remake the image surface, if needed
			if (surface == null || surface.Width != IntWidth || surface.Height != IntHeight)
			{
				if (surface != null)
				{
					surface.Destroy();
					_gch.Free();
				}
				imageData = new byte[IntWidth * IntHeight * 4];
				_gch = GCHandle.Alloc(imageData, GCHandleType.Pinned);
				surface = new ImageSurface(imageData, Format.ARGB32, IntWidth, IntHeight, 4 * IntWidth);
			}
			
			// render the control to the surface
			using (Context cr = new Context(surface))
			{
				cr.Operator = Operator.Source;
				cr.Color = new Cairo.Color(1, 1, 1, 0);
				cr.Paint();
				
				cr.Operator = Operator.Over;
				cr.MoveTo(0,0);
				RenderCairo(new RenderContext(cr, DecoratorService.Get(viewport)));
				
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
			if (RenderSize == null || LastPosition == null)
				return false;
			return pos >= LastPosition && pos <= (LastPosition + RenderSize);
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
			MakeDirty();
		}

		/// <summary>
		/// This will get called whenever the mouse leaves the region of the control.
		/// </summary>
		protected virtual void OnLeave(MouseEvent evt)
		{
			MakeDirty();
		}

		/// <summary>
		/// Handles a keyboard event.
		/// </summary>
		public override void OnKeyPress(KeyEvent evt)
		{
			
		}
		
		
		protected override void OnHitStateChanged(HitState oldVal)
		{
			base.OnHitStateChanged(oldVal);
			
			var pane = Pane;
			if (!oldVal.IsFocused() && IsFocused) // became focused
			{
				if (pane != null)
					pane.InFocus = this;
			}
			else if (oldVal.IsFocused() && !IsFocused) // lost focus
			{
				if (pane != null)
					pane.InFocus = null;
			}
		}


		#endregion
		
	}
}
