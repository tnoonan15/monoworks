//   Drawing.cs - MonoWorks Project
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
using System.Linq;
using System.Xml;
using System.IO;

using gl = Tao.OpenGl.Gl;

using MonoWorks.Base;
using MonoWorks.Rendering;
using MonoWorks.Modeling.Sketching;

namespace MonoWorks.Modeling
{
	
	/// <summary>
	/// The Drawing entity represents the root entity for a drawing.
	/// It stores drawing metadata as well as the drawing's top level entities. 
	/// </summary>
	public abstract class Drawing : Entity
	{
		
		static uint DocCounter = 0;
		
		/// <summary>
		/// Default constructor.
		/// </summary>
		public Drawing() : base()
		{			
			EntityManager = new EntityManager(this);
			
			RegisterEntity(this);
			
			ColorManager = new ColorManager();
			
			DocCounter++;
			Name = String.Format("drawing{0}", DocCounter);
			
			// initialize actions
			currentAction = -1;
			actionList = new List<Action>();

			Modified = false;

			// create the default reference geometry
			for (int i = 0; i < defaultPlanes.Length; i++)
			{
				defaultPlanes[i] = new RefPlane();
				defaultPlanes[i].Name = DimensionNames[i] + " Plane";
				defaultPlanes[i].IsLocked = true;
				AddReference(defaultPlanes[i]);

				Plane plane = new Plane();
				plane.Center = new Point();
				plane.Normal = new Vector();
				plane.Normal[i] = 1;
				defaultPlanes[i].Plane = plane;

				RefLine line = new RefLine();
				line.Name = DimensionNames[i] + " Axis";
				line.Direction = new Vector();
				line.Direction[i] = 1;
				defaultLines[i] = line;
				AddReference(line);
			}
		}
		
		
		/// <value>
		/// Returns itself.
		/// </value>
		public override Drawing TheDrawing
		{
			get { return this; }
		}
		
		/// <value>
		/// The color manager for this drawing.
		/// </value>
		public ColorManager ColorManager { get; private set; }
				
		/// <summary>
		/// Handles entity selection.
		/// </summary>
		public EntityManager EntityManager {get; private set; }



#region File I/O
				
		/// <summary> 
		/// Loads a drawing from a file.
		/// </summary>
		/// <param name="fileName">The name of the file.</param>
		/// <returns>A drawing or part, depending on file extension.</returns>
		public static Drawing FromFile(string fileName)
		{
			if (fileName.EndsWith("mwp")) // a part file
			{
			}
			return null;
		}

		/// <summary>
		/// Writes the drawing to a file.
		/// </summary>
		/// <param name="fileName">The file name.</param>
		public void SaveAs(string fileName)
		{
			Modified = false;

			XmlTextWriter writer = new XmlTextWriter(fileName, System.Text.Encoding.ASCII);
			writer.Formatting = Formatting.Indented;
			ToXml(writer);
			writer.Close();
			FileName = fileName;
		}

		/// <summary>
		/// Saves the file to the last used file name.
		/// </summary>
		public void Save()
		{
			SaveAs(FileName);
		}

		/// <summary>
		/// The file name of the drawing.
		/// </summary>
		public string FileName { get; private set; }

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

			Modified = true;
		}
		
		/// <summary>
		/// Undo the last action.
		/// </summary>
		public override void Undo()
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
		public override void Redo()
		{
			if (currentAction < actionList.Count-1)
			{
				currentAction++;
				actionList[currentAction].Redo();
			}
		}

		/// <summary>
		/// Whether the document has been modified and needs saving.
		/// </summary>
		public bool Modified { get; protected set; }
		
#endregion
		
		
#region Children
	
		/// <summary>
		/// Adds a sketch as a top-level entity.
		/// </summary>
		/// <param name="sketch"> A <see cref="Sketch"/> to add to the drawing. </param>
		public void AddSketch(Sketch sketch)
		{
			AddChild(sketch);
		}
	
	
		/// <summary>
		/// Adds reference geometry as a top-level entity.
		/// </summary>
		/// <param name="reference"> A <see cref="Reference"/> to add to the drawing. </param>
		public void AddReference(Reference reference)
		{
			AddChild(reference);
		}
	
	
		/// <summary>
		/// Adds a feature as a top-level entity.
		/// </summary>
		/// <param name="feature"> A <see cref="Feature"/> to add to the drawing. </param>
		public void AddFeature(Feature feature)
		{
			AddChild(feature);
		}
		
		
		/// <summary>
		/// Makes the drawing dirty.
		/// </summary>
		/// <remarks>Makes all reference items dirty as well.</remarks>
		public override void MakeDirty()
		{
			base.MakeDirty();
		}

		/// <summary>
		/// Gets called when a child is made dirty.
		/// </summary>
		public void ChildDirty(Entity child)
		{
			if (child is Feature)
			{
				MakeReferencesDirty();
			}		
		}

		/// <summary>
		/// Makes all reference entities at the top level of the drawing dirty.
		/// </summary>
		public void MakeReferencesDirty()
		{
			foreach (Entity ref_ in GetChildren<Reference>())
				ref_.MakeDirty();
		}
			
#endregion


#region Default Reference Entities

		/// <summary>
		/// Default reference planes.
		/// </summary>
		protected RefPlane[] defaultPlanes = new RefPlane[3];

		/// <summary>
		/// The reference plane orthagonal to the x axis.
		/// </summary>
		public RefPlane XPlane { get { return defaultPlanes[0]; } }

		/// <summary>
		/// The reference plane orthagonal to the x axis.
		/// </summary>
		public RefPlane YPlane { get { return defaultPlanes[1]; } }

		/// <summary>
		/// The reference plane orthagonal to the z axis.
		/// </summary>
		public RefPlane ZPlane { get { return defaultPlanes[2]; } }

		/// <summary>
		/// Default reference lines.
		/// </summary>
		protected RefLine[] defaultLines = new RefLine[3];

		/// <summary>
		/// The reference line on the x axis.
		/// </summary>
		public RefLine XAxis { get { return defaultLines[0]; } }

		/// <summary>
		/// The reference line on the y axis.
		/// </summary>
		public RefLine YAxis { get { return defaultLines[1]; } }

		/// <summary>
		/// The reference line on the z axis.
		/// </summary>
		public RefLine ZAxis { get { return defaultLines[2]; } }

#endregion



	}
	
}
