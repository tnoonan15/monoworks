// Control.cs - MonoWorks Project
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

using MonoWorks.Base;
using MonoWorks.Rendering;
using MonoWorks.Rendering.Events;

namespace MonoWorks.Rendering.Controls
{
	
	/// <summary>
	/// Base class for all renderable controls.
	/// </summary>
	public abstract class Control : Overlay
	{
		
		public Control() : base()
		{
			styleGroup = DefaultStyleGroup;
		}


#region Size and Position

		protected Coord position = new Coord();
		//// <value>
		/// The position of the lower left of the control.
		/// </value>
		public virtual Coord Position
		{
			get {return position;}
			set {position = value;}
		}

		protected Coord size = new Coord();
		/// <value>
		/// The rendering size of the control.
		/// </value>
		public virtual Coord Size
		{
			get {return size;}
			set {size = value;}
		}


#endregion


#region Rendering


		public override void ComputeGeometry()
		{
			base.ComputeGeometry();

			styleClass = styleGroup.GetClass(this);
		}

		public override void RenderOverlay(IViewport viewport)
		{
			base.RenderOverlay(viewport);

			
		}



#endregion


#region Hit Testing

		/// <summary>
		/// Performs the hit test on the rectangle defined by position and size.
		/// </summary>
		protected override bool HitTest(Coord pos)
		{
			return pos >= position && pos <= (position + size);
		}

#endregion

		
#region Mouse Handling
		
		public override void OnMouseMotion(MouseEvent evt)
		{
			base.OnMouseMotion(evt);
			
			if (!evt.Handled && HitTest(evt.Pos) && !IsSelected)
			{
				IsHovering = true;
				evt.Handle();
			}
			else
				IsHovering = false;
				
		}



#endregion
		

#region Style

		private static StyleGroup defaultStyleGroup = new StyleGroup();

		/// <value>
		/// Style group that gets applied to all new controls.
		/// </value>
		public static StyleGroup DefaultStyleGroup
		{
			get { return defaultStyleGroup; }
		}

		protected StyleGroup styleGroup;
		/// <summary>
		/// The style group this control will use to look up its style class.
		/// </summary>
		public StyleGroup StyleGroup
		{
			get { return styleGroup; }
			set
			{
				styleGroup = value;
				MakeDirty();
			}
		}


		/// <summary>
		/// The current style class to use to render the control.
		/// </summary>
		/// <remarks>This should be cached by compute geometry so it 
		/// doesn't need to be looked up every render cycle.</remarks>
		protected StyleClass styleClass;

#endregion

		
		
	}
}
