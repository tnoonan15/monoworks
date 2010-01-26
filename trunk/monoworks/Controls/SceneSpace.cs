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
		
		public override void Resize()
		{
			base.Resize();
			if (Root != null)
				Root.Resize(Width, Height);
		}		
		
		public override void Render()
		{
			base.Render();
			
			if (Root != null)
				Root.Render();
		}
		
		
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

