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


namespace MonoWorks.Rendering.Interaction
{	
	/// <summary>
	/// Possible user interaction types.
	/// </summary>
	public enum InteractionType {None, Select, Rotate, Pan, Dolly, Zoom};

	/// <summary>
	/// Base class for classes that handle user interaction in a scene.
	/// </summary>
	public abstract class AbstractInteractor : Actor, IKeyHandler
	{
		public AbstractInteractor()
		{
			RubberBand = new RubberBand();
		}

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
		/// Every interactor gets a rubber band to use as it pleases.
		/// </summary>
		protected RubberBand RubberBand { get; private set; }

		/// <summary>
		/// Cancels the current interaction. 
		/// </summary>
		public virtual void Cancel()
		{
		}

		/// <summary>
		/// Registers a button press event.
		/// </summary>
		public override void OnButtonPress(MouseButtonEvent evt)
		{
			anchor = evt.Pos;
			lastPos = evt.Pos;
		}

		/// <summary>
		/// Registers a button release event.
		/// </summary>
		public override void OnButtonRelease(MouseButtonEvent evt)
		{
		}

		/// <summary>
		/// Registers the motion event without performing any interaction.
		/// </summary>
		public override void OnMouseMotion(MouseEvent evt)
		{
			lastPos = evt.Pos;
		}


		public override void OnMouseWheel(MouseWheelEvent evt)
		{

		}

		public override void OnKeyPress(KeyEvent evt)
		{

		}

		public override void OnKeyRelease(KeyEvent evt)
		{

		}


		public override void RenderOverlay(Scene scene)
		{
			base.RenderOverlay(scene);

			// render the rubberband
			RubberBand.Render(scene);
		}
	}

	/// <summary>
	/// Generic interactor parameterized by scene type.
	/// </summary>
	public abstract class GenericInteractor<SceneType> : AbstractInteractor where SceneType : Scene
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		/// <param name="scene">The scene to interact with.</param>
		public GenericInteractor(SceneType scene)
		{
			Scene = scene;

		}
		
		/// <summary>
		/// The scene this interactor acts on. 
		/// </summary>
		public SceneType Scene { get; private set; }

		/// <summary>
		/// The scene's camera.
		/// </summary>
		public Camera Camera
		{
			get { return Scene.Camera; }
		}

		/// <summary>
		/// The scene's render list.
		/// </summary>
		public RenderList RenderList
		{
			get { return Scene.RenderList; }
		}

	}
}
