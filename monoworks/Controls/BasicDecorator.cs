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
	public enum FillType {Background, Editable, Selection};
	
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
		protected void FillRectangle(Coord relPos, Coord size, Corner rounded, FillType fillType, HitState hitState, AnchorLocation location)
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
		protected void FillTriangle(Coord size, HitState hitState, AnchorLocation triDirection, AnchorLocation gradDirection)
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
		protected void StrokeRectangle(Coord relPos, Coord size, Corner rounded, HitState hitState)
		{
			var point = Context.Push();
			var coord = new Coord(point.X + relPos.X, point.Y + relPos.Y).HalfCeiling;
			
			// draw the rectangle
			Context.Cairo.Operator = Cairo.Operator.Source;
			Context.Cairo.Color = GetColor(ColorType.Stroke, hitState).Cairo;
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
		}
		
		protected virtual void Decorate(Button button)
		{
			var parent = button.ParentControl;
			if (parent == null)
				FillRectangle(Coord.Zeros, button.RenderSize, AllCorners, FillType.Background, button.HitState, DefaultBackgroundLocation);
			else if (parent is ToolBar || parent is DialogFrame)
			{
				if (button.HitState != HitState.None)
				{
					if (!button.HitState.IsFocused()) // don't draw the background on focused toolbar buttons
						FillRectangle(Coord.Zeros, button.RenderSize, AllCorners, FillType.Background, button.HitState, DefaultBackgroundLocation);
					StrokeRectangle(Coord.Zeros, button.RenderSize, AllCorners, button.HitState);
				}
			}
			else
			{
				if (!button.HitState.IsFocused()) // don't draw the background on focused toolbar buttons
					FillRectangle(Coord.Zeros, button.RenderSize, AllCorners, FillType.Background, button.HitState, DefaultBackgroundLocation);
				StrokeRectangle(Coord.Zeros, button.RenderSize, AllCorners, button.HitState);
			}
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
			if (toolbar.Pane != null && toolbar.Pane is AnchorPane) // the toolbar is anchored
			{
				var location = (toolbar.Pane as AnchorPane).Location;
				Corner corner = AllCorners;
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
				Context.Cairo.LineWidth = StrokeWidth;
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
//			FillRectangle(Coord.Zeros, menuBox.RenderSize, Corner.None, FillType.Editable, menuBox.HitState, AnchorLocation.S);
//			StrokeRectangle(Coord.Zeros, menuBox.RenderSize, Corner.None, menuBox.HitState);
		}

		protected virtual void Decorate(Menu menu)
		{
			FillRectangle(Coord.Zeros, menu.RenderSize, Corner.None, FillType.Background, menu.HitState, AnchorLocation.SE);
			StrokeRectangle(Coord.Zeros, menu.RenderSize, Corner.None, menu.HitState);
		}
				
		#endregion
		
	}
}
