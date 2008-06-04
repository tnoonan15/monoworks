//   Document.cs - MonoWorks Project
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

using gl = Tao.OpenGl.Gl;

using MonoWorks.Base;

namespace MonoWorks.Model
{
	
	/// <summary>
	/// The Document entity represents the root entity for a document.
	/// It stores document metadata as well as the document's top level entities. 
	/// </summary>
	public class Document : Entity
	{
		
		static uint DocCounter = 0;
		
		/// <summary>
		/// Default constructor.
		/// </summary>
		public Document() : base()
		{
			entityRegistry = new Dictionary<long,Entity>();
			RegisterEntity(this);
			
			ColorManager = new ColorManager();
			
			DocCounter++;
			Name = String.Format("document{0}", DocCounter);
			
			selected = new List<Entity>();
			
			// initialize actions
			currentAction = -1;
			actionList = new List<Action>();
		}
		
		
		/// <value>
		/// Returns itself.
		/// </value>
		public override Document GetDocument()
		{
			return this;
		}
		

		protected ColorManager colorManager;
		/// <value>
		/// The color manager for this document.
		/// </value>
		public ColorManager ColorManager
		{
			get {return colorManager;}
			set {colorManager = value;}
		}
		
		
#region Entity Registry
		
		protected Dictionary<long, Entity> entityRegistry;
		
		/// <summary>
		/// Registers an entity with the document.
		/// The document keeps a flat list of entities it contains that can 
		/// be looked by id.
		/// </summary>
		/// <param name="entity"> A <see cref="Entity"/> to add to the document. </param>
		protected override void RegisterEntity(Entity entity)
		{
			if (!entityRegistry.ContainsKey(entity.Id))
				entityRegistry[entity.Id] = entity;
		}
		
		/// <summary>
		/// Returns the entity with the given id.
		/// </summary>
		/// <param name="id"> A valid entity id. </param>
		/// <returns> The <see cref="Entity"/> with the id. </returns>
		public Entity GetEntity(long id)
		{
			if (entityRegistry.ContainsKey(id))
				return entityRegistry[id];
			else
				throw new Exception(String.Format("Document does not contain an entity with id {0}", id));
		}
		
#endregion

		
#region Undo and Redo

		/// <summary>
		/// List of entity lists defining the entities that have had edit operations performed on them.
		/// </summary>
		protected List<Action> actionList;
		
		/// <summary>
		/// The current edit action index.
		/// </summary>
		protected int currentAction;
		
		/// <summary>
		/// Adds the given edit action to the action list.
		/// </summary>
		/// <param name="action"> A <see cref="Action"/>. </param>
		public void AddAction(Action action)
		{
			// remove all actions after the current one
			actionList.RemoveRange(currentAction+1, actionList.Count - currentAction - 1);
			
			actionList.Add(action);
			currentAction = actionList.Count - 1;
		}
		
		/// <summary>
		/// Undo the last action.
		/// </summary>
		public void Undo()
		{
			if (currentAction > -1)
			{
				actionList[currentAction].Undo();
				currentAction--;
			}
		}
		
		/// <summary>
		/// Undo the last undone action.
		/// </summary>
		public void Redo()
		{
			if (currentAction < actionList.Count-1)
			{
				currentAction++;
				actionList[currentAction].Redo();
			}
		}
		
#endregion
		
		
		
#region Children
	
		/// <summary>
		/// Adds a sketch as a top-level entity.
		/// </summary>
		/// <param name="sketch"> A <see cref="Sketch"/> to add to the document. </param>
		public void AddSketch(Sketch sketch)
		{
			AddChild(sketch);
		}
	
	
		/// <summary>
		/// Adds reference geometry as a top-level entity.
		/// </summary>
		/// <param name="reference"> A <see cref="Reference"/> to add to the document. </param>
		public void AddReference(Reference reference)
		{
			AddChild(reference);
		}
	
	
		/// <summary>
		/// Adds a feature as a top-level entity.
		/// </summary>
		/// <param name="feature"> A <see cref="Feature"/> to add to the document. </param>
		public void AddFeature(Feature feature)
		{
			AddChild(feature);
		}
		
		
		/// <summary>
		/// Makes the document dirty.
		/// </summary>
		/// <remarks>Makes all reference items dirty as well.</remarks>
		public override void MakeDirty()
		{
			base.MakeDirty();
		}

	
#endregion
		
		
#region Rendering
		
		/// <summary>
		/// Computes the geometry of the document.
		/// </summary>
		public override void ComputeGeometry()
		{
			base.ComputeGeometry();
		}
		
		/// <summary>
		/// Renders the document to the given viewport.
		/// </summary>
		/// <param name="viewport"> A <see cref="Viewport"/> to render to. </param>
		public new void Render(IViewport viewport)
		{
			viewport.Camera.Place();
			
			base.Render(viewport);
			
			// render the opaque geometry
			RenderOpaque(viewport);
			
			// render the transparent geometry
			RenderTransparent(viewport);
			
			// render the hit line
			if (hitLine != null)
			{
//				gl.glBegin(gl.GL_LINE);
//				gl.glVertex3d(hitLine[0][0], hitLine[0][1], hitLine[0][2]);
//				gl.glVertex3d(hitLine[1][0], hitLine[1][1], hitLine[1][2]);
//				gl.glEnd();
			}
		}
		
#endregion
		
		
#region Hit Test
				
		private Vector[] hitLine;
		
		/// <summary>
		/// Performs a hit test with two vectors lying on a 3D line.
		/// </summary>
		/// <param name="v1"> A <see cref="Vector"/> on the hit line. </param>
		/// <param name="v2"> A <see cref="Vector"/> on the hit line. </param>
		/// <returns> True if the entity was hit. </returns>
		public override bool HitTest(Vector v1, Vector v2)
		{
			lastSelected = null;
			selected.Clear();
			bool somethingChanged = false;
			if (base.HitTest(v1, v2)) // only perform hit test on children if it hits the document bounding box
			{
				foreach (Entity entity in entityRegistry.Values)
				{
					if (entity != this)
					{
						bool hit = entity.HitTest(v1, v2);
						if (hit != entity.IsSelected)
						{
							entity.IsSelected = hit;
							somethingChanged = true;
							if (entity.IsSelected)
								selected.Add(entity);
						}
						if (entity.IsSelected)
							lastSelected = entity;
					}
				}
			}
			
			hitLine = new Vector[]{v1, v2};
			
			return somethingChanged;
		}
		
#endregion
		
		
#region Selection

		protected List<Entity> selected;
		/// <value>
		/// The list of selected entities.
		/// </value>
		public List<Entity> Selected
		{
			get {return selected;}
		}
		
		/// <summary>
		/// Deselects all children.
		/// </summary>
		public void DeselectAll()
		{
			foreach (Entity child in selected)
				child.IsSelected = false;
		}
		
		protected Entity lastSelected;
		/// <value>
		/// The entity that was last selected.
		/// </value>
		public Entity LastSelected
		{
			get {return lastSelected;}
			set {lastSelected = value;}
		}
		
#endregion
		
	}
	
}
