// 
//  RingBar.cs - MonoWorks Project
//  
//  Author:
//       Andy Selvig <ajselvig@gmail.com>
// 
//  Copyright (c) 2010 Andy Selvig
// 
//  This library is free software; you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as
//  published by the Free Software Foundation; either version 2.1 of the
//  License, or (at your option) any later version.
// 
//  This library is distributed in the hope that it will be useful, but
//  WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
//  Lesser General Public License for more details.
// 
//  You should have received a copy of the GNU Lesser General Public
//  License along with this library; if not, write to the Free Software
//  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA

using System;

using MonoWorks.Base;

namespace MonoWorks.Controls
{
	/// <summary>
	/// Arranges a collection of ring buttons into a circle.
	/// </summary>
	public class RingBar : GenericContainer<RingButton>
	{

		public RingBar()
		{
		}
		
		
		/// <summary>
		/// The outer radius of the ring.
		/// </summary>
		[MwxProperty]
		public double OuterRadius {get; set;}

		/// <summary>
		/// The inner radius of the ring.
		/// </summary>
		[MwxProperty]
		public double InnerRadius { get; set; }
		
		#region Rendering
				
		public override void ComputeGeometry()
		{
			base.ComputeGeometry();
			
			RenderWidth = OuterRadius * 2;
			RenderHeight = OuterRadius * 2;
		}

		protected override void Render(RenderContext rc)
		{
			base.Render(rc);
			
			rc.Push();
			rc.Cairo.Color = new Cairo.Color(0, 0, 1);
			rc.Cairo.LineWidth = 1;
			rc.Cairo.MoveTo(OuterRadius * 2, OuterRadius);
			rc.Cairo.Arc(OuterRadius, OuterRadius, OuterRadius, 0, 2 * Math.PI);
			rc.Cairo.Stroke();
			rc.Cairo.MoveTo(OuterRadius + InnerRadius , OuterRadius);
			rc.Cairo.Arc(OuterRadius, OuterRadius, InnerRadius, 0, 2 * Math.PI);
			rc.Cairo.Stroke();
			rc.Pop();
		}

		
		#endregion
		
	}
}
