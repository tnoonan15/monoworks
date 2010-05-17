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
	public class SceneBook : SceneContainer
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
		
		public override Scene Current {
			get { return base.Current; }
			set {
				base.Current = value;
				_selector.RefreshButtons();
			}
		}

		
		public override void Resize()
		{
			base.Resize();
			
			_selector.UserSize.X = Width;
			_selector.MakeDirty();
			_pane.ComputeGeometry();
			_pane.Origin.X = 0;
			_pane.Origin.Y = Height - _pane.RenderHeight;
			
			foreach (var scene in Children)
			{
				scene.Resize(Width, Height - _pane.RenderHeight);
			}
		}

		public override void RenderOpaque()
		{
			base.RenderOpaque();
			
			if (Current != null)
				Current.RenderOpaque();
		}

		public override void RenderOverlay()
		{
			base.RenderOverlay();
			
			if (Current != null)
				Current.RenderOverlay();
		}

		public override void RenderTransparent()
		{
			base.RenderTransparent();
			
			if (Current != null)
				Current.RenderTransparent();
		}
		
		
		#region Interaction
		
		public override void OnButtonPress(MouseButtonEvent evt)
		{
			base.OnButtonPress(evt);
			
			if (Current != null)
				Current.OnButtonPress(evt);
		}

		public override void OnButtonRelease(MouseButtonEvent evt)
		{
			base.OnButtonRelease(evt);
			
			if (Current != null)
				Current.OnButtonRelease(evt);
		}

		public override void OnMouseMotion(MouseEvent evt)
		{
			base.OnMouseMotion(evt);
			
			if (Current != null)
				Current.OnMouseMotion(evt);
		}

		public override void OnMouseWheel(MouseWheelEvent evt)
		{
			base.OnMouseWheel(evt);
			
			if (Current != null)
				Current.OnMouseWheel(evt);
		}

		public override void OnKeyPress(KeyEvent evt)
		{
			base.OnKeyPress(evt);
			
			if (Current != null)
				Current.OnKeyPress(evt);
		}

		public override void OnKeyRelease(KeyEvent evt)
		{
			base.OnKeyRelease(evt);
			
			if (Current != null)
				Current.OnKeyRelease(evt);
		}

		
		#endregion


	}
	
	
	/// <summary>
	/// The buttons that allow the user to select scenes from a scene book.
	/// </summary>
	public class SceneBookSelector : GenericStack<SceneButton>
	{
		
		internal SceneBookSelector(SceneBook book)
		{
			RenderSize = new Coord();
			MinSize = new Coord(0, 20);
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
				var button = new SceneButton(scene);
				AddChild(button);
				var sceneRef = scene;
				button.Clicked += delegate(object sender, EventArgs e) {
					_book.Current = sceneRef;
					UpdateButtons();
				};
			}
			UpdateButtons();
		}
		
		/// <summary>
		/// Refreshes the state of the buttons.
		/// </summary>
		internal void RefreshButtons()
		{
			foreach (var button in Children)
			{
				if (button.Scene == _book.Current && !button.IsSelected)
					button.Select();
				else if (button.Scene != _book.Current && button.IsSelected)
					button.Deselect();
			}
		}
		
		/// <summary>
		/// Updates the selection state of the buttons.
		/// </summary>
		public void UpdateButtons()
		{
			foreach (var child in Children)
			{
				var button = child as SceneButton;
				if (_book.Current != null && button.Scene == _book.Current)
					button.IsSelected = true;
				else
					button.IsSelected = false;
			}
		}
		
		public override void ComputeGeometry()
		{
			base.ComputeGeometry();
			MinSize = RenderSize;
			ApplyUserSize();
		}
				
		protected override void Render(RenderContext context)
		{
			base.Render(context);

			context.Cairo.Color = context.Decorator.GetColor(ColorType.BackgroundStart, HitState.Selected).Cairo;
			context.Cairo.LineWidth = 2;
			context.Cairo.MoveTo(0, RenderHeight - 1);
			context.Cairo.RelLineTo(RenderWidth, 0);
			context.Cairo.Stroke();
		}
	}
	
	
}

