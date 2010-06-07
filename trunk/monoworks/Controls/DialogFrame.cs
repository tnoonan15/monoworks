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
			UserSize = new Coord(200, 200);
			
			var closeIcon = new Image(ResourceHelper.GetStream("close.png"));
			CloseButton = new Button(closeIcon) {
				Padding = 3
			};
			CloseButton.Clicked += delegate(object sender, EventArgs e) {
				if (Closed != null)
					Closed(this, e);
			};
			CloseButton.ParentControl = this;
			
			_titleLabel = new Label() {
				Padding = 3
			};
		}
		
		
		private Button CloseButton;
		
		private Label _titleLabel;
		
		/// <summary>
		/// The title displayed in the title bar.
		/// </summary>
		public string Title
		{
			get {return _titleLabel.Body;}
			set {_titleLabel.Body = value;}
		}
		
		public override void ComputeGeometry()
		{
			base.ComputeGeometry();
			
			// determine the size based on the children
			MinSize = new Coord();
			foreach (var child in Children)
			{
				MinSize = Coord.Max(MinSize, child.RenderSize);
			}
			MinSize.Y += TitleHeight;
			
			ApplyUserSize();
			
			// place the close button
			if (CloseButton.IsDirty)
				CloseButton.ComputeGeometry();
			CloseButton.Origin = new Coord(RenderWidth - Padding - CloseButton.RenderWidth, Padding);
			
			// place the title label
			if (_titleLabel.IsDirty)
				_titleLabel.ComputeGeometry();
			_titleLabel.Origin = new Coord((RenderWidth - _titleLabel.RenderWidth) / 2, Padding);
		}
		
		/// <summary>
		/// The height of the title bar.
		/// </summary>
		public static double TitleHeight {	get; private set;}
		
		protected override void Render(RenderContext context)
		{
			// render the decorations
			CloseButton.RenderCairo(context);
			_titleLabel.RenderCairo(context);
			
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
			
			CloseButton.OnButtonPress(evt);
		}

		public override void OnButtonRelease(MouseButtonEvent evt)
		{
			base.OnButtonRelease(evt);
			
			CloseButton.OnButtonRelease(evt);
		}

		public override void OnMouseMotion(MouseEvent evt)
		{
			base.OnMouseMotion(evt);
			
			CloseButton.OnMouseMotion(evt);
		}
		
		#endregion

		
	}
}
