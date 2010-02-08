// 
//  TreeItem.cs - MonoWorks Project
//  
//  Author:
//       Andy Selvig <ajselvig@gmail.com>
// 
//  Copyright (c) 2010 Andy Selvig
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
using System.Collections.Generic;

using MonoWorks.Base;
using MonoWorks.Framework;
using MonoWorks.Rendering;
using MonoWorks.Rendering.Events;

namespace MonoWorks.Controls
{
	/// <summary>
	/// An item in a TreeView.
	/// </summary>
	public class TreeItem : GenericContainer<TreeItem>, IStringParsable
	{
		public TreeItem()
		{
			IsExpanded = true;
		}
		
		
		static TreeItem()
		{
			
			_hiddenIcon = new Image(ResourceHelper.GetStream("tree-hidden.png"));
			_expandedIcon = new Image(ResourceHelper.GetStream("tree-expanded.png"));
		}
		
		private string _text;
		/// <summary>
		/// The text displayed in the item.
		/// </summary>
		[MwxProperty]
		public string Text {
			get {return _text;}
			set {
				_text = value;
				MakeDirty();
			}
		}	
		
		public void Parse(string valString)
		{
			Text = valString;
		}
		
		private string _iconName;
		/// <summary>
		/// The name of the icon in the icon list to use.
		/// </summary>
		[MwxProperty]
		public string IconName
		{
			get { return _iconName; }
			set {
				_iconName = value;
				MakeDirty();
			}
		}
		
		private bool _isExpanded;
		/// <summary>
		/// Whether or not the item is expanded to show its children.
		/// </summary>
		[MwxProperty]
		public bool IsExpanded
		{
			get { return _isExpanded; }
			set {
				_isExpanded = value;
				MakeDirty();
			}
		}
		
		/// <summary>
		/// The tree view this item belongs to.
		/// </summary>
		public TreeView TreeView
		{
			get {
				if (Parent != null)
				{
					if (Parent is TreeItem)
						return (Parent as TreeItem).TreeView;
					else if (Parent is TreeView)
						return Parent as TreeView;
					else
						throw new Exception("Tree items should only be children of other tree items and tree views.");
				}
				return null;
			}
		}

		
		#region Expand Icons

		/// <summary>
		/// The width of the expand icons.
		/// </summary>
		private const double _expandIconWidth = 16;
		
		private static readonly Image _expandedIcon;

		private static readonly Image _hiddenIcon;
		
		#endregion
		
		
		#region Rendering

		/// <summary>
		/// A dummy surface used by Cairo to compute the text extents.
		/// </summary>
		private static readonly Cairo.ImageSurface DummySurface = new Cairo.ImageSurface(Cairo.Format.ARGB32, 128, 128);
		
		private Image _icon;
		
		/// <summary>
		/// The height taken up by just this item (not its children) during the last render.
		/// </summary>
		private double _myHeight;
		
		public override void ComputeGeometry()
		{
			base.ComputeGeometry();
			
			if (TreeView == null)
				return;
		
			var treeView = TreeView;
			
			// get the icon
			if (IconName != null)
				_icon = treeView.IconList[IconName];
			if (_icon != null)
			{
				if (_icon.IsDirty)
					_icon.ComputeGeometry();
				MinSize.X = _icon.RenderWidth + Padding + _expandIconWidth;
				MinSize.Y = _icon.RenderHeight;
			}
			else
			{
				MinSize.X = _expandIconWidth;
				MinSize.Y = 0;
			}
			
			// compute the text extents
			using (var cr = new Cairo.Context(DummySurface)) {
				cr.SetFontSize(treeView.FontSize);
				var extents = cr.TextExtents(Text);
				MinSize.X += extents.Width + 2 * Padding;
				MinSize.Y = Math.Max(MinSize.Y, extents.Height + 2 * Padding);
			}
			
			// store my height
			_myHeight = MinSize.Y;
			
			// compute size of children
			if (IsExpanded)
			{
				var indent = treeView.Indent;
				foreach (var child in Children)
				{
					child.Origin.X = indent;
					child.Origin.Y = MinSize.Y;
					MinSize.Y += child.RenderHeight;
					MinSize.X = Math.Max(MinSize.X, indent + child.RenderWidth);
				}
			}
			
			RenderSize = MinSize;
		}
		
		
		protected override void Render(RenderContext context)
		{
			if (Parent != null && Parent is TreeItem && !(Parent as TreeItem).IsExpanded)
				return;
			
			base.Render(context);
			
			var point = context.Cairo.CurrentPoint;
			
			// highlighting
			if (IsHovering || IsSelected)
			{
				context.Cairo.Save();
				context.Cairo.Color = context.Decorator.GetColor(ColorType.HighlightStart, HitState).Cairo;
				context.Cairo.Rectangle(point, RenderWidth, _myHeight);
				context.Cairo.Fill();
				context.Cairo.MoveTo(point);
				context.Cairo.Restore();
			}
			
			// render the text
			context.Cairo.MoveTo(point.X + Padding + _expandIconWidth, point.Y + Padding + context.Cairo.FontExtents.Height - 2);
			if (_icon != null) {
				context.Cairo.RelMoveTo(_icon.RenderWidth + Padding, context.Cairo.FontExtents.Height - _icon.RenderHeight);
			}
			context.Cairo.ShowText(Text);
			context.Cairo.MoveTo(point);
			
			// render the expand icon, if applicable
			if (NumChildren > 0)
			{
				if (IsExpanded)
					_expandedIcon.RenderHere(context);
				else
					_hiddenIcon.RenderHere(context);
			}
			
			// render the icon
			if (_icon != null)
			{
				context.Cairo.RelMoveTo(_expandIconWidth, 0);
				_icon.RenderHere(context);
			}
			context.Cairo.MoveTo(point);
			
		}
			
		#endregion
		
		
		#region Interaction
		
		/// <summary>
		/// Toggles the IsExpanded property.
		/// </summary>
		public void ToggleExpanded()
		{
			IsExpanded = !IsExpanded;
		}
		
		public override void OnButtonPress(MouseButtonEvent evt)
		{
			base.OnButtonPress(evt);
			
			if (!HitTest(evt.Pos))
				return;
			
			// whether it hit me and not my children
			var hitMe = evt.Pos.Y - LastPosition.Y <= _myHeight;
			
			if (evt.Button == 1)
			{
				if (hitMe)
				{
					Select();
					QueuePaneRender();
				}
				
				if (evt.Pos.X - LastPosition.X <= _expandIconWidth)
					ToggleExpanded();
				else if (evt.Multiplicity == ClickMultiplicity.Double && hitMe)
					ToggleExpanded();
			}
		}
		
		public override void OnButtonRelease(MouseButtonEvent evt)
		{
			base.OnButtonRelease(evt);
		}

		public override void OnMouseMotion(MouseEvent evt)
		{
			base.OnMouseMotion(evt);
			
			IsHovering = evt.Pos >= LastPosition && evt.Pos.X <= (LastPosition.X + RenderWidth) && 
				evt.Pos.Y - LastPosition.Y <= _myHeight;
			QueuePaneRender();
		}

		
		#endregion
		
		
		#region Selection
		
		public override void Select()
		{
			base.Select();
		
			if (TreeView != null)
				TreeView.Select(this, this);
		}
		
		/// <summary>
		/// Deselects the item and all of its children.
		/// </summary>
		public void DeselectAll(object sender)
		{
			IsSelected = false;
			foreach (var child in Children)
			{
				child.DeselectAll(sender);
			}
		}

		/// <summary>
		/// Selects the item and all of its children.
		/// </summary>
		public void SelectAll(object sender)
		{
			IsSelected = true;
			foreach (var child in Children)
			{
				child.SelectAll(sender);
			}
		}
		
		#endregion
		
	}
}

