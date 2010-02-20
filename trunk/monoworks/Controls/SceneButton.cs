// 
//  SceneButton.cs - MonoWorks Project
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
	/// A button representing a scene in a SceneContainer.
	/// </summary>
	public class SceneButton : Container
	{
		internal SceneButton(Scene scene)
		{
			Scene = scene;
			_label = new Label(scene.Name);
			AddChild(_label);
//			IsHoverable = true;
			
			var closeIcon = new Image(ResourceHelper.GetStream("close.png"));
			_closeButton = new Button(closeIcon) { 
				Padding = 3,
				IsHoverable = true
			};
			_closeButton.Clicked += delegate(object sender, EventArgs e) {
//				if (Closed != null)
//					Closed(this, e);
			};
			AddChild(_closeButton);
		}

		/// <summary>
		/// The scene represented by this button.
		/// </summary>
		public Scene Scene
		{
			get;
			private set;
		}
		
				
		#region Rendering
		
		private Label _label;
		
		private Button _closeButton;
		
		public override void ComputeGeometry()
		{
			base.ComputeGeometry();
			
//			if (_label.IsDirty)
//				_label.ComputeGeometry();
//			if (_closeButton.IsDirty)
//				_closeButton.ComputeGeometry();
			
			_label.Origin.X = Padding;
			_label.Origin.Y = Padding;
			
			_closeButton.Origin.X = _label.RenderWidth + 2 * Padding;
			_closeButton.Origin.Y = Padding;
			
			RenderWidth = _label.RenderWidth + 3 * Padding + _closeButton.RenderWidth;
			RenderHeight = Math.Max(_label.RenderHeight, _closeButton.RenderHeight) + 2 * Padding;
		}

		protected override void Render(RenderContext context)
		{
			base.Render(context);
			
//			_label.RenderCairo(context);
//			_closeButton.RenderCairo(context);
		}
				
		#endregion
		
		
		#region Interaction
		
		/// <summary>
		/// Gets raised when the user clicks the main portion of the button.
		/// </summary>
		public event EventHandler Clicked;
		
		
		
		/// <summary>
		/// The relative point of the last button press.
		/// </summary>
//		private Coord _anchor;

		
		public override void OnButtonPress(MouseButtonEvent evt)
		{
			//			_anchor = evt.Pos - Origin;
			if (_label.HitTest(evt.Pos))
			{
				if (Clicked != null)
					Clicked(this, new EventArgs());
			}
			else 
			{
				_closeButton.OnButtonPress(evt);
			}
			
		}
		
		public override void OnMouseMotion(MouseEvent evt)
		{
			//			base.OnMouseMotion(evt);
			
			_closeButton.OnMouseMotion(evt);
		}

		
		#endregion

		
		
	}
}

