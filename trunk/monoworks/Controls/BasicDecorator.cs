// BasicDecorator.cs - MonoWorks Project
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
using System.Xml.Serialization;


using MonoWorks.Base;
using MonoWorks.Rendering;
using MonoWorks.Rendering.Events;


namespace MonoWorks.Controls
{
	
	/// <summary>
	/// Provides a basic decorator that should be good enough for most situations.
	/// </summary>
	[XmlRoot]
	public class BasicDecorator : AbstractDecorator
	{
		
		public BasicDecorator() : base()
		{
			DefaultBackgroundLocation = AnchorLocation.SE;
			
			BackgroundStartColors = new ColorGroup(
			                  			new Color(1f, 1f, 1f, 0.6f),
			                  			new Color(1f, 1f, 1f, 0.9f),
			                  			new Color(0.9f, 0.9f, 0.9f, 0.5f)
			                        );
			BackgroundStopColors = new ColorGroup(
			                  			new Color(1f, 1f, 1f, 0.9f),
			                  			new Color(1f, 1f, 1f, 0.6f),
			                  			new Color(0.9f, 0.9f, 1f, 0.5f)
			                        );
		}
		
		
		
#region Background Rendering
		
		/// <value>
		/// The orientation used to render the background if the decorator doesn't override it.
		/// </value>
		public AnchorLocation DefaultBackgroundLocation {get; set;}
		
		/// <summary>
		/// The color group for the start of the background gradient.
		/// </summary>
		public ColorGroup BackgroundStartColors;
		
		/// <summary>
		/// The color group for the stop of the background gradient.
		/// </summary>
		public ColorGroup BackgroundStopColors;
		
		/// <summary>
		/// The default background color if none can be found.
		/// </summary>
		protected Color defaultBackgroundColor = new Color(0.5f, 0.5f, 0.5f);
				
		/// <summary>
		/// Creates a gradient for the given position, size, and anchor location.
		/// </summary>
		protected Cairo.LinearGradient GenerateGradient(Coord size, AnchorLocation location, 
		                                                Color startColor, Color stopColor)
		{
			Cairo.LinearGradient grad = null;
			switch (location)
			{
			case AnchorLocation.E:
				grad = new Cairo.LinearGradient(size.X, 0, 0, 0);
				break;
			case AnchorLocation.NE:
				grad = new Cairo.LinearGradient(size.X, 0, 0, size.Y);
				break;
			case AnchorLocation.N:
				grad = new Cairo.LinearGradient(0, 0, 0, size.Y);
				break;
			case AnchorLocation.NW:
				grad = new Cairo.LinearGradient(0, 0, size.X, size.Y);
				break;
			case AnchorLocation.W:
				grad = new Cairo.LinearGradient(0, 0, size.X, 0);
				break;
			case AnchorLocation.SW:
				grad = new Cairo.LinearGradient(0, size.Y, size.X, 0);
				break;
			case AnchorLocation.S:
				grad = new Cairo.LinearGradient(0, size.Y, 0, 0);
				break;
			case AnchorLocation.SE:
				grad = new Cairo.LinearGradient(size.X, size.Y, 0, 0);
				break;
			}
			
			// assign the colors
			if (startColor == null)
				startColor = defaultBackgroundColor;
			grad.AddColorStop(0, startColor.Cairo);
			if (stopColor == null)
				stopColor = defaultBackgroundColor;
			grad.AddColorStop(1, stopColor.Cairo);
			
			return grad;
		}
		
		/// <summary>
		/// Renders a rectangle portion of a control.
		/// </summary>
		protected void FillRectangle(Coord size, HitState hitState, AnchorLocation location)
		{			
			var point = Context.Push();
			
			// Create the gradient
			Cairo.LinearGradient grad = GenerateGradient(size, location, 
			                                             BackgroundStartColors[hitState],
			                                             BackgroundStopColors[hitState]);
			// draw the rectangle
			Context.Cairo.Operator = Cairo.Operator.Source;
			Context.Cairo.Pattern = grad;
			Context.Cairo.Rectangle(point, size.X, size.Y);
			Context.Cairo.Fill();
			
			Context.Pop();
		}
		
		/// <summary>
		/// Renders a triangle portion of a control.
		/// </summary>
		protected void FillTriangle(Coord size, HitState hitState, AnchorLocation triDirection, AnchorLocation gradDirection)
		{
			Context.Push();
			
			// Create the gradient
			Cairo.LinearGradient grad = GenerateGradient(size / 2.0, gradDirection, 
			                                             BackgroundStartColors[hitState],
			                                             BackgroundStopColors[hitState]);
			// draw the triangle
			Context.Cairo.Operator = Cairo.Operator.Source;
			Context.Cairo.Pattern = grad;
			switch (triDirection)
			{
			case AnchorLocation.NE:
				Context.Cairo.RelLineTo(size.X, 0);
				Context.Cairo.RelLineTo(0, size.Y);
				break;
			case AnchorLocation.NW:
				Context.Cairo.RelLineTo(size.X, 0);
				Context.Cairo.RelLineTo(-size.X, size.Y);
				break;
			case AnchorLocation.SE:
				Context.Cairo.MoveTo(size.X, 0);
				Context.Cairo.RelLineTo(0, size.Y);
				Context.Cairo.RelLineTo(-size.X, 0);
				break;
			case AnchorLocation.SW:
				Context.Cairo.RelLineTo(0, size.Y);
				Context.Cairo.RelLineTo(size.X, 0);
				break;
			case AnchorLocation.N:
				Context.Cairo.RelMoveTo(0, size.Y);
				Context.Cairo.RelLineTo(size.X, 0);
				Context.Cairo.RelLineTo(-size.X/2.0, size.Y);
				break;
			case AnchorLocation.E:
				Context.Cairo.RelLineTo(0, size.Y);
				Context.Cairo.RelLineTo(size.X, -size.Y/2.0);
				break;
			case AnchorLocation.S:
				Context.Cairo.RelLineTo(size.X, 0);
				Context.Cairo.RelLineTo(-size.X/2.0, size.Y);
				break;
			case AnchorLocation.W:
				Context.Cairo.RelMoveTo(size.X, 0);
				Context.Cairo.RelLineTo(0, size.Y);
				Context.Cairo.RelLineTo(-size.X, -size.Y/2.0);
				break;
			default:
				throw new Exception(triDirection.ToString() + " is currently not a valid triangle location");
			}
			Context.Cairo.Fill();
			
			Context.Pop();
		}
		
#endregion
		
		
#region Decorating
		
		public override void Decorate(Control2D control)
		{			
			if (control is CornerButtons)
			{
				Decorate(control as CornerButtons);
				return;
			}
			
			if (!(control is Button || control is ToolBar))
				return;
			
			if (control.Pane != null && control.Pane is AnchorPane) // the control is anchored
			{
				if (!(control is Button) || control.HitState != HitState.None)
					FillRectangle(control.RenderSize, control.HitState, (control.Pane as AnchorPane).Location);
			}
			else // the control is not anchored
			{
				FillRectangle(control.RenderSize, control.HitState, DefaultBackgroundLocation);
			}
		}
		
		/// <summary>
		/// Decorates a corner button.
		/// </summary>
		protected void Decorate(CornerButtons control)
		{
//			Console.WriteLine("decorating corner buttons with HitState1 {0} and HitState2 {1}", 
//			                  control.HitState1, control.HitState2);
			
			// define the sizes for the horizontal and vertical triangles that make up each button
			var horiSize = new Coord(control.RenderWidth, control.RenderHeight/2.0);
			var vertSize = new Coord(control.RenderWidth/2.0, control.RenderHeight);
			
			// get the colors used to create the gradients
//			if (control.HitState1 == HitState.None && control.HitState2 == HitState.None) // do everything as one big triangle
//			{
//				FillTriangle(control.Size, control.HitState, (AnchorLocation)control.Corner,(AnchorLocation)control.Corner);
//				return;
//			}
			
			
			Context.Push();
			switch (control.Corner)
			{
			case Corner.NE:
				FillTriangle(horiSize, control.HitState1, AnchorLocation.S, (AnchorLocation)control.Corner);
				Context.Cairo.RelMoveTo(control.RenderSize.X / 2.0, 0);
				FillTriangle(vertSize, control.HitState2, AnchorLocation.W, (AnchorLocation)control.Corner);
				break;
			case Corner.NW:
				FillTriangle(horiSize, control.HitState1, AnchorLocation.S, (AnchorLocation)control.Corner);
				FillTriangle(vertSize, control.HitState2, AnchorLocation.E, (AnchorLocation)control.Corner);
				break;
			}
			Context.Pop();
		}
		
#endregion
		
	}
}
