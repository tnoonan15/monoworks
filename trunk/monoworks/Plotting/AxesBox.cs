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
	public enum AxesArrangement {Origin, Outside, All, None};

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

			Arrangement = AxesArrangement.Origin;
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
				if (arrangement == AxesArrangement.All)
					numAxes = 12;
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
		/// Updates the axes based on arrangement and bounds.
		/// </summary>
		protected void UpdateAxes()
		{
			switch (arrangement)
			{
			case AxesArrangement.Origin: // the axes should be placed at the lowest end of each range

				break;
			default:
				throw new Exception(String.Format("arrangement {0} not supported", arrangement));
			}
		}

		/// <summary>
		/// Updates the axes based on the camera position and arrangement, then renders them.
		/// </summary>
		/// <param name="viewport"> A <see cref="IViewport"/>. </param>
		protected void RenderAxes(IViewport viewport)
		{
			Console.WriteLine("rendering the axes");
			// update the positions
			switch (arrangement)
			{
			case AxesArrangement.Origin: // the axes should be placed at the lowest end of each range
				axes[0].Start = viewport.Camera.WorldToScreen(bounds.Minima);
				axes[0].Stop = viewport.Camera.WorldToScreen(new Vector(bounds.Maxima[0], bounds.Minima[1], bounds.Minima[2]));
				axes[1].Start = viewport.Camera.WorldToScreen(bounds.Minima);
				axes[1].Stop = viewport.Camera.WorldToScreen(new Vector(bounds.Minima[0], bounds.Maxima[1], bounds.Minima[2]));
				axes[2].Start = viewport.Camera.WorldToScreen(bounds.Minima);
				axes[2].Stop = viewport.Camera.WorldToScreen(new Vector(bounds.Minima[0], bounds.Minima[1], bounds.Maxima[2]));
				break;

			case AxesArrangement.Outside: // the axes should be placed along the oustide of the viewable area
				
				break;

			default:
				throw new Exception(String.Format("arrangement {0} not supported", arrangement));
			}

			// perform the rendering
			foreach (Axis axis in axes)
				axis.RenderOverlay(viewport);
		}

		#endregion



		#region Geometry

		protected Transform plotToRenderSpace = new Transform();
		/// <summary>
		/// Transformation to go from plot to render space.
		/// </summary>
		public Transform PlotToRenderSpace
		{
			get { return plotToRenderSpace; }
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
					foreach (Plottable child in children)
					{
						child.UpdateBounds();
						if (child.PlotBounds.IsSet)
							plotBounds.Resize(child.PlotBounds);
					}
				}

				if (plotBounds.IsSet) // only proceed if the plot bounds are set
				{
					// compute the plot-render transformation
					plotToRenderSpace.Compute(plotBounds, bounds);

					// force the children to recompute their bounds
					foreach (Plottable child in children)
					{
						child.ComputeGeometry();
					}
				}

			}

			// udpate the axes
			UpdateAxes();

		}

		#endregion



		#region Rendering

		public override void RenderOpaque(IViewport viewport)
		{
			base.RenderOpaque(viewport);


			//bounds.Render(viewport);

			gl.glColor3b(0, 0, 0);

			foreach (Plottable child in children)
				child.RenderOpaque(viewport);
			
		}

		public override void RenderOverlay(IViewport viewport)
		{
			base.RenderOverlay(viewport);


			// render the axes
			RenderAxes(viewport);

			//foreach (Plottable child in children)
			//    child.RenderOverlay(viewport);
		}

		#endregion

	}
}
