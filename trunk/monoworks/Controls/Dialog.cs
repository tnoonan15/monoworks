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
using System.Xml;

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
		
		
		public override void AddChild(Renderable child)
		{
			if (child is Control2D)
				Control = child as Control2D;
			else
				throw new Exception("Children of Dialog must be a Control2D.");
		}

		
		/// <summary>
		/// The title displayed in the title bar.
		/// </summary>
		[MwxProperty]
		public string Title
		{
			get {return _frame.Title;}
			set {_frame.Title = value;}
		}		
		
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

		
		public override void RenderOverlay(Scene scene)
		{
			base.RenderOverlay(scene);
			
			// shade out the background
			gl.glColor4d(0.85, 0.85, 0.85, 0.7);
			gl.glBegin(gl.GL_POLYGON);
			gl.glVertex2i(0, 0);
			gl.glVertex2d(scene.Width, 0);
			gl.glVertex2d(scene.Width, scene.Height);
			gl.glVertex2d(0, scene.Height);
			gl.glEnd();
			
			_overlayPane.Origin = new Coord((Math.Round(scene.Width - _frame.RenderWidth)/2), 
			                                Math.Round((scene.Height - _frame.RenderHeight)/2));
			
			
			_overlayPane.RenderOverlay(scene);
		}

		
		#region Interaction
				
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
		
		public override void OnKeyPress(KeyEvent evt)
		{
			base.OnKeyPress(evt);
			
			_overlayPane.OnKeyPress(evt);
		}
		
		#endregion

		
	}
}