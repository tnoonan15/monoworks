// ArrayDataSet.cs - MonoWorks Project
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
using System.IO;


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
			SetSize(rows, cols);
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

		protected PlotIndex displayIndex = null;
		/// <value>
		/// The indices of the values to display.
		/// </value>
		public PlotIndex DisplayIndex
		{
			get {return displayIndex;}
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
		/// Sets the size of the array.
		/// </summary>
		/// <param name="rows"> The number of rows.</param>
		/// <param name="cols"> The number of columns.</param>
		/// <remarks> This will only destroy data if you're making it smaller.</remarks>
		public void SetSize(int rows, int cols)
		{
			// TODO: copy old data over
			data = new double[rows, cols];
			
			// resize the display index
			if (displayIndex == null)
				displayIndex = new PlotIndex(rows);
			else
				displayIndex.Resize(rows);
			
			// add extra column names
			while (columnNames.Count < cols)
			{
				columnNames.Add("column" + columnNameCount.ToString());
				columnNameCount++;
			}
			
			// remove unecessary column names
			if (cols < columnNames.Count)
				columnNames.RemoveRange(cols, columnNames.Count - cols - 1);
		}
		
#endregion


#region Column Info

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
		
		/// <summary>
		/// Gets the index of points in the given column that fall between min and max.
		/// </summary>
		/// <param name="col"> </param>
		/// <param name="min"> </param>
		/// <param name="max"> </param>
		/// <returns> </returns>
		public PlotIndex GetColumnIndex(int col, double min, double max)
		{
			if (col < 0 || col > NumColumns)
				throw new Exception("Column out of bounds.");
			
			PlotIndex index = new PlotIndex(NumRows);			
			for (int r=0; r<NumRows; r++)
			{
				if (data[r, col] < min || data[r,col] > max)
					index[r] = false;
			}
			return index;
		}

#endregion


#region Column Names

		protected List<string> columnNames = new List<string>();
		/// <summary>
		/// The column names.
		/// </summary>
		public IEnumerable<string> ColumnNames
		{
			get {return columnNames;}
		}
		
		/// <summary>
		/// Counts the number of unique column names created.
		/// </summary>
		protected int columnNameCount = 0;
		
		/// <summary>
		/// Sets the name of the given column.
		/// </summary>
		/// <param name="index"> The column index. </param>
		/// <param name="name"> The column's name. </param>
		public void SetColumnName(int index, string name)
		{
			if (index < 0 || index >= NumRows)
				throw new Exception(index.ToString() + " is not a valid index.");
			
			columnNames[index] = name;
		}
		
		/// <summary>
		/// Gets the column name for a given index.
		/// </summary>
		/// <param name="index"> The column index. </param>
		public string GetColumnName(int index)
		{
			if (index < 0 || index >= NumColumns)
				throw new Exception(index.ToString() + " is not a valid index.");
			return columnNames[index];
		}
		
#endregion



#region File I/O

		/// <summary>
		/// Loads a data set from a delimited text file.
		/// </summary>
		/// <param name="filePath"></param>
		/// <param name="delimiter"></param>
		/// <returns></returns>
		//public static ArrayDataSet FromFile(string filePath, string delimiter)
		//{
		//    ArrayDataSet dataSet = new ArrayDataSet();
		//    dataSet.FromFile(filePath, delimiter);
		//    return dataSet;
		//}

		/// <summary>
		/// Reads data from a delimited text file.
		/// </summary>
		/// <param name="filePath"></param>
		/// <param name="delimiter"></param>
		public void FromFile(string filePath, char delimiter)
		{
			StreamReader reader = File.OpenText(filePath);

			// parse the header
			string headerLine = reader.ReadLine();
			string[] headers = headerLine.Split(delimiter);
			int numCols = headers.Length;

            // read the rows
            long pos = reader.BaseStream.Position;
            bool keepGoing = true;
            List<string[]> rows = new List<string[]>();
            while (keepGoing)
            {
                string line = reader.ReadLine();
                if (line == null || line.Length == 0)
                    keepGoing = false;
                else
                {
                    string[] row = line.Split(delimiter);
                    if (row.Length != numCols) 
                        throw new IOException(String.Format("Row {0} has {1} components when it should have {2}.", rows.Count, row.Length, numCols));
                    rows.Add(row);
                }

                if (reader.EndOfStream)
                    keepGoing = false;
            }

            // initialize
            SetSize(rows.Count, numCols);
            columnNames.Clear();
            foreach (string header in headers)
                columnNames.Add(header);

            // parse the data
            for (int r=0; r<rows.Count; r++)
            {
                for (int c = 0; c < numCols; c++)
                    data[r, c] = Double.Parse(rows[r][c]);
            }

			reader.Close();
		}



#endregion


	}
}
