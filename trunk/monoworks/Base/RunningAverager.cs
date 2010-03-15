// 
//  RunningAverager.cs - MonoWorks Project
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
using System.Collections.Generic;

namespace MonoWorks.Base
{
	/// <summary>
	/// Performs a runnning average on a set of doubles.
	/// </summary>
	public class RunningAverager
	{
		public RunningAverager(int num)
		{
			NumToAverage = num;
			_queue = new Queue<double>(num);
		}

		/// <summary>
		/// The number to include in the average.
		/// </summary>
		public int NumToAverage { get; set; }
		
		private Queue<double> _queue;
		
		/// <summary>
		/// Adds a value to the running average.
		/// </summary>
		public void Add(double val)
		{
			_queue.Enqueue(val);
			if (_queue.Count > NumToAverage)
				_queue.Dequeue();
		}
		
		/// <summary>
		/// Computes the average to the current set of points.
		/// </summary>
		public double Compute()
		{
			double total = 0;
			foreach (var val in _queue)
			{
				total += val;
			}
			return total / (double)_queue.Count;
		}
		
	}
}

