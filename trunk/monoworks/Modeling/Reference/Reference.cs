// Reference.cs - MonoWorks Project
//
// Copyright (C) 2008 Andy Selvig
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

using MonoWorks.Base;
using MonoWorks.Rendering;

using gl = Tao.OpenGl.Gl;


namespace MonoWorks.Modeling
{
	
	/// <summary>
	/// Reference is the base cass for all reference entities.
	/// </summary>
	public abstract class Reference : Entity
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public Reference() : base()
		{
		}

		/// <summary>
		/// Hit tolerance for reference entities.
		/// </summary>
		public const double HitTol = 6;
				
		/// <summary>
		/// If the reference color is transparent, renders it.
		/// </summary>
		/// <param name="viewport"> A <see cref="Viewport"/> to render to. </param>
		public override void RenderTransparent(Viewport viewport)
		{
			viewport.RenderManager.EnableAntialiasing();
			base.RenderTransparent(viewport);

			Color fill = ModelingOptions.Global.GetColor("ref-fill", hitState);
			if (!fill.IsOpaque)
			{
				fill.Setup();
				RenderFill(viewport);
			}

			Color edge = ModelingOptions.Global.GetColor("ref-edge", hitState);
			if (!edge.IsOpaque)
			{
				edge.Setup();
				if (hitState == HitState.None)
					gl.glLineWidth(1f);
				else
					gl.glLineWidth(2f);
				RenderEdge(viewport);
			}
			viewport.RenderManager.DisableAntialiasing();
		}
		
		/// <summary>
		/// Render the reference item.
		/// </summary>
		/// <param name="viewport"> A <see cref="Viewport"/> to render to. </param>
		public override void RenderOpaque(Viewport viewport)
		{
			viewport.RenderManager.EnableAntialiasing();
			base.RenderOpaque(viewport);

			Color fill = ModelingOptions.Global.GetColor("ref-fill", hitState);
			if (fill.IsOpaque)
			{
				fill.Setup();
				RenderFill(viewport);
			}

			Color edge = ModelingOptions.Global.GetColor("ref-edge", hitState);
			if (edge.IsOpaque)
			{
				edge.Setup();
				if (hitState == HitState.None)
					gl.glLineWidth(1f);
				else
					gl.glLineWidth(2f);
				RenderEdge(viewport);
			}
			viewport.RenderManager.DisableAntialiasing();
		}

		/// <summary>
		/// Subclasses must override to render the filled part of the reference.
		/// </summary>
		public abstract void RenderFill(Viewport viewport);

		/// <summary>
		/// Subclasses must override to render the edge part of the reference.
		/// </summary>
		public abstract void RenderEdge(Viewport viewport);

	}



	/// <summary>
	/// Extension methods to make Rendering interact more directly with Reference entities.
	/// </summary>
	public static class ReferenceExtensions
	{

		/// <summary>
		/// Animates the camera to look at the reference plane.
		/// </summary>
		/// <param name="camera"></param>
		/// <param name="refPlane"></param>
		public static void AnimateTo(this Camera camera, RefPlane refPlane)
		{
			double width = refPlane.RenderWidth;
			double distance = 0.7*width / (camera.FoV * 0.5).Tan();
			Vector center = refPlane.RenderCenter;
			camera.AnimateTo(center, center + refPlane.Plane.Normal * distance, refPlane.RenderUpVector);
		}

	}


}
