// 
//  SceneBook.cs - MonoWorks Project
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
using System.Collections.Generic;

using MonoWorks.Base;
using MonoWorks.Rendering;
using MonoWorks.Rendering.Events;

namespace MonoWorks.Controls
{
	/// <summary>
	/// A scene collection that arranges the scenes into a tab book.
	/// </summary>
	public class SceneBook : SceneCollection
	{
		public SceneBook(Viewport viewport) : base(viewport)
		{
			_selector = new SceneBookSelector(this);
			_selector.UserSize = new Coord();
			_pane = new OverlayPane(_selector);
			RenderList.AddOverlay(_pane);
		}
		
		private readonly SceneBookSelector _selector;
		
		private readonly OverlayPane _pane;
		
		public override void Add(Scene scene)
		{
			base.Add(scene);
			
			if (NumScenes == 1)
				Current = scene;
			
			_selector.RemakeButtons();
		}

		public override void Remove(Scene scene)
		{
			base.Remove(scene);
			
			_selector.RemakeButtons();
		}

		
		public override void Resize()
		{
			base.Resize();
			
			_selector.UserSize.X = Width;
			_pane.ComputeGeometry();
			_pane.Origin.X = 0;
			_pane.Origin.Y = Height - _pane.RenderHeight/2.0;
			
			foreach (var scene in Children)
			{
				scene.Resize(Width, Height - _pane.RenderHeight);
			}
		}
		
		public override void Render()
		{
			base.Render();
			if (Current != null)
				Current.Render();
		
		}
		
		
		#region Interaction
		
		public override void OnButtonPress(MouseButtonEvent evt)
		{
			base.OnButtonPress(evt);
			
			_selector.OnButtonPress(evt);
			
			if (!evt.Handled && Current != null)
				Current.OnButtonPress(evt);
		}

		public override void OnButtonRelease(MouseButtonEvent evt)
		{
			base.OnButtonRelease(evt);
			
			_selector.OnButtonRelease(evt);
			
			if (!evt.Handled && Current != null)
				Current.OnButtonRelease(evt);
		}

		public override void OnMouseMotion(MouseEvent evt)
		{
			base.OnMouseMotion(evt);
			
			_selector.OnMouseMotion(evt);
			
			if (!evt.Handled && Current != null)
				Current.OnMouseMotion(evt);
		}

		public override void OnMouseWheel(MouseWheelEvent evt)
		{
			base.OnMouseWheel(evt);
			
			_selector.OnMouseWheel(evt);
			
			if (!evt.Handled && Current != null)
				Current.OnMouseWheel(evt);
		}

		
		#endregion


	}
	
	
	/// <summary>
	/// A button in a SceneBookSelector.
	/// </summary>
	internal class SceneBookButton : Button
	{
		internal SceneBookButton(Scene scene)
		{
			Scene = scene;
			LabelString = scene.Name;
			ButtonStyle = ButtonStyle.Label;
		}
		
		/// <summary>
		/// The scene represented by this button.
		/// </summary>
		internal Scene Scene { get; private set; }
		
		
	}
	
	
	/// <summary>
	/// The buttons that allow the user to select scenes from a scene book.
	/// </summary>
	internal class SceneBookSelector : Stack
	{
		
		internal SceneBookSelector(SceneBook book)
		{
			RenderSize = new Coord();
			Padding = 0;
			_book = book;
			Orientation = Orientation.Horizontal;
		}
		
		private SceneBook _book;
		
		/// <summary>
		/// Forces the selector to remake its buttons based on the book's scenes.
		/// </summary>
		internal void RemakeButtons()
		{
			Clear();
			foreach (var scene in _book.Children)
			{
				var button = new SceneBookButton(scene);
				Add(button);
			}
		}
		
		public override void ComputeGeometry()
		{
			base.ComputeGeometry();			
			if (UserSize != null)
				RenderSize.X = UserSize.X;
		}
				
		protected override void Render(RenderContext context)
		{
			base.Render(context);
			
		}
	}
	
	
}

