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
	/// The types of colors that the basic decorator stores color groups for.
	/// </summary>
	public enum ColorType {BackgroundStart, BackgroundStop, Stroke, Text, EditableStart, EditableStop};
	
	/// <summary>
	/// Provides a basic decorator that should be good enough for most situations.
	/// </summary>
	[XmlRoot]
	public class BasicDecorator : AbstractDecorator
	{
		
		public BasicDecorator() : base()
		{
			DefaultBackgroundLocation = AnchorLocation.SE;
			
			SetColorGroup(ColorType.BackgroundStart,
				new ColorGroup(
      			new Color(1f, 1f, 1f, 0.6f),
      			new Color(1f, 1f, 1f, 0.9f),
      			new Color(0.9f, 0.9f, 0.9f, 0.5f)
            ));
			
			SetColorGroup(ColorType.BackgroundStop,
				new ColorGroup(
      			new Color(1f, 1f, 1f, 0.9f),
      			new Color(1f, 1f, 1f, 0.6f),
      			new Color(0.9f, 0.9f, 1f, 0.5f)
            ));
			
			SetColorGroup(ColorType.Stroke,
				new ColorGroup(
				new Color(0.5f, 0.5f, 0.5f),
				new Color(0.5f, 0.5f, 0.5f),
				new Color(0.5f, 0.5f, 0.8f)
			));
			
			SetColorGroup(ColorType.EditableStart,
				new ColorGroup(
      			new Color(1f, 1f, 1f, 0.8f),
      			new Color(0.95f, 0.95f, 0.95f, 0.9f),
      			new Color(1f, 1f, 1f, 0.9f)
            ));
			
			SetColorGroup(ColorType.EditableStop,
				new ColorGroup(
      			new Color(1f, 1f, 1f, 1f),
      			new Color(1f, 1f, 1f, 1f),
      			new Color(0.9f, 0.9f, 1f, 1f)
            ));
			
			SelectionColor = new Color(0.2f, 0.3f, 1f, 0.5f);
			
			StrokeWidth = 0.5;
			CornerRadius = 6;
		}
		
		
		/// <summary>
		/// The radius of rounded corners.
		/// </summary>
		public double CornerRadius {get; set;}
		
		/// <summary>
		/// A Corner value specifcying all of the corners.
		/// </summary>
		public const Corner AllCorners = Corner.NE | Corner.NW | Corner.SE | Corner.SW;
		
		
		#region Colors
		
		/// <summary>
		/// Associates color types with their groups.
		/// </summary>
		private Dictionary<ColorType,ColorGroup> _colorGroups = new Dictionary<ColorType, ColorGroup>();
		
		/// <summary>
		/// Gets the color group that stores colors for the given type.
		/// </summary>
		public ColorGroup GetColorGroup(ColorType colorType)
		{
			ColorGroup colorGroup = null;
			if (!_colorGroups.TryGetValue(colorType, out colorGroup))
			{
				colorGroup = new ColorGroup();
				_colorGroups[colorType] = colorGroup;
			}
			return colorGroup;
		}
		
		/// <summary>
		/// Assigns a new color group for the given type.
		/// </summary>
		public void SetColorGroup(ColorType colorType, ColorGroup colorGroup)
		{
			_colorGroups[colorType] = colorGroup;
		}
		
		/// <summary>
		/// Returns the color associated with the given type and hit state.
		/// </summary>
		public Color GetColor(ColorType colorType, HitState hitState)
		{
			return GetColorGroup(colorType).GetColor(hitState);
		}
		
		#endregion
		
		
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
		protected void FillRectangleBackground(Coord size, Corner rounded, HitState hitState, AnchorLocation location)
		{
			var point = Context.Push();
			
			// Create the gradient
			Cairo.LinearGradient grad = GenerateGradient(point.Coord(), size, location, 
			                                             GetColor(ColorType.BackgroundStart, hitState),
			                                             GetColor(ColorType.BackgroundStop, hitState));
			// draw the rectangle
			Context.Cairo.Operator = Cairo.Operator.Source;
			Context.Cairo.Pattern = grad;
			RectanglePath(point.Coord().Round + 1, size.Floor - 2, rounded);
			Context.Cairo.Fill();
			
			Context.Pop();
		}
		
		/// <summary>
		/// Renders a rectangular editable portion of a control.
		/// </summary>
		protected void FillRectangleEditable(Coord size, Corner rounded, HitState hitState, AnchorLocation location)
		{
			var point = Context.Push();
			
			// Create the gradient
			Cairo.LinearGradient grad = GenerateGradient(point.Coord(), size, location, 
			                                             GetColor(ColorType.EditableStart, hitState),
			                                             GetColor(ColorType.EditableStop, hitState));
			// draw the rectangle
			Context.Cairo.Operator = Cairo.Operator.Source;
			Context.Cairo.Pattern = grad;
			RectanglePath(point.Coord().Round + 1, size.Floor - 2, rounded);
			Context.Cairo.Fill();
			
			Context.Pop();
		}
		
		/// <summary>
		/// Fills a rectangle with a solid color.
		/// </summary>
		protected void FillSolidRectangle(Coord size, Corner rounded, Color color)
		{
			var point = Context.Push();
			
			// draw the rectangle
			Context.Cairo.Operator = Cairo.Operator.Source;
			Context.Cairo.Color = color.Cairo;
			RectanglePath(point.Coord().Round + 1, size.Floor - 2, rounded);
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
		/// <param name="size">
		/// The size of the rectangle to outline (assumed to be at 0,0 origin).
		/// </param>
		/// <param name="hitState">
		/// Used to look up which color to use for the stroke.
		/// </param>
		protected void StrokeRectangle(Coord size, Corner rounded, HitState hitState)
		{
			var point = Context.Push();			
			
			// draw the rectangle
			Context.Cairo.Operator = Cairo.Operator.Source;
			Context.Cairo.Color = GetColor(ColorType.Stroke, hitState).Cairo;
			Context.Cairo.LineWidth = StrokeWidth;
			RectanglePath(point.Coord().HalfCeiling, size.Floor-1, rounded);
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
			if (control is ToolBar) 
			{
				Decorate(control as ToolBar);
			}
			if (control is TextBox) 
			{
				Decorate(control as TextBox);
			}
		}
		
		protected virtual void Decorate(Button button)
		{
			var parent = button.ParentControl;
			if (parent == null)
				FillRectangleBackground(button.RenderSize, AllCorners, button.HitState, DefaultBackgroundLocation);
			else if (parent is ToolBar || parent is DialogFrame)
			{
				if (button.HitState != HitState.None)
					FillRectangleBackground(button.RenderSize, AllCorners, button.HitState, DefaultBackgroundLocation);
			}
			else
			{
				FillRectangleBackground(button.RenderSize, AllCorners, button.HitState, DefaultBackgroundLocation);
				StrokeRectangle(button.RenderSize, AllCorners, button.HitState);
			}
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
				FillRectangleBackground(toolbar.RenderSize, corner, toolbar.HitState, location);
			}
			else // the toolbar is not anchored
			{
				FillRectangleBackground(toolbar.RenderSize, AllCorners, toolbar.HitState, DefaultBackgroundLocation);
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
			FillRectangleBackground(dialog.RenderSize, AllCorners, HitState.None, AnchorLocation.SE);
			FillRectangleBackground(new Coord(dialog.RenderWidth, DialogFrame.TitleHeight), 
			              Corner.NE | Corner.NW, HitState.None, AnchorLocation.S);
			StrokeRectangle(dialog.RenderSize, AllCorners, HitState.None);
		}
		
		protected virtual void Decorate(TextBox textBox)
		{
			FillRectangleEditable(textBox.RenderSize, Corner.None, textBox.HitState, AnchorLocation.S);
			StrokeRectangle(textBox.RenderSize, Corner.None, HitState.None);
			if (textBox.IsFocused)
			{
				
			}
		}
				
		#endregion
		
	}
}
