//   Sketch.cs - MonoWorks Project
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

using MonoWorks.Base;
using MonoWorks.Rendering;

namespace MonoWorks.Modeling.Sketching
{
	
	/// <summary>
	/// The Sketch class is a container and canvas for 2D entities. 
	/// </summary>
	public class Sketch : Entity
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		/// <param name="plane">The reference plane that the sketch lies on.</param>
		public Sketch() : base()
		{
		}
		
		/// <summary>
		/// Constructor with a plane.
		/// </summary>
		/// <param name="plane">The reference plane that the sketch lies on.</param>
		public Sketch(RefPlane plane) : this()
		{
			Plane = plane;
		}
				
				
		#region Attributes
						
		/// <summary>
		/// The plane that the sketch lies on.
		/// </summary>
		[MwxProperty]
		public RefPlane Plane
		{
			get {return (RefPlane)this["Plane"];}
			set {this["Plane"]= value;}
		}
				
		#endregion
		
		
		/// <summary>
		/// Adds a child sketchable.
		/// </summary>
		/// <param name="sketchable"> A <see cref="Sketchable"/> to add as a chid. </param>
		public virtual void AddChild(Sketchable sketchable)
		{
			base.AddChild(sketchable);
		}
		
		/// <value>
		/// Returns a list of sketchables.
		/// </value>
		public IEnumerable<Sketchable> Sketchables
		{
			get
			{
				List<Sketchable> kids = new List<Sketchable>();
				foreach (Entity child in Children)
				{
					kids.Add((Sketchable)child);
				}
				return kids;
			}
		}

		
#region Rendering
		
		/// <summary>
		/// Computes the raw points needed to draw the sketch.
		/// </summary>
		public override void ComputeGeometry()
		{
			base.ComputeGeometry();
		}
		

		public override void RenderOpaque(Scene scene)
		{
			scene.RenderManager.EnableAntialiasing();
			base.RenderOpaque(scene);
			scene.RenderManager.DisableAntialiasing();
		}
		
		
#endregion
		
	}
}
