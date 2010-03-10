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
using System.Collections.Generic;
using System.Xml.Serialization;


using MonoWorks.Base;
using MonoWorks.Rendering;
using MonoWorks.Rendering.Events;


namespace MonoWorks.Controls
{
	
	/// <summary>
	/// Maps to individual or sets of color types for filling.
	/// </summary>
	public enum FillType {Background, Editable, Selection, Highlight};
	
	/// <summary>
	/// Provides a basic decorator that should be good enough for most situations.
	/// </summary>
	[XmlRoot]
	public class BasicDecorator : DecoratorBase
	{
		
		public BasicDecorator() : base()
		{
			CornerRadius = 6;
			DefaultBackgroundLocation = AnchorLocation.SE;
			
			StrokeWidth = 0.5;
			
			FocusColor = new Color(0, 0.8f, 0, 0.1f);
		}
		
		
		/// <summary>
		/// The radius of rounded corners.
		/// </summary>
		public double CornerRadius {get; set;}
		
		/// <summary>
		/// A Corner value specifcying all of the corners.
		/// </summary>
		public const Corner AllCorners = Corner.NE | Corner.NW | Corner.SE | Corner.SW;
				
		
		#region Paths
		
		/// <summary>
		/// Creates a rectangular path with the given corners rounded.
		/// </summary>
		protected virtual void RectanglePath(Coord point, Coord size, Corner rounded)
		{
			Context.Cairo.NewPath();
			
			// NW corner
			if ((rounded & Corner.NW) == Corner.NW)
			{
				Context.Cairo.MoveTo(point.X, point.Y + CornerRadius);
				Context.Cairo.Arc(point.X + CornerRadius, point.Y + CornerRadius, CornerRadius, Math.PI, -Math.PI / 2);
				if ((rounded & Corner.NE) == Corner.NE)
					Context.Cairo.RelLineTo(size.X - 2 * CornerRadius, 0);
				else
					Context.Cairo.RelLineTo(size.X - CornerRadius, 0);
			}
			else
			{
				Context.Cairo.MoveTo(point.X, point.Y);
				if ((rounded & Corner.NE) == Corner.NE)
					Context.Cairo.RelLineTo(size.X - CornerRadius, 0);
				else
					Context.Cairo.RelLineTo(size.X, 0);
			}
			
			// NE corner
			if ((rounded & Corner.NE) == Corner.NE)
			{
				Context.Cairo.Arc(point.X + size.X - CornerRadius, point.Y + CornerRadius, CornerRadius, -Math.PI / 2, 0);
				if ((rounded & Corner.SE) == Corner.SE)
					Context.Cairo.RelLineTo(0, size.Y - 2 * CornerRadius);
				else
					Context.Cairo.RelLineTo(0, size.Y - CornerRadius);
			}
			else
			{
				if ((rounded & Corner.SE) == Corner.SE)
					Context.Cairo.RelLineTo(0, size.Y - CornerRadius);
				else
					Context.Cairo.RelLineTo(0, size.Y);
			}
			
			// SE corner
			if ((rounded & Corner.SE) == Corner.SE)
			{
				Context.Cairo.Arc(point.X + size.X - CornerRadius, point.Y + size.Y - CornerRadius, CornerRadius, 0, Math.PI / 2);
				if ((rounded & Corner.SW) == Corner.SW)
					Context.Cairo.RelLineTo(-size.X + 2 * CornerRadius, 0);
				else
					Context.Cairo.RelLineTo(-size.X + CornerRadius, 0);
			}
			else
			{
				if ((rounded & Corner.SW) == Corner.SW)
					Context.Cairo.RelLineTo(-size.X + CornerRadius, 0);
				else
					Context.Cairo.RelLineTo(-size.X, 0);
			}
			
			// SW corner
			if ((rounded & Corner.SW) == Corner.SW)
			{
				Context.Cairo.Arc(point.X + CornerRadius, point.Y + size.Y - CornerRadius, CornerRadius, Math.PI / 2, Math.PI);
				if ((rounded & Corner.NW) == Corner.NW)
					Context.Cairo.RelLineTo(0, -size.Y + 2 * CornerRadius);
				else
					Context.Cairo.RelLineTo(0, -size.Y + CornerRadius);
			}
			else
			{
				if ((rounded & Corner.NW) == Corner.NW)
					Context.Cairo.RelLineTo(0, -size.Y + CornerRadius);
				else
					Context.Cairo.RelLineTo(0, -size.Y);
			}
			
			Context.Cairo.ClosePath();
		}
		
		#endregion
		
			
		#region Filling
		
		/// <value>
		/// The orientation used to render the background if the decorator doesn't override it.
		/// </value>
		public AnchorLocation DefaultBackgroundLocation {get; set;}
		
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
			var r = (size.X + size.Y) / 2.0;
			switch (location)
			{
			case AnchorLocation.E:
				grad = new Cairo.LinearGradient(x + size.X, y, x, y);
				break;
			case AnchorLocation.NE:
				grad = new Cairo.LinearGradient(x + r, y, x, y + r);
				break;
			case AnchorLocation.N:
				grad = new Cairo.LinearGradient(x, y, x, y + size.Y);
				break;
			case AnchorLocation.NW:
				grad = new Cairo.LinearGradient(x, y, x + r, y + r);
				break;
			case AnchorLocation.W:
				grad = new Cairo.LinearGradient(x, y, x + size.X, y);
				break;
			case AnchorLocation.SW:
				grad = new Cairo.LinearGradient(x, y + r, x + r, y);
				break;
			case AnchorLocation.S:
				grad = new Cairo.LinearGradient(x, y + size.Y, x, y);
				break;
			case AnchorLocation.SE:
				grad = new Cairo.LinearGradient(x + r, y + r, x, y);
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
		/// Renders a rectangular background portion of a control.
		/// </summary>
		protected virtual void FillRectangle(Coord relPos, Coord size, Corner rounded, FillType fillType, HitState hitState, AnchorLocation location)
		{
			var point = Context.Push();
			var coord = new Coord(point.X + relPos.X, point.Y + relPos.Y);
			
			switch (fillType)
			{
			case FillType.Background:
				Context.Cairo.Pattern = GenerateGradient(coord, size, location, 
                                             GetColor(ColorType.BackgroundStart, hitState),
                                             GetColor(ColorType.BackgroundStop, hitState));
				break;
			case FillType.Editable:
				Context.Cairo.Pattern = GenerateGradient(coord, size, location, 
                                             GetColor(ColorType.EditableStart, hitState),
                                             GetColor(ColorType.EditableStop, hitState));
				break;
			case FillType.Highlight:
				Context.Cairo.Pattern = GenerateGradient(coord, size, location, 
                                             GetColor(ColorType.HighlightStart, hitState),
                                             GetColor(ColorType.HighlightStop, hitState));
				break;
			case FillType.Selection:
				Context.Cairo.Color = SelectionColor.Cairo;
				break;
			}
			
			// draw the rectangle
			Context.Cairo.Operator = Cairo.Operator.Source;
			RectanglePath(coord.Round + 1, size.Floor - 2, rounded);
			Context.Cairo.Fill();
			
			Context.Pop();
		}
		
		/// <summary>
		/// Renders a triangle portion of a control.
		/// </summary>
		protected virtual void FillTriangle(Coord size, HitState hitState, AnchorLocation triDirection, AnchorLocation gradDirection)
		{
			var point = Context.Push();
			
			// Create the gradient
			Cairo.LinearGradient grad = GenerateGradient(point.Coord(), size / 2.0, gradDirection, 
			                                             GetColor(ColorType.BackgroundStart, hitState),
			                                             GetColor(ColorType.BackgroundStop, hitState));
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
		
		
		#region Focus Highlighting
		
		/// <summary>
		/// The color used to highlight controls in focus.
		/// </summary>
		public Color FocusColor { get; set; }
		
		/// <summary>
		/// Highlights inside a rectangle defined by the position and size.
		/// </summary>
		protected virtual void InnerHighlightRectangle(Coord relPos, Coord size, Corner rounded)
		{
			var point = Context.Push();
			
			// create the gradient
			var radius = Math.Sqrt(size.X * size.X / 4 + size.Y * size.Y / 4);
			var cx = point.X + relPos.X + size.X / 2;
			var cy = point.Y + relPos.Y + size.Y / 2;
			var grad = new Cairo.RadialGradient(cx, cy, radius / 4.0, cx, cy, radius);
			grad.AddColorStop(0, new Cairo.Color(1, 1, 1, 0));
			grad.AddColorStop(1, FocusColor.Cairo);
			Context.Cairo.Pattern = grad;
			
			// draw the rectangle path
			RectanglePath(point.Coord() + relPos, size, rounded);
		//			var ar = size.Y / size.X;
		//			Context.Cairo.Scale(1, ar);
			Context.Cairo.Fill();
			
			Context.Pop();
		}
		
		/// <summary>
		/// Highlights outside a rectangle defined by the position and size.
		/// </summary>
		protected virtual void OuterHighlightRectangle(Coord relPos, Coord size, Corner rounded)
		{
			// TODO: make outer highlight not suck
			var point = Context.Push();
			
			// create the gradient
			var radius = Math.Min(size.X, size.Y);
			var cx = point.X + relPos.X + size.X / 2;
			var cy = point.Y + relPos.Y + size.Y / 2;
			var grad = new Cairo.RadialGradient(cx, cy, radius, cx, cy, radius * 1.5);
			grad.AddColorStop(0, FocusColor.Cairo);
			grad.AddColorStop(1, new Cairo.Color(1, 1, 1, 0));
			Context.Cairo.Pattern = grad;
			
			// draw the rectangle path
			Context.Cairo.Arc(cx, cy, radius * 1.5, 0, 2 * Math.PI);
//			RectanglePath(point.Coord() + relPos, size, rounded);
//			var ar = size.Y / size.X;
//			Context.Cairo.Scale(1, ar);
			Context.Cairo.Fill();
			
			Context.Pop();
		}
		
		#endregion
		
		
		#region Stroking
		
		/// <summary>
		/// The width of lines drawn using the Stroke* methods.
		/// </summary>
		public double StrokeWidth { get; set; }
				
		/// <summary>
		/// Outlines a rectangle with the given size.
		/// </summary>
		/// <param name="relPos">
		/// The relative position of the rectangle.
		/// </para>
		/// <param name="size">
		/// The size of the rectangle to outline (assumed to be at 0,0 origin).
		/// </param>
		/// <param name="hitState">
		/// Used to look up which color to use for the stroke.
		/// </param>
		protected virtual void StrokeRectangle(Coord relPos, Coord size, Corner rounded, HitState hitState)
		{
			var point = Context.Push();
			var coord = new Coord(point.X + relPos.X, point.Y + relPos.Y).HalfCeiling;
			
			// draw the rectangle
			Context.Cairo.Operator = Cairo.Operator.Source;
			Context.Cairo.Color = GetColor(ColorType.Stroke, hitState).Cairo;
			if (hitState.IsFocused())
				Context.Cairo.LineWidth = 2 * StrokeWidth;
			else
				Context.Cairo.LineWidth = StrokeWidth;
			RectanglePath(coord, size.Floor-1, rounded);
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
			if (control is MessageBoxFrame)
			{
				Decorate(control as MessageBoxFrame);
				return;
			}
			if (control is SceneButton)
			{
				Decorate(control as SceneButton);
				return;
			}
			if (control is SceneBookSelector)
			{
				Decorate(control as SceneBookSelector);
				return;
			}
			if (control is Button)
			{
				Decorate(control as Button);
				return;
			}
			if (control is ToolBar)
			{
				Decorate(control as ToolBar);
				return;
			}
			if (control is TextBox)
			{
				Decorate(control as TextBox);
				return;
			}
			if (control is Slider)
			{
				Decorate(control as Slider);
				return;
			}
			if (control is CheckBox)
			{
				Decorate(control as CheckBox);
				return;
			}
			if (control is MenuBox)
			{
				Decorate(control as MenuBox);
				return;
			}
			if (control is Menu)
			{
				Decorate(control as Menu);
				return;
			}
			if (control is ProgressBar)
			{
				Decorate(control as ProgressBar);
				return;
			}
			if (control is ProgressDial)
			{
				Decorate(control as ProgressDial);
				return;
			}
			if (control is TreeItem)
			{
				Decorate(control as TreeItem);
				return;
			}
		}
		
		protected virtual void Decorate(Button button)
		{
			var parent = button.ParentControl;
			if (parent == null)
				FillRectangle(Coord.Zeros, button.RenderSize, AllCorners, FillType.Background, button.HitState, DefaultBackgroundLocation);
			else if (parent is ToolBar || parent is DialogFrame || parent is SceneButton)
			{
				if (button.HitState != HitState.None)
				{
					FillRectangle(Coord.Zeros, button.RenderSize, AllCorners, FillType.Background, button.HitState, DefaultBackgroundLocation);
					StrokeRectangle(Coord.Zeros, button.RenderSize, AllCorners, button.HitState);
				}
			}
			else
			{
				FillRectangle(Coord.Zeros, button.RenderSize, AllCorners, FillType.Background, button.HitState, DefaultBackgroundLocation);
				StrokeRectangle(Coord.Zeros, button.RenderSize, AllCorners, button.HitState);
			}
			
			if (button.IsFocused)
				InnerHighlightRectangle(Coord.Zeros, button.RenderSize, AllCorners);
		}
		
		protected virtual void Decorate(SceneButton button)
		{
			var parent = button.ParentControl;
			if (parent == null)
				FillRectangle(Coord.Zeros, button.RenderSize, Corner.None, FillType.Background, button.HitState, AnchorLocation.S);
			else
			{
				FillRectangle(Coord.Zeros, button.RenderSize, Corner.None, FillType.Background, button.HitState, AnchorLocation.S);
				StrokeRectangle(Coord.Zeros, button.RenderSize, Corner.None, button.HitState);
			}
		}
		
		protected virtual void Decorate(SceneBookSelector selector)
		{
			FillRectangle(Coord.Zeros, selector.RenderSize, Corner.None, FillType.Editable, selector.HitState, AnchorLocation.S);
		}
		
		protected virtual void Decorate(ToolBar toolbar)
		{			
			Corner corner = AllCorners;
			if (toolbar.Pane != null && toolbar.Pane is AnchorPane) // the toolbar is anchored
			{
				var location = (toolbar.Pane as AnchorPane).Location;
				switch (location)
				{
				case AnchorLocation.N:
					corner = Corner.SE | Corner.SW;
					break;
				case AnchorLocation.E:
					corner = Corner.NW | Corner.SW;
					break;
				case AnchorLocation.S:
					corner = Corner.NW | Corner.NE;
					break;
				case AnchorLocation.W:
					corner = Corner.NE | Corner.SE;
					break;					
				}
				FillRectangle(Coord.Zeros, toolbar.RenderSize, corner, FillType.Background, toolbar.HitState, location);
			}
			else if (toolbar.ParentControl != null && toolbar.ParentControl is IOrientable) // it belongs to something that has an orientation, like a stack
			{
				AnchorLocation location = AnchorLocation.E;
				if ((toolbar.ParentControl as IOrientable).Orientation == Orientation.Horizontal)
					location = AnchorLocation.S;			
				FillRectangle(Coord.Zeros, toolbar.RenderSize, Corner.None, FillType.Background, toolbar.HitState, location);
			}
			else // the toolbar is not anchored
			{
				FillRectangle(Coord.Zeros, toolbar.RenderSize, AllCorners, FillType.Background, toolbar.HitState, DefaultBackgroundLocation);
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
			FillRectangle(Coord.Zeros, dialog.RenderSize, AllCorners, FillType.Background, HitState.None, AnchorLocation.SE);
			FillRectangle(Coord.Zeros, new Coord(dialog.RenderWidth, DialogFrame.TitleHeight), 
			              Corner.NE | Corner.NW, FillType.Background, HitState.None, AnchorLocation.S);
			StrokeRectangle(Coord.Zeros, dialog.RenderSize, AllCorners, HitState.None);
		}
		
		protected virtual void Decorate(MessageBoxFrame box)
		{
			FillRectangle(Coord.Zeros, box.RenderSize, AllCorners, FillType.Background, HitState.None, AnchorLocation.SE);
			StrokeRectangle(Coord.Zeros, box.RenderSize, AllCorners, HitState.None);
		}
		
		protected virtual void Decorate(TextBox textBox)
		{
			FillRectangle(Coord.Zeros, textBox.RenderSize, Corner.None, FillType.Editable, textBox.HitState, AnchorLocation.S);
			StrokeRectangle(Coord.Zeros, textBox.RenderSize, Corner.None, textBox.HitState);
		}
		
		protected virtual void Decorate(Slider slider)
		{
			// draw the line
			var pos = Context.Push();
			Context.Cairo.Color = GetColor(ColorType.Stroke, slider.HitState).Cairo;
			Context.Cairo.LineWidth = 3;
			Context.Cairo.MoveTo(pos.X + slider.LineStart.X, pos.Y + slider.LineStart.Y);
			Context.Cairo.LineTo(pos.X + slider.LineStop.X, pos.Y + slider.LineStop.Y);
			Context.Cairo.Stroke();
			Context.Pop();
			
			// draw the indicator
			FillRectangle(slider.IndicatorPosition, slider.IndicatorSize, Corner.None, FillType.Editable, slider.HitState, AnchorLocation.SE);
			StrokeRectangle(slider.IndicatorPosition, slider.IndicatorSize, Corner.None, slider.HitState);
		}
		
		protected virtual void Decorate(CheckBox checkBox)
		{
			// draw the box
			var pos = new Coord(checkBox.Padding, checkBox.Padding);
			var size = new Coord(CheckBox.BoxSize, CheckBox.BoxSize);
			FillRectangle(pos, size, Corner.None, FillType.Editable, checkBox.HitState, AnchorLocation.SE);
			StrokeRectangle(pos, size, Corner.None, checkBox.HitState);
			
			// draw the check
			if (checkBox.IsSelected)
			{
				var absPos = Context.Push();
				Context.Cairo.Color = GetColor(ColorType.Stroke, checkBox.HitState).Cairo;
				Context.Cairo.LineWidth = 2 *StrokeWidth;
				Context.Cairo.MoveTo(absPos.X + pos.X + 1, absPos.Y + pos.Y + 1);
				Context.Cairo.RelLineTo(CheckBox.BoxSize - 2, CheckBox.BoxSize - 2);
				Context.Cairo.Stroke();
				Context.Cairo.MoveTo(absPos.X + pos.X + 1, absPos.Y + pos.Y + size.Y - 1);
				Context.Cairo.RelLineTo(CheckBox.BoxSize - 2, -CheckBox.BoxSize + 2);
				Context.Cairo.Stroke();
				Context.Pop();
			}
		}
		
		protected virtual void Decorate(MenuBox menuBox)
		{
			FillRectangle(menuBox.ButtonOrigin, menuBox.ButtonSize, Corner.None, FillType.Background, menuBox.HitState, AnchorLocation.SE);
			StrokeRectangle(menuBox.ButtonOrigin, menuBox.ButtonSize, Corner.None, menuBox.HitState);
		}

		protected virtual void Decorate(Menu menu)
		{
			FillRectangle(Coord.Zeros, menu.RenderSize, Corner.None, FillType.Background, menu.HitState, AnchorLocation.SE);
			StrokeRectangle(Coord.Zeros, menu.RenderSize, Corner.None, menu.HitState);
		}

		protected virtual void Decorate(ProgressBar bar)
		{
			AnchorLocation backLoc, foreLoc;
			var progressSize = bar.RenderSize.Copy();
			var foreOrigin = Coord.Zeros;
			if (bar.Orientation == Orientation.Horizontal)
			{
				progressSize.X = progressSize.X * bar.Value;
				backLoc = AnchorLocation.N;
				foreLoc = AnchorLocation.S;
			}
			else // vertical
			{
				progressSize.Y = progressSize.Y * bar.Value;
				backLoc = AnchorLocation.W;
				foreLoc = AnchorLocation.E;
				foreOrigin = new Coord(0, bar.RenderHeight - progressSize.Y).Ceiling;
			}
			FillRectangle(Coord.Zeros, bar.RenderSize, Corner.None, FillType.Background, bar.HitState, backLoc);
			FillRectangle(foreOrigin, progressSize, Corner.None, FillType.Highlight, bar.HitState, foreLoc);
			StrokeRectangle(Coord.Zeros, bar.RenderSize, Corner.None, bar.HitState);
		}

		protected virtual void Decorate(ProgressDial dial)
		{
			var pos = Context.Push();
			var coord = pos.Coord();
			Context.Cairo.Operator = Cairo.Operator.Source;
			Context.Cairo.LineWidth = StrokeWidth;
			var strokeColor = GetColor(ColorType.Stroke, dial.HitState).Cairo;
			var cx = dial.RenderWidth / 2.0 + pos.X;
			var cy = dial.RenderHeight / 2.0 + pos.Y;
			var outerRadius = dial.RenderWidth/2;
			
			// draw the outer background circle
			Context.Cairo.Pattern = GenerateGradient(coord, dial.RenderSize, AnchorLocation.NW, 
			                                        GetColor(ColorType.BackgroundStart, dial.HitState), 
													GetColor(ColorType.BackgroundStop, dial.HitState));
			Context.Cairo.Arc(cx, cy, outerRadius, 0, 2 * Math.PI);
			Context.Cairo.Fill();
			Context.Cairo.Color = strokeColor;
			Context.Cairo.Arc(cx, cy, outerRadius, 0, 2 * Math.PI);
			Context.Cairo.Stroke();
			
			// draw the outer highlighted circle
			var grad = new Cairo.RadialGradient(cx, cy, dial.InnerRadius, cx, cy, outerRadius);
			grad.AddColorStop(1, GetColor(ColorType.HighlightStart, dial.HitState).Cairo);
			grad.AddColorStop(0, GetColor(ColorType.HighlightStop, dial.HitState).Cairo);
			Context.Cairo.Pattern = grad;
			Context.Cairo.Arc(cx, cy, outerRadius, -Math.PI/2.0, dial.Value * 2 * Math.PI - Math.PI/2.0);
			Context.Cairo.LineTo(cx, cy);
			Context.Cairo.Fill();
			Context.Cairo.Color = strokeColor;
			Context.Cairo.MoveTo(cx, cy);
			Context.Cairo.RelLineTo(0, -outerRadius);
			Context.Cairo.Stroke();
						
			// draw the inner circle
			Context.Cairo.Pattern = GenerateGradient(new Coord(pos.X + dial.RenderWidth/2.0 - dial.InnerRadius, 
														pos.Y + dial.RenderHeight/2.0 - dial.InnerRadius),
														new Coord(dial.InnerRadius * 2, dial.InnerRadius * 2), 
														AnchorLocation.SE, 
			                                            GetColor(ColorType.BackgroundStart, dial.HitState),
			                                            GetColor(ColorType.BackgroundStop, dial.HitState));
			Context.Cairo.Arc(cx, cy, dial.InnerRadius, 0, 2 * Math.PI);
			Context.Cairo.Fill();
			Context.Cairo.Color = strokeColor;
			Context.Cairo.Arc(cx, cy, dial.InnerRadius, 0, 2 * Math.PI);
			Context.Cairo.Stroke();
			
			
			Context.Pop();
		}

		protected virtual void Decorate(TreeItem item)
		{
			if (item.IsHovering || item.IsSelected)
			{
				StrokeRectangle(item.HoverOffset, item.HoverSize, AllCorners, item.HitState);
				if (item.IsSelected)
				{
					FillRectangle(item.HoverOffset, item.HoverSize, AllCorners, FillType.Background, item.HitState, AnchorLocation.S);	
				}
			}
		}
				
		#endregion
		
	}
}
