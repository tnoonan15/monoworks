//   BoxedSketchable.cs - MonoWorks Project
//
//    Copyright Andy Selvig 2008
//
//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU Lesser General Public License as published 
//    by the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU Lesser General Public License for more details.
//
//    You should have received a copy of the GNU Lesser General Public 
//    License along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using MonoWorks.Base;
using MonoWorks.Rendering;
using MonoWorks.Rendering.Events;

using gl = Tao.OpenGl.Gl;

namespace MonoWorks.Modeling.Sketching
{
	/// <summary>
	/// A sketchable that has four corners that define its size and position.
	/// </summary>
	public class BoxedSketchable : Sketchable
	{

		public BoxedSketchable(Sketch sketch)
			: base(sketch)
		{

			Anchor2 = null;
		}


#region The Anchors

		/// <summary>
		/// First anchor of the sketchable.
		/// </summary>
		[MwxProperty("anchor1")]
		public Point Anchor1
		{
			get { return GetAttribute("anchor1") as Point; }
			set { SetAttribute("anchor1", value); }
		}

		/// <summary>
		/// Second anchor of the sketchable.
		/// </summary>
		[MwxProperty("anchor2")]
		public Point Anchor2
		{
			get { return GetAttribute("anchor2") as Point; }
			set { SetAttribute("anchor2", value); }
		}

		/// <summary>
		/// Inverts the corners that the anchors are on.
		/// </summary>
		public void InvertAnchors()
		{
			Anchor1.SetPosition(solidPoints[1]);
			Anchor2.SetPosition(solidPoints[3]);
		}

		/// <summary>
		/// Call this method to let the rectangle know that the values of the anchors may have changed.
		/// </summary>
		public void AnchorsUpdated()
		{
			RaiseAttributeUpdated("anchor1");
			RaiseAttributeUpdated("anchor2");
		}

#endregion

		/// <summary>
		/// The tilt from the x axis.
		/// </summary>
		[MwxProperty("tilt")]
		public Angle Tilt
		{
			get { return GetAttribute("tilt") as Angle; }
			set { SetAttribute("tilt", value); }
		}

		/// <summary>
		/// The center of the box.
		/// </summary>
		public Point Center
		{
			get { return (Anchor1 + Anchor2) / 2.0; }
		}

	}
}
