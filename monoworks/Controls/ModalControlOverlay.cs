// 
//  ModalControlOverlay.cs - MonoWorks Project
//  
//  Author:
//       Andy Selvig <ajselvig@gmail.com>
// 
//  Copyright (c) 2010 Andy Selvig
// 
//  This library is free software; you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as
//  published by the Free Software Foundation; either version 2.1 of the
//  License, or (at your option) any later version.
// 
//  This library is distributed in the hope that it will be useful, but
//  WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
//  Lesser General Public License for more details.
// 
//  You should have received a copy of the GNU Lesser General Public
//  License along with this library; if not, write to the Free Software
//  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA

using System;
using MonoWorks.Base;
using MonoWorks.Rendering;
using MonoWorks.Rendering.Events;

namespace MonoWorks.Controls
{
	/// <summary>
	/// A general modal overlay that hosts a single control.
	/// </summary>
	public class GenericModalControlOverlay<ControlType> : ModalOverlay where ControlType : Control2D
	{
		public GenericModalControlOverlay()
		{
			_overlayPane = new OverlayPane();
			CloseOnOutsideClick = true;
		}
		
		public GenericModalControlOverlay(ControlType control) : this()
		{
			Control = control;
		}
		
		private readonly OverlayPane _overlayPane;


		public override void AddChild(IMwxObject child)
		{
			if (child is ControlType)
				Control = child as ControlType;
			else
				throw new Exception("Children of ModalControlOverlay must be a Control2D.");
		}

		/// <summary>
		/// The contents of the dialog.
		/// </summary>
		public virtual ControlType Control
		{
			get { return _overlayPane.Control as ControlType; }
			set {
				_overlayPane.Control = value;
				MakeDirty();
			}
		}

		public override void ComputeGeometry()
		{
			base.ComputeGeometry();

			_overlayPane.ComputeGeometry();
		}


		public override void RenderOverlay(Scene scene)
		{
			base.RenderOverlay(scene);

			_overlayPane.RenderOverlay(scene);
		}
		
		/// <summary>
		/// The origin of the control being rendered. 
		/// </summary>
		public Coord Origin
		{
			get {return _overlayPane.Origin;}
			set {_overlayPane.Origin = value;}
		}
		
		/// <summary>
		/// Centers the control in the scene. 
		/// </summary>
		public void Center(Scene scene)
		{
			if (_overlayPane.IsDirty)
				_overlayPane.ComputeGeometry();
			Origin.X = Math.Round(scene.Width - _overlayPane.RenderWidth) / 2;
			Origin.Y = Math.Round(scene.Height - _overlayPane.RenderHeight) / 2;
		}
		
		public override void OnSceneResized(Scene scene)
		{
			base.OnSceneResized(scene);
		}


		#region Interaction
		
		/// <summary>
		/// If true, the modal overlay will be closed when the user clicks outside of the control. 
		/// </summary>
		[MwxProperty]
		public bool CloseOnOutsideClick { get;	set; }

		public override void OnButtonPress(MouseButtonEvent evt)
		{
			base.OnButtonPress(evt);

			_overlayPane.OnButtonPress(evt);
			
			if (CloseOnOutsideClick && !evt.IsHandled && !_overlayPane.Control.HitTest(evt.Pos))
				Close();
		}

		public override void OnButtonRelease(MouseButtonEvent evt)
		{
			base.OnButtonRelease(evt);

			_overlayPane.OnButtonRelease(evt);
		}

		public override void OnMouseMotion(MouseEvent evt)
		{
			base.OnMouseMotion(evt);

			_overlayPane.OnMouseMotion(evt);
		}

		public override void OnMouseWheel(MouseWheelEvent evt)
		{
			base.OnMouseWheel(evt);

			_overlayPane.OnMouseWheel(evt);
			if (!evt.IsHandled) // don't let anyone else handle the event
				evt.Handle(this);
		}

		public override void OnKeyPress(KeyEvent evt)
		{
			base.OnKeyPress(evt);

			_overlayPane.OnKeyPress(evt);
		}

		#endregion

	}
	
	
	public class ModalControlOverlay : GenericModalControlOverlay<Control2D>
	{
		
	}
	
}
