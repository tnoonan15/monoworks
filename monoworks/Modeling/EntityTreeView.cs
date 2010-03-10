// 
//  EntityTree.cs - MonoWorks Project
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
using System.Reflection;

using MonoWorks.Base;
using MonoWorks.Controls;

namespace MonoWorks.Modeling
{
	/// <summary>
	/// Tree view that displays all entities in a drawing.
	/// </summary>
	public class EntityTreeView : GenericTreeView<EntityTreeItem>
	{
		public EntityTreeView()
		{
			// populate the icon list
			var asm = System.Reflection.Assembly.GetExecutingAssembly();
			var iconPrefix = "MonoWorks.Modeling.Icons.tree-";
			foreach (var name in asm.GetManifestResourceNames())
			{
				if (name.StartsWith(iconPrefix))
				{
					var iconName = name.Remove(0, iconPrefix.Length);
					iconName = iconName.Split('.')[0];
					IconList.Add(iconName, new Image(asm.GetManifestResourceStream(name)));
				}
			}
		}
		
		private Drawing _drawing;
		/// <summary>
		/// The drawing that acts as the root item.
		/// </summary>
		public Drawing Drawing {
			get { return _drawing; }
			set {
				_drawing = value;
				Reload();
			}
		}
		
		/// <summary>
		/// Reloads the tree based on the current drawing.
		/// </summary>
		public void Reload()
		{
			Clear();
			AddChild(new EntityTreeItem(Drawing));
		}
		
	}
}

