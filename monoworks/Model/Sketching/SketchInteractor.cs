// SketchManager.cs - MonoWorks Project
//
//  Copyright (C) 2009 Andy Selvig
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
//
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA 

using System;
using System.Collections.Generic;

using MonoWorks.Rendering;
using MonoWorks.Rendering.Interaction;
using MonoWorks.Rendering.Events;

namespace MonoWorks.Model
{
	/// <summary>
	/// Interactor for sketching.
	/// </summary>
	public class SketchInteractor : AbstractInteractor
	{
		public SketchInteractor(Viewport viewport, Sketch sketch) : base(viewport)
		{
			Sketch = sketch;
		}

		/// <summary>
		/// The current sketch.
		/// </summary>
		public Sketch Sketch { get; private set; }


#region The Sketchable

		/// <summary>
		/// The current sketchable being edited.
		/// </summary>
		private Sketchable sketchable = null;

		/// <summary>
		/// The current sketcher.
		/// </summary>
		private AbstractSketcher sketcher = null;

		/// <summary>
		/// Adds a new sketchable to the sketch.
		/// </summary>
		/// <param name="sketchable"></param>
		public void AddSketchable(Sketchable sketchable)
		{
			this.sketchable = sketchable;
			Sketch.AddChild(sketchable);

			if (sketchable is Line)
				sketcher = new LineSketcher(Sketch, sketchable as Line);
		}

#endregion


#region Ending The Sketching

		/// <summary>
		/// Cancel the sketching.
		/// </summary>
		public void Cancel()
		{
			if (sketcher != null)
			{
				sketcher.Apply();
				sketcher = null;
			}
		}

		/// <summary>
		/// Apply the sketching.
		/// </summary>
		public void Apply()
		{
			if (sketcher != null)
			{
				sketcher.Apply();
				sketcher = null;
			}
		}

#endregion

#region Mouse and Keyboard Handling

		public override void OnButtonPress(MouseButtonEvent evt)
		{
			if (sketcher != null)
				sketcher.OnButtonPress(evt);
		}

		public override void OnButtonRelease(MouseButtonEvent evt)
		{
			if (sketcher != null)
				sketcher.OnButtonRelease(evt);
		}

		public override void OnMouseMotion(MouseEvent evt)
		{
			if (sketcher != null)
				sketcher.OnMouseMotion(evt);
		}

		public override void OnMouseWheel(MouseWheelEvent evt)
		{
			if (sketcher != null)
				sketcher.OnMouseWheel(evt);
		}

		public override void OnKeyPress(KeyEvent evt)
		{
			if (evt.Handled)
				return;

			if (evt.SpecialKey == SpecialKey.Enter || evt.SpecialKey == SpecialKey.Escape)
				Apply();
			else if (sketcher != null)
				sketcher.OnKeyPress(evt);

		}

#endregion


#region Rendering

		/// <summary>
		/// Pass the rendering to the sketcher, if there is one.
		/// </summary>
		public override void RenderOpaque(Viewport viewport)
		{
			base.RenderOpaque(viewport);
			if (sketcher != null)
				(sketcher as Actor).RenderOpaque(viewport);
		}

		/// <summary>
		/// Pass the rendering to the sketcher, if there is one.
		/// </summary>
		public override void RenderTransparent(Viewport viewport)
		{
			base.RenderOpaque(viewport);
			if (sketcher != null)
				(sketcher as Actor).RenderTransparent(viewport);
		}

		/// <summary>
		/// Pass the rendering to the sketcher, if there is one.
		/// </summary>
		public override void RenderOverlay(Viewport viewport)
		{
			base.RenderOpaque(viewport);
			if (sketcher != null)
				(sketcher as Actor).RenderOverlay(viewport);
		}

#endregion


	}
}
