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
			
			var closeIcon = new Image(ResourceHelper.GetStream("close.png"));
			CloseButton = new Button(closeIcon) { 
				Padding = 3,
				IsHoverable = true
			};
			CloseButton.Clicked += Scene.AttemptClosing;
			AddChild(CloseButton);
		}

		/// <summary>
		/// The scene represented by this button.
		/// </summary>
		public Scene Scene
		{
			get;
			private set;
		}
		
		/// <summary>
		/// The container that the scene currently belongs to, or null if it's floating.
		/// </summary>
		public SceneContainer GetContainer()
		{
			return Scene.Parent as SceneContainer; 
		}
				
		#region Rendering
		
		private Label _label;
		
		/// <summary>
		/// The close button next to the label.
		/// </summary>
		public Button CloseButton {get; private set;}
		
		public override void ComputeGeometry()
		{
			base.ComputeGeometry();
			
			_label.Origin.X = Padding;
			_label.Origin.Y = Padding;
			
			CloseButton.Origin.X = _label.RenderWidth + 2 * Padding;
			CloseButton.Origin.Y = Padding;
			
			RenderWidth = _label.RenderWidth + 3 * Padding + CloseButton.RenderWidth;
			RenderHeight = Math.Max(_label.RenderHeight, CloseButton.RenderHeight) + 2 * Padding;
		}

		protected override void Render(RenderContext context)
		{
			base.Render(context);
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
				CloseButton.OnButtonPress(evt);
			}
			
		}
		
		public override void OnMouseMotion(MouseEvent evt)
		{
			CloseButton.OnMouseMotion(evt);
		}
		
		#endregion

		
		
	}
}

