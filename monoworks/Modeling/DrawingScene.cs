// 
//  DrawingScene.cs - MonoWorks Project
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

using MonoWorks.Rendering;
using MonoWorks.Modeling.SceneControls;

namespace MonoWorks.Modeling
{
	/// <summary>
	/// A scene that contains a drawing.
	/// </summary>
	public class DrawingScene : Scene
	{
		public DrawingScene(Viewport viewport)
			: base(viewport)
		{
			Camera.SetViewDirection(ViewDirection.Standard);
			
			_controller = new DrawingController(this);
			
			EnableViewInteractor = true;
		}
				
		public DrawingScene(Viewport viewport, Drawing drawing) : this(viewport)
		{
			Drawing = drawing;
		}
		
		
		private DrawingController _controller;
		
		private Drawing _drawing;
		/// <summary>
		/// The drawing in this scene.
		/// </summary>
		public Drawing Drawing
		{
			get { return _drawing; }
			set {
				if (_drawing != null)
					RenderList.RemoveActor(_drawing);
				_drawing = value;
				RenderList.AddActor(_drawing);
				
				_drawing.AttributeUpdated += OnDrawingAttributeUpdated;
				PrimaryInteractor = new DrawingInteractor(this, Drawing);
				UpdateName();
				
				_controller.ReloadTree();
			}
		}

		/// <summary>
		/// Handles attribute updated events from the drawing.
		/// </summary>
		private void OnDrawingAttributeUpdated(Entity entity, string attrName)
		{
			UpdateName();
		}
		
		/// <summary>
		/// Updates the scene name based on the drawing's name and save state.
		/// </summary>
		private void UpdateName()
		{
			Name = _drawing.Name + (_drawing.IsModified ? "*" : "");
		}
		
		
		
	}
}

