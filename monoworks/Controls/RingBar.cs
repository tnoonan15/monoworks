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
			OuterRadius = 72;
			InnerRadius = 30;
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

		/// <summary>
		/// The angle span of the child button.
		/// </summary>
		public Angle DiffAngle { get; private set; }


		#region Rendering
				
		public override void ComputeGeometry()
		{
			//base.ComputeGeometry();
			
			RenderWidth = OuterRadius * 2;
			RenderHeight = OuterRadius * 2;

			DiffAngle = Angle.TwoPi / NumChildren;
			var currentAngle = new Angle();
			foreach (var child in Children)
			{
				child.CenterAngle = currentAngle;
				currentAngle += DiffAngle;
				child.ComputeGeometry();
			}
		}

		/// <summary>
		/// Draws the outlines to the given context.
		/// </summary>
		/// <remarks>This can be used by decorators to avoid doing the geometry themselves.</remarks>
		public void DrawOutlines(Cairo.Context cr)
		{
			// outer circle
			cr.MoveTo(OuterRadius * 2 - 1, OuterRadius);
			cr.Arc(OuterRadius, OuterRadius, OuterRadius - 1, 0, 2 * Math.PI);
			cr.Stroke();

			// inner circle
			cr.MoveTo(OuterRadius + InnerRadius - 1, OuterRadius);
			cr.Arc(OuterRadius, OuterRadius, InnerRadius - 1, 0, 2 * Math.PI);
			cr.Stroke();

			// division lines
			var startAngle = DiffAngle / 2;
			var lineStart = new Coord(0, InnerRadius).Rotate(startAngle);
			var lineStop = new Coord(0, OuterRadius).Rotate(startAngle);
			for (int i = 0; i < NumChildren; i++)
			{
				cr.MoveTo(OuterRadius + lineStart.X, OuterRadius + lineStart.Y);
				cr.LineTo(OuterRadius + lineStop.X, OuterRadius + lineStop.Y);
				cr.Stroke();
				lineStart = lineStart.Rotate(DiffAngle);
				lineStop = lineStop.Rotate(DiffAngle);
			}

		}

		#endregion

	}
}
