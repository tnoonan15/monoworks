////   Document.cs - MonoWorks Project
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

using gl = Tao.OpenGl.Gl;

namespace MonoWorks.Model
{
	
	/// <summary>
	/// The Document entity represents the root entity for a document.
	/// It stores document metadata as well as the document's top level entities. 
	/// </summary>
	public class Document : Entity
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public Document() : base()
		{
		}

		
#region Children
	
		/// <summary>
		/// Adds a sketch as a top-level entity.
		/// </summary>
		/// <param name="sketch"> A <see cref="Sketch"/> to add to the document. </param>
		public void AddSketch(Sketch sketch)
		{
			children.Add(sketch);
		}
	
	
		/// <summary>
		/// Adds reference geometry as a top-level entity.
		/// </summary>
		/// <param name="reference"> A <see cref="Reference"/> to add to the document. </param>
		public void AddReference(Reference reference)
		{
			children.Add(reference);
		}
	
	
		/// <summary>
		/// Adds a feature as a top-level entity.
		/// </summary>
		/// <param name="feature"> A <see cref="Feature"/> to add to the document. </param>
		public void AddFeature(Feature feature)
		{
			children.Add(feature);
		}
	
#endregion
		
		
#region Rendering
		
		/// <summary>
		/// Renders the document to the given viewport.
		/// </summary>
		/// <param name="viewport"> A <see cref="Viewport"/> to render to. </param>
		public override void Render(IViewport viewport)
		{
//			gl.glShadeModel(gl.GL_SMOOTH);
				
			viewport.Camera.Place();
			foreach (Entity child in children)
			{
				child.Render(viewport);
			}
		}
		
#endregion
		
		
	}
	
}
