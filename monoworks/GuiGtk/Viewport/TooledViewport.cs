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
		public TooledViewport(ViewportUsage usage)
		{
			this.usage = usage;
			viewport = new Viewport();
			PackEnd(viewport, true, true, 0);
			
			LoadIcons();
			
			GenerateToolbar();
			
			ShowAll();
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
				uiManager.AddUiFromResource("MonoWorks.GuiGtk.Viewport.Cad.ui");
			else
				uiManager.AddUiFromResource("MonoWorks.GuiGtk.Viewport.Plotting.ui");
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
			}		
		}
		
		/// <summary>
		/// Sets the camera to the given view direction.
		/// </summary>
		/// <param name="direction"> A <see cref="ViewDirection"/>. </param>
		public void SetViewDirection(ViewDirection direction)
		{
			Console.WriteLine("set view direction {0}", direction);
		}
		
		
		protected void OnProjectionChanged(object sender, EventArgs args)
		{
			viewport.Camera.ToggleProjection();
			viewport.PaintGL();
		}
		
#endregion
		
	}
}
