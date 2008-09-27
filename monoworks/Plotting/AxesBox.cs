// AxesBox.cs - MonoWorks Project
//
//  Copyright (C) 2008 Andy Selvig
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

using gl = Tao.OpenGl.Gl;

using MonoWorks.Base;
using MonoWorks.Rendering;

namespace MonoWorks.Plotting
{
	/// <summary>
	/// How the axes are arranged.
	/// </summary>
	public enum AxesArrangement {Origin, Outside};

	/// <summary>
	/// How the axes are resized.
	/// </summary>
	public enum ResizeMode { Auto, Manual };

	
	/// <summary>
	/// Container for children that displays axes.
	/// </summary>
	public class AxesBox : Plottable
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public AxesBox()
			: this(null)
		{
		}

		/// <summary>
		/// Constructor with parent assignment.
		/// </summary>
		/// <param name="parent"> The parent <see cref="AxesBox"/>.</param>
		public AxesBox(AxesBox parent)
			: base(parent)
		{
			bounds.Minima = new Vector(-1, -1, -1);
			bounds.Maxima = new Vector(1, 1, 1);

			Arrangement = AxesArrangement.Outside;
			
			// make the grids
			for (int i=0; i<grids.Length; i++)
				grids[i] = new Grid(this);			
			grids[0].Axes[0] = axes[0];
			grids[0].Axes[1] = axes[1];
			grids[1].Axes[0] = axes[1];
			grids[1].Axes[1] = axes[2];
			grids[2].Axes[0] = axes[0];
			grids[2].Axes[1] = axes[2];
			
//			title = new TextRenderer();
//			title.Text = "";
		}

				

#region Children

		/// <summary>
		/// The children being plotted on this axes.
		/// </summary>
		protected List<Plottable> children = new List<Plottable>();

		/// <summary>
		/// Adds a plottable as a child.
		/// </summary>
		/// <param name="plottable"> A <see cref="Plottable"/>. </param>
		public void AddChild(Plottable plottable)
		{
			if (plottable is Axis)
				axes.Add(plottable as Axis);
			else
				children.Add(plottable);
			plottable.Parent = this;
		}

		/// <summary>
		/// Removes a child.
		/// </summary>
		/// <param name="plottable"> A <see cref="Plottable"/> that is a child of the axes. </param>
		public void RemoveChild(Plottable plottable)
		{
			if (plottable is Axis)
				axes.Remove(plottable as Axis);
			else
				children.Remove(plottable);
			plottable.Parent = null;
		}

#endregion


#region Resizing

		
		public override void OnViewportResized(IViewport viewport)
		{
			base.OnViewportResized(viewport);
			
			bounds.Minima = new Vector(-1, -1, -1);
			bounds.Maxima = new Vector(1, 1, 1);
			
			if (viewport.InteractionState.Mode == InteractionMode.Select2D)
			{
				double edgeFactor = 0.32 * viewport.Camera.ViewportToWorldScaling;
				Vector center = viewport.Camera.Center;
//				Console.WriteLine("edge scaling: {0}, center: {1}, width: {2}, height: {3}", edgeFactor, center, viewport.WidthGL, viewport.HeightGL);
				switch (viewport.Camera.LastDirection)
				{
				case ViewDirection.Front:
				case ViewDirection.Back:
					bounds.Minima[0] = center[0] - edgeFactor * viewport.WidthGL;
					bounds.Maxima[0] = center[0] + edgeFactor * viewport.WidthGL;
					bounds.Minima[2] = center[2] - edgeFactor * viewport.HeightGL;
					bounds.Maxima[2] = center[2] + edgeFactor * viewport.HeightGL;
					break;
				case ViewDirection.Left:
				case ViewDirection.Right:
					bounds.Minima[1] = center[1] - edgeFactor * viewport.WidthGL;
					bounds.Maxima[1] = center[1] + edgeFactor * viewport.WidthGL;
					bounds.Minima[2] = center[2] - edgeFactor * viewport.HeightGL;
					bounds.Maxima[2] = center[2] + edgeFactor * viewport.HeightGL;
					break;
				case ViewDirection.Top:
				case ViewDirection.Bottom:
					bounds.Minima[0] = center[0] - edgeFactor * viewport.WidthGL;
					bounds.Maxima[0] = center[0] + edgeFactor * viewport.WidthGL;
					bounds.Minima[1] = center[1] - edgeFactor * viewport.HeightGL;
					bounds.Maxima[1] = center[1] + edgeFactor * viewport.HeightGL;
					break;
				}
			}
			
			MakeDirty();
		}

		
		public override void OnViewDirectionChanged(IViewport viewport)
		{
			base.OnViewDirectionChanged(viewport);
			
			// reset the resizing mode
			resizeMode = ResizeMode.Auto;
			
			OnViewportResized(viewport);
		}

		
#endregion
		

#region Axes

		protected AxesArrangement arrangement;
		/// <value>
		/// The axes arrangement.
		/// </value>
		public AxesArrangement Arrangement
		{
			get { return arrangement; }
			set
			{
				arrangement = value;

				// regenerate the axes
				int numAxes = 3;
				axes.Clear();
				axes.Capacity = numAxes;
				for (int n = 0; n < numAxes; n++)
					new Axis(this);
			}
		}

		protected ResizeMode resizeMode = ResizeMode.Auto;
		/// <summary>
		/// How the axes are resized.
		/// </summary>
		/// <remarks> If Auto, the axes will automatically be sized to fit everything inside of them.
		/// If Manual, they will be fixed and only changed explicitely by the user.</remarks>
		public ResizeMode ResizeMode
		{
			get { return resizeMode; }
			set { resizeMode = value; }
		}


		/// <summary>
		/// The individual axes.
		/// </summary>
		protected List<Axis> axes = new List<Axis>();


		/// <summary>
		/// Updates the axes based on plot bounds.
		/// </summary>
		protected void UpdateAxes()
		{
			// generate the ticks
			foreach (Axis axis in axes)
				axis.GenerateTicks(axes.IndexOf(axis));
		}

		/// <summary>
		/// Updates the axes based on the camera position and arrangement, then renders them.
		/// </summary>
		/// <param name="viewport"> A <see cref="IViewport"/>. </param>
		protected void RenderAxes(IViewport viewport)
		{
			// update the positions
			switch (arrangement)
			{
			case AxesArrangement.Origin: // the axes should be placed at the lowest end of each range
				axes[0].Start = bounds.Minima;
				axes[1].Start = axes[0].Start;
				axes[2].Start = axes[0].Start;
				axes[0].Stop = new Vector(bounds.Maxima[0], bounds.Minima[1], bounds.Minima[2]);
				axes[1].Stop = new Vector(bounds.Minima[0], bounds.Maxima[1], bounds.Minima[2]);
				axes[2].Stop = new Vector(bounds.Minima[0], bounds.Minima[1], bounds.Maxima[2]);
				break;

			case AxesArrangement.Outside: // the axes should be placed along the oustide of the viewable area
				Vector[] edges = bounds.GetOutsideEdges(viewport);
				for (int i = 0; i < 3; i++)
				{
					axes[i].Start = edges[2 * i];
					axes[i].Stop = edges[2 * i + 1];
					axes[i].DirtyTicks();
				}
				break;

			default:
				throw new Exception(String.Format("arrangement {0} not supported", arrangement));
			}

			// perform the rendering
			foreach (Axis axis in axes)
				axis.RenderOverlay(viewport);
		}

#endregion

		
#region Grids
		
		/// <value>
		/// The grids.
		/// </value>
		protected Grid[] grids = new Grid[3];
		
		/// <summary>
		/// The furthest corner of the bounding box from the camera.
		/// </summary>
		protected Vector furthestCorner = null;
				
		/// <summary>
		/// Update the positions of the grids.
		/// </summary>
		protected void UpdateGrids(IViewport viewport)
		{
			// check if the furthest point from the camera is the same
			Vector furthestCorner_ = bounds.FurthestCorner(viewport.Camera.Position);
			if (furthestCorner == null || furthestCorner_ != furthestCorner)
			{
				furthestCorner = furthestCorner_;
				foreach (Grid grid in grids)
					grid.Corner = furthestCorner;
			}
		}
		
		/// <summary>
		/// Render the grids.
		/// </summary>
		protected void RenderGrids(IViewport viewport)
		{			
			// render them
			foreach (Grid grid in grids)
				grid.RenderOpaque(viewport);
		}
		
#endregion


#region Geometry

		protected Transform plotToWorldSpace = new Transform();
		/// <summary>
		/// Transformation to go from plot to world space.
		/// </summary>
		public Transform PlotToWorldSpace
		{
			get { return plotToWorldSpace; }
		}

		/// <summary>
		/// Computes the box geometry.
		/// </summary>
		public override void ComputeGeometry()
		{
			base.ComputeGeometry();


			if (children.Count > 0)
			{

				// get the new plot bounds if automatically sized
				if (resizeMode == ResizeMode.Auto) // automatically resize
				{
					plotBounds.Reset();
					foreach (Plottable child in children)
					{
						child.UpdateBounds();
						if (child.PlotBounds.IsSet)
							plotBounds.Resize(child.PlotBounds);
					}

					// make the plot bounds pretty
					plotBounds.Prettify();
				}

				if (plotBounds.IsSet) // only proceed if the plot bounds are set
				{					
					// compute the plot-render transformation
					plotToWorldSpace.Compute(plotBounds, bounds);

					// udpate the axes
					UpdateAxes();

					// force the children to recompute their geometry
					foreach (Plottable child in children)
					{
						child.ComputeGeometry();
					}
				}

			}


		}

#endregion



#region Title


//		protected TextRenderer title = new TextRenderer(14);
//		/// <summary>
//		/// The title.
//		/// </summary>
//		public string Title
//		{
//			get { return title.Text; }
//			set { title.Text = value; }
//		}


#endregion


#region Rendering

		public override void RenderOpaque(IViewport viewport)
		{
			base.RenderOpaque(viewport);

			gl.glColor3b(0, 0, 0);
			
			// render the grids
			UpdateGrids(viewport);
			RenderGrids(viewport);
			
			// define the clip planes
			double[] eq = new double[]{1, 0, 0, bounds.Maxima[0]};
			gl.glClipPlane(gl.GL_CLIP_PLANE0, eq);
			eq = new double[]{-1, 0, 0, -bounds.Minima[0]};
			gl.glClipPlane(gl.GL_CLIP_PLANE1, eq);
			eq = new double[]{0, 1, 0, bounds.Maxima[1]};
			gl.glClipPlane(gl.GL_CLIP_PLANE2, eq);
			eq = new double[]{0, -1, 0, -bounds.Minima[1]};
			gl.glClipPlane(gl.GL_CLIP_PLANE3, eq);
			eq = new double[]{0, 0, 1, bounds.Maxima[2]};
			gl.glClipPlane(gl.GL_CLIP_PLANE4, eq);
			eq = new double[]{0, 0, -1, -bounds.Minima[2]};
			gl.glClipPlane(gl.GL_CLIP_PLANE5, eq);
			
			// enable clipping
			for (int i=0; i<6; i++)
				gl.glEnable(gl.GL_CLIP_PLANE0 + i);
			
			// render the children
			foreach (Plottable child in children)
				child.RenderOpaque(viewport);
			
			// disable clipping
			for (int i=0; i<6; i++)
				gl.glDisable(gl.GL_CLIP_PLANE0 + i);
			
		}

		public override void RenderOverlay(IViewport viewport)
		{
			base.RenderOverlay(viewport);


			// render the axes
			RenderAxes(viewport);

			// render the title
//			title.Position = new Vector(300, 300, 0);
//			title.Text = "Blah Blah";
//			title.RenderOverlay(viewport);

			// render the child overlays
			foreach (Plottable child in children)
				child.RenderOverlay(viewport);
		}

#endregion
		
		
#region Mouse Interaction
		
		/// <summary>
		/// If in 2D mode, translate the axes limits and prevent the viewport from handling the interaction.
		/// </summary>
		public override bool HandlePan(IViewport viewport, double dx, double dy)
		{			
			if (viewport.InteractionState.Mode == InteractionMode.Select2D)
			{
				resizeMode = ResizeMode.Manual;
				// determine the difference to apply to the axes ranges
				Vector diff = (viewport.Camera.RightVec * dx + viewport.Camera.UpVector * dy) * viewport.Camera.ViewportToWorldScaling; 
				plotBounds.Translate(diff / plotToWorldSpace.Scaling);
				MakeDirty();
				return true;
			}
			else
				return false;
		}
		
		public override bool HandleDolly(IViewport viewport, double factor)
		{
			if (viewport.InteractionState.Mode == InteractionMode.Select2D)
			{
				resizeMode = ResizeMode.Manual;
				plotBounds.Expand(1 + factor);
				MakeDirty();
				return true;
			}
			else
				return false;
		}


		
#endregion

	}
}
