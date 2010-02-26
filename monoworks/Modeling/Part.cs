// Part.cs - MonoWorks Project
//
//  Copyright (C) 2009 Andy Selvig
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

namespace MonoWorks.Modeling
{
	
	
	public class Part : Drawing
	{
		
		public Part() : base()
		{
		}

		/// <value>
		/// The part's material (used in realistic mode).
		/// </value>
		[MwxProperty]
		public Material Material
		{
			get { return (Material)this["Material"]; }
			set { this["Material"] = value; }
		}

		/// <value>
		/// The part's color when being rendered in cartoon mode.
		/// </value>
		[MwxProperty]
		public Color CartoonColor
		{
			get { return (Color)this["CartoonColor"]; }
			set { this["CartoonColor"] = value; }
		}

		public override void RenderOpaque(Scene scene)
		{
			base.RenderOpaque(scene);

			switch (scene.RenderManager.ColorMode)
			{
			case ColorMode.Cartoon:
				CartoonColor.Setup();
				break;
			case ColorMode.Realistic:
				Material.Setup();
				break;
			}
		}



	}
}
