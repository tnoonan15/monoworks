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
			
			StrokeColors = new ColorGroup(
				new Color(0.5f, 0.5f, 0.5f),
				new Color(0.5f, 0.5f, 0.5f),
				new Color(0.5f, 0.5f, 0.5f)
			);
			StrokeWidth = 1;
		}
		
		
			
		#region Background Rendering
		
		/// <value>
		/// The orientation used to render the background if the decorator doesn't override it.
		/// </value>
		public AnchorLocation DefaultBackgroundLocation {get; set;}
		
		/// <summary>
		/// The color group for the start of the background gradient.
		/// </summary>
		public ColorGroup BackgroundStartColors {get; private set;}
		
		/// <summary>
		/// The color group for the stop of the background gradient.
		/// </summary>
		public ColorGroup BackgroundStopColors { get; private set; }
		
		/// <summary>
		/// The default background color if none can be found.
		/// </summary>
		protected static Color defaultBackgroundColor = new Color(0.5f, 0.5f, 0.5f);
				
		/// <summary>
		/// Creates a gradient for the given position, size, and anchor location.
		/// </summary>
		protected Cairo.LinearGradient GenerateGradient(Coord point, Coord size, AnchorLocation location, 
		                                                Color startColor, Color stopColor)
		{
			Cairo.LinearGradient grad = null;
			var x = point.X;
			var y = point.Y;
			switch (location)
			{
			case AnchorLocation.E:
				grad = new Cairo.LinearGradient(x + size.X, y, x, y);
				break;
			case AnchorLocation.NE:
				grad = new Cairo.LinearGradient(x + size.X, y, x, y + size.Y);
				break;
			case AnchorLocation.N:
				grad = new Cairo.LinearGradient(x, y, x, y + size.Y);
				break;
			case AnchorLocation.NW:
				grad = new Cairo.LinearGradient(x, y, x + size.X, y + size.Y);
				break;
			case AnchorLocation.W:
				grad = new Cairo.LinearGradient(x, y, x + size.X, y);
				break;
			case AnchorLocation.SW:
				grad = new Cairo.LinearGradient(x, y + size.Y, x + size.X, y);
				break;
			case AnchorLocation.S:
				grad = new Cairo.LinearGradient(x, y + size.Y, x, y);
				break;
			case AnchorLocation.SE:
				grad = new Cairo.LinearGradient(x + size.X, y + size.Y, x, y);
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
			Cairo.LinearGradient grad = GenerateGradient(point.Coord(), size, location, 
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
			var point = Context.Push();
			
			// Create the gradient
			Cairo.LinearGradient grad = GenerateGradient(point.Coord(), size / 2.0, gradDirection, 
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
		
		
		#region Foreground Rendering

		/// <summary>
		/// The color group for stroking (drawing lines).
		/// </summary>
		public ColorGroup StrokeColors { get; private set; }
		
		/// <summary>
		/// The width of lines drawn using the Stroke* methods.
		/// </summary>
		public double StrokeWidth { get; set; }
				
		/// <summary>
		/// Outlines a rectangle with the given size.
		/// </summary>
		/// <param name="size">
		/// The size of the rectangle to outline (assumed to be at 0,0 origin).
		/// </param>
		/// <param name="hitState">
		/// Used to look up which color to use for the stroke.
		/// </param>
		protected void StrokeRectangle(Coord size, HitState hitState)
		{
			var point = Context.Push();
			Context.Cairo.Color = StrokeColors.GetColor(hitState).Cairo;
			Context.Cairo.Rectangle(point, size.X, size.Y);
			Context.Cairo.Stroke();
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
			if (control is DialogFrame)
			{
				Decorate(control as DialogFrame);
				return;
			}
			if (control is Button)
			{
				Decorate(control as Button);
			}
			if (control is ToolBar) {
				Decorate(control as ToolBar);
			}
		}
		
		protected virtual void Decorate(Button button)
		{
			var parent = button.Parent;
			if (parent == null)
				FillRectangle(button.RenderSize, button.HitState, DefaultBackgroundLocation);
			else if (parent is ToolBar || parent is DialogFrame)
			{
				if (button.HitState != HitState.None)
					FillRectangle(button.RenderSize, button.HitState, DefaultBackgroundLocation);
			}
			else
				FillRectangle(button.RenderSize, button.HitState, DefaultBackgroundLocation);
		}
		
		
		protected virtual void Decorate(ToolBar toolbar)
		{			
			if (toolbar.Pane != null && toolbar.Pane is AnchorPane) // the toolbar is anchored
			{
				FillRectangle(toolbar.RenderSize, toolbar.HitState, (toolbar.Pane as AnchorPane).Location);
			}
			else // the toolbar is not anchored
			{
				FillRectangle(toolbar.RenderSize, toolbar.HitState, DefaultBackgroundLocation);
			}
		}
		
		protected virtual void Decorate(CornerButtons control)
		{			
			// define the sizes for the horizontal and vertical triangles that make up each button
			var horiSize = new Coord(control.RenderWidth, control.RenderHeight/2.0);
			var vertSize = new Coord(control.RenderWidth/2.0, control.RenderHeight);
			
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
		
		protected virtual void Decorate(DialogFrame dialog)
		{
			FillRectangle(dialog.RenderSize, HitState.None, AnchorLocation.SE);
			FillRectangle(new Coord(dialog.RenderWidth, DialogFrame.TitleHeight), HitState.None, AnchorLocation.S);
			StrokeRectangle(dialog.RenderSize, HitState.None);
		}
				
		#endregion
		
	}
}
