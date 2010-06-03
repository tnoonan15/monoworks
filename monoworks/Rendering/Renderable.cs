// Renderable.cs - MonoWorks Project
//
//  Copyright (C) 2008 Andy Selvig
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
using System.Xml;

using MonoWorks.Base;

using MonoWorks.Rendering.Events;

namespace MonoWorks.Rendering
{

	/// <summary>
	/// Delegate for handling generic renderable events.
	/// </summary>
	public delegate void RenderableHandler(Renderable sender);
	
	
	/// <summary>
	/// Base class for renderable objects.
	/// </summary>
	public abstract class Renderable : IMwxObject, IMouseHandler, IKeyHandler
	{

		public Renderable()
		{
			IsVisible = true;
			IsDirty = true;

			var typeName = GetType().ToString().Split('.').Last();
			int count = 0;
			_nameCounts.TryGetValue(typeName, out count);
			Name = String.Format("{0}_{1}", typeName, count);
			_nameCounts[typeName] = count + 1;
		}

		/// <summary>
		/// Keeps track of how many off each type have been created.
		/// </summary>
		private static Dictionary<string, int> _nameCounts = new Dictionary<string, int>();

		/// <summary>
		/// True if the renderable is dirty and needs its geometry recomputed.
		/// </summary>
		public bool IsDirty { get; protected set; }

		/// <summary>
		/// Makes the renderable dirty.
		/// </summary>
		public virtual void MakeDirty()
		{
			IsDirty = true;
		}
		
		/// <summary>
		/// The name of the renderable.
		/// </summary>
		[MwxProperty]
		public virtual string Name {get;set;}
		
		/// <summary>
		/// The parent renderable to this one.
		/// </summary>
		public virtual IMwxObject Parent {get; set;}
		
		/// <summary>
		/// This must be implemented by subclasses if they plan on being able to support mwx children.
		/// </summary>
		/// <remarks>Subclasses should not call this method, as it throws a NotImplementedException.</remarks>
		public virtual void AddChild(IMwxObject child)
		{
			throw new NotImplementedException(String.Format("Type {0} does not support adding children.", GetType()));
		}

		/// <summary>
		/// Default implementation that returns an empty list.
		/// </summary>
		public virtual IEnumerable<IMwxObject> GetMwxChildren()
		{
			return new List<IMwxObject>();
		}

		/// <summary>
		/// Called when the scene changes size.
		/// </summary>
		/// <param name="scene"> A <see cref="Scene"/>. </param>
		public virtual void OnSceneResized(Scene scene)
		{
		}

		/// <summary>
		/// Called when the scene changes view direction.
		/// </summary>
		/// <param name="scene"> A <see cref="Scene"/>. </param>
		public virtual void OnViewDirectionChanged(Scene scene)
		{
		}
		
		
		#region Rendering

		/// <summary>
		/// Whether to render the renderable.
		/// </summary>
		public bool IsVisible { get; set; }


		/// <summary>
		/// Forces the renderable to compute its geometry.
		/// </summary>
		public virtual void ComputeGeometry()
		{
			IsDirty = false;
		}
		
		
		/// <summary>
		/// Renders the overlay portion of the renderable, 
		/// </summary>
		/// <param name="scene"> A <see cref="Scene"/> to render to. </param>
		public virtual void RenderOverlay(Scene scene)
		{			
			if (IsDirty)
				ComputeGeometry();
		}
		
		
		/// <summary>
		/// Allows the renderable to directly handle a pan event.
		/// </summary>
		/// <param name="scene"> The <see cref="Scene"/> on which the interaction was performed. </param>
		/// <param name="dx"> The travel in the x screen dimension.</param>
		/// <param name="dy"> The travel in the y screen dimension.</param>
		/// <returns> True to block the scene from dealing with the interaction itself. </returns>
		public virtual bool HandlePan(Scene scene, double dx, double dy)
		{
			return false;
		}		
		
		/// <summary>
		/// Allows the renderable to directly handle a dolly event.
		/// </summary>
		/// <param name="scene"> The <see cref="Scene"/> on which the interaction was performed. </param>
		/// <param name="factor"> The dolly factor.</param>
		/// <returns> True to block the scene from dealing with the interaction itself. </returns>
		public virtual bool HandleDolly(Scene scene, double factor)
		{
			return false;
		}

        /// <summary>
        /// Allows the renderable to directly handle a zoom event.
        /// </summary>
        /// <param name="scene"> The <see cref="Scene"/> on which the interaction was performed. </param>
        /// <param name="rubberBand"> The rubber band encompassing the zoom.</param>
        /// <returns> True to block the scene from dealing with the interaction itself. </returns>
        public virtual bool HandleZoom(Scene scene, RubberBand rubberBand)
        {
            return false;
		}

		#endregion
		
		
		#region Hit Test and selection

		private HitState _hitState = HitState.None;
		
		/// <summary>
		/// Gets raised when the hit state of teh renderable changes.
		/// </summary>
		public event HitStateChangedHandler HitStateChanged;
		
		/// <value>
		/// The current hit state of the control.
		/// </value>
		public HitState HitState 
		{
			get {return _hitState;}
		}		
		
		/// <summary>
		/// Forcably sets the hit state.
		/// </summary>
		/// <remarks>This should only be done if you know what you're doing.
		/// Otherwise, use IsHovering, IsSelected, etc.</remarks>
		public virtual void SetHitState(object sender, HitState newVal)
		{
			_hitState = newVal;
		}
		
		/// <summary>
		/// Gets called whenever the hit state of the renderable changes.
		/// </summary>
		private void OnHitStateChanged(object sender, HitState oldVal)
		{
			if (HitStateChanged != null && oldVal != HitState)
				HitStateChanged(sender, new HitStateChangedEvent(oldVal, HitState, this));
		}

		/// <value>
		/// Whether the cursor is hovering over the renderable.
		/// </value>
		public virtual bool IsHovering
		{
			get {return _hitState.IsHovering();}
			set
			{
				var oldVal = _hitState;
				if (value)
					_hitState |= HitState.Hovering;
				else
					_hitState &= ~HitState.Hovering;
				OnHitStateChanged(this, oldVal);
			}
		}
		
		/// <value>
		/// Whether the renderable is selected.
		/// </value>
		public bool IsSelected
		{
			get { return _hitState.IsSelected(); }
			set
			{
				var oldVal = _hitState;
				if (value)
					_hitState |= HitState.Selected;
				else
					_hitState &= ~HitState.Selected;
				OnHitStateChanged(this, oldVal);
			}
		}
		
		/// <summary>
		/// Sets IsSelected to true.
		/// </summary>
		public virtual void Select()
		{
			IsSelected = true;
		}
		
		/// <summary>
		/// Sets IsSelected to false.
		/// </summary>
		public virtual void Deselect()
		{
			IsSelected = false;
		}

		/// <summary>
		/// Makes the selected state opposite of what it was before.
		/// </summary>
		public void ToggleSelection()
		{
			IsSelected = !IsSelected;
		}
		
		/// <value>
		/// Whether the renderable is focused.
		/// </value>
		/// <remarks>Doesn't generally have much meaning, but it's used for specific things like controls.</remarks>
		public bool IsFocused
		{
			get { return _hitState.IsFocused(); }
			set
			{
				var oldVal = _hitState;
				if (value)
					_hitState |= HitState.Focused;
				else
					_hitState &= ~HitState.Focused;
				OnHitStateChanged(this, oldVal);
			}
		}
		
		/// <summary>
		/// Sets IsFocused to true.
		/// </summary>
		public virtual void GrabFocus()
		{
			IsFocused = true;
		}
		
		/// <summary>
		/// Sets IsFocused to false.
		/// </summary>
		public virtual void ReleaseFocus()
		{
			IsFocused = false;
		}

		/// <summary>
		/// The string used to describe the selection.
		/// </summary>
		public virtual string SelectionDescription
		{
			get
			{
				return "";
			}
		}

		#endregion
				
				
		#region Interaction
				
		public virtual void OnButtonPress(MouseButtonEvent evt) {}
		
		public virtual void OnButtonRelease(MouseButtonEvent evt) {}

		public virtual void OnMouseMotion(MouseEvent evt) { }

		public virtual void OnMouseWheel(MouseWheelEvent evt) { }
		
		public virtual void OnKeyPress(KeyEvent evt) {}
		
		public virtual void OnKeyRelease(KeyEvent evt) {}
						
		#endregion

	}
	
}
