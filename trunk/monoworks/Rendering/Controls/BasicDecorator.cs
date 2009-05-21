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


namespace MonoWorks.Rendering.Controls
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
			                  			new Color(0.8f, 0.8f, 0.8f, 0.5f),
			                  			new Color(0.9f, 0.9f, 0.9f, 0.5f),
			                  			new Color(0.9f, 0.9f, 0.9f, 0.5f)
			                        );
			BackgroundStopColors = new ColorGroup(
			                  			new Color(1f, 1f, 1f, 0.5f),
			                  			new Color(0.9f, 0.9f, 1f, 0.5f),
			                  			new Color(1f, 1f, 1f, 0.5f)
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
		
		public override void RenderBackground(Cairo.Context cr, Control2D control)
		{
			if (!(control is Button || control is ToolBar))
				return;
			
			if (control.Pane != null && control.Pane is AnchorPane) // the control is anchored
			{
				if (!(control is Button) || control.HitState != HitState.None)
					RenderBackground(cr, control, (control.Pane as AnchorPane).Location);
			}
			else // the control is not anchored
			{
				RenderBackground(cr, control, DefaultBackgroundLocation);
			}
		}
		
		/// <summary>
		/// Renders the background with the given location.
		/// </summary>
		protected void RenderBackground(Cairo.Context cr, Control2D control, AnchorLocation location)
		{			
			cr.Save();
			
			// create the gradient
			Cairo.LinearGradient grad = null;
			switch (location)
			{
			case AnchorLocation.E:
				grad = new Cairo.LinearGradient(control.Width, 0, 0, 0);
				break;
			case AnchorLocation.NE:
				grad = new Cairo.LinearGradient(control.Width, 0, 0, control.Height);
				break;
			case AnchorLocation.N:
				grad = new Cairo.LinearGradient(0, 0, 0, control.Height);
				break;
			case AnchorLocation.NW:
				grad = new Cairo.LinearGradient(0, 0, control.Width, control.Height);
				break;
			case AnchorLocation.W:
				grad = new Cairo.LinearGradient(0, 0, control.Width, 0);
				break;
			case AnchorLocation.SW:
				grad = new Cairo.LinearGradient(0, control.Height, control.Width, 0);
				break;
			case AnchorLocation.S:
				grad = new Cairo.LinearGradient(0, control.Height, 0, 0);
				break;
			case AnchorLocation.SE:
				grad = new Cairo.LinearGradient(control.Width, control.Height, 0, 0);
				break;
			}
			
			// assign the colors
			var startColor = BackgroundStartColors[control.HitState];
			if (startColor == null)
				startColor = defaultBackgroundColor;
			grad.AddColorStop(0, startColor.Cairo);
			var stopColor = BackgroundStopColors[control.HitState];
			if (stopColor == null)
				stopColor = defaultBackgroundColor;
			grad.AddColorStop(1, stopColor.Cairo);
			
//			Console.WriteLine("start color: {0}, stop color: {1}", startColor, stopColor);
			cr.Operator = Cairo.Operator.Source;
			var point = cr.CurrentPoint;
			cr.Pattern = grad;
			cr.Rectangle(point, control.Width, control.Height);
			cr.Fill();
			cr.MoveTo(point);
			cr.Restore();
		}
		
#endregion
		
	}
}
