// Axes.cs - MonoWorks Project
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
	/// A single axis (a part of an axes box).
	/// </summary>
	public class Axis : Plottable
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		/// <param name="parent"></param>
		public Axis(AxesBox parent)
			: base(parent)
		{
			tickLength = 8;
		}


		protected Vector start = new Vector();
		/// <summary>
		/// The starting position of the axis.
		/// </summary>
		public Vector Start
		{
			get { return start; }
			set { start = value; }
		}

		protected Vector stop = new Vector();
		/// <summary>
		/// The stopping position of the axes.
		/// </summary>
		public Vector Stop
		{
			get { return stop; }
			set { stop = value; }
		}



#region Ticks

		protected double tickLength;
		/// <summary>
		/// The length of the drawn tick.
		/// </summary>
		public double TickLength
		{
			get { return tickLength; }
			set { tickLength = value; }
		}


		protected double[] tickVals;
		/// <summary>
		/// The values of the ticks in plot space.
		/// </summary>
		public double[] TickVals
		{
			get { return tickVals; }
		}

		protected TextRenderer[] tickLabels;
		/// <summary>
		/// The labels on the ticks.
		/// </summary>
		public TextRenderer[] TickLabels
		{
			get { return tickLabels; }
		}


		/// <summary>
		/// The dimension that this axis represents.
		/// </summary>
		protected int dimension;

		/// <summary>
		/// The positions of the ticks in 3D world space.
		/// </summary>
		protected Vector[] tickPositions;

		/// <summary>
		/// Automatically generates the ticks based on the parent's plot bounds.
		/// </summary>
		/// <param name="dim"> The dimension that this axis represents.</param>
		public void GenerateTicks(int dim)
		{
			dimension = dim;
			double min = parent.PlotBounds.Minima[dim];
			double max = parent.PlotBounds.Maxima[dim];
			double range = max- min;
			double step = Bounds.NiceStep(min, max);

			// determine the number of ticks
			double numTicks = Math.Floor(range / step); // number of ticks
			if (numTicks < 1) // this should never happen, there must be an error in NiceStep()
				throw new Exception("The number of ticks is less than one. There might be and error in Bounds.NiceStep().");

			// compute the tick values
			tickVals = new double[(int)numTicks];
			tickVals[0] = Math.Ceiling(min / step) * step;
			for (int i = 1; i < (int)numTicks; i++)
				tickVals[i] = tickVals[i - 1] + step;

			// store the tick labels
			tickLabels = new TextRenderer[tickVals.Length];
			for (int i = 0; i < (int)numTicks; i++)
			{
				tickLabels[i] = new TextRenderer(10);
				tickLabels[i].Text = tickVals[i].ToString();
			}
		}


		protected bool ticksDirty = true;
		/// <summary>
		/// Forces the ticks to be dirty.
		/// </summary>
		public void DirtyTicks()
		{
			ticksDirty = true;
		}

		/// <summary>
		/// Computes the tick positions based on the start and stop vectors.
		/// </summary>
		protected void ComputeTickPositions()
		{
			if (ticksDirty) // only do this if the ticks are dirty
			{
				// compute the step
				double step = tickVals[1] - tickVals[0];
				double worldStep = parent.PlotToWorldSpace.Scaling[dimension] * step; // the step in world coordinates

				// compute the tick positions
				tickPositions = new Vector[tickVals.Length];
				for (int i = 0; i < tickVals.Length; i++)
				{
					tickPositions[i] = start.Copy();
					tickPositions[i][dimension] += i * worldStep;
				}
				ticksDirty = false;
			}
		}

#endregion



		public override void RenderOverlay(IViewport viewport)
		{
			base.RenderOverlay(viewport);

			// get the screen coordinates of the axis
			ScreenCoord startCoord = viewport.Camera.WorldToScreen(start);
			ScreenCoord stopCoord = viewport.Camera.WorldToScreen(stop);

			// flip the coordinates around if they aren't in x order (this helps with drawing ticks)
			if (startCoord.X > stopCoord.X)
			{
				ScreenCoord dummy = stopCoord;
				stopCoord = startCoord;
				startCoord = dummy;
			}

			gl.glBegin(gl.GL_LINES);

			// the main axis line
			gl.glVertex2i(startCoord.X, startCoord.Y);
			gl.glVertex2i(stopCoord.X, stopCoord.Y);

			// get the angle of the main axis line
			bool isVertical = false, ishorizontal = false;
			Angle tickAngle = new Angle();
			if (startCoord.X == stopCoord.X) // the axis is vertical
			{
				isVertical = true;
				if (startCoord.X < viewport.WidthGL / 2) // it's on the left side
					tickAngle = Angle.Pi();
				else
					tickAngle = new Angle();
				//Console.WriteLine("axis {0} is vertical with angle {1}", dimension, tickAngle);
			}
			if (startCoord.Y == stopCoord.Y) // the axis is horizontal
			{
				ishorizontal = true;
				if (startCoord.Y < viewport.HeightGL / 2) // it's on the bottom side
					tickAngle = Angle.Pi() * 1.5;
				else
					tickAngle = Angle.Pi() * 0.5;
				//Console.WriteLine("axis {0} is horizontal with angle {1}", dimension, tickAngle);
			}
			if (ishorizontal && isVertical) // the axis is singular, don't render it
				return;
			if (!ishorizontal && !isVertical) // the axis is sloped
			{
				tickAngle = Angle.ArcTan(stopCoord.Y - startCoord.Y, stopCoord.X - startCoord.X);
				//Console.WriteLine("axis {0} goes from {1} to {2} with tick angle {3}", dimension, startCoord, stopCoord, tickAngle * 180 / Math.PI);

				// determine which direction to flip it
				double sign = 1;
				if (dimension == 2) // need to flip the sign on the z axis
					sign = -1;
				if (startCoord.Y < viewport.HeightGL / 2) // it's on the bottom side
					tickAngle -= new Angle(sign * Math.PI / 2.0);
				else
					tickAngle += new Angle(sign * Math.PI / 2.0);

			}

			// compute tick position
			ComputeTickPositions();

			// update tick alignment
			ISE.FTFontAlign newAlignment = ISE.FTFontAlign.FT_ALIGN_CENTERED;
			//if (tickAngle 

			// the ticks
			for (int i = 0; i < tickPositions.Length; i++)
			{
				startCoord = viewport.Camera.WorldToScreen(tickPositions[i]);
				gl.glVertex2d(startCoord.X, startCoord.Y); // the point where the tick intersects the axis
				gl.glVertex2d(startCoord.X + tickLength * tickAngle.Cos(), startCoord.Y + tickLength * tickAngle.Sin()); //the other point 



				// store the label position
				tickLabels[i].Position[0] = startCoord.X + 2 * tickLength * tickAngle.Cos();
				tickLabels[i].Position[1] = startCoord.Y + 2 * tickLength * tickAngle.Sin();
			}

			gl.glEnd();

			// render the labels
			for (int i = 0; i < tickPositions.Length; i++)
			{
				tickLabels[i].Alignment = newAlignment;
				tickLabels[i].RenderOverlay(viewport);
			}
		}
	

	}
}
