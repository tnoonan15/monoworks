// Plottable.cs - MonoWorks Project
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

using gl = Tao.OpenGl.Gl;

using MonoWorks.Rendering;

namespace MonoWorks.Plotting
{

	/// <summary>
	/// Base class for plottable objects.
	/// </summary>
	public abstract class Plottable : Actor
	{

		/// <summary>
		/// Default constructor.
		/// </summary>
		public Plottable(AxesBox parent)
			: base()
		{
			ParentAxes = parent;
			if (parent != null)
				parent.AddChild(this);
		}

		~Plottable()
		{
			// Windows doesn't like this
			//ClearDisplayList();
		}

		/// <value>
		/// The parent axes.
		/// </value>
		public AxesBox ParentAxes { get; private set; }

        protected string title = "";
        /// <summary>
        /// The title of the plottable.
        /// </summary>
        public virtual string Title
        {
            get { return title; }
            set { title = value; }
        }

		/// <summary>
		/// Makes the parent axes box dirty also.
		/// </summary>
		public override void MakeDirty()
		{
			if (ParentAxes != null)
				ParentAxes.MakeDirty();
			base.MakeDirty();
		}


		protected Color _color = new Color();
		/// <summary>
		/// The color.
		/// </summary>
		public Color Color
		{
			get { return _color; }
			set { _color = value; }
		}




#region Bounds

		/// <summary>
		/// Updates the plot bounds based on the children.
		/// </summary>
		public virtual void UpdateBounds()
		{

		}

		protected Bounds _plotBounds = new Bounds();
		/// <summary>
		/// The bounds of the plottable in plot space.
		/// </summary>
		/// <remarks> This differs from bounds, which is in rendering space.</remarks>
		public Bounds PlotBounds
		{
			get { return _plotBounds; }
			set { _plotBounds = value; }
		}

#endregion



#region Geometry

		/// <summary>
		/// Each plottable gets a display list.
		/// </summary>
		protected int _displayList = 0;

		/// <summary>
		/// Ensures the display list is cleared.
		/// </summary>
		/// <remarks> Does nothing if this list hasn't been set yet.</remarks>
		public void ClearDisplayList()
		{
			if (gl.glIsList(_displayList) != 0)
			{
				gl.glDeleteLists(_displayList, 1);
			}
		}
		
		
		/// <summary>
		/// Calls the plottable display list if it's valid.
		/// </summary>
		public void CallDisplayList()
		{			
			if (gl.glIsList(_displayList) != 0)
				gl.glCallList(_displayList);
		}

		public override void ComputeGeometry()
		{
			base.ComputeGeometry();
			ClearDisplayList();

		}

#endregion



#region Rendering


		public override void RenderOpaque(Scene scene)
		{
			base.RenderOpaque(scene);

			_color.Setup();
		}


		public override void RenderOverlay(Scene scene)
		{
			base.RenderOverlay(scene);

			_color.Setup();
		}

#endregion


	}
}
