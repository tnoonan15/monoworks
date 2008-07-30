using System;
using System.Collections.Generic;

using gl = Tao.OpenGl.Gl;

using MonoWorks.Base;

namespace MonoWorks.Plotting
{
	/// <summary>
	/// Plots a set of points from an ArrayDataSet.
	/// </summary>
	public class PointPlot : Plottable
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		/// <param name="parent"> The parent axes box.</param>
		public PointPlot(AxesBox parent)
			: base(parent)
		{
			color = new MonoWorks.Rendering.Color(0F, 0F, 1F);
		}


		protected ArrayDataSet dataSet = null;
		/// <summary>
		/// The data set to use.
		/// </summary>
		public ArrayDataSet DataSet
		{
			get { return dataSet; }
			set { dataSet = value; }
		}


		#region The Column Indices

		protected int[] indices = new int[]{0, 1, 2};
		/// <summary>
		/// The indices of the columns to plot.
		/// </summary>
		public int[] Indices
		{
			get { return indices; }
			set { indices = value; }
		}

		/// <summary>
		/// The index of the x column.
		/// </summary>
		public int ColumnX
		{
			get { return indices[0]; }
			set { indices[0] = value; }
		}

		/// <summary>
		/// The index of the y column.
		/// </summary>
		public int ColumnY
		{
			get { return indices[1]; }
			set { indices[1] = value; }
		}

		/// <summary>
		/// The index of the z column.
		/// </summary>
		public int ColumnZ
		{
			get { return indices[2]; }
			set { indices[2] = value; }
		}

		#endregion


		#region Geometry

		/// <summary>
		/// Update the bounds to fit everything in.
		/// </summary>
		public override void UpdateBounds()
		{
			base.UpdateBounds();

			Vector minima = new Vector();
			Vector maxima = new Vector();
			for (int i = 0; i < indices.Length; i++)
			{
				if (indices[i] < 0 || indices[i] >= dataSet.NumColumns)
					throw new Exception(String.Format("Index {0} is out of range", indices[i]));

				double min, max;
				dataSet.ColumnMinMax(indices[i], out min, out max);
				minima[i] = min;
				maxima[i] = max;
			}
			plotBounds.Minima = minima;
			plotBounds.Maxima = maxima;

		}


		/// <summary>
		/// Compute the plot geometry.
		/// </summary>
		public override void ComputeGeometry()
		{
			base.ComputeGeometry();

			// generate the display list
			gl.glNewList(displayList, gl.GL_COMPILE);


			gl.glPointSize(3F);
			gl.glBegin(gl.GL_POINTS);
			for (int r = 0; r < dataSet.NumRows; r++ )
			{
				double x, y, z;
				x = dataSet[r, indices[0]];
				y = dataSet[r, indices[1]];
				z = dataSet[r, indices[2]];
				parent.PlotToWorldSpace.Apply(ref x, ref y, ref z);
				//Console.WriteLine("adding vertex at {0}, {1}, {2}", x, y, z);
				gl.glVertex3d(x, y, z);
			}
			gl.glEnd();

			gl.glEndList();
		}


		public override void RenderOpaque(MonoWorks.Rendering.IViewport viewport)
		{
			base.RenderOpaque(viewport);

			if (gl.glIsList(displayList) != 0)
				gl.glCallList(displayList);

		}


		#endregion

	}
}
