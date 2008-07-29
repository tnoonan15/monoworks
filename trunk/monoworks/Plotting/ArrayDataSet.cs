using System;
using System.Collections.Generic;


namespace MonoWorks.Plotting
{
	/// <summary>
	/// Data set containing an array of data to be used on 
	/// </summary>
	public class ArrayDataSet : DataSet
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public ArrayDataSet()
			: this(1024, 3)
		{
		}

		/// <summary>
		/// Initialization constructor. 
		/// </summary>
		/// <param name="rows"> The number of rows.</param>
		/// <param name="cols"> The number of columns.</param>
		public ArrayDataSet(int rows, int cols)
			: base()
		{
			data = new double[rows, cols];
		}


		/// <summary>
		/// Sets the size of the array.
		/// </summary>
		/// <param name="rows"> The number of rows.</param>
		/// <param name="cols"> The number of columns.</param>
		/// <remarks> This will only destroy data if you're making it smaller.</remarks>
		public void SetSize(int rows, int cols)
		{
			// TODO: copy old data over
			data = new double[rows, cols];
		}

		/// <summary>
		/// The number of columns.
		/// </summary>
		public int NumColumns
		{
			get { return data.GetLength(1); }
		}

		/// <summary>
		/// The number of rows.
		/// </summary>
		public int NumRows
		{
			get { return data.GetLength(0); }
		}


		#region The Data

		/// <summary>
		/// The data.
		/// </summary>
		protected double[,] data;

		/// <summary>
		/// Access the array data by index.
		/// </summary>
		/// <param name="row"></param>
		/// <param name="col"></param>
		/// <returns></returns>
		public double this[int row, int col]
		{
			get
			{
				return data[row, col];
			}
			set
			{
				data[row, col] = value;
			}
		}

		/// <summary>
		/// Computes the min and max of a column.
		/// </summary>
		/// <param name="col"> The column index.</param>
		/// <param name="min"> The column min.</param>
		/// <param name="max"> the column max.</param>
		public void ColumnMinMax(int col, out double min, out double max)
		{
			if (NumRows > 0)
			{
				if (col >= NumColumns)
					throw new Exception("column index is out of range.");

				min = data[0, col];
				max = data[0, col];
				for (int r = 1; r < NumRows; r++)
				{
					min = Math.Min(min, data[r, col]);
					max = Math.Max(max, data[r, col]);
				}
			}
			else
				throw new Exception("The array must have data to compute min and max.");
		}

		#endregion


	}
}
