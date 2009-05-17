// PaneControls.cs - MonoWorks Project
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

using System.IO;

using MonoWorks.Base;
using MonoWorks.Rendering;
using MonoWorks.Rendering.Controls;
using MonoWorks.Rendering.Interaction;
using MonoWorks.GuiGtk;
using MonoWorks.GuiGtk.Framework;

namespace MonoWorks.DemoGtk
{
	
	/// <summary>
	/// Contains the rendering controls demo.
	/// </summary>
	public class PaneControls : Gtk.HBox
	{
		
		public PaneControls() : base()
		{
			// add the viewport
			adapter = new ViewportAdapter();
			PackEnd(adapter);
			
			// east toolbar
			ToolBar toolbar = new ToolBar();
			toolbar.Orientation = Orientation.Vertical;
			toolbar.ButtonStyle = ButtonStyle.ImageOverLabel;

			string iconPath = Directory.GetCurrentDirectory() + "/../../../Resources/icons48/apply.png";
			var image = new Image(iconPath);
			var button = new Button("Button 1", image);
			button.Clicked += delegate(object sender, EventArgs e) {
				Console.WriteLine("clicked button 1");
			};
			toolbar.Add(button);

			iconPath = Directory.GetCurrentDirectory() + "/../../../Resources/icons48/3d.png";
			image = new Image(iconPath);
			button = new Button("Button 2", image);
			button.Clicked += delegate(object sender, EventArgs e) {
				Console.WriteLine("clicked button 2");
			};
			toolbar.Add(button);

			var toolAnchor = new Anchor(toolbar, AnchorLocation.E);
			Viewport.RenderList.AddOverlay(toolAnchor);
			
			
			// floating toolbar
			toolbar = new ToolBar();
			toolbar.Orientation = Orientation.Vertical;
			toolbar.ButtonStyle = ButtonStyle.ImageNextToLabel;

			iconPath = Directory.GetCurrentDirectory() + "/../../../Resources/icons48/apply.png";
			image = new Image(iconPath);
			button = new Button("Button 1", image);
			toolbar.Add(button);

			iconPath = Directory.GetCurrentDirectory() + "/../../../Resources/icons48/3d.png";
			image = new Image(iconPath);
			button = new Button("Button 2", image);
			toolbar.Add(button);

			var toolActor = new ActorPane(toolbar);
			Viewport.RenderList.AddActor(toolActor);
						
			
			Viewport.Camera.SetViewDirection(ViewDirection.Standard);
		}
		
		protected ViewportAdapter adapter;
		
		/// <summary>
		/// The viewport.
		/// </summary>
		protected Viewport Viewport
		{
			get {return adapter.Viewport;}
		}
		
	}
}
