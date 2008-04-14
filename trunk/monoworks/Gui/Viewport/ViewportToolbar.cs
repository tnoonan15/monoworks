// ViewportToolbar.cs - MonoWorks Project
//
// Copyright (C) 2008 Andy Selvig
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA

using System;

using Qyoto;

using MonoWorks.Model;

namespace MonoWorks.Gui
{
	
	/// <summary>
	/// The ViewportToolbar is present in every DocFrame above the viewport.
	/// It contains controls for changing the document view properties.
	/// </summary>
	public class ViewportToolbar : QToolBar
	{
		
		
		protected Viewport viewport;
		
		/// <summary>
		/// Default constructor.
		/// </summary>
		/// <param name="parent"> The parent <see cref="QWidget"/>. </param>
		/// <param name="viewport"> The associated <see cref="Viewport"/>. </param>
		public ViewportToolbar(QWidget parent, Viewport viewport) : base("Document", parent)
		{
			this.viewport = viewport;
			
			CreateActions();
		}
		

		protected void CreateActions()
		{
			QAction action;
			
			action = new QAction(ResourceManager.GetIcon("standard-view"), "Standard View", this);
			action.StatusTip = "Go to the standard view";
			this.AddAction(action);
			
			action = new QAction(ResourceManager.GetIcon("front-view"), "Front View", this);
			action.StatusTip = "View the front side of the scene";
			this.AddAction(action);
			
			action = new QAction(ResourceManager.GetIcon("back-view"), "Back View", this);
			action.StatusTip = "View the back side of the scene";
			this.AddAction(action);
			
			action = new QAction(ResourceManager.GetIcon("top-view"), "Top View", this);
			action.StatusTip = "View the top side of the scene";
			this.AddAction(action);
			
			action = new QAction(ResourceManager.GetIcon("bottom-view"), "Bottom View", this);
			action.StatusTip = "View the bottom side of the scene";
			this.AddAction(action);
			
			action = new QAction(ResourceManager.GetIcon("right-view"), "Right View", this);
			action.StatusTip = "View the right side of the scene";
			this.AddAction(action);
			
			action = new QAction(ResourceManager.GetIcon("left-view"), "Left View", this);
			action.StatusTip = "View the left side of the scene";
			this.AddAction(action);
		}
		
	}
}
