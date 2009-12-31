// 
//  Ease.cs
//  
//  Author:
//       Andy Selvig <ajselvig@gmail.com>
// 
//  Copyright (c) 2009 Andy Selvig
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

namespace MonoWorks.Rendering
{
	/// <summary>
	/// Types of easing that can be performed by the Ease methods.
	/// </summary>
	public enum EaseType {Linear, Quadratic, Cubic};
	
	/// <summary>
	/// Possible directions of animation easing.
	/// </summary>
	public enum EaseDirection {In, Out, InOut};

	/// <summary>
	/// Provides easing functions for animations.
	/// </summary>
	public static class Ease
	{
		/// <summary>
		/// Provides an easing factor for the given animation progress.
		/// </summary>
		/// <param name="progress"> The fractional progress of the animation (from 0 to 1). </param>
		/// <param name="type"> The type of easing to perform. </param>
		/// <param name="direction"> The direction of the animation. </param>
		/// <returns> The factor to use in the animation. </returns>
		public static double Factor(double progress, EaseType type, EaseDirection direction)
		{
			if (direction == EaseDirection.In)
				return InFactor(progress, type);
			else if (direction == EaseDirection.Out)
				return OutFactor(progress, type);
			else
				return InOutFactor(progress, type);
		}
		
		/// <summary>
		/// Provides an ease-in factor for the given animation progress.
		/// </summary>
		/// <param name="progress"> The fractional progress of the animation (from 0 to 1). </param>
		/// <param name="type"> The type of easing to perform. </param>
		/// <returns> The factor to use in the animation. </returns>
		public static double InFactor(double progress, EaseType type)
		{
			switch (type)
			{
			case EaseType.Linear:
				return progress;
			case EaseType.Quadratic:
				return progress * progress;
			case EaseType.Cubic:
				return progress * progress * progress;
			}
			throw new NotImplementedException("Don't know how to ease in for " + type.ToString());
		}
		
		/// <summary>
		/// Provides an ease-out factor for the given animation progress.
		/// </summary>
		/// <param name="progress"> The fractional progress of the animation (from 0 to 1). </param>
		/// <param name="type"> The type of easing to perform. </param>
		/// <returns> The factor to use in the animation. </returns>
		public static double OutFactor(double progress, EaseType type)
		{
			switch (type)
			{
			case EaseType.Linear:
				return progress;
			case EaseType.Quadratic:
				return 2 * progress - progress * progress;
			case EaseType.Cubic:
				return progress * (progress * progress - 3*progress + 3);
			}
			throw new NotImplementedException("Don't know how to ease out for " + type.ToString());
		}
		
		/// <summary>
		/// Provides an ease-in-out factor for the given animation progress.
		/// </summary>
		/// <param name="progress"> The fractional progress of the animation (from 0 to 1). </param>
		/// <param name="type"> The type of easing to perform. </param>
		/// <returns> The factor to use in the animation. </returns>
		/// <remarks> This basically uses the in-factor for the first half
		/// and the out-factor for the second half.</remarks>
		public static double InOutFactor(double progress, EaseType type)
		{
			if (progress < 0.5)
				return InFactor(progress*2, type) / 2.0;
			else
				return OutFactor(progress*2 - 1, type) / 2.0 + 0.5;
		}
		
	}
}
