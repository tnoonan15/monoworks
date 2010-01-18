// 
//  SceneContainer.cs - MonoWorks Project
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
	/// Base class for containers of scenes like SceneBook and SceneStack.
	/// </summary>
	public class SceneContainer : Scene
	{
		public SceneContainer(Viewport viewport) : base(viewport)
		{
		}
		
		
		
		/// <summary>
		/// The parent scene.
		/// </summary>
		public Scene Parent { get; set; }
		
		private readonly List<Scene> _scenes = new List<Scene>();
		/// <summary>
		/// All Scenes in the collection.
		/// </summary>
		public IEnumerable<Scene> Children
		{
			get { return _scenes; }
		}
		
		/// <summary>
		/// Adds a scene to the collection.
		/// </summary>
		public virtual void Add(Scene scene)
		{
			_scenes.Add(scene);
		}

		/// <summary>
		/// Removes a scene to the collection.
		/// </summary>
		public virtual void Remove(Scene scene)
		{
			_scenes.Remove(scene);
		}
		
		/// <summary>
		/// The number of scenes in the collection.
		/// </summary>
		public int NumScenes
		{
			get { return _scenes.Count;}
		}
		
		private Scene _current;
		/// <summary>
		/// The current scene in the collection.
		/// </summary>
		public Scene Current
		{
			get { return _current; }
			set 
			{
				_current = value;
				
			}
		}
		
	}


	/// <summary>
	/// A button representing a scene in a SceneContainer.
	/// </summary>
	public class SceneButton : Button
	{
		internal SceneButton(Scene scene)
		{
			Scene = scene;
			LabelString = scene.Name;
			ButtonStyle = ButtonStyle.Label;
			IsTogglable = true;
			IsHoverable = true;
		}

		/// <summary>
		/// The scene represented by this button.
		/// </summary>
		public Scene Scene
		{
			get;
			private set;
		}
		
		
		#region Interaction
		
		/// <summary>
		/// The relative point of the last button press.
		/// </summary>
		private Coord _anchor;
		
		public override void OnButtonPress(MouseButtonEvent evt)
		{
			base.OnButtonPress(evt);
			
			_anchor = evt.Pos - Origin;
		}
		
		#endregion

		
		
	}
}

