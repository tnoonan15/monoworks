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
using MonoWorks.Rendering.Events;

namespace MonoWorks.Rendering
{
	/// <summary>
	/// Base class for scenes that contain other scenes.
	/// </summary>
	public class SceneContainer : Scene
	{
		public SceneContainer(Viewport viewport) : base(viewport)
		{
		}
		
		
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
			scene.Parent = this;
		}

		/// <summary>
		/// Removes a scene to the collection.
		/// </summary>
		public virtual void Remove(Scene scene)
		{
			var index = _scenes.IndexOf(scene);
			_scenes.Remove(scene);
			scene.Parent = null;
			if (Current == scene)
			{
				if (NumScenes > 0)
				{
					if (index > 0)
						Current = _scenes[index - 1];
					else
						Current = _scenes[0];
				}
				else
					Current = null;
			}
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
		public virtual Scene Current
		{
			get { return _current; }
			set 
			{
				_current = value;
				
			}
		}
		
		public override Scene GetCurrent()
		{
			return _current.GetCurrent();
		}

		
		protected override void MakeChildCurrent(Scene child)
		{
			base.MakeCurrent();
			
			Current = child;
		}

		
	}
}

