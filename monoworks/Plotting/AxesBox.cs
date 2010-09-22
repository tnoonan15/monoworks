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
using MonoWorks.Controls;

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
			_bounds.Minima = new Vector(-1, -1, -1);
			_bounds.Maxima = new Vector(1, 1, 1);

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

			titlePane = new LabelPane();
//			titleDef.HorizontalAlignment = HorizontalAlignment.Center;
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

			_bounds.Minima = new Vector(-1, -1, -1);
			_bounds.Maxima = new Vector(1, 1, 1);

			ResizeMode = ResizeMode.Auto;
		}

		
		public override void OnSceneResized(Scene scene)
		{
			base.OnSceneResized(scene);
			
			if (scene.Use2dInteraction)
			{
				double edgeFactor = 0.35 * scene.Camera.SceneToWorldScaling;
				if (double.IsInfinity(edgeFactor))
					return;
				Vector center = scene.Camera.Center;
//				Console.WriteLine("edge scaling: {0}, center: {1}, width: {2}, height: {3}", edgeFactor, center, scene.Width, scene.Height);
				switch (scene.Camera.LastDirection)
				{
				case ViewDirection.Front:
				case ViewDirection.Back:
					_bounds.Minima[0] = center[0] - edgeFactor * scene.Width;
					_bounds.Maxima[0] = center[0] + edgeFactor * scene.Width;
					_bounds.Minima[2] = center[2] - edgeFactor * scene.Height;
					_bounds.Maxima[2] = center[2] + edgeFactor * scene.Height;
					break;
				case ViewDirection.Left:
				case ViewDirection.Right:
					_bounds.Minima[1] = center[1] - edgeFactor * scene.Width;
					_bounds.Maxima[1] = center[1] + edgeFactor * scene.Width;
					_bounds.Minima[2] = center[2] - edgeFactor * scene.Height;
					_bounds.Maxima[2] = center[2] + edgeFactor * scene.Height;
					break;
				case ViewDirection.Top:
				case ViewDirection.Bottom:
					_bounds.Minima[0] = center[0] - edgeFactor * scene.Width;
					_bounds.Maxima[0] = center[0] + edgeFactor * scene.Width;
					_bounds.Minima[1] = center[1] - edgeFactor * scene.Height;
					_bounds.Maxima[1] = center[1] + edgeFactor * scene.Height;
					break;
				}
			}

			foreach (Grid grid in grids)
				grid.MakeDirty();
			MakeDirty();
		}

		
		public override void OnViewDirectionChanged(Scene scene)
		{
			base.OnViewDirectionChanged(scene);
			
			// reset the resizing mode
			resizeMode = ResizeMode.Auto;
			
			OnSceneResized(scene);
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
			set
			{
				resizeMode = value;
				MakeDirty();
			}
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
		/// <param name="scene"> A <see cref="Scene"/>. </param>
		protected void RenderAxes(Scene scene)
		{

			// perform the rendering
			foreach (Axis axis in axes)
				axis.RenderOverlay(scene);
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
		protected void RenderGrids(Scene scene)
		{
			// update axes the positions
			Vector corner = null;
			switch (arrangement)
			{
			case AxesArrangement.Origin: // the axes should be placed at the lowest end of each range
				axes[0].Start = _bounds.Minima;
				axes[1].Start = axes[0].Start;
				axes[2].Start = axes[0].Start;
				axes[0].Stop = new Vector(_bounds.Maxima[0], _bounds.Minima[1], _bounds.Minima[2]);
				axes[1].Stop = new Vector(_bounds.Minima[0], _bounds.Maxima[1], _bounds.Minima[2]);
				axes[2].Stop = new Vector(_bounds.Minima[0], _bounds.Minima[1], _bounds.Maxima[2]);
				corner = new Vector(_bounds.Maxima[0], _bounds.Minima[1], _bounds.Minima[2]);
				if (lastCorner == null || corner != lastCorner)
				{
					foreach (Grid grid in grids)
						grid.Corner = corner;
					lastCorner = corner;
				}
				break;

			case AxesArrangement.Outside: // the axes should be placed along the oustide of the viewable area
				Vector[] edges = _bounds.GetOutsideEdges(scene);
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
				grid.RenderOpaque(scene);

			// tell the axes about the corner
			Coord center = scene.Camera.WorldToScreen(corner);
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

			// clear the legend
			if (Legend != null)
				Legend.Clear();

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
					_plotBounds.Reset();
					foreach (Plottable child in children)
					{
						child.UpdateBounds();
						if (child.PlotBounds.IsSet)
							_plotBounds.Resize(child.PlotBounds);
					}

					// make the plot bounds pretty
					_plotBounds.Prettify();

					// ensure all dimensions have non-zero span
					_plotBounds.EnsureNonZeroRanges();
				}

			}
			else // no plot children
			{
				_plotBounds.Minima = new Vector(-1, -1, -1);
				_plotBounds.Maxima = new Vector(1, 1, 1);
			}

			// compute the plot-render transformation
			plotToWorldSpace.Compute(_plotBounds, _bounds);

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


		protected LabelPane titlePane;
		/// <summary>
		/// The title.
		/// </summary>
		public override string Title
		{
			get { return titlePane.Label.Body; }
			set { titlePane.Label.Body = value; }
		}

		/// <summary>
		/// Renders the title.
		/// </summary>
		protected void RenderTitle(Scene scene)
		{
			// determine the top and center of the bounds
			double top = 0;
			double left = 0;
			double right = 0;
			foreach (Vector corner in _bounds.Corners)
			{
				Coord coord = scene.Camera.WorldToScreen(corner);
				if (Array.IndexOf(_bounds.Corners, corner)==0) // the first one
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
			
			titlePane.Origin = new Vector((left+right)/2, top + 32, 0);
			titlePane.RenderOverlay(scene);
		}

#endregion


#region Rendering
		
		/// <summary>
		/// Enables clipping for the content rendering.
		/// </summary>
		/// <param name="scene"> A <see cref="Scene"/>. </param>
		protected void EnableClipping(Scene scene)
		{			
			// define the clip planes
			double[] eq = new double[]{1, 0, 0, _bounds.Maxima[0]};
			gl.glClipPlane(gl.GL_CLIP_PLANE0, eq);
			eq = new double[]{-1, 0, 0, -_bounds.Minima[0]};
			gl.glClipPlane(gl.GL_CLIP_PLANE1, eq);
			eq = new double[]{0, 1, 0, _bounds.Maxima[1]};
			gl.glClipPlane(gl.GL_CLIP_PLANE2, eq);
			eq = new double[]{0, -1, 0, -_bounds.Minima[1]};
			gl.glClipPlane(gl.GL_CLIP_PLANE3, eq);
			eq = new double[]{0, 0, 1, _bounds.Maxima[2]};
			gl.glClipPlane(gl.GL_CLIP_PLANE4, eq);
			eq = new double[]{0, 0, -1, -_bounds.Minima[2]};
			gl.glClipPlane(gl.GL_CLIP_PLANE5, eq);
			
			for (int i=0; i<6; i++)
				gl.glEnable(gl.GL_CLIP_PLANE0 + i);
			
			// disable clipping for planes orthagonal to view direction
			if (scene.Use2dInteraction)
			{
				switch (scene.Camera.LastDirection)
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
		/// <param name="scene"> A <see cref="Scene"/>. </param>
		protected void DisableClipping(Scene scene)
		{
			for (int i=0; i<6; i++)
				gl.glDisable(gl.GL_CLIP_PLANE0 + i);
		}

		public override void RenderOpaque(Scene scene)
		{
			base.RenderOpaque(scene);

			if (!IsVisible)
				return;

			gl.glColor3b(0, 0, 0);
			
			// render the grids
			RenderGrids(scene);
			
			// enable clipping
			EnableClipping(scene);
			
			// render the children
			foreach (var child in GetChildren<AbstractPlot>())
				child.RenderOpaque(scene);
			
			// disable clipping
			DisableClipping(scene);
		}

		public override void RenderOverlay(Scene scene)
		{
			base.RenderOverlay(scene);

			if (!IsVisible)
				return;

			// render the axes
			RenderAxes(scene);

			// render the title
			RenderTitle(scene);

			// render the child overlays
			foreach (var child in GetChildren<AbstractPlot>())
				child.RenderOverlay(scene);
		}

#endregion
		
		
#region Mouse Interaction
		
		/// <summary>
		/// If in 2D mode, translate the axes limits and prevent the scene from handling the interaction.
		/// </summary>
		public override bool HandlePan(Scene scene, double dx, double dy)
		{			
			if (scene.Use2dInteraction)
			{
				resizeMode = ResizeMode.Manual;
				// determine the difference to apply to the axes ranges
				Vector diff = (scene.Camera.RightVec * dx - scene.Camera.UpVector * dy) * scene.Camera.SceneToWorldScaling; 
				_plotBounds.Translate(diff / plotToWorldSpace.Scaling);
				MakeDirty();
				return true;
			}
			else
				return false;
		}
		
		public override bool HandleDolly(Scene scene, double factor)
		{
			if (scene.Use2dInteraction)
			{
				resizeMode = ResizeMode.Manual;
				_plotBounds.Expand(1 - factor);
				MakeDirty();
				return true;
			}
			else
				return false;
		}


        public override bool HandleZoom(Scene scene, RubberBand rubberBand)
        {
            if (scene.Use2dInteraction)
			{
				ResizeMode = ResizeMode.Manual;
				Vector min = scene.Camera.ScreenToWorld(rubberBand.Min, false);
				Vector max = scene.Camera.ScreenToWorld(rubberBand.Max, false);
				min = plotToWorldSpace.InverseApply(min);
				max = plotToWorldSpace.InverseApply(max);
				switch (scene.Camera.LastDirection)
				{
				case ViewDirection.Front:
				case ViewDirection.Back:
					_plotBounds.Minima[0] = min.X;
					_plotBounds.Minima[2] = min.Z;
					_plotBounds.Maxima[0] = max.X;
					_plotBounds.Maxima[2] = max.Z;
					break;
				case ViewDirection.Left:
				case ViewDirection.Right:
					_plotBounds.Minima[1] = min[1];
					_plotBounds.Minima[2] = min[2];
					_plotBounds.Maxima[1] = max[1];
					_plotBounds.Maxima[2] = max[2];
					break;
				case ViewDirection.Top:
				case ViewDirection.Bottom:
					_plotBounds.Minima[0] = min[0];
					_plotBounds.Minima[1] = min[1];
					_plotBounds.Maxima[0] = max[0];
					_plotBounds.Maxima[1] = max[1];
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


#region Legend

		/// <summary>
		/// The legend that the axes populates when it computes geometry.
		/// </summary>
		public Legend Legend { get; set; }

#endregion


	}
}
