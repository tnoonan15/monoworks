// 
//  TreeView.cs - MonoWorks Project
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
using MonoWorks.Rendering;
using MonoWorks.Rendering.Events;

namespace MonoWorks.Controls
{
	/// <summary>
	/// A hierarchical view of items. Each item has text and possibly an icon.
	/// </summary>
	public class TreeView : GenericContainer<TreeItem>
	{
		public TreeView()
		{
			IconList = new IconList();
			_fontSize = 12;
		}
		
		
		/// <summary>
		/// The icons available for all tree items.
		/// </summary>
		[MwxProperty]
		public IconList IconList {get; private set;}
		
		
		public override void AddChild(IMwxObject child)
		{
			if (child is IconList)
			{
				child.Parent = this;
				IconList = child as IconList;
			}
			else if (child is TreeItem)
				AddChild(child as TreeItem);
			else
				throw new Exception("Don't know what to do with TreeView children of type " + child.GetType());
		}

		
		private double _indent = 12;
		/// <summary>
		/// The indentation for each depth level.
		/// </summary>
		[MwxProperty]
		public double Indent {
			get { return _indent; }
			set {
				_indent = value;
				MakeDirty();
			}
		}

		private double _fontSize;
		/// <summary>
		/// The font size (in pixels).
		/// </summary>
		[MwxProperty]
		public double FontSize {
			get { return _fontSize; }
			set {
				_fontSize = value;
				MakeDirty();
			}
		}

		#region Rendering
		
		public override void ComputeGeometry()
		{
			base.ComputeGeometry();
			
			// compute size of children
			MinSize.X = 0;
			MinSize.Y = 0;
			foreach (var child in Children)
			{
				child.Origin.X = Indent;
				child.Origin.Y = MinSize.Y;
				MinSize.Y += child.RenderHeight;
				MinSize.X = Math.Max(MinSize.X, Indent + child.RenderWidth);
			}
			
			RenderSize = MinSize;
		}

		protected override void Render(RenderContext context)
		{
			context.Cairo.SetFontSize(_fontSize);
			context.Cairo.Color = context.Decorator.GetColor(ColorType.Text, HitState.None).Cairo;
			
			base.Render(context);
		}
		
		#endregion
		
		
		#region Selection
		
		private bool _allowMultiSelect;
		/// <summary>
		/// Whether or not more than one item can be selected at a time.
		/// </summary>
		[MwxProperty]
		public bool AllowMultiSelect {
			get { return _allowMultiSelect; }
			set {
				_allowMultiSelect = value;
				if (!_allowMultiSelect)
				{
					DeselectAll(this);
				}
			}
		}
		
		/// <summary>
		/// Deselects all items.
		/// </summary>
		public virtual void DeselectAll(object sender)
		{
			foreach (var child in Children)
			{
				child.DeselectAll(sender);
			}
		}

		/// <summary>
		/// Selects all items.
		/// </summary>
		/// <remarks>Throws an exception if AllowMultiSelect is false.</remarks>
		public virtual void SelectAll(object sender)
		{
			if (!AllowMultiSelect)
				throw new Exception("Can't select all tree items if AllowMultiSelect isn't true.");
			foreach (var child in Children)
			{
				child.SelectAll(sender);
			}
		}
		
		/// <summary>
		/// Selects a single item.
		/// </summary>
		/// <remarks>If AllowMultiSelect is false, DeselectAll() will be automatically called.</remarks>
		public virtual void Select(object sender, TreeItem item)
		{
			if (!AllowMultiSelect)
				DeselectAll(this);
			item.IsSelected = true;
		}

		/// <summary>
		/// Deselects a single item.
		/// </summary>
		public virtual void Deselect(object sender, TreeItem item)
		{
			item.IsSelected = false;
		}
		
		#endregion
		
	}
	
	
}

