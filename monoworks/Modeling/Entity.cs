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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

using gl=Tao.OpenGl.Gl;

using MonoWorks.Base;
using MonoWorks.Rendering;
using MonoWorks.Modeling.Sketching;

namespace MonoWorks.Modeling
{	

	/// <summary>
	/// The Entity class is the base class for all MonoWorks model objects.
	/// </summary>
	public class Entity : Actor
	{
		
#region Counting

		/// <summary>
		/// Counter for the ID given to each new entity.
		/// </summary>
		protected static int IdCounter = 0;
		
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
			MetaData = GetMetadata(this);
			
			ParentEntity = null;
			IdCounter++;
			id = IdCounter;
			IsDirty = false;
			_bounds = new Bounds();
			_children = new List<Entity>();
			
			// initialize momentos
			momentos = new List<Momento>();
			workingMomento = DefaultMomento();
			currentMomentoIndex = 0;
			Snapshot();
			
			HitStateChanged += OnHitStateChanged;
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
		
		private static Dictionary<string, EntityMetaData> _metaData = new Dictionary<string, EntityMetaData>();
		
		
		protected EntityMetaData GetMetadata(Entity e)
		{
			var className = e.ClassName;
			EntityMetaData metaData = null;
			if (_metaData.TryGetValue(className, out metaData))
				return metaData;
			metaData = new EntityMetaData(e.GetType());
			_metaData[className] = metaData;
			return metaData;
		}
		
		/// <value>
		/// This entity's meta data.
		/// </value>
		public EntityMetaData MetaData { get; private set; }

		/// <summary>
		/// This is a queue to the user interface to determine if the entity is editable.
		/// </summary>
		[MwxProperty]
		public bool IsLocked
		{
			get { return (bool)this["IsLocked"]; }
			set { this["IsLocked"] = value; } 
		}


		/// <summary>
		/// Names of the primary dimensions.
		/// </summary>
		public readonly string[] DimensionNames = { "X", "Y", "Z" };

		private void OnHitStateChanged(object sender, HitStateChangedEvent evt)
		{
			var parent = ParentDrawing;
			if (parent != null && 
				((evt.OldValue.IsSelected() && !evt.NewValue.IsSelected()) || 
					(!evt.OldValue.IsSelected() && evt.NewValue.IsSelected())))
				parent.OnSelectionChanged();
		}

					
		#region The Drawing
		
		/// <value>
		/// Returns the drawing this entity belongs to.
		/// </value>
		public virtual Drawing ParentDrawing
		{
			get {
				return ParentEntity.ParentDrawing;
			}
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
			foreach (var attribute in MetaData.Attributes)
			{
				momento[attribute.Name] = attribute.Instantiate();
			}
			momento["name"] = ClassName + GetCount(ClassName).ToString();
			momento["locked"] = false;
			return momento;
		}
		
		/// <summary>
		/// Removes the momentos after the current one.
		/// </summary>
		public void RemoveLeadingMomentos()
		{
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
				throw new Exception(ClassName + " does not contain an attribute named " + name + ".");
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
				if (AttributeUpdated != null)
					AttributeUpdated(this, name);
			}
			else
				throw new Exception(ClassName + " does not contain an attribute named " + name + ".");
		}

		/// <summary>
		/// Handler for the AttributeUpdated event.
		/// </summary>
		public delegate void AttributeUpdatedHandler(Entity entity, string attrName);

		/// <summary>
		/// Raised whenever an attribute of an entity is changed.
		/// </summary>
		public event AttributeUpdatedHandler AttributeUpdated;

		/// <summary>
		/// Raise the AttributeUpdated event for the given attribute name.
		/// </summary>
		protected void RaiseAttributeUpdated(string attrName)
		{
			if (AttributeUpdated != null)
				AttributeUpdated(this, attrName);
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
		public override string Name
		{
			get {
				return (string)workingMomento["name"];
			}
			set {
				base.Name = value;
				//workingMomento["name"] = value;
			}
		}
		
		
		protected int id;
		/// <value>
		/// The entity id.
		/// </value>
		public int Id
		{
			get {return id;}
		}
				
		#endregion		
		
		
		#region Children
		
		private List<Entity> _children;	
		
		/// <value>
		/// Read-only access to the children.
		/// </value>
		public IEnumerable<Entity> Children
		{
			get {return _children;}
		}

		/// <summary>
		/// Gets all the children that inherit from T.
		/// </summary>
		/// <typeparam name="T">Entity or one of its subclasses.</typeparam>
		public IEnumerable<T> GetChildren<T>() where T : Entity
		{
			return from child in _children
				   where child is T
				   select child as T;
		}		
		
		public override void Deselect()
		{
			base.Deselect();
			
			foreach (var child in _children)
				child.Deselect();
		}

		
		/// <value>
		/// The parent of this entity.
		/// </value>
		public Entity ParentEntity { get; private set; }
		
		public override IMwxObject Parent
		{
			get {
				return ParentEntity;
			}
			set {
				ParentEntity = value as Entity;
			}
		}
		
		public override void AddChild(IMwxObject child)
		{
			if (child is Entity)
				AddChild(child as Entity);
			else
				throw new Exception("Entities can only have children that are other entities, not " + child.GetType());
		}

		public override IList<IMwxObject> GetMwxChildren()
		{
			var kids = new List<IMwxObject>();
			foreach (var icon in Children)
				kids.Add(icon);
			return kids;
		}

	
		/// <summary>
		/// AddChild a child.
		/// </summary>
		/// <param name="child"> An <see cref="Entity"/> to add as a child. </param>
		protected virtual void AddChild(Entity child)
		{
			var existing = GetChild(child.Name);
			if (existing != null)
				RemoveChild(existing);
			_children.Add(child);
			child.Parent = this;
			MakeDirty();
		}
		
		/// <summary>
		/// Removes the child from the entity's children list.
		/// </summary>
		/// <param name="child"> The <see cref="Entity"/> to remove. </param>
		public virtual void RemoveChild(Entity child)
		{
			_children.Remove(child);
		}
		
		/// <value>
		/// The number of children the entity has.
		/// </value>
		public virtual int NumChildren
		{
			get {return _children.Count;}
		}
		
		/// <summary>
		/// Gets the nth child of the entity.
		/// Throws an exception if n is out of range.
		/// </summary>
		/// <param name="n"> The child index. </param>
		/// <returns> The nth child of the entity. </returns>
		public virtual Entity GetNthChild(int n)
		{
			if (n>-1 && n<_children.Count)
				return _children[n];
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
			return _children.IndexOf(child);
		}

		/// <summary>
		/// Returns true if the entity contains the given child.
		/// </summary>
		public bool ContainsChild(Entity child)
		{
			return _children.Contains(child);
		}
		
		/// <summary>
		/// Gets a child entity by name.
		/// </summary>
		/// <remarks>This doesn't enforce type safety. Consider using GetChild<T> instead.</remarks>
		public Entity GetChild(string name)
		{
			var kids = from child in Children
				where child.Name == name
				select child;
			if (kids.Count() > 1)
				throw new Exception("Child name " + name + " didn't resolve to a unique child of " + Name + ". This should never happen!");
			if (kids.Count() == 1)
				return kids.First();
			return null;
		}
		
		
		public List<Entity> GetSelected()
		{
			var selected = new List<Entity>();
			var mine = from child in Children
				where child.IsSelected
				select child;
			selected.AddRange(mine);
			foreach (var child in Children)
			{
				selected.AddRange(child.GetSelected());
			}
			return selected;
		}
				
		#endregion
				
				
		#region Rendering			
		
		/// <summary>
		/// Makes the entity dirty.
		/// </summary>
		public override void MakeDirty()
		{
			base.MakeDirty();
			ParentDirty();
		}

		/// <summary>
		/// Makes the parent dirty.
		/// </summary>
		protected void ParentDirty()
		{
			if (ParentEntity != null)
			{
				ParentEntity.IsDirty = true;
				ParentEntity.ParentDirty();
			}
		}
		
		/// <summary>
		/// Computes the geometry of entity and stores it for rendering.
		/// </summary>
		public override void ComputeGeometry()
		{
			if (!IsDirty)
				return;

			base.ComputeGeometry();

			_bounds.Reset();
			
			// update the children's geometry and resize the bounds
			foreach (Entity child in GetChildren<Feature>())
			{
				if (child != null)
				{
					if (child.IsDirty)
						child.ComputeGeometry();
					_bounds.Resize(child.Bounds);
				}
			}
			foreach (Entity child in GetChildren<Sketch>())
			{
				if (child != null)
				{
					if (child.IsDirty)
						child.ComputeGeometry();
					_bounds.Resize(child.Bounds);
				}
			}
			foreach (Entity child in GetChildren<Sketchable>())
			{
				if (child != null)
				{
					if (child.IsDirty)
						child.ComputeGeometry();
					_bounds.Resize(child.Bounds);
				}
			}
			foreach (Entity child in GetChildren<Reference>())
			{
				if (child != null)
				{
					if (child.IsDirty)
						child.ComputeGeometry();
				}
			}


		}
		
				
		/// <summary>
		/// Renders the opaque portion of the entity.
		/// </summary>
		/// <param name="scene"> A <see cref="Scene"/> to render to. </param>
		public override void RenderOpaque(Scene scene)
		{
			base.RenderOpaque(scene);
			foreach (Entity child in _children)
				child.RenderOpaque(scene);
		}
		
		/// <summary>
		/// Renders the transparent portion of the entity, 
		/// </summary>
		/// <param name="scene"> A <see cref="Scene"/> to render to. </param>
		public override void RenderTransparent(Scene scene)
		{
			base.RenderTransparent(scene);
			foreach (Entity child in _children)
				child.RenderTransparent(scene);
		}
		
		/// <summary>
		/// Renders the overlay portion of the entity.
		/// </summary>
		/// <param name="scene"> A <see cref="Scene"/> to render to. </param>
		public override void RenderOverlay(Scene scene)
		{
			base.RenderOverlay(scene);
			foreach (Entity child in _children)
				child.RenderOverlay(scene);
		}
		
		#endregion
		

	}
}
