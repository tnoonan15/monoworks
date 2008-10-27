using System;
using System.Collections.Generic;

namespace MonoWorks.Plotting
{
	/// <summary>
	/// Represents plottables that take up plot space inside an axes.
	/// </summary>
	public abstract class AbstractPlot : Plottable
	{

		public AbstractPlot(AxesBox parent)
			: base(parent)
		{

		}
	}
}
