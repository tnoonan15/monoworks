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
using MonoWorks.Framework;
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
			
			// northeast buttons
			var cornerButtons = new CornerButtons(Corner.NE);
			cornerButtons.Image1 = new Image(GetIcon("apply", 22));
			cornerButtons.Action1 += delegate(object sender, EventArgs e) {
				Console.WriteLine("clicked apply");
			};
			cornerButtons.Image2 = new Image(GetIcon("cancel", 22));
			cornerButtons.Action2 += delegate(object sender, EventArgs e) {
				Console.WriteLine("clicked cancel");
			};
			var cornerAnchor = new AnchorPane(cornerButtons, AnchorLocation.NE);
			Viewport.RenderList.AddOverlay(cornerAnchor);
			
			
			// northwest buttons
			cornerButtons = new CornerButtons(Corner.NW);
			cornerButtons.Image1 = new Image(GetIcon("zoom-in", 22));
			cornerButtons.Action1 += delegate(object sender, EventArgs e) {
				Console.WriteLine("clicked zoom-in");
			};
			cornerButtons.Image2 = new Image(GetIcon("zoom-out", 22));
			cornerButtons.Action2 += delegate(object sender, EventArgs e) {
				Console.WriteLine("clicked zoom-out");
			};
			cornerAnchor = new AnchorPane(cornerButtons, AnchorLocation.NW);
			Viewport.RenderList.AddOverlay(cornerAnchor);
			
			
			// east toolbar
			ToolBar toolbar = new ToolBar();
			toolbar.Orientation = Orientation.Vertical;
			toolbar.ButtonStyle = ButtonStyle.ImageOverLabel;
			
			var image = new Image(GetIcon("apply", 48));
			var button = new Button("Apply", image);
			button.Clicked += delegate(object sender, EventArgs e) {
				Console.WriteLine("clicked apply");
			};
			toolbar.Add(button);

			image = new Image(GetIcon("cancel", 48));
			button = new Button("Cancel", image);
			button.Clicked += delegate(object sender, EventArgs e) {
				Console.WriteLine("clicked cancel");
			};
			toolbar.Add(button);

			var toolAnchor = new AnchorPane(toolbar, AnchorLocation.E);
			Viewport.RenderList.AddOverlay(toolAnchor);
			
			
			// floating toolbar
			toolbar = new ToolBar();
			toolbar.Orientation = Orientation.Vertical;
			toolbar.ButtonStyle = ButtonStyle.ImageNextToLabel;

			image = new Image(GetIcon("edit-undo", 48));
			button = new Button("Undo", image);
			toolbar.Add(button);

			image = new Image(GetIcon("edit-redo", 48));
			button = new Button("Redo", image);
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
		
		
		/// <summary>
		/// Gets an icon with the given name and size from the resources.
		/// </summary>
		private Stream GetIcon(string name, int size)
		{
			return ResourceHelper.GetStream(String.Format("icons{0}.{1}.png", size, name), "MonoWorks.Resources");
		}
		
	}
}
