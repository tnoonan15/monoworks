// TooledViewport.cs - MonoWorks Project
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

using MonoWorks.Rendering;
using System.Reflection;
using System.Resources;

using MonoWorks.Rendering.Interaction;

namespace MonoWorks.GuiGtk
{
	
	/// <summary>
	/// A viewport with a toolbar at the top.
	/// </summary>
	public class TooledViewport : Gtk.VBox
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		/// <remarks> Defaults to CAD usage.</remarks>
		public TooledViewport() : this (ViewportUsage.CAD)
		{
		}
				
		/// <summary>
		/// Constructor that initializes usage.
		/// </summary>
		/// <param name="usage"> A <see cref="ViewportUsage"/>. </param>
		public TooledViewport(ViewportUsage usage) : this(usage, true)
		{
		}
				
		/// <summary>
		/// Constructor that initializes usage and whether to show toolbar.
		/// </summary>
		public TooledViewport(ViewportUsage usage, bool showToolbar)
		{			
			this.usage = usage;
			viewport = new Viewport();
			PackEnd(viewport, true, true, 0);
			
			LoadIcons();
			
			if (showToolbar)
				GenerateToolbar();
			
//			ShowAll();
		}
		
		
		protected Viewport viewport;
		/// <value>
		/// The viewport.
		/// </value>
		public Viewport Viewport
		{
			get {return viewport;}
			set {viewport = value;}
		}
		
		protected ViewportUsage usage;
		/// <value>
		/// The viewport's usage.
		/// </value>
		/// <remarks> CAD usage has more buttons that aren't needed in plotting. </remarks>
		public ViewportUsage Usage
		{
			get {return usage;}
			set {usage = value;}
		}
		
		
#region Icons
	
		
		protected static Gtk.IconFactory iconFactory = null;
		
		/// <summary>
		/// Loads the icons from the resources into the icon factory.
		/// </summary>
		protected static void LoadIcons()
		{
			if (iconFactory == null)
			{
				iconFactory = new Gtk.IconFactory();
				Assembly asm = Assembly.GetExecutingAssembly();
				foreach (string name in asm.GetManifestResourceNames())
				{
					if (name.EndsWith("png"))
					{
						string[] comps = name.Split('.');
						Gdk.Pixbuf pixBuf = new Gdk.Pixbuf(asm.GetManifestResourceStream(name));
						Gtk.IconSet iconSet = new Gtk.IconSet(pixBuf);
						iconFactory.Add(comps[comps.Length-2], iconSet);
					}
				}
				iconFactory.AddDefault();
			}
		}
		
#endregion
		

#region The Toolbar
	
		/// <summary>
		/// The toolbar.
		/// </summary>
		protected Gtk.Toolbar toolbar = null;
		
		/// <summary>
		/// Generates (or regenerates) the toolbar based on the usage.
		/// </summary>
		public void GenerateToolbar()
		{
			// remove existing toolbar
			if (toolbar != null)
			{
				this.Remove(toolbar);
			}
			
			CreateActions();

			// load the toolbar from the ui file for this usage
			Gtk.UIManager uiManager = new Gtk.UIManager();
			uiManager.InsertActionGroup(actionGroup, 0);
			if (usage == ViewportUsage.CAD)
				uiManager.AddUiFromResource("Cad.ui");
			else
				uiManager.AddUiFromResource("Plotting.ui");
			toolbar = (Gtk.Toolbar)uiManager.GetWidget("/ViewportToolbar");
			toolbar.ToolbarStyle = Gtk.ToolbarStyle.Icons;
			PackStart(toolbar, false, true, 0);
			
			// add the projection control
//			string[] projections = new string[]{"Perspective", "Parallel"};
//			Gtk.ComboBox projCombo = new Gtk.ComboBox(projections);
//			projCombo.Changed += OnProjectionChanged;
//			toolbar.Insert(projCombo, 0);
			
		}
		
#endregion
		
		
#region Actions
		
		Gtk.ActionGroup actionGroup = null;
		
		/// <summary>
		/// Create the actions.
		/// </summary>
		protected void CreateActions()
		{
			
			// create the action group
			if (actionGroup == null)
			{
				actionGroup = new Gtk.ActionGroup("Viewport");
				Gtk.Action action;
				
				action = new Gtk.Action("StandardView", "Standard View", "Standard 3D View", "standard-view");
				action.Activated += delegate(object sender, EventArgs args)
				{
					SetViewDirection(ViewDirection.Standard);
				};
				actionGroup.Add(action);
				action = new Gtk.Action("FrontView", "Front View", "Front View", "front-view");
				action.Activated += delegate(object sender, EventArgs args)
				{
					SetViewDirection(ViewDirection.Front);
				};
				actionGroup.Add(action);
				action = new Gtk.Action("BackView", "Back View", "Back View", "back-view");
				action.Activated += delegate(object sender, EventArgs args)
				{
					SetViewDirection(ViewDirection.Back);
				};
				actionGroup.Add(action);
				action = new Gtk.Action("LeftView", "Left View", "Left View", "left-view");
				action.Activated += delegate(object sender, EventArgs args)
				{
					SetViewDirection(ViewDirection.Left);
				};
				actionGroup.Add(action);
				action = new Gtk.Action("RightView", "Right View", "Right View", "right-view");
				action.Activated += delegate(object sender, EventArgs args)
				{
					SetViewDirection(ViewDirection.Right);
				};
				actionGroup.Add(action);
				action = new Gtk.Action("TopView", "Top View", "Top View", "top-view");
				action.Activated += delegate(object sender, EventArgs args)
				{
					SetViewDirection(ViewDirection.Top);
				};
				actionGroup.Add(action);
				action = new Gtk.Action("BottomView", "Bottom View", "Bottom View", "bottom-view");
				action.Activated += delegate(object sender, EventArgs args)
				{
					SetViewDirection(ViewDirection.Bottom);
				};
				actionGroup.Add(action);
				
				action = new Gtk.ToggleAction("TogglePerspective", "Toggle Perspective Projection", "Toggle Perspective Projection", "perspective");
				action.Activate();
				action.Activated += OnProjectionChanged;
				actionGroup.Add(action);

				// interaction mode actions
				Gtk.RadioAction radio = new Gtk.RadioAction("Interact2D", "2D Mode", "2D Mode", "2d", 0);
				radio.Activated += delegate(object sender, EventArgs args)
				{ SetInteractionState(InteractionState.Interact2D);};
				actionGroup.Add(radio);
				GLib.SList modeGroup = radio.Group;
				
				radio = new Gtk.RadioAction("Interact3D", "3D Selection Mode", "3D Selection Mode", "3dSelect", 1);
				radio.Activated += delegate(object sender, EventArgs args)
				{ SetInteractionState(InteractionState.Interact3D);};
				radio.Group = modeGroup;
				modeGroup = radio.Group;
				actionGroup.Add(radio);
				
				radio = new Gtk.RadioAction("View3D", "3D View Mode", "3D View Mode", "3dInteract", 2);
				radio.Activate();
				radio.Activated += delegate(object sender, EventArgs args)
				{ SetInteractionState(InteractionState.View3D);};
				radio.Group = modeGroup;
				actionGroup.Add(radio);
			}		
		}
		
		/// <summary>
		/// Sets the camera to the given view direction.
		/// </summary>
		/// <param name="direction"> A <see cref="ViewDirection"/>. </param>
		public void SetViewDirection(ViewDirection direction)
		{
			viewport.Camera.SetViewDirection(direction);
			viewport.PaintGL();
			viewport.ResizeGL();
		}
		
		/// <summary>
		/// Handles changing the projection of the camera.
		/// </summary>
		protected void OnProjectionChanged(object sender, EventArgs args)
		{
			viewport.Camera.ToggleProjection();
			viewport.PaintGL();
		}
		
		/// <summary>
		/// Sets the interaction state of the viewport.
		/// </summary>
		/// <param name="state"> A <see cref="InteractionState"/>. </param>
		public void SetInteractionState(InteractionState state)
		{
			if (state == InteractionState.Interact2D) // force to front parallel for 2D viewing
			{
				viewport.Camera.Projection = Projection.Parallel;
				viewport.Camera.SetViewDirection(ViewDirection.Front);
			}
			else if (viewport.RenderableInteractor.State == InteractionState.Interact2D) // transitioning out of 2D
			{
				viewport.Camera.Projection = Projection.Perspective;
				viewport.Camera.SetViewDirection(ViewDirection.Standard);				
			}
			viewport.RenderableInteractor.State = state;
			viewport.ResizeGL();
			viewport.PaintGL();
		}
		
#endregion
		
	}
}
