// 
//  ProgressDial.cs - MonoWorks Project
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

using MonoWorks.Rendering;

namespace MonoWorks.Controls
{
	/// <summary>
	/// Progress indicator that represents progress as an angle.
	/// </summary>
	public class ProgressDial : ProgressIndicator
	{
		public ProgressDial()
		{
		}

		
		/// <summary>
		/// Default inner radius.
		/// </summary>
		private const double DefaultInnerRadius = 10;
				
		private double _innerRadius = DefaultInnerRadius;
		/// <summary>
		/// The radius of the center part of the dial.
		/// </summary>
		[MwxProperty]
		public double InnerRadius
		{
			get { return _innerRadius; }
			set {
				_innerRadius = value;
				MakeDirty();
			}
		}
		
		/// <summary>
		/// Minimum radius if the user doesn't define one.
		/// </summary>
		private double _minRadius = 32;

		public override void ComputeGeometry()
		{
			base.ComputeGeometry();
			
			// try to make the dial a circle
			double radius = _minRadius;
			if (UserSize != null)
				radius = Math.Min(UserSize.X / 2.0, UserSize.Y / 2.0);
			
			RenderSize.X = radius * 2;
			RenderSize.Y = radius * 2;
			
		}
	}
}

