//   Entity.cs - MonoWorks Project
//
//    Copyright Andy Selvig 2008
//
//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU Lesser General Public License as published 
//    by the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU Lesser General Public License for more details.
//
//    You should have received a copy of the GNU Lesser General Public 
//    License along with this program.  If not, see <http://www.gnu.org/licenses/>.

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
		
		protected static long IdCounter = 0; 
		
		/// <summary>
		/// Default constructor.
		/// </summary>
		protected Entity()
		{
			parent = null;
			IdCounter++;
			id = IdCounter;
			dirty = false;
			bounds = new Bounds();
			children = new EntityList();
			dependencies = new EntityList();
			momentos = new List<Momento>();
			AddMomento();
			currentMomento = 0;
		}


		
#region The Document
		
		protected Document document;
		
		/// <value>
		/// Returns the document this entity belongs to.
		/// </value>
		public virtual Document GetDocument()
		{
			return document;
		}
		
		protected void SetDocument(Document document)
		{
			this.document = document;
		}
		
#endregion
		
		
		

#region Children
		
		protected EntityList children;	
		
		/// <value>
		/// Read-only access to the children.
		/// </value>
		public EntityList Children
		{
			get {return children;}
		}
		
		/// <summary>
		/// Registers an entity with the document.
		/// The document keeps a flat list of entities it contains that can 
		/// be looked by id.
		/// </summary>
		/// <param name="entity"> A <see cref="Entity"/> to add to the document. </param>
		protected virtual void RegisterEntity(Entity entity)
		{
			document.RegisterEntity(entity);
		}	
		
		protected Entity parent;
		/// <value>
		/// The parent of this entity.
		/// </value>
		public Entity Parent
		{
			get {return parent;}
		}
	
		/// <summary>
		/// Add a child.
		/// </summary>
		/// <param name="child"> An <see cref="Entity"/> to add as a child. </param>
		protected virtual void AddChild(Entity child)
		{
			children.Add(child);
			child.SetDocument(GetDocument());
			RegisterEntity(child);
			child.parent = this;
		}
		
		/// <summary>
		/// Removes the child from the entity's children list.
		/// </summary>
		/// <param name="child"> The <see cref="Entity"/> to remove. </param>
		protected virtual void RemoveChild(Entity child)
		{
			children.Remove(child);
		}
		
		/// <value>
		/// The number of children the entity has.
		/// </value>
		public virtual int NumChildren
		{
			get {return children.Count;}
		}
		
		/// <summary>
		/// Gets the nth child of the entity.
		/// Throws an exception if n is out of range.
		/// </summary>
		/// <param name="n"> The child index. </param>
		/// <returns> The nth child of the entity. </returns>
		public virtual Entity GetNthChild(int n)
		{
			if (n>-1 && n<children.Count)
				return children[n];
			else
				throw new Exception("n is out of range.");
		}
		
		/// <summary>
		/// Gets the index of the given child.
		/// </summary>
		/// <param name="child"> A <see cref="Entity"/> that belongs to this one. </param>
		/// <returns> The child's index. </returns>
		public virtual int ChildIndex(Entity child)
		{
			return children.IndexOf(child);
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
			momento["name"] = "entity";
			momentos.Add(momento);
			MakeDirty();
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
		
		/// <value>
		/// Gets the item name.
		/// </value>
		public string Name
		{
			get {return (string)CurrentMomento["name"];}
			set {CurrentMomento["name"] = value;}
		}
		
		
		protected long id;
		/// <value>
		/// Gets the item id.
		/// </value>
		public long Id
		{
			get {return id;}
		}
		
#endregion
		
		
#region Rendering
		
		/// <summary>
		/// True if the entity is dirty and needs its geometry recomputed.
		/// </summary>
		protected bool dirty;
		

		protected Bounds bounds;
		/// <summary>
		/// The bounding box of the entity.
		/// Should be updated by ComputeGeometry().
		/// </summary>
		public Bounds Bounds
		{
			get {return bounds;}
			set {bounds = value;}
		}
		
		
		/// <summary>
		/// Makes the entity dirty.
		/// </summary>
		public void MakeDirty()
		{
			dirty = true;
			if (parent != null)
				parent.MakeDirty();
		}
		
		/// <summary>
		/// Computes the geometry of entity and stores it for rendering.
		/// </summary>
		public virtual void ComputeGeometry()
		{
			
			dirty = false;
			
			foreach (Entity child in children)
			{
				child.ComputeGeometry();
				bounds.Resize(child.Bounds);
			}
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
//			bounds.Render(viewport);
		}
		
#endregion
		
	}
}
