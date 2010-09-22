// 
//  SceneSpace.cs - MonoWorks Project
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
	/// A space that can contain multiple scenes and arrange them like a tab book or next to each other.
	/// </summary>
	public class SceneSpace : Scene
	{
		public SceneSpace(Viewport viewport) : base(viewport)
		{
			foreach (var side in Enum.GetValues(typeof(Side)))
			{
				var overlay = new OverlayPane();
				_gutterOverlays[(Side)side] = overlay;
				RenderList.AddOverlay(overlay);
			}
			
			// create the gutters
			_gutters[Side.E] = new Stack(Orientation.Vertical) { Padding = 0 };
			_gutters[Side.S] = new Stack(Orientation.Horizontal) { Padding = 0 };
			_gutters[Side.W] = new Stack(Orientation.Vertical) { Padding = 0 };
			_gutters[Side.N] = new Stack(Orientation.Horizontal) { Padding = 0 };
			
			foreach (var side in Enum.GetValues(typeof(Side)))
				_gutterOverlays[(Side)side].Control = _gutters[(Side)side];
			
			EnableViewInteractor = false;
		}
		
		private SceneContainer _root;
		/// <summary>
		/// The root of the scene tree.
		/// </summary>
		public SceneContainer Root {
			get { return _root; }
			set {
				_root = value;
				_root.Parent = this;
			}
		}
		
		
		public override Scene GetCurrent()
		{
			return _root.GetCurrent();
		}

		
		public override void Resize()
		{
			base.Resize();
			if (Root != null)
				Root.Resize(Width, Height);
			
			_relayout = true;
		}
		
		private bool _relayout;

		public override void Render()
		{
			// see if the gutters need their geometry computed
			foreach (var side in _gutters.Keys)
			{
				if (_gutters[side].IsDirty)
				{
					_relayout = true;
					break;
				}
			}

			base.Render();

			// relayout the gutters
			if (_relayout)
			{
				_gutterOverlays[Side.W].Origin.Y = Height - _gutterOverlays[Side.N].RenderHeight;
				_gutterOverlays[Side.N].Origin.Y = Height - _gutterOverlays[Side.N].RenderHeight;
				// TODO: Controls - implement SceneSpace layout for east and south gutters

				var rootOrigin = new Coord(_gutters[Side.W].RenderWidth, _gutters[Side.S].RenderHeight);
				Root.ViewportOffset = rootOrigin;
				Root.Resize(Width - rootOrigin.X - _gutters[Side.E].RenderWidth,
					Height - rootOrigin.Y - _gutters[Side.N].RenderHeight);
				_relayout = false;
			}

			if (Root != null)
			{
				Root.Render();
			}
		}
		
		
		
		#region Gutters

		private readonly Dictionary<Side,Stack> _gutters = new Dictionary<Side, Stack>();

		private readonly Dictionary<Side, OverlayPane> _gutterOverlays = new Dictionary<Side, OverlayPane>();
		
		/// <summary>
		/// Adds a control to one of the gutters.
		/// </summary>
		public void AddToGutter(Side side, Control2D control)
		{
			_gutters[side].AddChild(control);
		}

		/// <summary>
		/// Removes a control to one of the gutters.
		/// </summary>
		public void RemoveFromGutter(Control2D control)
		{
			foreach (var gutter in _gutters.Values)
			{
				if (gutter.ContainsChild(control))
					gutter.RemoveChild(control);
			}
		}
		
		#endregion
		
		
		#region Interaction

		public override void OnButtonPress(MouseButtonEvent evt)
		{
			base.OnButtonPress(evt);
			
			if (Root != null)
				Root.OnButtonPress(evt);
		}

		public override void OnButtonRelease(MouseButtonEvent evt)
		{
			base.OnButtonRelease(evt);
			
			if (Root != null)
				Root.OnButtonRelease(evt);
		}

		public override void OnMouseMotion(MouseEvent evt)
		{
			base.OnMouseMotion(evt);
			
			if (Root != null)
				Root.OnMouseMotion(evt);
		}

		public override void OnMouseWheel(MouseWheelEvent evt)
		{
			base.OnMouseWheel(evt);
			
			if (Root != null)
				Root.OnMouseWheel(evt);
		}
		
		public override void OnKeyPress(KeyEvent evt)
		{
			base.OnKeyPress(evt);
			
			if (Root != null)
				Root.OnKeyPress(evt);
		}

		
		#endregion
		
	}
}

