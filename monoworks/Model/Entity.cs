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

using gl=Tao.OpenGl.Gl;

using MonoWorks.Base;

using MonoWorks.Rendering;

namespace MonoWorks.Model
{
		
	using EntityList = List<Entity>;
	

	/// <summary>
	/// The Entity class is the base class for all MonoWorks model objects.
	/// </summary>
	public class Entity : Renderable3D
	{
		
#region Counting

		/// <summary>
		/// Counter for the ID given to each new entity.
		/// </summary>
		protected static long IdCounter = 0;
		
		/// <summary>
		/// Dictionary of counters used to give unique names to each new entity.
		/// </summary>
		protected static Dictionary<string, int> entityCounters = new Dictionary<string,int>();
		
		/// <summary>
		/// Gets the counter for the given entity type.
		/// </summary>
		/// <param name="name"> An entity type name. </param>
		/// <returns> The current count for the given entity. </returns>
		protected static int GetCount(string name)
		{
			if (!entityCounters.ContainsKey(name)) // there isn't already an entry for this type
				entityCounters[name] = 0;
			
			entityCounters[name]++;
			return entityCounters[name];
		}
		
#endregion
		
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
			
			// initialize momentos
			momentos = new List<Momento>();
			workingMomento = DefaultMomento();
			currentMomentoIndex = 0;
			Snapshot();
		}
		
		/// <value>
		/// The unqualified name of the class.
		/// </value>
		public string ClassName
		{
			get
			{
				string[] nameComps = this.GetType().ToString().Split('.');
				return nameComps[nameComps.Length-1];
			}
		}
		
		/// <value>
		/// This entity's meta data.
		/// </value>
		public EntityMetaData MetaData
		{
			get {return EntityMetaData.TopLevel.GetEntity(ClassName);}
		}
		
#region The Drawing
		
		protected Drawing drawing;
		
		/// <value>
		/// Returns the drawing this entity belongs to.
		/// </value>
		public virtual Drawing GetDrawing()
		{
			return drawing;
		}
		
		protected void SetDrawing(Drawing drawing)
		{
			this.drawing = drawing;
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
		protected int currentMomentoIndex;
		
		/// <summary>
		/// The working momento.
		/// </summary>
		protected Momento workingMomento;		
		
		/// <summary>
		/// Generates the default momento.
		/// </summary>
		protected Momento DefaultMomento()
		{
			Momento momento = new Momento();
			foreach (AttributeMetaData attribute in MetaData.AttributeList)
			{
				if (!attribute.IsEntity)
					momento[attribute.Name] = attribute.Instantiate();
			}
			momento["name"] = ClassName + Entity.GetCount(ClassName).ToString();
			return momento;
		}
		
		/// <summary>
		/// Removes the momentos after the current one.
		/// </summary>
		public void RemoveLeadingMomentos()
		{
//			Console.WriteLine("current momento: {0}, num momentos: {1}", currentMomentoIndex, momentos.Count);
			if (currentMomentoIndex < momentos.Count-1)
				momentos.RemoveRange(currentMomentoIndex+1, momentos.Count-currentMomentoIndex);
		}
		
		/// <summary>
		/// Takes a snapshot of the current momento and makes a new one.
		/// </summary>
		public void Snapshot()
		{
			RemoveLeadingMomentos();
			momentos.Add(workingMomento.Duplicate());
			currentMomentoIndex = momentos.Count - 1;
		}
		
		/// <summary>
		/// Reverts the working momento to the current one.
		/// </summary>
		public void Revert()
		{
			workingMomento = momentos[currentMomentoIndex].Duplicate();
			MakeDirty();
		}
		
		/// <summary>
		/// Goes to the previous momento, if there is one.
		/// </summary>
		public virtual void Undo()
		{
			if (currentMomentoIndex >0)
			{
				currentMomentoIndex--;
				workingMomento = momentos[currentMomentoIndex];
				MakeDirty();
			}
		}
		
		/// <summary>
		/// Goes to the next momento, if there is one.
		/// </summary>
		public virtual void Redo()
		{
			if (currentMomentoIndex < momentos.Count-1)
			{
				currentMomentoIndex++;
				workingMomento = momentos[currentMomentoIndex];
				MakeDirty();
			}
		}
				
		/// <summary>
		/// Returns the attribute of the given name.
		/// </summary>
		/// <param name="name"> Name of the attribute. </param>
		/// <returns> The attribute </returns>
		public object GetAttribute(string name)
		{
			if (MetaData.ContainsAttribute(name))
				return workingMomento[name];
			else
				throw new Exception("This entity does not contain an attribute named " + name + ".");
		}
		
		/// <summary>
		/// Sets an attribute.
		/// </summary>
		/// <param name="name"> The attribute's name. </param>
		/// <param name="value"> The attribute's value. </param>
		public void SetAttribute(string name, object value)
		{
			if (MetaData.ContainsAttribute(name))
			{
				workingMomento[name] = value;
				MakeDirty();
			}
			else
				throw new Exception("This entity does not contain an attribute named " + name + ".");
		}
		
		/// <value>
		/// Attribute accessors.
		/// </value>
		public object this[string name]
		{
			get {return GetAttribute(name);}
			set {SetAttribute(name, value);}
		}
				
		/// <value>
		/// Gets the item name.
		/// </value>
		public string Name
		{
			get {return (string)workingMomento["name"];}
			set {workingMomento["name"] = value;}
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
		/// Registers an entity with the drawing.
		/// The drawing keeps a flat list of entities it contains that can be looked up by id.
		/// </summary>
		/// <param name="entity"> A <see cref="Entity"/> to add to the drawing. </param>
		protected void RegisterEntity(Entity entity)
		{
			GetDrawing().EntityManager.RegisterEntity(entity);
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
			child.SetDrawing(GetDrawing());
			RegisterEntity(child);
			child.parent = this;
			MakeDirty();
		}
		
		/// <summary>
		/// Removes the child from the entity's children list.
		/// </summary>
		/// <param name="child"> The <see cref="Entity"/> to remove. </param>
		public virtual void RemoveChild(Entity child)
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
			AddChild(child);
			AddDependency(child);
		}
		
#endregion

		
		
#region Rendering
			
		
		/// <summary>
		/// Makes the entity dirty.
		/// </summary>
		public override void MakeDirty()
		{
			base.MakeDirty();
			if (parent != null)
				parent.MakeDirty();
		}
		
		/// <summary>
		/// Makes only the entity dirty (not its parent).
		/// </summary>
		public virtual void ForceDirty()
		{
			dirty = true;
		}
		
		/// <summary>
		/// Computes the geometry of entity and stores it for rendering.
		/// </summary>
		public override void ComputeGeometry()
		{
			base.ComputeGeometry();
			
			// update the children's geometry and resize the bounds
			foreach (Entity child in children)
			{
				child.ComputeGeometry();
				if (child is Reference)
					child.ForceDirty();
				else
					bounds.Resize(child.Bounds);
			}
		}
		
				
		/// <summary>
		/// Renders the opaque portion of the entity.
		/// </summary>
		/// <param name="viewport"> A <see cref="IViewport"/> to render to. </param>
		public override void RenderOpaque(Viewport viewport)
		{
			base.RenderOpaque(viewport);
			foreach (Entity child in children)
				child.RenderOpaque(viewport);
			if (IsHovering)
			{
				gl.glLineWidth( 1.0f);
				ColorManager.Global["Blue"].Setup();
				bounds.Render(viewport);
			}
			else if (IsSelected)
			{
				gl.glLineWidth( 2.0f);
				ColorManager.Global["Red"].Setup();
				bounds.Render(viewport);
			}
		}
		
		/// <summary>
		/// Renders the transparent portion of the entity, 
		/// </summary>
		/// <param name="viewport"> A <see cref="IViewport"/> to render to. </param>
		public override void RenderTransparent(Viewport viewport)
		{
			base.RenderTransparent(viewport);
			foreach (Entity child in children)
				child.RenderTransparent(viewport);
		}
		
		/// <summary>
		/// Renders the overlay portion of the entity.
		/// </summary>
		/// <param name="viewport"> A <see cref="IViewport"/> to render to. </param>
		public override void RenderOverlay(Viewport viewport)
		{
			base.RenderOverlay(viewport);
			foreach (Entity child in children)
				child.RenderOverlay(viewport);
		}

		
#endregion
		
		
		
	}
}
