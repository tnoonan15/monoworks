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
		
	using EntityList = List<Entity>;
	

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
			ParentEntity = null;
			IdCounter++;
			id = IdCounter;
			IsDirty = false;
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

		/// <summary>
		/// This is a queue to the user interface to determine if the entity is editable.
		/// </summary>
		public bool IsLocked
		{
			get { return (bool)this["locked"]; }
			set { this["locked"] = value; } 
		}


		/// <summary>
		/// Names of the primary dimensions.
		/// </summary>
		public readonly string[] DimensionNames = { "X", "Y", "Z" };

		
#region The Drawing
		
		protected Drawing drawing;
		
		/// <value>
		/// Returns the drawing this entity belongs to.
		/// </value>
		public virtual Drawing TheDrawing
		{
			get {return drawing;}
			set { drawing = value; }
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
				//Console.WriteLine("creating default attribute {0} for entity {1}", attribute.Name, MetaData.Name);
				if (attribute.IsEntity)
					momento[attribute.Name] = null;
				else
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
				if (AttributeUpdated != null)
					AttributeUpdated(this, name);
			}
			else
				throw new Exception("This entity does not contain an attribute named " + name + ".");
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
				workingMomento["name"] = value;
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
		
		protected EntityList children;	
		
		/// <value>
		/// Read-only access to the children.
		/// </value>
		public IEnumerable<Entity> Children
		{
			get {return children;}
		}

		/// <summary>
		/// Gets all the children that inherit from T.
		/// </summary>
		/// <typeparam name="T">Entity or one of its subclasses.</typeparam>
		public IEnumerable<T> GetChildren<T>() where T : Entity
		{
			return from child in children
				   where child is T
				   select child as T;
//			var kids = new List<T>();
//			foreach (var child in children)
//			{
//				if (child is T)
//					kids.Add(child as T);
//			}
//			return kids;
		}

		/// <summary>
		/// Registers an entity with the entity manager.
		/// The entity manager keeps a flat list of entities it contains that can be looked up by id.
		/// </summary>
		/// <param name="entity"> A <see cref="Entity"/> to add to the drawing. </param>
		protected void RegisterEntity(Entity entity)
		{
			TheDrawing.EntityManager.RegisterEntity(entity);
		}	
		
		/// <value>
		/// The parent of this entity.
		/// </value>
		public Entity ParentEntity { get; private set; }
		
		public override Renderable Parent
		{
			get {
				return ParentEntity;
			}
			set {
				ParentEntity = value as Entity;
			}
		}
	
		/// <summary>
		/// AddChild a child.
		/// </summary>
		/// <param name="child"> An <see cref="Entity"/> to add as a child. </param>
		protected virtual void AddChild(Entity child)
		{
			children.Add(child);
			child.TheDrawing = TheDrawing;
			child.Parent = this;
			RegisterEntity(child);
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

		/// <summary>
		/// Returns true if the entity contains the given child.
		/// </summary>
		public bool ContainsChild(Entity child)
		{
			return children.Contains(child);
		}
		
#endregion
		

#region Dependencies
		
		protected EntityList dependencies;			
	
		/// <summary>
		/// AddChild a dependency.
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

			//if (parent != null)
			//    parent.MakeDirty();
			ParentDirty();

			if (TheDrawing != null)
				TheDrawing.ChildDirty(this);
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

			bounds.Reset();
			
			// update the children's geometry and resize the bounds
			foreach (Entity child in GetChildren<Feature>())
			{
				if (child != null)
				{
					if (child.IsDirty)
						child.ComputeGeometry();
					bounds.Resize(child.Bounds);
				}
			}
			foreach (Entity child in GetChildren<Sketch>())
			{
				if (child != null)
				{
					if (child.IsDirty)
						child.ComputeGeometry();
					bounds.Resize(child.Bounds);
				}
			}
			foreach (Entity child in GetChildren<Sketchable>())
			{
				if (child != null)
				{
					if (child.IsDirty)
						child.ComputeGeometry();
					bounds.Resize(child.Bounds);
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
			foreach (Entity child in children)
				child.RenderOpaque(scene);
		}
		
		/// <summary>
		/// Renders the transparent portion of the entity, 
		/// </summary>
		/// <param name="scene"> A <see cref="Scene"/> to render to. </param>
		public override void RenderTransparent(Scene scene)
		{
			base.RenderTransparent(scene);
			foreach (Entity child in children)
				child.RenderTransparent(scene);
		}
		
		/// <summary>
		/// Renders the overlay portion of the entity.
		/// </summary>
		/// <param name="scene"> A <see cref="Scene"/> to render to. </param>
		public override void RenderOverlay(Scene scene)
		{
			base.RenderOverlay(scene);
			foreach (Entity child in children)
				child.RenderOverlay(scene);
		}

		
#endregion


#region File I/O

		/// <summary>
		/// Recursively writes the entity and all of its children to an XML file.
		/// </summary>
		/// <param name="writer"></param>
		public virtual void ToXml(XmlWriter writer)
		{
			writer.WriteStartElement(ClassName);

			writer.WriteAttributeString("id", id.ToString());

			// write the attributes
			foreach (AttributeMetaData attribute in MetaData.AttributeList)
			{
				object attrObject = this[attribute.Name];
				if (attribute.IsEntity)
					writer.WriteAttributeString(attribute.Name, (attrObject as Entity).Id.ToString());
				else if (attrObject is IList)
					writer.WriteAttributeString(attribute.Name, (attrObject as IList).ListString());
				else
					writer.WriteAttributeString(attribute.Name, attrObject.ToString());
			}

			// write the children
			foreach (Entity child in children)
				child.ToXml(writer);

			writer.WriteEndElement();
		}



#endregion

	}
}
