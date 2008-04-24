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
using System.Collections.Generic;

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
			
			solidActions = new Dictionary<SolidMode,QAction>();
			
			colorActions = new Dictionary<ColorMode,QAction>(); 
			
			CreateActions();
		}
		

		protected void CreateActions()
		{
			QAction action;
			
			// view actions
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

			
			this.AddSeparator();
			
			
			// wireframe action
			wireframeAction = new QAction(ResourceManager.GetIcon("wireframe"), "Wireframe", this);
			wireframeAction.StatusTip = "Toggles the display of the wireframe";
			wireframeAction.Checkable = true;
			this.AddAction(wireframeAction);
			Connect(wireframeAction, SIGNAL("triggered()"), this, SLOT("ToggleWireframe()"));

			
			this.AddSeparator();
			
			
			// solid actions
			action = new QAction(ResourceManager.GetIcon("nosolid"), "No Solid", this);
			action.StatusTip = "Don't display solid surfaces";
			action.Checkable = true;
			this.AddAction(action);
			solidActions[SolidMode.None] = action;
			Connect(action, SIGNAL("triggered()"), this, SLOT("NoSolid()"));
			
			action = new QAction(ResourceManager.GetIcon("flat"), "Flat", this);
			action.StatusTip = "Display flat solid surfaces";
			action.Checkable = true;
			this.AddAction(action);
			solidActions[SolidMode.Flat] = action;
			Connect(action, SIGNAL("triggered()"), this, SLOT("FlatSolid()"));
			
			action = new QAction(ResourceManager.GetIcon("smooth"), "Smooth", this);
			action.StatusTip = "Display smooth solid surfaces";
			action.Checkable = true;
			this.AddAction(action);
			solidActions[SolidMode.Smooth] = action;
			Connect(action, SIGNAL("triggered()"), this, SLOT("SmoothSolid()"));
			
			UpdateSolidActions();

			
			this.AddSeparator();
			
			
			// color actions
			action = new QAction(ResourceManager.GetIcon("cartoon"), "Cartoon", this);
			action.StatusTip = "Draw solid surfaces in cartoon colors";
			action.Checkable = true;
			this.AddAction(action);
			colorActions[ColorMode.Cartoon] = action;
			Connect(action, SIGNAL("triggered()"), this, SLOT("CartoonMode()"));
			
			action = new QAction(ResourceManager.GetIcon("realistic"), "Realistic", this);
			action.StatusTip = "Draw solid surfaces in realistic colors and materials";
			action.Checkable = true;
			this.AddAction(action);
			colorActions[ColorMode.Realistic] = action;
			Connect(action, SIGNAL("triggered()"), this, SLOT("RealisticMode()"));
			
			UpdateColorActions();
			
		}
		

		
#region Wireframe
		
		protected QAction wireframeAction;
		
		/// <summary>
		/// Toggles the display of the wireframe.
		/// </summary>
		[Q_SLOT("ToggleWireframe()")]
		public void ToggleWireframe()
		{
			viewport.RenderManager.ShowWireframe = wireframeAction.Checked;
			viewport.Paint();
		}
		
#endregion
		
		
#region Solid Actions
		
		protected Dictionary<SolidMode, QAction> solidActions;
		
		/// <summary>
		/// Don't display solid surfaces.
		/// </summary>
		[Q_SLOT("NoSolid()")]
		public void NoSolid()
		{
			viewport.RenderManager.SolidMode = SolidMode.None;
			UpdateSolidActions();
			viewport.Initialize();
		}
		
		/// <summary>
		/// Display flat solid surfaces.
		/// </summary>
		[Q_SLOT("FlatSolid()")]
		public void FlatSolid()
		{
			viewport.RenderManager.SolidMode = SolidMode.Flat;
			UpdateSolidActions();
			viewport.Initialize();
		}
		
		/// <summary>
		/// Display smooth solid surfaces.
		/// </summary>
		[Q_SLOT("SmoothSolid()")]
		public void SmoothSolid()
		{
			viewport.RenderManager.SolidMode = SolidMode.Smooth;
			UpdateSolidActions();
			viewport.Initialize();
		}
		
		/// <summary>
		/// Updates the solid actions to be consistent with the render manager.
		/// </summary>
		protected void UpdateSolidActions()
		{
			foreach (int mode in Enum.GetValues(typeof(SolidMode)))
			{
				if (viewport.RenderManager.SolidMode == (SolidMode)mode)
					solidActions[(SolidMode)mode].Checked = true;
				else
					solidActions[(SolidMode)mode].Checked = false;
			}
		}
		
#endregion
		
		
#region Solid Actions
		
		protected Dictionary<ColorMode, QAction> colorActions;
		
		/// <summary>
		/// Set color mode to cartoon.
		/// </summary>
		[Q_SLOT("CartoonMode()")]
		public void CartoonMode()
		{
			viewport.RenderManager.ColorMode = ColorMode.Cartoon;
			UpdateColorActions();
			viewport.Paint();
		}
		
		/// <summary>
		/// Set color mode to realistic.
		/// </summary>
		[Q_SLOT("RealisticMode()")]
		public void RealisticMode()
		{
			viewport.RenderManager.ColorMode = ColorMode.Realistic;
			UpdateColorActions();
			viewport.Paint();
		}
		
		/// <summary>
		/// Updates the color actions to be consistent with the render manager.
		/// </summary>
		protected void UpdateColorActions()
		{
			foreach (int mode in Enum.GetValues(typeof(ColorMode)))
			{
				if (viewport.RenderManager.ColorMode == (ColorMode)mode)
					colorActions[(ColorMode)mode].Checked = true;
				else
					colorActions[(ColorMode)mode].Checked = false;
			}
		}
		
#endregion
		
	}
}
