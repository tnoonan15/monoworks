// Animator.cs - MonoWorks Project
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
using System.Timers;


namespace MonoWorks.Rendering
{
	/// <summary>
	/// Provides animation functionality to a viewport.
	/// </summary>
	public class Animator
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		/// <param name="viewport">The viewport to animate on.</param>
		public Animator(Viewport viewport)
		{
			this.viewport = viewport;

			interval = 0.1;
			timer = new Timer(1/interval);
			timer.Elapsed += OnTick;
		}

		protected double interval;

		protected Viewport viewport;

		protected Timer timer;

		/// <summary>
		/// The duration for each animatable.
		/// </summary>
		protected Dictionary<IAnimatable, double> durations = new Dictionary<IAnimatable, double>();

		/// <summary>
		/// The current times for each animatable.
		/// </summary>
		protected Dictionary<IAnimatable, double> times = new Dictionary<IAnimatable, double>();

		/// <summary>
		/// Register an animation.
		/// </summary>
		/// <param name="animatable">The animating object.</param>
		/// <param name="duration">The duration of the animation.</param>
		public void RegisterAnimation(IAnimatable animatable, double duration)
		{
			durations[animatable] = duration;
			times[animatable] = 0;
			timer.Start();
		}

		/// <summary>
		/// Remove an animation and stop the timer if there are none left.
		/// </summary>
		/// <param name="animatable"></param>
		public void RemoveAnimation(IAnimatable animatable)
		{
			durations.Remove(animatable);
			times.Remove(animatable);
			animatable.EndAnimation();
			if (durations.Count == 0)
				timer.Stop();
		}

		/// <summary>
		/// Handles the timer tick.
		/// </summary>
		void OnTick(object sender, ElapsedEventArgs e)
		{
			IAnimatable[] animatables = new IAnimatable[durations.Count];
			durations.Keys.CopyTo(animatables, 0);
			foreach (IAnimatable animatable in animatables)
			{
				times[animatable] += interval;
				animatable.Animate(times[animatable] / durations[animatable]);
				if (times[animatable] >= durations[animatable])
					RemoveAnimation(animatable);
			}
			viewport.PaintGL();
		}

	}
}
