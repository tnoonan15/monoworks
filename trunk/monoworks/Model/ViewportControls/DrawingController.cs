// Controller.cs - MonoWorks Project
//
//  Copyright (C) 2009 Andy Selvig
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
//
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA 

using System;
using System.Collections.Generic;

using MonoWorks.Framework;
using MonoWorks.Rendering;
using MonoWorks.Rendering.Interaction;
using MonoWorks.Rendering.Controls;
using MonoWorks.Rendering.ViewportControls;

namespace MonoWorks.Model.ViewportControls
{
	/// <summary>
	/// Controller for a Model viewport.
	/// </summary>
	public class DrawingController : ViewportController
	{
		public DrawingController(Viewport viewport, IAttributePanel attributePanel)
			: base(viewport)
		{
			this.attributePanel = attributePanel;

			UiManager.LoadStream(ResourceHelper.GetStream("Viewport.ui"));
			OnSolidModeChanged();

			// get ready for sketching
			sketchApplyCancel = new ApplyCancelControl();
			sketchApplyCancel.IsVisible = false;
			sketchApplyCancel.Apply += OnApplySketch;
			sketchApplyCancel.Cancel += OnCancelSketch;
			viewport.RenderList.AddOverlay(sketchApplyCancel);
		}


#region Shading Actions

		
		protected readonly Dictionary<SolidMode, string> solidModeNames = new Dictionary<SolidMode, string>
		{{SolidMode.None,"No Solid"}, {SolidMode.Flat,"Flat Shaded"}, {SolidMode.Smooth,"Smooth Shaded"}};
		
		
		[Action("Wireframe")]
		public void OnWireframe()
		{
			if (UiManager.HasToolbar("Shading"))
			{
				ToolBar toolbar = UiManager.GetToolbar("Shading");
				viewport.RenderManager.ShowWireframe = toolbar.GetButton("Wireframe").IsSelected;
			}
		}

		[Action("No Solid")]
		public void OnNoSolid()
		{
			viewport.RenderManager.SolidMode = SolidMode.None;
			OnSolidModeChanged();
		}

		[Action("Flat Shaded")]
		public void OnFlatShaded()
		{
			viewport.RenderManager.SolidMode = SolidMode.Flat;
			OnSolidModeChanged();
		}

		[Action("Smooth Shaded")]
		public void OnSmoothShaded()
		{
			viewport.RenderManager.SolidMode = SolidMode.Smooth;
			OnSolidModeChanged();
		}

		/// <summary>
		/// Updates the controls based on a new solid rendering mode.
		/// </summary>
		public void OnSolidModeChanged()
		{
			if (UiManager.HasToolbar("Shading"))
			{
				ToolBar toolbar = UiManager.GetToolbar("Shading");
				string solidString = solidModeNames[viewport.RenderManager.SolidMode];
				foreach (Button button in toolbar)
				{
					if (button.LabelString == solidString)
						button.IsSelected = true;
					else if (button.LabelString != "Wireframe") // don't touch the wireframe button
						button.IsSelected = false;
				}
			}
		}

#endregion



#region Context Management

		/// <summary>
		/// The location of the primary toolbar.
		/// </summary>
		protected ContextLocation primaryLoc = ContextLocation.E;

		/// <summary>
		/// Convenience method to add a context to the primary context location.
		/// </summary>
		/// <param name="context"></param>
		protected void AddPrimaryContext(string context)
		{
			ContextLayer.AddContext(primaryLoc, context);
		}

		/// <summary>
		/// The last drawing to be used on the viewport.
		/// </summary>
		protected Drawing drawing = null;

		/// <summary>
		/// The last entity to be selected.
		/// </summary>
		protected Entity entity = null;

		/// <summary>
		/// Handles the selection being changed, 
		/// update the context toolbar.
		/// </summary>
		public void OnSelectionChanged(Drawing drawing)
		{
			this.drawing = drawing;
			OnContextChanged();
		}
		
		
		/// <summary>
		/// Handles the selection/interaction context changing.
		/// </summary>
		protected void OnContextChanged()
		{
			attributePanel.Hide();

			ContextLayer.ClearContexts(primaryLoc);

			if (drawing.EntityManager.NumSelected == 0) // nothing selected
			{
				if (IsSketching)
					AddPrimaryContext("Sketch");
				else
					AddPrimaryContext("AddRef");
			}
			else // something selected
			{
				if (drawing.EntityManager.NumSelected == 1) // only one selected 
				{
					entity = drawing.EntityManager.Selected[0];

					// add sketch context if it's a plane
					if (entity is RefPlane && !IsSketching)
						AddPrimaryContext("AddSketch");

					// only edit if it's not locked
					if (!entity.IsLocked) 
					{
						AddPrimaryContext("Edit");
						AddPrimaryContext("Delete");
					}
				}
				else // multiple entities selected
				{
					foreach (Entity entity in drawing.EntityManager.Selected)
						Console.WriteLine("entity: " + entity.Name);
				}

			}

			viewport.PaintGL();
		}

#endregion



#region Add-Delete Actions

		[Action("Sketch")]
		public void AddSketch()
		{
			Console.WriteLine("add sketch");
		}

		[Action("Ref Point")]
		public void AddRefPoint()
		{
			Console.WriteLine("add ref point");
		}

		[Action("Ref Line")]
		public void AddRefLine()
		{
			Console.WriteLine("add ref line");
		}

		[Action("Ref Plane")]
		public void AddRefPlane()
		{
			Console.WriteLine("add ref plane");
		}


		[Action()]
		public void Delete()
		{
			Console.WriteLine("delete");
		}



#endregion


#region Entity Editing

		protected IAttributePanel attributePanel;

		[Action()]
		public void Edit()
		{
			if (entity == null)
				throw new Exception("The Edit action should never be called without lastEntity set.");

			attributePanel.Show(this, entity);
		}

		/// <summary>
		/// Handles an attribute being changed by an attribute control.
		/// </summary>
		/// <param name="attrControl"></param>
		public void OnAttributeChanged(IAttributeControl attrControl)
		{
			viewport.PaintGL();
		}

		/// <summary>
		/// Handles an attribute panel being hiddein.
		/// </summary>
		/// <param name="panel"></param>
		public void OnAttributePanelHidden(IAttributePanel panel)
		{
			viewport.PaintGL();
		}

#endregion



#region Sketching

		
		//// <value>
		/// Whether the user is currently sketching.
		/// </value>
		protected bool IsSketching {get {return sketchApplyCancel.IsVisible;}}

		/// <summary>
		/// The sketch interactor.
		/// </summary>
		protected SketchInteractor sketchInteractor = null;

		/// <summary>
		/// Control for applying/cancelling sketch changes.
		/// </summary>
		private ApplyCancelControl sketchApplyCancel;

		/// <summary>
		/// The primary interactor used by the viewport rigbht before the sketching starts.
		/// </summary>
		private AbstractInteractor primaryInteractor = null;

		/// <summary>
		/// Creates or edits a sketch.
		/// </summary>
		[Action("Sketch")]
		public void OnSketch()
		{
			if (entity is RefPlane)
			{
				Sketch sketch = new Sketch(entity as RefPlane);
				drawing.AddSketch(sketch);
				viewport.Camera.AnimateTo(entity as RefPlane);

				// switch out the primary interactor
				sketchInteractor = new SketchInteractor(viewport, sketch);
				primaryInteractor = viewport.PrimaryInteractor;
				viewport.PrimaryInteractor = sketchInteractor;
				viewport.InteractionState = InteractionState.Interact3D;

				drawing.EntityManager.DeselectAll(null);

				sketchApplyCancel.IsVisible = true;
				OnContextChanged();
			}
			else
				throw new Exception("Trying to sketch an entity that isn't a plane. This should never happen.");

		}

		/// <summary>
		/// Things to do when the sketching ends, whether or not it was applied or cancelled.
		/// </summary>
		private void OnEndSketch()
		{
			sketchApplyCancel.IsVisible = false;
			viewport.PrimaryInteractor = primaryInteractor;
			sketchInteractor = null;
			OnContextChanged();
		}

		/// <summary>
		/// Handles the sketching being applied.
		/// </summary>
		public void OnApplySketch(object sender, EventArgs args)
		{
			OnEndSketch();
		}

		/// <summary>
		/// Handles the sketching being canceled.
		/// </summary>
		public void OnCancelSketch(object sender, EventArgs args)
		{
			OnEndSketch();
		}

		/// <summary>
		/// Adds a line to the current sketch.
		/// </summary>
		[Action("Line")]
		public void OnSketchLine()
		{
			sketchInteractor.AddSketchable(new Line());
		}

		/// <summary>
		/// Adds a arc to the current sketch.
		/// </summary>
		[Action("Arc")]
		public void OnSketchArc()
		{
			sketchInteractor.AddSketchable(new Arc());
		}

		/// <summary>
		/// Adds a spline to the current sketch.
		/// </summary>
		[Action("Spline")]
		public void OnSketchSpline()
		{
			sketchInteractor.AddSketchable(new Spline());
		}

#endregion



	}
}
