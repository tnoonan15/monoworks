// 
//  Controlscs
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

using MonoWorks.Base;
using MonoWorks.Framework;
using MonoWorks.Rendering;
using MonoWorks.Rendering.Interaction;
using MonoWorks.Controls;

namespace MonoWorks.Demo
{
	/// <summary>
	/// Demo scene containing a bunch of controls.
	/// </summary>
	public class ControlsScene : Scene
	{
		public ControlsScene(Viewport viewport) : base(viewport)
		{
			Name = "Controls";
			
			// add an ActorInteractor to the viewport
			PrimaryInteractor = new ActorInteractor(this);
			
			
			// load the mwx file
			var mwx = new MwxSource(ResourceHelper.GetStream("demo.mwx"));
			
			
			// northeast buttons
			var cornerButtons = new CornerButtons(Corner.NE);
			cornerButtons.Image1 = Image.GetIcon("apply", 22);
			cornerButtons.Action1 += delegate(object sender, EventArgs e) {
				   Console.WriteLine("clicked apply");
			   };
			cornerButtons.Image2 = Image.GetIcon("cancel", 22);
			cornerButtons.Action2 += delegate(object sender, EventArgs e) {
				   Console.WriteLine("clicked cancel");
			   };
			var cornerAnchor = new AnchorPane(cornerButtons, AnchorLocation.NE);
			RenderList.AddOverlay(cornerAnchor);
			
			
			// northwest buttons
			cornerButtons = new CornerButtons(Corner.NW);
			cornerButtons.Image1 = Image.GetIcon("zoom-in", 22);
			cornerButtons.Action1 += delegate(object sender, EventArgs e) {
				  Console.WriteLine("clicked zoom-in");
			  };
			cornerButtons.Image2 = Image.GetIcon("zoom-out", 22);
			cornerButtons.Action2 += delegate(object sender, EventArgs e) {
				  Console.WriteLine("clicked zoom-out");
			  };
			cornerAnchor = new AnchorPane(cornerButtons, AnchorLocation.NW);
			RenderList.AddOverlay(cornerAnchor);
			
			
			// east toolbar
			ToolBar toolbar = new ToolBar();
			toolbar.Orientation = Orientation.Vertical;
			toolbar.ButtonStyle = ButtonStyle.ImageOverLabel;
			
			var image = Image.GetIcon("apply", 48);
			var button = new Button("Apply", image);
			button.Clicked += delegate(object sender, EventArgs e) {
				 Console.WriteLine("clicked apply");
			 };
			toolbar.Add(button);
			
			image = Image.GetIcon("cancel", 48);
			button = new Button("Cancel", image);
			button.Clicked += delegate(object sender, EventArgs e) {
				 Console.WriteLine("clicked cancel");
			 };
			toolbar.Add(button);
			
			var toolAnchor = new AnchorPane(toolbar, AnchorLocation.E);
			RenderList.AddOverlay(toolAnchor);
			
			
			// the controls dialog
			var controlsDialog = mwx.GetRenderable<Dialog>("controls-dialog");
			
			// attach the slider to its value label
			var slider = mwx.GetRenderable<Slider>("slider");
			var sliderValue = mwx.GetRenderable<Label>("sliderValue");
			sliderValue.Body = slider.Value.ToString("##.##");
			slider.ValueChanged += delegate(object sender, DoubleChangedEvent evt)
			{
				sliderValue.Body = evt.NewValue.ToString("##.##");
			};
			
			// attach the ForceStep checkbox to the slider
			var forceStepCheck = mwx.GetRenderable<CheckBox>("forceStepCheck");
			forceStepCheck.CheckChanged += delegate(object sender, BoolChangedEvent evt)
			{
				slider.ForceStep = evt.NewValue;
			};
			
			// floating toolbar
			toolbar = new ToolBar();
			toolbar.Orientation = Orientation.Vertical;
			toolbar.ButtonStyle = ButtonStyle.ImageNextToLabel;
			
			image = Image.GetIcon("controls-dialog", 48);
			button = new Button("Controls Dialog", image);
			button.Clicked += delegate(object sender, EventArgs e) { 
				// show controls dialog
				ShowModal(controlsDialog);
			};
			toolbar.Add(button);
			
			image = new Image(ResourceHelper.GetStream("linear-progress.png"));
			button = new Button("Linear Progress Bar", image);
			button.Clicked += delegate(object sender, EventArgs e) { 
				ShowModal(controlsDialog);
			};
			toolbar.Add(button);
			
			image = new Image(ResourceHelper.GetStream("radial-progress.png"));
			button = new Button("Radial Progress Bar", image);
			button.Clicked += delegate(object sender, EventArgs e) { 
				ShowModal(controlsDialog);
			};
			toolbar.Add(button);

			var toolActor = new ActorPane(toolbar);
			toolActor.Normal = new Vector(0, -1, 0);
			RenderList.AddActor(toolActor);
						
			
			Camera.SetViewDirection(ViewDirection.Standard);
		}
	}
}

