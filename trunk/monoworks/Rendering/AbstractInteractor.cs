// AbstractInteractor.cs - MonoWorks Project
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

using MonoWorks.Base;
using MonoWorks.Rendering.Events;


namespace MonoWorks.Rendering
{

	/// <summary>
	/// Possible interactoin modifiers.
	/// </summary>
	public enum InteractionModifier { None = 3, Shift, Control, Alt };


	/// <summary>
	/// Base class for classes that handle user interaction from the viewport.
	/// </summary>
	public abstract class AbstractInteractor : IMouseHandler
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		/// <param name="viewport">The viewport to interact with.</param>
		public AbstractInteractor(IViewport viewport)
		{
			this.renderList = viewport.RenderList;
			this.viewport = viewport;
		}


		protected RenderList renderList;

		protected IViewport viewport;


		protected Coord anchor;
		/// <summary>
		/// The interaction anchor.
		/// </summary>
		public Coord Anchor
		{
			get { return anchor; }
			set { anchor = value; }
		}

		protected Coord lastPos;
		/// <summary>
		/// The last interaction point.
		/// </summary>
		public Coord LastPos
		{
			get { return lastPos; }
			set { lastPos = value; }
		}

		/// <summary>
		/// Registers a button press event.
		/// </summary>
		/// <param name="pos"></param>
		/// <param name="button"></param>
		/// <param name="modifier"></param>
		public virtual void OnButtonPress(MouseButtonEvent evt)
		{
			anchor = evt.Pos;
			lastPos = evt.Pos;
		}

		/// <summary>
		/// Registers a button release event.
		/// </summary>
		/// <param name="pos"></param>
		public virtual void OnButtonRelease(MouseEvent evt)
		{
		}

		/// <summary>
		/// Registers the motion event without performing any interaction.
		/// </summary>
		/// <param name="pos"></param>
		public virtual void OnMouseMotion(MouseEvent evt)
		{
			lastPos = evt.Pos;
		}

	}
}
