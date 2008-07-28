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

using MonoWorks.Rendering;

namespace MonoWorks.Plotting
{
	/// <summary>
	/// How the axes are arranged.
	/// </summary>
	public enum AxesArrangement {Origin, Closest, All, None};

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
			children.Add(plottable);
			plottable.Parent = this;
		}

		/// <summary>
		/// Removes a child.
		/// </summary>
		/// <param name="plottable"> A <see cref="Plottable"/> that is a child of the axes. </param>
		public void RemoveChild(Plottable plottable)
		{
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
				if (value != arrangement)
				{
					arrangement = value;

					// regenerate the axes
					int numAxes = 3;
					if (arrangement == AxesArrangement.All)
						numAxes = 12;
					axes.Clear();
					axes.Capacity = numAxes;
					for (int n = 0; n < numAxes; n++)
						axes.Add(new Axis(this));
				}
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
		/// Updates the axes based on the camera position and arrangement.
		/// </summary>
		/// <param name="viewport"> A <see cref="IViewport"/>. </param>
		protected void UpdateAxes(IViewport viewport)
		{
			switch (arrangement)
			{
			case AxesArrangement.Origin: // the axes should be placed at the lowest end of each range

				break;
			default:
				throw new Exception(String.Format("arrangement {0} not supported", arrangement));
			}
		}

		#endregion






		/// <summary>
		/// Computes the box geometry.
		/// </summary>
		public override void ComputeGeometry()
		{
			base.ComputeGeometry();

			// get the new plot bounds if automatically sized
			if (resizeMode == ResizeMode.Auto) // automatically resize
			{
				foreach (Plottable child in children)
					plotBounds.Resize(child.PlotBounds);
			}


			// force the children to recompute their bounds
			foreach (Plottable child in children)
			{
				child.ComputeGeometry();
			}

		}


	}
}
