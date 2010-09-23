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
using MonoWorks.Controls;

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
			_tickLength = 8;

			_labelPane = new LabelPane() {
				OriginLocation = AnchorLocation.Center
			};
			_labelPane.Label.Body = "label";

			Start = new Vector();
			Stop = new Vector();
		}


		/// <summary>
		/// The starting position of the axis.
		/// </summary>
		public Vector Start { get; set; }

		/// <summary>
		/// The stopping position of the axes.
		/// </summary>
		public Vector Stop { get; set; }

		/// <summary>
		/// The center of the axes box as it's rendered on the screen.
		/// </summary>
		/// <remarks>This is used to determine where to place the tick marks.</remarks>
		public Coord AxesCenter { get; set; }
		
		
#region Label

		protected LabelPane _labelPane;
		
		/// <value>
		/// The axis label.
		/// </value>
		public string Label
		{
			get {return _labelPane.Label.Body;}
			set {_labelPane.Label.Body = value;}
		}
		
#endregion
		

#region Ticks

		protected double _tickLength;
		/// <summary>
		/// The length of the drawn tick.
		/// </summary>
		public double TickLength
		{
			get { return _tickLength; }
			set { _tickLength = value; }
		}


		protected double[] _tickVals;
		/// <summary>
		/// The values of the ticks in plot space.
		/// </summary>
		public double[] TickVals
		{
			get { return _tickVals; }
		}

		protected LabelPane[] _tickLabels;
		/// <summary>
		/// The labels on the ticks.
		/// </summary>
		public LabelPane[] TickLabels
		{
			get { return _tickLabels; }
		}


		protected int _dimension;
		/// <summary>
		/// The dimension that this axis represents.
		/// </summary>
		public int Dimension
		{
			get {return _dimension;}
		}

		/// <summary>
		/// The positions of the ticks in 3D world space.
		/// </summary>
		protected Vector[] _tickPositions;

		/// <summary>
		/// Automatically generates the ticks based on the parent's plot bounds.
		/// </summary>
		/// <param name="dim"> The dimension that this axis represents.</param>
		public void GenerateTicks(int dim)
		{
			_dimension = dim;
			double min = ParentAxes.PlotBounds.Minima[dim];
			double max = ParentAxes.PlotBounds.Maxima[dim];
			
			// get the tick values
			_tickVals = Bounds.NiceRange(min, max, true);

			// store the tick labels
			_tickLabels = new LabelPane[_tickVals.Length];
			for (int i = 0; i < _tickVals.Length; i++)
			{
				_tickLabels[i] = new LabelPane() {
					OriginLocation = AnchorLocation.Center
				};
				_tickLabels[i].Label.Body = String.Format("{0:0.###}", _tickVals[i]);
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
				double step = _tickVals[0] - ParentAxes.PlotBounds.Minima[_dimension];
				return ParentAxes.PlotToWorldSpace.Scaling[_dimension] * step; // the step in world coordinates
			}
		}	
		
		/// <value>
		/// Tick step (everything but first) in world coords.
		/// </value>
		public double WorldTickStep
		{
			get
			{
				double step = _tickVals[1] - _tickVals[0];
				return ParentAxes.PlotToWorldSpace.Scaling[_dimension] * step; // the step in world coordinates
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
				_tickPositions = new Vector[_tickVals.Length];
				_tickPositions[0] = Start.Copy();
				_tickPositions[0][_dimension] += FirstWorldTickStep;
				for (int i = 1; i < _tickVals.Length; i++)
				{
					_tickPositions[i] = _tickPositions[i-1].Copy();
					_tickPositions[i][_dimension] += worldStep;
				}
				ticksDirty = false;
			}
		}

		#endregion


		#region Rendering

		public override void RenderOverlay(Scene scene)
		{
			base.RenderOverlay(scene);

			if (!IsVisible)
				return;

			// get the screen coordinates of the axis
			Coord startCoord = scene.Camera.WorldToScreen(Start);
			Coord stopCoord = scene.Camera.WorldToScreen(Stop);

			// flip the coordinates around if they aren't in x order (this helps with drawing ticks)
			if (startCoord.X > stopCoord.X)
			{
				Coord dummy = stopCoord;
				stopCoord = startCoord;
				startCoord = dummy;
			}

			// the main axis line
			// RIGHT NOW, THIS IS COLLIDING WITH THE GRID AND LOOKS FUNNY
			gl.glBegin(gl.GL_LINES);
			gl.glVertex2d(startCoord.X, startCoord.Y);
			gl.glVertex2d(stopCoord.X, stopCoord.Y);
			gl.glEnd();

			// get the angle of the main axis line
			bool isVertical = false, ishorizontal = false;
			Angle tickAngle = new Angle();
			if (startCoord.X == stopCoord.X) // the axis is vertical
			{
				isVertical = true;
				if (startCoord.X < scene.Width / 2) // it's on the left side
					tickAngle = Angle.Pi();
				else
					tickAngle = new Angle();
			}
			if (startCoord.Y == stopCoord.Y) // the axis is horizontal
			{
				ishorizontal = true;
				if (startCoord.Y < scene.Height / 2) // it's on the bottom side
					tickAngle = Angle.Pi() * 1.5;
				else
					tickAngle = Angle.Pi() * 0.5;
			}
			if (ishorizontal && isVertical) // the axis is singular, don't render it
				return;
			if (!ishorizontal && !isVertical) // the axis is sloped
			{
				tickAngle = Angle.ArcTan(stopCoord.Y - startCoord.Y, stopCoord.X - startCoord.X);
				
				// determine which direction to flip it
				Angle angleToCenter = (stopCoord - startCoord).AngleTo(AxesCenter-startCoord);
				if (angleToCenter.Value >= 0)
					tickAngle.DecByHalfPi();
				else
					tickAngle.IncByHalfPi();
			}

			// compute tick position
			ComputeTickPositions();
			
			// render the axis label
			double labelOffset = 70;
			Coord labelPos = (startCoord + stopCoord) / 2;
			_labelPane.Origin = labelPos + tickAngle.ToCoord() * labelOffset;
			//if (_labelPane.Label.Body.Length > 5 && tickAngle.ToCoord().Orientation == Orientation.Horizontal)
			//    _labelPane.Angle = Angle.Pi() / 2;
			//else
			//    _labelPane.Angle = new Angle();
			_labelPane.RenderOverlay(scene);

			// render the ticks
			gl.glBegin(gl.GL_LINES);
			for (int i = 0; i < _tickPositions.Length; i++)
			{
				startCoord = scene.Camera.WorldToScreen(_tickPositions[i]);
				gl.glVertex2d(startCoord.X, startCoord.Y); // the point where the tick intersects the axis
				gl.glVertex2d(startCoord.X + _tickLength * tickAngle.Cos(), startCoord.Y + _tickLength * tickAngle.Sin()); //the other point 

				// store the label position
				double labelFactor = 3;
				if (tickAngle.ToCoord().Orientation == Orientation.Horizontal)
					labelFactor = 4;
				_tickLabels[i].Origin = startCoord + tickAngle.ToCoord() * labelFactor * _tickLength;
			}
			gl.glEnd();

			// render the tick labels
			for (int i = 0; i < _tickPositions.Length; i++)
			{
				_tickLabels[i].RenderOverlay(scene);
			}


		}

		#endregion

	}
}
