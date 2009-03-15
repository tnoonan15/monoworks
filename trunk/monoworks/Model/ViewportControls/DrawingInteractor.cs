// DrawingInteractor.cs - MonoWorks Project
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
using MonoWorks.Rendering.Events;
using MonoWorks.Rendering.Interaction;
using MonoWorks.Framework;
using MonoWorks.Model.Sketching;

namespace MonoWorks.Model.ViewportControls
{
	
	/// <summary>
	/// Interactor for top level drawing entities (features, reference entities, sketches).
	/// </summary>
	public class DrawingInteractor : AbstractInteractor
	{
		
		public DrawingInteractor(Viewport viewport, Drawing drawing) : base(viewport)
		{
			this.drawing = drawing;
		}
		
		protected Drawing drawing;
		
		public override void OnButtonPress(MouseButtonEvent evt)
		{
			base.OnButtonPress(evt);

			if (sketcher != null)
				sketcher.OnButtonPress(evt);
		}

		
		public override void OnButtonRelease(MouseButtonEvent evt)
		{
			base.OnButtonRelease(evt);

			if (evt.Handled)
				return;

			if (sketcher != null)
				sketcher.OnButtonRelease(evt);
			else
			{
				if (evt.Button != 1)
					return;

				// deselect everything, if necessary
				if (evt.Modifier != InteractionModifier.Shift)
				{
					drawing.EntityManager.DeselectAll(null);
				}

				// find which entity was hit
				Entity hitEntity = null;
				if (IsSketching)
					hitEntity = HitEntity<Sketchable>(Sketch, evt);
				else
					hitEntity = HitEntity(evt);

				if (hitEntity != null)
				{
					drawing.EntityManager.Select(null, hitEntity);
					evt.Handle();

					if (hitEntity is Sketchable && IsSketching && Sketch.ContainsChild(hitEntity))
						SetSketachable(hitEntity as Sketchable);
				}
			}
		}
		
		
		public override void OnMouseMotion(MouseEvent evt)
		{
			base.OnMouseMotion(evt);

			foreach (Entity entity in drawing.Children)
				entity.IsHovering = false;
			if (IsSketching)
			{
				foreach (var entity in Sketch.Children)
					entity.IsHovering = false;
			}

			if (sketcher != null)
				sketcher.OnMouseMotion(evt);
			
			if (evt.Handled)
				return;

			Entity hitEntity = null;
			if (IsSketching)
				hitEntity = HitEntity<Sketchable>(Sketch, evt);
			else
				hitEntity = HitEntity(evt);
			if (hitEntity != null)
				hitEntity.IsHovering = true;
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
				ApplySketching();
			else if (sketcher != null)
				sketcher.OnKeyPress(evt);

		}

		
		/// <summary>
		/// Determines which, if any, entity was hit.
		/// </summary>
		/// <remarks>Entites are searched in this order: sketch, sketchable, features, references.</remarks>
		protected Entity HitEntity(MouseEvent evt)
		{
			Entity hit = HitEntity<Sketchable>(evt);
			if (hit != null)
				return hit;
			hit = HitEntity<Sketch>(evt);
			if (hit != null)
				return hit;
			hit = HitEntity<Feature>(evt);
			if (hit != null)
				return hit;
			hit = HitEntity<Reference>(evt);
			if (hit != null)
				return hit;
			return null;
		}

		/// <summary>
		/// Determines which, if any, entity was hit of the given type.
		/// </summary>
		protected T HitEntity<T>(MouseEvent evt) where T : Entity
		{
			return HitEntity<T>(drawing, evt);
		}

		/// <summary>
		/// Performs the hit test on the given parent.
		/// </summary>
		protected T HitEntity<T>(Entity parent, MouseEvent evt) where T : Entity
		{
			// gather a list of entities that were hit
			List<T> hits = new List<T>();
			foreach (T entity in parent.GetChildren<T>())
			{
				if (entity.HitTest(evt.HitLine))
					hits.Add(entity);
			}

			// perform depth test
			T front = null;
			double frontDist = 0;
			foreach (T entity in hits)
			{
				double dist_ = viewport.Camera.GetDistance(entity.LastHit);
				//double dist_ = viewport.Camera.GetDistance(entity.Bounds.Center);
				if (front == null || dist_ < frontDist)
				{
					front = entity;
					frontDist = dist_;
				}
			}
			return front;
		}


#region Sketching

		/// <summary>
		/// The current sketch being edited.
		/// </summary>
		public Sketch Sketch {get; private set;}

		/// <summary>
		/// Whether the interactor is sketching.
		/// </summary>
		public bool IsSketching
		{
			get { return Sketch != null; }
		}

		/// <summary>
		/// Begin sketching.
		/// </summary>
		public void BeginSketching(Sketch sketch)
		{
			Sketch = sketch;
		}

		/// <summary>
		/// Cancel the sketching.
		/// </summary>
		public void CancelSketching()
		{
			if (sketcher != null)
			{
				sketcher.Apply();
				sketcher = null;
			}
			Sketch = null;
		}

		/// <summary>
		/// Apply the sketching.
		/// </summary>
		public void ApplySketching()
		{
			if (sketcher != null)
			{
				sketcher.Apply();
				sketcher = null;
			}
			Sketch = null;
		}

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
			SetSketachable(sketchable);
		}

		/// <summary>
		/// Sets the sketchable that is currently being edited.
		/// </summary>
		public void SetSketachable(Sketchable sketchable)
		{
			if (sketchable is Line)
				sketcher = new LineSketcher(sketchable as Line);
			else if (sketchable is Rectangle)
				sketcher = new RectangleSketcher(sketchable as Rectangle);
			else
				throw new NotImplementedException();

			sketcher.SketchApplied += OnSketchApplied;
		}

		/// <summary>
		/// Handler for the sketcher's SketchApplied event.
		/// </summary>
		public void OnSketchApplied()
		{
			sketcher = null;
			drawing.EntityManager.DeselectAll(null);
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
				sketcher.RenderOpaque(viewport);
		}

		/// <summary>
		/// Pass the rendering to the sketcher, if there is one.
		/// </summary>
		public override void RenderTransparent(Viewport viewport)
		{
			base.RenderOpaque(viewport);
			if (IsSketching)
				Sketch.Plane.RenderGrid(viewport);
			if (sketcher != null)
				sketcher.RenderTransparent(viewport);
		}

		/// <summary>
		/// Pass the rendering to the sketcher, if there is one.
		/// </summary>
		public override void RenderOverlay(Viewport viewport)
		{
			base.RenderOpaque(viewport);
			if (sketcher != null)
				sketcher.RenderOverlay(viewport);
		}

#endregion



	}
}
