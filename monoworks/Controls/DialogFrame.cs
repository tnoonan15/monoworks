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
using MonoWorks.Framework;
using MonoWorks.Rendering;
using MonoWorks.Rendering.Events;

namespace MonoWorks.Controls
{

	/// <summary>
	/// The decorative frame that wraps the contents of a dialog.
	/// </summary>
	public class DialogFrame : Container
	{
		
		static DialogFrame()
		{
			TitleHeight = 24;
		}

		public DialogFrame() : base()
		{
			UserSize = new Coord(300, 300);
			
			var closeIcon = new Image(ResourceHelper.GetStream("close.png"));
			_closeButton = new Button(closeIcon);
			_closeButton.Clicked += delegate(object sender, EventArgs e) {
				if (Closed != null)
					Closed(this, e);
			};
			_closeButton.Parent = this;
		}
		
		
		private Button _closeButton;
		
		public override void ComputeGeometry()
		{
			base.ComputeGeometry();
			
			// determine the size based on the children
			MinSize = new Coord();
			foreach (var child in Children)
			{
				child.ComputeGeometry();
				MinSize = Coord.Max(MinSize, child.RenderSize);
			}
			MinSize.Y += TitleHeight;
			
			ApplyUserSize();
			
			// place the close button
			_closeButton.ComputeGeometry();
			_closeButton.Origin = new Coord(RenderWidth - padding - _closeButton.RenderWidth, padding);
		}
		
		/// <summary>
		/// The height of the title bar.
		/// </summary>
		public static double TitleHeight {	get; private set;}
		
		protected override void Render(RenderContext context)
		{
			// render the decorations
			_closeButton.RenderCairo(context);
			
			// render the content
			context.Cairo.RelMoveTo(0, TitleHeight);
			context.Push();
			base.Render(context);
			context.Pop();
		}
		
		/// <summary>
		/// This gets raised when the user hits the close button.
		/// </summary>
		public event EventHandler Closed;

		
		#region Mouse Interaction
				
		public override void OnButtonPress(MouseButtonEvent evt)
		{
			base.OnButtonPress(evt);
			
			_closeButton.OnButtonPress(evt);
			foreach (var child in Children)
				child.OnButtonPress(evt);
		}

		public override void OnButtonRelease(MouseButtonEvent evt)
		{
			base.OnButtonRelease(evt);
			
			_closeButton.OnButtonRelease(evt);
			foreach (var child in Children)
				child.OnButtonRelease(evt);
		}

		public override void OnMouseMotion(MouseEvent evt)
		{
			base.OnMouseMotion(evt);
			
			_closeButton.OnMouseMotion(evt);
			foreach (var child in Children)
				child.OnMouseMotion(evt);
		}
		
		#endregion

		
	}
}
