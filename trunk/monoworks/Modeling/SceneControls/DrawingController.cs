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
using MonoWorks.Controls.World;
using MonoWorks.Modeling.Sketching;

namespace MonoWorks.Modeling.SceneControls
{
	/// <summary>
	/// Controller for a Model Scene.
	/// </summary>
	public class DrawingController : WorldController<DrawingScene>
	{
		public DrawingController(DrawingScene scene)
			: base(scene)
		{
			Mwx.Parse(ResourceHelper.GetStream("Drawing.mwx"));
			
			OnSolidModeChanged();

			// get ready for sketching
			AnchorPane sketchAnchor = new AnchorPane(AnchorLocation.NE);
			sketchApplyCancel = new CornerButtons(Corner.NE);
			sketchApplyCancel.IsVisible = false;
			sketchApplyCancel.Action1 += OnApplySketch;
			sketchApplyCancel.Action2 += OnCancelSketch;
			sketchApplyCancel.Image1 = new Image(ResourceHelper.GetStream("dialog-ok.png"));
			sketchApplyCancel.Image2 = new Image(ResourceHelper.GetStream("dialog-cancel.png"));
			sketchAnchor.Control = sketchApplyCancel;
			Scene.RenderList.AddOverlay(sketchAnchor);
			
			// load the default toolbars
			Context(Side.N, "FullViewToolbar");
			Context(Side.N, "ExportToolbar");
			Context(Side.N, "ShadingToolbar");
			OnProjectionChanged();
			
			// load the tree view
			_treeView = Mwx.Get<EntityTreeView>("TreeView");
			Context(Side.W, "TreeView");
			
			ShowWorldInfo = true;
			
			Scene.DrawingChanged += OnDrawingChanged;
		}

		/// <summary>
		/// Handles the scene's drawing changing. 
		/// </summary>
		private void OnDrawingChanged(object sender, DrawingChangedEvent evt)
		{
			Scene.Drawing.SelectionChanged += OnSelectionChanged;
		}


		/// <summary>
		/// The drawing interactor attached to the Scene.
		/// </summary>
		protected DrawingInteractor DrawingInteractor
		{
			get { return Scene.PrimaryInteractor as DrawingInteractor; }
		}


		
		#region Shading Actions
		
		protected static readonly Dictionary<SolidMode, string> _solidModeNames = new Dictionary<SolidMode, string> {
			{SolidMode.None,"No Solid"}, 
			{SolidMode.Flat,"Flat Shaded"}, 
			{SolidMode.Smooth,"Smooth Shaded"}
		};
		
		
		[ActionHandler("Wireframe")]
		public void OnWireframe(object sender, EventArgs args)
		{
			Scene.RenderManager.ShowWireframe = (sender as Button).IsSelected;
		}

		[ActionHandler("No Solid")]
		public void OnNoSolid(object sender, EventArgs args)
		{
			Scene.RenderManager.SolidMode = SolidMode.None;
			OnSolidModeChanged();
		}

		[ActionHandler("Flat Shaded")]
		public void OnFlatShaded(object sender, EventArgs args)
		{
			Scene.RenderManager.SolidMode = SolidMode.Flat;
			OnSolidModeChanged();
		}

		[ActionHandler("Smooth Shaded")]
		public void OnSmoothShaded(object sender, EventArgs args)
		{
			Scene.RenderManager.SolidMode = SolidMode.Smooth;
			OnSolidModeChanged();
		}

		/// <summary>
		/// Updates the controls based on a new solid rendering mode.
		/// </summary>
		public void OnSolidModeChanged()
		{
			var shadingToolbar = Mwx.Get<ToolBar>("ShadingToolbar");
			if (shadingToolbar != null)
			{
				string solidString = _solidModeNames[Scene.RenderManager.SolidMode];
				foreach (Button button in shadingToolbar)
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
		protected Side primaryLoc = Side.E;

		/// <summary>
		/// Convenience method to add a context to the primary context location.
		/// </summary>
		/// <param name="context"></param>
		protected void AddPrimaryContext(string context)
		{
			Context(primaryLoc, context);
		}

		/// <summary>
		/// The last entity to be selected.
		/// </summary>
		protected Entity entity = null;

		/// <summary>
		/// Handles the selection being changed, 
		/// update the context toolbar.
		/// </summary>
		private void OnSelectionChanged(object sender, EventArgs args)
		{
			(Scene.PrimaryInteractor as DrawingInteractor).SketchableChanged += OnSketchableChanged;
			OnContextChanged();
		}
		
		
		/// <summary>
		/// Handles the selection/interaction context changing.
		/// </summary>
		protected void OnContextChanged()
		{

			ContextLayer.ClearContexts(primaryLoc);
			
			var selected = Scene.Drawing.GetSelected();

			if (selected.Count == 0) // nothing selected
			{
//				attributePanel.Hide();
				if (IsSketching)
					AddPrimaryContext("SketchToolbar");
				else
					AddPrimaryContext("AddRefToolbar");
			}
			else // something selected
			{
				if (selected.Count == 1) // only one selected 
				{
					entity = selected[0];

					// add sketch context if it's a plane
					if (entity is RefPlane && !IsSketching)
						AddPrimaryContext("AddSketchToolbar");
					else if (entity is Sketch)
					{
						AddPrimaryContext("EditSketchToolbar");
						AddPrimaryContext("FeaturesToolbar");
					}
					else if (!entity.IsLocked) // only edit if it's not locked
					{
						AddPrimaryContext("EditToolbar");
						AddPrimaryContext("DeleteToolbar");
					}
				}
				else // multiple entities selected
				{
					Console.WriteLine("muliple selection:");
					foreach (Entity entity in selected)
						Console.WriteLine("  entity: " + entity.Name);
				}

			}

			Scene.Paint();
		}
		
		#endregion

		
		#region Tree

		private EntityTreeView _treeView;
		
		/// <summary>
		/// Reloads the tree based on the current drawing. 
		/// </summary>
		public void ReloadTree()
		{
			_treeView.Drawing = Scene.Drawing;
		}
		
		
		#endregion
		

#region References

		[ActionHandler("Sketch")]
		public void AddSketch()
		{
			Console.WriteLine("add sketch");
		}

		[ActionHandler("Ref Point")]
		public void AddRefPoint()
		{
			Console.WriteLine("add ref point");
		}

		[ActionHandler("Ref Line")]
		public void AddRefLine()
		{
			Console.WriteLine("add ref line");
		}

		[ActionHandler("Ref Plane")]
		public void AddRefPlane()
		{
			Console.WriteLine("add ref plane");
		}

#endregion


#region Entity Editing

//		protected IAttributePanel attributePanel;

		[ActionHandler()]
		public void Edit(object sender, EventArgs args)
		{
			if (entity == null)
				throw new Exception("The Edit action should never be called without lastEntity set.");

//			attributePanel.Show(this, entity);
		}

		/// <summary>
		/// Handles an attribute being changed by an attribute control.
		/// </summary>
		/// <param name="attrControl"></param>
//		public void OnAttributeChanged(IAttributeControl attrControl)
//		{
//			Scene.Paint();
//		}

		/// <summary>
		/// Handles an attribute panel being hiddein.
		/// </summary>
		/// <param name="panel"></param>
//		public void OnAttributePanelHidden(IAttributePanel panel)
//		{
//			Scene.Paint();
//		}

		[ActionHandler()]
		public void Delete(object sender, EventArgs args)
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
		[ActionHandler("Add Sketch")]
		public void OnAddSketch(object sender, EventArgs args)
		{
			if (entity is RefPlane)
			{
				Sketch sketch = new Sketch(entity as RefPlane);
				Scene.Drawing.AddSketch(sketch);
				Scene.Camera.AnimateTo(entity as RefPlane);
				DrawingInteractor.BeginSketching(sketch);
				//				Scene.UsePrimaryInteractor = true;

				Scene.Drawing.Deselect();

				sketchApplyCancel.IsVisible = true;
				OnContextChanged();
			}
			else
				throw new Exception("Trying to sketch an entity that isn't a plane. This should never happen.");

		}

		/// <summary>
		/// Edits a sketch.
		/// </summary>
		[ActionHandler("Edit Sketch")]
		public void OnEditSketch(object sender, EventArgs args)
		{
			if (entity is Sketch)
			{
				Sketch sketch = entity as Sketch;
				Scene.Camera.AnimateTo(sketch.Plane);
				DrawingInteractor.BeginSketching(sketch);
				//				Scene.UsePrimaryInteractor = true;

				Scene.Drawing.Deselect();

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
			Scene.Drawing.MakeReferencesDirty();
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
//			attributePanel.Show(this, skethable);
		}

		/// <summary>
		/// Adds a line to the current sketch.
		/// </summary>
		[ActionHandler("Line")]
		public void OnSketchLine(object sender, EventArgs args)
		{
			DrawingInteractor.AddSketchable(new Line(DrawingInteractor.Sketch));
		}

		/// <summary>
		/// Adds a rectangle to the current sketch.
		/// </summary>
		[ActionHandler("Rectangle")]
		public void OnSketchRectangle(object sender, EventArgs args)
		{
			DrawingInteractor.AddSketchable(new Rectangle(DrawingInteractor.Sketch));
		}

		/// <summary>
		/// Adds a arc to the current sketch.
		/// </summary>
		[ActionHandler("Arc")]
		public void OnSketchArc(object sender, EventArgs args)
		{
			DrawingInteractor.AddSketchable(new Arc(DrawingInteractor.Sketch));
		}

		/// <summary>
		/// Adds a ellipse to the current sketch.
		/// </summary>
		[ActionHandler("Ellipse")]
		public void OnSketchEllipse(object sender, EventArgs args)
		{
			DrawingInteractor.AddSketchable(new Ellipse(DrawingInteractor.Sketch));
		}

		/// <summary>
		/// Adds a spline to the current sketch.
		/// </summary>
		[ActionHandler("Spline")]
		public void OnSketchSpline(object sender, EventArgs args)
		{
			DrawingInteractor.AddSketchable(new Spline(DrawingInteractor.Sketch));
		}

#endregion


#region Features

		/// <summary>
		/// Adds an extrusion based on a selected sketch.
		/// </summary>
		[ActionHandler("Extrusion")]
		public void OnAddExtrusion(object sender, EventArgs args)
		{
			var selected = Scene.Drawing.GetSelected();
			if (selected.Count != 1 || !(selected[0] is Sketch))
				throw new Exception("Attempting to add a feature to something other than a sketch. This should never happen.");
			
			Sketch sketch = selected[0] as Sketch;
			Extrusion extrusion = new Extrusion(sketch);
			Scene.Drawing.AddFeature(extrusion);
			Scene.Drawing.Deselect();
			extrusion.Select();
			entity = extrusion;
			Edit(this, new EventArgs()); // edit the extrusion

			Scene.Camera.AnimateTo(ViewDirection.Standard);
		}

		/// <summary>
		/// Adds a revolution based on a selected sketch.
		/// </summary>
		[ActionHandler("Revolution")]
		public void OnAddRevolution(object sender, EventArgs args)
		{
			var selected = Scene.Drawing.GetSelected();
			if (selected.Count != 1 || !(selected[0] is Sketch))
				throw new Exception("Attempting to add a feature to something other than a sketch. This should never happen.");

			Sketch sketch = selected[0] as Sketch;
			Revolution revolution = new Revolution(sketch);
			Scene.Drawing.AddFeature(revolution);
			Scene.Drawing.Deselect();
			revolution.Select();
			entity = revolution;

			Scene.Camera.AnimateTo(ViewDirection.Standard);
			Edit(this, new EventArgs()); // edit the extrusion
		}

		/// <summary>
		/// Adds a sweep based on a selected sketch.
		/// </summary>
		[ActionHandler("Sweep")]
		public void OnAddSweep(object sender, EventArgs args)
		{

		}

#endregion



	}
}
