// Label.cs - MonoWorks Project
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

using MonoWorks.Base;
using MonoWorks.Rendering;

namespace MonoWorks.Rendering.Controls
{
	
	/// <summary>
	/// Control containing just text.
	/// </summary>
	public class Label : Control
	{
		/// <summary>
		/// Initialization constructor.
		/// </summary>
		/// <param name="text"> The text to display. </param>
		public Label(string text) : base()
		{
			Text = text;

			textDef.Angle = new MonoWorks.Base.Angle(0);
		}


		private string text;
		/// <value>
		/// The text displayed by the label.
		/// </value>
		public string Text
		{
			get {return text;}
			set
			{
				text = value;
				textDef.Text = text;
				MakeDirty();
			}
		}

		public override Coord Position
		{
			get {return base.Position;}
			set
			{
				base.Position = value;
				textDef.Position = value;
				MakeDirty();
			}
		}


		/// <summary>
		/// The rendering definition of the text.
		/// </summary>
		private TextDef textDef = new TextDef(12);


		public override void ComputeGeometry()
		{
			base.ComputeGeometry();
			
			size = TextRenderer.GetExtents(textDef);
		}


		public override void RenderOverlay(IViewport viewport)
		{
			base.RenderOverlay(viewport);

			viewport.RenderText(textDef);
		}



		
	}
}
