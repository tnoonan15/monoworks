// 
//  Dialog.cs
//  
//  Author:
//       Andy Selvig <ajselvig@gmail.com>
// 
//  Copyright (c) 2009 Andy Selvig
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

using gl=Tao.OpenGl.Gl;

using MonoWorks.Base;
using MonoWorks.Rendering;
using MonoWorks.Rendering.Events;

namespace MonoWorks.Controls
{


	public class Dialog : ModalOverlay
	{

		public Dialog () : base()
		{
			_overlayPane = new OverlayPane();
			_frame = new DialogFrame();
			_overlayPane.Control = _frame;
			_frame.Closed += delegate(object sender, EventArgs e) {
				Close();
			};
		}
		
		
		private OverlayPane _overlayPane;
		
		private DialogFrame _frame;
		
		/// <summary>
		/// The contents of the dialog.
		/// </summary>
		public Control2D Control
		{
			get {
				if (_frame.NumChildren > 0)
					return _frame[0];
				return null;
			}
			set {
				_frame.SetChild(0, value);
			}
		}
		
		public override void ComputeGeometry()
		{
			base.ComputeGeometry();
			
			_frame.ComputeGeometry();
		}

		
		public override void RenderOverlay(Viewport viewport)
		{
			base.RenderOverlay(viewport);
			
			// shade out the background
			gl.glColor4d(0.85, 0.85, 0.85, 0.7);
			gl.glBegin(gl.GL_POLYGON);
			gl.glVertex2i(0, 0);
			gl.glVertex2i(viewport.WidthGL, 0);
			gl.glVertex2i(viewport.WidthGL, viewport.HeightGL);
			gl.glVertex2i(0, viewport.HeightGL);
			gl.glEnd();
			
			_overlayPane.Origin = new Coord((viewport.WidthGL - _frame.RenderWidth)/2, 
			                                (viewport.HeightGL - _frame.RenderHeight)/2);
			
			
			_overlayPane.RenderOverlay(viewport);
		}

		
		#region Mouse Interaction
				
		public override void OnButtonPress(MouseButtonEvent evt)
		{
			base.OnButtonPress(evt);
			
			_overlayPane.OnButtonPress(evt);
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
		
		#endregion

		
	}
}
