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
	
#endregion
		
		
#region Rendering
		
		/// <summary>
		/// Renders the document to the given viewport.
		/// </summary>
		/// <param name="viewport"> A <see cref="Viewport"/> to render to. </param>
		public override void Render(IViewport viewport)
		{
			viewport.Camera.Place();
			
			base.Render(viewport);
			
			// render the kids
			foreach (Entity child in children)
			{
				child.Render(viewport);
			}
			
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
			if (base.HitTest(v1, v2)) // only perform hit test on children if it hits the document bounding box
			{
				foreach (Entity entity in entityRegistry.Values)
				{
					if (entity != this)
					{
						entity.IsSelected = entity.HitTest(v1, v2);
					}
				}
			}
			
			hitLine = new Vector[]{v1, v2};
			
			return false;
		}

		
#endregion
		
	}
	
}