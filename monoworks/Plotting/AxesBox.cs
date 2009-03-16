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
using System.Linq;
using System.Collections.Generic;

using gl = Tao.OpenGl.Gl;

using MonoWorks.Base;
using MonoWorks.Rendering;
using MonoWorks.Rendering.Interaction;
using MonoWorks.Rendering.Events;

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
			for (int i = 0; i < grids.Length; i++)
			{
				axes[i] = new Axis(this);
				grids[i] = new Grid(this);
			}
			grids[0].Axes[0] = axes[0];
			grids[0].Axes[1] = axes[1];
			grids[1].Axes[0] = axes[1];
			grids[1].Axes[1] = axes[2];
			grids[2].Axes[0] = axes[0];
			grids[2].Axes[1] = axes[2];

			titleDef = new TextDef(16);
			titleDef.HorizontalAlignment = HorizontalAlignment.Center;
//			title.Text = "";
		}

				

#region Children

        protected List<Plottable> children = new List<Plottable>();
        /// <summary>
        /// The children being plotted on this axes.
        /// </summary>
        public IEnumerable<Plottable> Children
        {
            get { return children; }
        }

		/// <summary>
		/// Adds a plottable as a child.
		/// </summary>
		/// <param name="plottable"> A <see cref="Plottable"/>. </param>
		public void AddChild(Plottable plottable)
		{
			children.Add(plottable);
		}

		/// <summary>
		/// Removes a child.
		/// </summary>
		/// <param name="plottable"> A <see cref="Plottable"/> that is a child of the axes. </param>
		public void RemoveChild(Plottable plottable)
		{
			children.Remove(plottable);
		}


		/// <summary>
		/// Gets all the children that inherit from T.
		/// </summary>
		/// <typeparam name="T">Plottable or one of its subclasses.</typeparam>
		public IEnumerable<T> GetChildren<T>() where T : Plottable
		{
			return from child in children
				   where child is T
				   select child as T;
		}


#endregion


#region Resizing

		public override void ResetBounds()
		{

			bounds.Minima = new Vector(-1, -1, -1);
			bounds.Maxima = new Vector(1, 1, 1);
		}

		
		public override void OnViewportResized(Viewport viewport)
		{
			base.OnViewportResized(viewport);
			
			if (viewport.InteractionState == InteractionState.Interact2D)
			{
				double edgeFactor = 0.35 * viewport.Camera.ViewportToWorldScaling;
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

			foreach (Grid grid in grids)
				grid.MakeDirty();
			MakeDirty();
		}

		
		public override void OnViewDirectionChanged(Viewport viewport)
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
				MakeDirty();
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


		protected Axis[] axes = new Axis[3];
		/// <summary>
		/// The individual axes.
		/// </summary>
		public Axis[] Axes
		{
			get { return axes; }
		}


		protected string[] axisLabels = new string[]{"","",""};
		/// <value>
		/// The axis labels.
		/// </value>
		public string[] AxisLabels
		{
			get {return axisLabels;}
		}
		
		/// <summary>
		/// Updates the axis labels based on the children.
		/// </summary>
		protected void UpdateAxisLabels()
		{
			foreach (Plottable plottable in children)
			{
				if (plottable is PointPlot)
				{
					for (int i=0; i<3; i++)
					{
						axisLabels[i] = (plottable as PointPlot).GetColumnName(i);
					}
				}
			}
		}

		/// <summary>
		/// Updates the axes based on plot bounds.
		/// </summary>
		protected void UpdateAxes()
		{
			UpdateAxisLabels();
			
			// generate the ticks
			foreach (Axis axis in axes)
			{
				axis.GenerateTicks(Array.IndexOf(axes, axis));
				
				// assign the axis label
				axis.Label = axisLabels[Array.IndexOf(axes, axis)];
			}
		}

		/// <summary>
		/// Updates the axes based on the camera position and arrangement, then renders them.
		/// </summary>
		/// <param name="viewport"> A <see cref="Viewport"/>. </param>
		protected void RenderAxes(Viewport viewport)
		{

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
		/// Whether the grids are visible.
		/// </summary>
		public bool GridVisible
		{
			get { return grids[0].IsVisible; }
			set 
			{
				foreach (Grid grid in grids)
					grid.IsVisible = value;
			}
		}

		/// <summary>
		/// The last corner that the grids were set at.
		/// </summary>
		protected Vector lastCorner = null;
				
		/// <summary>
		/// Render the grids.
		/// </summary>
		protected void RenderGrids(Viewport viewport)
		{
			// update axes the positions
			Vector corner = null;
			switch (arrangement)
			{
			case AxesArrangement.Origin: // the axes should be placed at the lowest end of each range
				axes[0].Start = bounds.Minima;
				axes[1].Start = axes[0].Start;
				axes[2].Start = axes[0].Start;
				axes[0].Stop = new Vector(bounds.Maxima[0], bounds.Minima[1], bounds.Minima[2]);
				axes[1].Stop = new Vector(bounds.Minima[0], bounds.Maxima[1], bounds.Minima[2]);
				axes[2].Stop = new Vector(bounds.Minima[0], bounds.Minima[1], bounds.Maxima[2]);
				corner = new Vector(bounds.Maxima[0], bounds.Minima[1], bounds.Minima[2]);
				if (lastCorner == null || corner != lastCorner)
				{
					foreach (Grid grid in grids)
						grid.Corner = corner;
					lastCorner = corner;
				}
				break;

			case AxesArrangement.Outside: // the axes should be placed along the oustide of the viewable area
				Vector[] edges = bounds.GetOutsideEdges(viewport);
				for (int i = 0; i < 3; i++)
				{
					axes[i].Start = edges[2 * i];
					axes[i].Stop = edges[2 * i + 1];
					axes[i].DirtyTicks();
				}

				// figure out the grid corner
				if (edges[1] == edges[2])
					corner = new Vector(edges[0].X, edges[3].Y, edges[0].Z);
				else if (edges[0] == edges[3])
					corner = new Vector(edges[1].X, edges[2].Y, edges[0].Z);
				else if (edges[1] == edges[3])
					corner = new Vector(edges[0].X, edges[2].Y, edges[0].Z);
				else if (edges[0] == edges[2])
					corner = new Vector(edges[1].X, edges[3].Y, edges[0].Z);
				if (corner == null)
					throw new Exception("Something is wrong with the grid corner finding. Otherwise, you wouldn't be seeing this message.");
				if (lastCorner == null || corner != lastCorner)
				{
					foreach (Grid grid in grids)
						grid.Corner = corner;
					lastCorner = corner;
				}

				break;

			default:
				throw new NotImplementedException(String.Format("arrangement {0} not supported", arrangement));
			}

			// render them
			foreach (Grid grid in grids)
				grid.RenderOpaque(viewport);

			// tell the axes about the corner
			Coord center = viewport.Camera.WorldToScreen(corner);
			foreach (var axis in axes)
				axis.AxesCenter = center;
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

			// count the plots
			int childCount = 0;
			foreach (Plottable child in Children)
			{
				if (child is AbstractPlot)
					childCount++;
			}

			if (childCount > 0)
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

					// ensure all dimensions have non-zero span
					plotBounds.EnsureNonZeroRanges();
				}

			}
			else // no plot children
			{
				plotBounds.Minima = new Vector(-1, -1, -1);
				plotBounds.Maxima = new Vector(1, 1, 1);
			}

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

#endregion



#region Title


		protected TextDef titleDef;
		/// <summary>
		/// The title.
		/// </summary>
		public override string Title
		{
			get { return titleDef.Text; }
			set { titleDef.Text = value; }
		}

		/// <summary>
		/// Renders the title.
		/// </summary>
		protected void RenderTitle(Viewport viewport)
		{
			// determine the top and center of the bounds
			double top = 0;
			double left = 0;
			double right = 0;
			foreach (Vector corner in bounds.Corners)
			{
				Coord coord = viewport.Camera.WorldToScreen(corner);
				if (Array.IndexOf(bounds.Corners, corner)==0) // the first one
				{
					top = coord.Y;
					left = coord.X;
					right = coord.X;
				}
				if (coord.Y > top)
					top = coord.Y;
				if (coord.X < left)
					left = coord.X;
				if (coord.X > right)
					right = coord.X;
			}
			
			titleDef.Position = new Coord((left+right)/2, top + 32);
			viewport.RenderText(titleDef);
		}

#endregion


#region Rendering
		
		/// <summary>
		/// Enables clipping for the content rendering.
		/// </summary>
		/// <param name="viewport"> A <see cref="Viewport"/>. </param>
		protected void EnableClipping(Viewport viewport)
		{			
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
			
			for (int i=0; i<6; i++)
				gl.glEnable(gl.GL_CLIP_PLANE0 + i);
			
			// disable clipping for planes orthagonal to view direction
			if (viewport.InteractionState == InteractionState.Interact2D)
			{
				switch (viewport.Camera.LastDirection)
				{
				case ViewDirection.Front:
				case ViewDirection.Back:
					gl.glDisable(gl.GL_CLIP_PLANE2);
					gl.glDisable(gl.GL_CLIP_PLANE3);
					break;
				case ViewDirection.Left:
				case ViewDirection.Right:
					gl.glDisable(gl.GL_CLIP_PLANE0);
					gl.glDisable(gl.GL_CLIP_PLANE1);
					break;
				case ViewDirection.Top:
				case ViewDirection.Bottom:
					gl.glDisable(gl.GL_CLIP_PLANE4);
					gl.glDisable(gl.GL_CLIP_PLANE5);
					break;
				}
			}
		}
		
		/// <summary>
		/// Disables clipping for the content rendering.
		/// </summary>
		/// <param name="viewport"> A <see cref="Viewport"/>. </param>
		protected void DisableClipping(Viewport viewport)
		{
			for (int i=0; i<6; i++)
				gl.glDisable(gl.GL_CLIP_PLANE0 + i);
		}

		public override void RenderOpaque(Viewport viewport)
		{
			base.RenderOpaque(viewport);

			if (!IsVisible)
				return;

			gl.glColor3b(0, 0, 0);
			
			// render the grids
			RenderGrids(viewport);
			
			// enable clipping
			EnableClipping(viewport);
			
			// render the children
			foreach (var child in GetChildren<AbstractPlot>())
				child.RenderOpaque(viewport);
			
			// disable clipping
			DisableClipping(viewport);
		}

		public override void RenderOverlay(Viewport viewport)
		{
			base.RenderOverlay(viewport);

			if (!IsVisible)
				return;

			// render the axes
			RenderAxes(viewport);

			// render the title
			RenderTitle(viewport);

			// render the child overlays
			foreach (var child in GetChildren<AbstractPlot>())
				child.RenderOverlay(viewport);
		}

#endregion
		
		
#region Mouse Interaction
		
		/// <summary>
		/// If in 2D mode, translate the axes limits and prevent the viewport from handling the interaction.
		/// </summary>
		public override bool HandlePan(Viewport viewport, double dx, double dy)
		{			
			if (viewport.InteractionState == InteractionState.Interact2D)
			{
				resizeMode = ResizeMode.Manual;
				// determine the difference to apply to the axes ranges
				Vector diff = (viewport.Camera.RightVec * dx - viewport.Camera.UpVector * dy) * viewport.Camera.ViewportToWorldScaling; 
				plotBounds.Translate(diff / plotToWorldSpace.Scaling);
				MakeDirty();
				return true;
			}
			else
				return false;
		}
		
		public override bool HandleDolly(Viewport viewport, double factor)
		{
			if (viewport.InteractionState == InteractionState.Interact2D)
			{
				resizeMode = ResizeMode.Manual;
				plotBounds.Expand(1 - factor);
				MakeDirty();
				return true;
			}
			else
				return false;
		}


        public override bool HandleZoom(Viewport viewport, RubberBand rubberBand)
        {
            if (viewport.InteractionState == InteractionState.Interact2D)
			{
				ResizeMode = ResizeMode.Manual;
				Vector min = viewport.Camera.ScreenToWorld(rubberBand.Min, false);
				Vector max = viewport.Camera.ScreenToWorld(rubberBand.Max, false);
				min = plotToWorldSpace.InverseApply(min);
				max = plotToWorldSpace.InverseApply(max);
				switch (viewport.Camera.LastDirection)
				{
				case ViewDirection.Front:
				case ViewDirection.Back:
					plotBounds.Minima[0] = min.X;
					plotBounds.Minima[2] = min.Z;
					plotBounds.Maxima[0] = max.X;
					plotBounds.Maxima[2] = max.Z;
					break;
				case ViewDirection.Top:
				case ViewDirection.Bottom:
					plotBounds.Minima[1] = min[1];
					plotBounds.Minima[2] = min[2];
					plotBounds.Maxima[1] = max[1];
					plotBounds.Maxima[2] = max[2];
					break;
				case ViewDirection.Left:
				case ViewDirection.Right:
					plotBounds.Minima[0] = min[0];
					plotBounds.Minima[1] = min[1];
					plotBounds.Maxima[0] = max[0];
					plotBounds.Maxima[1] = max[1];
					break;
				}
				MakeDirty();
                return true;
            }
            return false;
        }


		public override void OnButtonPress(MouseButtonEvent evt)
		{
			base.OnButtonPress(evt);
			foreach (Plottable child in children)
				child.OnButtonPress(evt);
		}

		public override void OnButtonRelease(MouseButtonEvent evt)
		{
			base.OnButtonRelease(evt);
			foreach (Plottable child in children)
				child.OnButtonRelease(evt);
		}

		public override void Deselect()
		{
			base.Deselect();

			foreach (Plottable child in children)
			{
				child.Deselect();
			}

		}

		public override string SelectionDescription
		{
			get
			{
				foreach (Plottable child in children)
				{
					if (child.IsSelected)
						return child.SelectionDescription;
				}
				return "";
			}
		}
		
#endregion

	}
}
