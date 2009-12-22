// 
//  DialogFrame.cs
//  
//  Author:
//       Andy Selvig <ajselvig@gmail.com>
// 
//  Copyright (c) 2009 andy
// 
//  This library is free software; you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as
//  published by the Free Software Foundation; either version 2.1 of the
//  License, or (at your option) any later version.
// 
//  This library is distributed in the hope that it will be useful, but
//  WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
//  Lesser General Public License for more details.
// 
//  You should have received a copy of the GNU Lesser General Public
//  License along with this library; if not, write to the Free Software
//  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA

using System;

using MonoWorks.Base;
using MonoWorks.Rendering;

namespace MonoWorks.Controls
{

	/// <summary>
	/// The decorative frame that wraps the contents of a dialog.
	/// </summary>
	public class DialogFrame : Container
	{

		public DialogFrame() : base()
		{
			RenderSize = new Coord(300, 300);
			TitleHeight = 16;
		}
		
		
		public override void ComputeGeometry()
		{
			base.ComputeGeometry();
			
			RenderSize = new Coord();
			// determine the size based on the children
			foreach (var child in Children)
			{
				child.ComputeGeometry();
				RenderSize = Coord.Max(RenderSize, child.RenderSize);
			}
			RenderSize.Y += TitleHeight;
			
		}
		
		/// <summary>
		/// The height of the title bar.
		/// </summary>
		public double TitleHeight {	get; private set;}
		
		protected override void Render(RenderContext context)
		{
			context.Push();
			
			// render the background
			context.Cairo.Color = new Cairo.Color(1.0, 1.0, 1.0);
			context.Cairo.Rectangle(0, 0, RenderWidth, RenderHeight);
			context.Cairo.Fill();
			
			// render the decorations
			context.Cairo.Color = new Cairo.Color(0, 0, 1.0);
			context.Cairo.Rectangle(0, 0, RenderWidth, TitleHeight);
			context.Cairo.Fill();
			
			context.Pop();
			Console.WriteLine ("rendering frame with size {0}", RenderSize);
			
			context.Cairo.RelMoveTo(0, TitleHeight);
			context.Push();
			base.Render(context);
			context.Pop();
		}


		
	}
}
