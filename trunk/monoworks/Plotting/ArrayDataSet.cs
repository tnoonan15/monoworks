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
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.IO;


namespace MonoWorks.Plotting
{
	/// <summary>
	/// Possible column types when parsing a file. 
	/// </summary>
	/// <remarks>These do not yet have any meaning inside the data set.</remarks>
	public enum ColumnType { Double, Time };


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

		/// <summary>
		/// Adds the given number of columns to the data set.
		/// </summary>
		public void AddColumns(int num)
		{
			if (NumRows == 0)
				throw new Exception("The data set must be non-empty before you can add columns.");

			// make the new data
			double[,] data_ = new double[NumRows, NumColumns + num];
			for (int r = 0; r < NumRows; r++)
			{
				for (int c = 0; c < NumColumns; c++)
				{
					data_[r, c] = data[r, c];
				}
			}
			int numColumns = NumColumns;
			data = data_;

			// add the column names
			for (int c = numColumns; c < NumColumns; c++)
			{
				columnNames.Add("column " + c.ToString());
			}
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
					if (Double.IsNaN(data[r, col]))
						continue;
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
				if (!Double.IsNaN(data[r,col]) && (data[r, col] < min || data[r,col] > max))
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
			if (index < 0 || index >= NumColumns)
				throw new Exception(index.ToString() + " is not a valid index.");

			columnNames[index] = name;
			RaiseChanged();
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

		/// <summary>
		/// Renames column oldName to newName.
		/// </summary>
		public void Rename(string oldName, string newName)
		{
			for (int c = 0; c < columnNames.Count; c++)
			{
				if (columnNames[c] == oldName)
					columnNames[c] = newName;
			}
		}

#endregion


		#region File I/O



		/// <summary>
		/// Reads data from a delimited text file.
		/// </summary>
		/// <remarks>The delimiter will be automagically determined (can be tabs or commas).</remarks>
		public void FromFile(string filePath)
		{
			StreamReader reader = File.OpenText(filePath);

			try
			{

				// get the header and determine delimiter
				char delimiter = 'n';
				char[] delimiters = new char[] { ',', '\t' };
				string headerLine = reader.ReadLine();
				foreach (char delim in delimiters)
				{
					if (headerLine.Contains(delim))
						delimiter = delim;
				}
				if (delimiter == 'n')
					throw new Exception("Could not find a valid delimiter (tab or space) in the file header.");
				string[] headers = headerLine.Split(delimiter);
				int numCols = headers.Length;
				ColumnType[] columnTypes = new ColumnType[numCols];

				// read the rows
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

						// determine the column types
						if (rows.Count == 1) // this is the first row
						{
							for (int c = 0; c < numCols; c++)
							{
								if (row[c].Contains(":"))
									columnTypes[c] = ColumnType.Time;
								else
									columnTypes[c] = ColumnType.Double;
							}
						}
					}

					if (reader.EndOfStream)
						keepGoing = false;
				}

				// initialize
				SetSize(rows.Count, Math.Max(numCols, 3));// there needs to be at least 3 columns for plotting to work
				columnNames.Clear();
				foreach (string header in headers)
					columnNames.Add(header);
				for (int c = numCols; c < NumColumns; c++) // fill in empty columns
					columnNames.Add("zeros " + (c-numCols).ToString());

				// parse the data
				for (int r = 0; r < rows.Count; r++)
				{
					for (int c = 0; c < numCols; c++)
					{
						if (columnTypes[c] == ColumnType.Time)
						{
							try
							{
								data[r, c] = DateTime.Parse(rows[r][c]).TimeOfDay.TotalSeconds;
							}
							catch (Exception)
							{
								throw new Exception(String.Format("Value {0} ({1}, {2}) is not a valid time.", rows[r][c], r, c));
							}
						}
						else //double
						{
							try
							{
								if (rows[r][c].Length == 0) // treat empty values as NaN
									data[r, c] = Double.NaN;
								else
									data[r, c] = Double.Parse(rows[r][c]);
							}
							catch (Exception)
							{
								throw new Exception(String.Format("Value {0} ({1}, {2}) is not a valid double.", rows[r][c], r, c));
							}
						}
					}
				}

			}
			catch (Exception ex)
			{
				throw ex;
			}
			finally
			{
				reader.Close();
			}
		}

		/// <summary>
		/// Writes the data set to a file.
		/// </summary>
		public void ToFile(string filePath)
		{
			char delimiter = '\t'; // do we need to let the user decide?

			StreamWriter writer = new StreamWriter(filePath);

			// write the header
			foreach (var name in columnNames)
			{
				writer.Write(name);
				writer.Write(delimiter);
			}
			writer.WriteLine();

			// write the data
			for (int r = 0; r < NumRows; r++)
			{
				for (int c = 0; c < NumColumns; c++)
				{
					if (!Double.IsNaN(data[r, c]))
						writer.Write(data[r, c]);
					writer.Write(delimiter);
				}
				writer.WriteLine();
			}

			writer.Close();
		}

#endregion


#region Bit Parsing

		/// <summary>
		/// Parse the given column into bits and store their values 
		/// into new columns with automatically generated names.
		/// </summary>
		/// <param name="colName">The name of the column to parse.</param>
		public void ParseBits(string colName)
		{
			int col = columnNames.IndexOf(colName);
			if (col >= 0)
				ParseBits(col);
			else
				throw new Exception(colName + " is an invalid column name.");
		}

		/// <summary>
		/// Parse the given column into bits and store their values 
		/// into new columns with automatically generated names.
		/// </summary>
		/// <param name="col">The index of the column to parse.</param>
		public void ParseBits(int col)
		{
			if (col < 0 || col >= NumColumns)
				throw new Exception(String.Format("column {0} is out of bounds", col));

			// determine the number of bits needed
			double min, max;
			ColumnMinMax(col, out min, out max);
			int numBits = (int)Math.Ceiling(Math.Log(max, 2));

			// add the columns
			int startCol = NumColumns;
			AddColumns(numBits);

			for (int c = 0; c < numBits; c++)
			{
				columnNames[c + startCol] = columnNames[col] + " bit" + c.ToString();
				int colBit = (int)Math.Pow(2, c);
				for (int r = 0; r < NumRows; r++)
				{
					if (Double.IsNaN(data[r, c]))
						continue;
					if (((int)data[r, col] & colBit) == colBit)
						data[r, c + startCol] = 1;
					else
						data[r, c + startCol] = 0;
				}
			}

			RaiseChanged();
		}

#endregion


	}
}
