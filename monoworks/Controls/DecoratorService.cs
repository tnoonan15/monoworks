// DecoratorService.cs - MonoWorks Project
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

namespace MonoWorks.Controls
{

	/// <summary>
	/// Provides decorators to the controls while they're rendering.
	/// </summary>
	public static class DecoratorService
	{

		static DecoratorService()
		{
			Default = new BasicDecorator();
		}
		
		public static DecoratorBase Default {get; set;}
		
		/// <summary>
		/// Associates a decorator with each scene.
		/// </summary>
		private static Dictionary<Scene,DecoratorBase> _decorators = 
			new Dictionary<Scene, DecoratorBase>();
		
		public static DecoratorBase Get(Scene scene)
		{
			DecoratorBase decorator;
			if (_decorators.TryGetValue(scene, out decorator))
				return decorator;
			return Default;
		}
		
	}
}
