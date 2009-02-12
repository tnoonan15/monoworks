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
			
			label = new TextDef(12);
			label.Text = "label";
			label.HorizontalAlignment = HorizontalAlignment.Center;
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

		
#region Label

		protected TextDef label;
		
		/// <value>
		/// The axis label.
		/// </value>
		public string Label
		{
			get {return label.Text;}
			set {label.Text = value;}
		}
		
#endregion
		

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

		protected TextDef[] tickLabels;
		/// <summary>
		/// The labels on the ticks.
		/// </summary>
		public TextDef[] TickLabels
		{
			get { return tickLabels; }
		}


		protected int dimension;
		/// <summary>
		/// The dimension that this axis represents.
		/// </summary>
		public int Dimension
		{
			get {return dimension;}
		}

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
			
			// get the tick values
			tickVals = Bounds.NiceRange(min, max, true);

			// store the tick labels
			tickLabels = new TextDef[tickVals.Length];
			for (int i = 0; i < tickVals.Length; i++)
			{
				tickLabels[i] = new TextDef(11);
				tickLabels[i].Text = String.Format("{0:0.###}", tickVals[i]);
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
		
		/// <value>
		/// First tick step in world coords.
		/// </value>
		public double FirstWorldTickStep
		{
			get
			{
				double step = tickVals[0] - parent.PlotBounds.Minima[dimension];
				return parent.PlotToWorldSpace.Scaling[dimension] * step; // the step in world coordinates
			}
		}	
		
		/// <value>
		/// Tick step (everything but first) in world coords.
		/// </value>
		public double WorldTickStep
		{
			get
			{
				double step = tickVals[1] - tickVals[0];
				return parent.PlotToWorldSpace.Scaling[dimension] * step; // the step in world coordinates
			}
		}		

		/// <summary>
		/// Computes the tick positions based on the start and stop vectors.
		/// </summary>
		protected void ComputeTickPositions()
		{
			if (ticksDirty) // only do this if the ticks are dirty
			{
				// compute the step
				double worldStep = WorldTickStep;

				// compute the tick positions
				tickPositions = new Vector[tickVals.Length];
				tickPositions[0] = start.Copy();
				tickPositions[0][dimension] += FirstWorldTickStep;
				for (int i = 1; i < tickVals.Length; i++)
				{
					tickPositions[i] = tickPositions[i-1].Copy();
					tickPositions[i][dimension] += worldStep;
				}
				ticksDirty = false;
			}
		}

#endregion



		public override void RenderOverlay(Viewport viewport)
		{
			base.RenderOverlay(viewport);

			if (!IsVisible)
				return;

			// get the screen coordinates of the axis
			Coord startCoord = viewport.Camera.WorldToScreen(start);
			Coord stopCoord = viewport.Camera.WorldToScreen(stop);

			// flip the coordinates around if they aren't in x order (this helps with drawing ticks)
			if (startCoord.X > stopCoord.X)
			{
				Coord dummy = stopCoord;
				stopCoord = startCoord;
				startCoord = dummy;
			}

			// the main axis line
			// RIGHT NOW, THIS IS COLLIDING WITH THE GRID AND LOOKS FUNNY
//			gl.glVertex2i(startCoord.X, startCoord.Y);
//			gl.glVertex2i(stopCoord.X, stopCoord.Y);

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
			
			// render the axis label
			double labelOffset = 80;
			Coord labelPos = (startCoord + stopCoord) / 2;
			label.Position = labelPos + tickAngle.ToCoord() * labelOffset;
			if (label.Text.Length > 4 && tickAngle.ToCoord().Orientation == Orientation.Horizontal)
				label.Angle = Angle.Pi() / 2;
			else
				label.Angle = new Angle();
			viewport.RenderText(label);

			// update tick alignment
			HorizontalAlignment newAlignment = HorizontalAlignment.Center;
			//if (tickAngle 

			// render the ticks
			gl.glBegin(gl.GL_LINES);
			for (int i = 0; i < tickPositions.Length; i++)
			{
				startCoord = viewport.Camera.WorldToScreen(tickPositions[i]);
				gl.glVertex2d(startCoord.X, startCoord.Y); // the point where the tick intersects the axis
				gl.glVertex2d(startCoord.X + tickLength * tickAngle.Cos(), startCoord.Y + tickLength * tickAngle.Sin()); //the other point 

				// store the label position
				double labelFactor = 3;
				if (tickAngle.ToCoord().Orientation == Orientation.Horizontal)
					labelFactor = 4;
				tickLabels[i].Position = startCoord + tickAngle.ToCoord() * labelFactor * tickLength;
			}

			gl.glEnd();

			// render the tick labels
			for (int i = 0; i < tickPositions.Length; i++)
				tickLabels[i].HorizontalAlignment = newAlignment;
			viewport.RenderText(tickLabels);
			
		}
	

	}
}
