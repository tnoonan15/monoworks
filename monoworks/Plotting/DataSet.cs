// DataSet.cs - MonoWorks Project
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



namespace MonoWorks.Plotting
{
	/// <summary>
	/// Base class for plotting data sets.
	/// </summary>
	public abstract class DataSet
	{

		public DataSet()
		{
		}

		/// <summary>
		/// Handler for the Changed event.
		/// </summary>
		public delegate void DataSetChangedHandler(DataSet dataSet);

		/// <summary>
		/// This gets raised whenever the size, arrangement, or configuration of the dataset is changed.
		/// </summary>
		/// <remarks>This will not necessarily get called when values inside the data set changed.</remarks>
		public event DataSetChangedHandler Changed;

		/// <summary>
		/// Internally used to raise the Changed event.
		/// </summary>
		protected void RaiseChanged()
		{
			if (Changed != null)
				Changed(this);
		}

	}
}
