////   Entity.cs - MonoWorks Project
////
////    Copyright Andy Selvig 2008
////
////    This program is free software: you can redistribute it and/or modify
////    it under the terms of the GNU Lesser General Public License as published 
////    by the Free Software Foundation, either version 3 of the License, or
////    (at your option) any later version.
////
////    This program is distributed in the hope that it will be useful,
////    but WITHOUT ANY WARRANTY; without even the implied warranty of
////    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
////    GNU Lesser General Public License for more details.
////
////    You should have received a copy of the GNU Lesser General Public 
////    License along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;

namespace MonoWorks.Model
{
		
	using EntityList = List<Entity>;
		

	/// <summary>
	/// The Entity class is the base class for all MonoWorks model objects.
	/// </summary>
	public class Entity
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		protected Entity()
		{
			dirty = false;
			children = new EntityList();
			dependencies = new EntityList();
			momentos = new List<Momento>();
			AddMomento();
			currentMomento = 0;
		}
		

#region Children
		
		protected EntityList children;			
	
		/// <summary>
		/// Add a child.
		/// </summary>
		/// <param name="child"> An <see cref="Entity"/> to add as a child. </param>
		protected virtual void AddChild(Entity child)
		{
			children.Add(child);
		}
		
		/// <summary>
		/// Removes the child from the entity's children list.
		/// </summary>
		/// <param name="child"> The <see cref="Entity"/> to remove. </param>
		protected virtual void RemoveChild(Entity child)
		{
			children.Remove(child);
		}
		
#endregion
		

#region Dependencies
		
		protected EntityList dependencies;			
	
		/// <summary>
		/// Add a dependency.
		/// </summary>
		/// <param name="child"> An <see cref="Entity"/> to add as a dependency. </param>
		protected virtual void AddDependency(Entity child)
		{
			dependencies.Add(child);
		}
		
		/// <summary>
		/// Removes the child from the entity's dependency list.
		/// </summary>
		/// <param name="child"> The <see cref="Entity"/> to remove. </param>
		protected virtual void RemoveDependency(Entity child)
		{
			dependencies.Remove(child);
		}
		
		
		/// <summary>
		/// Adds an entity as a child and dependencty.
		/// </summary>
		/// <param name="child"> A <see cref="Entity"/> that's a child and dependency. </param>
		protected virtual void AddDependantChild(Entity child)
		{
			children.Add(child);
			dependencies.Add(child);
		}
		
#endregion
		

#region Momentos
		
		/// <summary>
		/// List of momentos.
		/// </summary>
		protected List<Momento> momentos;
		
		/// <summary>
		/// Current momento number.
		/// </summary>
		protected int currentMomento;
		
		/// <summary>
		/// Appends a momento to the momento list.
		/// </summary>
		protected virtual void AddMomento()
		{
			Momento momento = new Momento();
			momentos.Add(momento);
		}
		
		/// <value>
		/// The current momento.
		/// </value>
		protected Momento CurrentMomento
		{
			get {return momentos[currentMomento];}
		}
		
		/// <summary>
		/// Returns the attribute of the given name.
		/// </summary>
		/// <param name="name"> Name of the attribute. </param>
		/// <returns> The attribute </returns>
		public object GetAttribute(string name)
		{
			if (CurrentMomento.ContainsKey(name))
				return CurrentMomento[name];
			else
				throw new Exception("This entity does not contain an attribute named " + name + ".");
		}
		
		
#endregion
		
		
#region Rendering
		
		/// <summary>
		/// True if the entity is dirty and needs its geometry recomputed.
		/// </summary>
		protected bool dirty;
		
		/// <summary>
		/// Makes the entity dirty.
		/// </summary>
		public void MakeDirty()
		{
			dirty = true;
		}
		
		/// <summary>
		/// Computes the geometry of entity and stores it for rendering.
		/// </summary>
		public virtual void ComputeGeometry()
		{
			dirty = false;
		}
		
		
		/// <summary>
		/// Renders the entity to the given viewport.
		/// This must be implemented by subclasses.
		/// </summary>
		/// <param name="viewport"> A <see cref="IViewport"/> to render to. </param>
		public virtual void Render(IViewport viewport)
		{
			if (dirty)
				ComputeGeometry();
		}
		
#endregion
		
	}
}
