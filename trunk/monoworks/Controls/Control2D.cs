// Control2D.cs - MonoWorks Project
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

using Cairo;

using MonoWorks.Base;
using MonoWorks.Rendering;
using MonoWorks.Rendering.Events;
using System.Runtime.InteropServices;



namespace MonoWorks.Controls
{	
	
	/// <summary>
	/// Handler for simple control events.
	/// </summary>
	public delegate void ControlEventHandler(Renderable2D control);
	
	
	/// <summary>
	/// Base class for all renderable controls.
	/// </summary>
	public abstract class Control2D : Renderable2D
	{
		protected Control2D()
			: base()
		{
		}
		
		
		#region Rendering

		
		/// <summary>
		/// Renders the control to a Cairo context.
		/// </summary>
		/// <remarks>Calls Render() internally.</remarks>
		public void RenderCairo(RenderContext context)
		{
			if (IsDirty)
				ComputeGeometry();
			context.Cairo.RelMoveTo(Origin.X, Origin.Y);
			
			var point = context.Cairo.CurrentPoint;
			LastPosition = new Coord(point.X, point.Y);
			
			context.Decorator.Decorate(this);
			
			Render(context);
			
			context.Cairo.RelMoveTo(-Origin.X, -Origin.Y);
						
		}

		/// <summary>
		/// Performs the 2D rendering of the control.
		/// </summary>
		protected virtual void Render(RenderContext rc)
		{
		}
		

		#endregion
				
				
		#region Image Data

		private GCHandle _gch;
		
		/// <summary>
		/// Renders the control to an internal image surface.
		/// </summary>
		public override void RenderImage(Scene scene)
		{
			if (IsDirty)
				ComputeGeometry();

			// nothing to see here, folks. move along
			if (IntWidth == 0 || IntHeight == 0)
				return;
			
			// remake the image surface, if needed
			if (_surface == null || _surface.Width != IntWidth || _surface.Height != IntHeight)
			{
				if (_surface != null)
				{
					_surface.Destroy();
					_gch.Free();
				}
				_imageData = new byte[IntWidth * IntHeight * 4];
				_gch = GCHandle.Alloc(_imageData, GCHandleType.Pinned);
				_surface = new ImageSurface(ref _imageData, Format.ARGB32, IntWidth, IntHeight, 4 * IntWidth); // windows
				//_surface = new ImageSurface(_imageData, Format.ARGB32, IntWidth, IntHeight, 4 * IntWidth);
				
			}
			
			// render the control to the surface
			using (var cr = new Context(_surface))
			{
				cr.Operator = Operator.Source;
				cr.Color = new Cairo.Color(1, 1, 1, 0);
				cr.Paint();

				cr.Operator = Operator.Over;
				cr.MoveTo(0,0);
				RenderCairo(new RenderContext(cr, DecoratorService.Get(scene)));
				
				_surface.Flush();
			};
			
		}
		
		private ImageSurface _surface;
		
		#endregion
		
		
	}
}
