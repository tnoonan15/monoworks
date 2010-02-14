// DrawingController.cs - MonoWorks Project
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

using MonoWorks.Base;
using MonoWorks.Rendering;
using MonoWorks.Rendering.Interaction;
using MonoWorks.Controls;
using MonoWorks.Controls.StandardScene;
using MonoWorks.Modeling.Sketching;

namespace MonoWorks.Modeling.SceneControls
{
	/// <summary>
	/// Controller for a Model Scene.
	/// </summary>
	public class DrawingController : StandardSceneController
	{
		public DrawingController(Scene scene, IAttributePanel attributePanel)
			: base(scene)
		{
			this.attributePanel = attributePanel;
			
			OnSolidModeChanged();

			LoadStandardToolbars();
			ContextLayer.AddContext(Side.N, "Shading");

			// get ready for sketching
			AnchorPane sketchAnchor = new AnchorPane(AnchorLocation.NE);
			sketchApplyCancel = new CornerButtons(Corner.NE);
			sketchApplyCancel.IsVisible = false;
			sketchApplyCancel.Action1 += OnApplySketch;
			sketchApplyCancel.Action2 += OnCancelSketch;
			sketchApplyCancel.Image1 = new Image(ResourceHelper.GetStream("apply.png", "MonoWorks.Rendering"));
			sketchApplyCancel.Image2 = new Image(ResourceHelper.GetStream("cancel.png", "MonoWorks.Rendering"));
			sketchAnchor.Control = sketchApplyCancel;
			Scene.RenderList.AddOverlay(sketchAnchor);
			
		}



		/// <summary>
		/// The drawing interactor attached to the Scene.
		/// </summary>
		protected DrawingInteractor DrawingInteractor
		{
			get { return Scene.PrimaryInteractor as DrawingInteractor; }
		}



#region Shading Actions

		
		protected readonly Dictionary<SolidMode, string> solidModeNames = new Dictionary<SolidMode, string>
		{{SolidMode.None,"No Solid"}, {SolidMode.Flat,"Flat Shaded"}, {SolidMode.Smooth,"Smooth Shaded"}};
		
		
		[Action("Wireframe")]
		public void OnWireframe()
		{
//			if (UiManager.HasToolbar("Shading"))
//			{
//				ToolBar toolbar = UiManager.GetToolbar("Shading");
//				Scene.RenderManager.ShowWireframe = toolbar.GetButton("Wireframe").IsSelected;
//			}
		}

		[Action("No Solid")]
		public void OnNoSolid()
		{
			Scene.RenderManager.SolidMode = SolidMode.None;
			OnSolidModeChanged();
		}

		[Action("Flat Shaded")]
		public void OnFlatShaded()
		{
			Scene.RenderManager.SolidMode = SolidMode.Flat;
			OnSolidModeChanged();
		}

		[Action("Smooth Shaded")]
		public void OnSmoothShaded()
		{
			Scene.RenderManager.SolidMode = SolidMode.Smooth;
			OnSolidModeChanged();
		}

		/// <summary>
		/// Updates the controls based on a new solid rendering mode.
		/// </summary>
		public void OnSolidModeChanged()
		{
//			if (UiManager.HasToolbar("Shading"))
//			{
//				ToolBar toolbar = UiManager.GetToolbar("Shading");
//				string solidString = solidModeNames[Scene.RenderManager.SolidMode];
//				foreach (Button button in toolbar)
//				{
//					if (button.LabelString == solidString)
//						button.IsSelected = true;
//					else if (button.LabelString != "Wireframe") // don't touch the wireframe button
//						button.IsSelected = false;
//				}
//			}
		}

#endregion


#region Context Management

		/// <summary>
		/// The location of the primary toolbar.
		/// </summary>
		protected Side primaryLoc = Side.E;

		/// <summary>
		/// Convenience method to add a context to the primary context location.
		/// </summary>
		/// <param name="context"></param>
		protected void AddPrimaryContext(string context)
		{
			ContextLayer.AddContext(primaryLoc, context);
		}

		/// <summary>
		/// The last drawing to be used on the Scene.
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
			(Scene.PrimaryInteractor as DrawingInteractor).SketchableChanged += OnSketchableChanged;
			OnContextChanged();
		}
		
		
		/// <summary>
		/// Handles the selection/interaction context changing.
		/// </summary>
		protected void OnContextChanged()
		{

			ContextLayer.ClearContexts(primaryLoc);

			if (drawing.EntityManager.NumSelected == 0) // nothing selected
			{
				attributePanel.Hide();
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
					else if (entity is Sketch)
					{
						AddPrimaryContext("EditSketch");
						AddPrimaryContext("Features");
					}
					else if (!entity.IsLocked) // only edit if it's not locked
					{
						AddPrimaryContext("Edit");
						AddPrimaryContext("Delete");
					}
				}
				else // multiple entities selected
				{
					Console.WriteLine("muliple selection:");
					foreach (Entity entity in drawing.EntityManager.Selected)
						Console.WriteLine("  entity: " + entity.Name);
				}

			}

			Scene.Paint();
		}

#endregion


#region References

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
			Scene.Paint();
		}

		/// <summary>
		/// Handles an attribute panel being hiddein.
		/// </summary>
		/// <param name="panel"></param>
		public void OnAttributePanelHidden(IAttributePanel panel)
		{
			Scene.Paint();
		}

		[Action()]
		public void Delete()
		{
			Console.WriteLine("delete");
		}


#endregion


#region Sketching

		
		/// <value>
		/// Whether the user is currently sketching.
		/// </value>
		protected bool IsSketching {get {return sketchApplyCancel.IsVisible;}}

		/// <summary>
		/// Control for applying/cancelling sketch changes.
		/// </summary>
		private CornerButtons sketchApplyCancel;


		/// <summary>
		/// Creates a sketch.
		/// </summary>
		[Action("AddChild Sketch")]
		public void OnAddSketch()
		{
			if (entity is RefPlane)
			{
				Sketch sketch = new Sketch(entity as RefPlane);
				drawing.AddSketch(sketch);
				Scene.Camera.AnimateTo(entity as RefPlane);
				DrawingInteractor.BeginSketching(sketch);
//				Scene.UsePrimaryInteractor = true;

				drawing.EntityManager.DeselectAll(null);

				sketchApplyCancel.IsVisible = true;
				OnContextChanged();
			}
			else
				throw new Exception("Trying to sketch an entity that isn't a plane. This should never happen.");

		}

		/// <summary>
		/// Edits a sketch.
		/// </summary>
		[Action("Edit Sketch")]
		public void OnEditSketch()
		{
			if (entity is Sketch)
			{
				Sketch sketch = entity as Sketch;
				Scene.Camera.AnimateTo(sketch.Plane);
				DrawingInteractor.BeginSketching(sketch);
//				Scene.UsePrimaryInteractor = true;

				drawing.EntityManager.DeselectAll(null);

				sketchApplyCancel.IsVisible = true;
				OnContextChanged();
			}
			else
				throw new Exception("Trying to sketch an entity that isn't a Sketch. This should never happen.");
		}

		/// <summary>
		/// Things to do when the sketching ends, whether or not it was applied or cancelled.
		/// </summary>
		private void OnEndSketch()
		{
			drawing.MakeReferencesDirty();
			sketchApplyCancel.IsVisible = false;
			OnContextChanged();
		}

		/// <summary>
		/// Handles the sketching being applied.
		/// </summary>
		public void OnApplySketch(object sender, EventArgs args)
		{
			DrawingInteractor.ApplySketching();
			OnEndSketch();
		}

		/// <summary>
		/// Handles the sketching being canceled.
		/// </summary>
		public void OnCancelSketch(object sender, EventArgs args)
		{
			DrawingInteractor.CancelSketching();
			OnEndSketch();
		}

		/// <summary>
		/// Handles the currently selected changing.
		/// </summary>
		public void OnSketchableChanged(Sketchable skethable)
		{
			attributePanel.Show(this, skethable);
		}

		/// <summary>
		/// Adds a line to the current sketch.
		/// </summary>
		[Action("Line")]
		public void OnSketchLine()
		{
			DrawingInteractor.AddSketchable(new Line(DrawingInteractor.Sketch));
		}

		/// <summary>
		/// Adds a rectangle to the current sketch.
		/// </summary>
		[Action("Rectangle")]
		public void OnSketchRectangle()
		{
			DrawingInteractor.AddSketchable(new Rectangle(DrawingInteractor.Sketch));
		}

		/// <summary>
		/// Adds a arc to the current sketch.
		/// </summary>
		[Action("Arc")]
		public void OnSketchArc()
		{
			DrawingInteractor.AddSketchable(new Arc(DrawingInteractor.Sketch));
		}

		/// <summary>
		/// Adds a ellipse to the current sketch.
		/// </summary>
		[Action("Ellipse")]
		public void OnSketchEllipse()
		{
			DrawingInteractor.AddSketchable(new Ellipse(DrawingInteractor.Sketch));
		}

		/// <summary>
		/// Adds a spline to the current sketch.
		/// </summary>
		[Action("Spline")]
		public void OnSketchSpline()
		{
			DrawingInteractor.AddSketchable(new Spline(DrawingInteractor.Sketch));
		}

#endregion


#region Features

		/// <summary>
		/// Adds an extrusion based on a selected sketch.
		/// </summary>
		[Action("Extrusion")]
		public void OnAddExtrusion()
		{
			if (drawing.EntityManager.NumSelected != 1 ||
				!(drawing.EntityManager.Selected[0] is Sketch))
				throw new Exception("Attempting to add a feature to something other than a sketch. This should never happen.");
			
			Sketch sketch = drawing.EntityManager.Selected[0] as Sketch;
			Extrusion extrusion = new Extrusion(sketch);
			drawing.AddFeature(extrusion);
			drawing.EntityManager.DeselectAll(null);
			drawing.EntityManager.Select(null, extrusion);
			entity = extrusion;
			Edit(); // edit the extrusion

			Scene.Camera.AnimateTo(ViewDirection.Standard);
		}

		/// <summary>
		/// Adds a revolution based on a selected sketch.
		/// </summary>
		[Action("Revolution")]
		public void OnAddRevolution()
		{
			if (drawing.EntityManager.NumSelected != 1 ||
				!(drawing.EntityManager.Selected[0] is Sketch))
				throw new Exception("Attempting to add a feature to something other than a sketch. This should never happen.");

			Sketch sketch = drawing.EntityManager.Selected[0] as Sketch;
			Revolution revolution = new Revolution(sketch);
			drawing.AddFeature(revolution);
			drawing.EntityManager.DeselectAll(null);
			drawing.EntityManager.Select(null, revolution);
			entity = revolution;

			Scene.Camera.AnimateTo(ViewDirection.Standard);
			Edit(); // edit the extrusion
		}

		/// <summary>
		/// Adds a sweep based on a selected sketch.
		/// </summary>
		[Action("Sweep")]
		public void OnAddSweep()
		{

		}

#endregion



	}
}
