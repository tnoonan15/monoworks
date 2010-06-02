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
			//			cornerButtons.Image1 = Image.GetIcon("apply", 22);
			cornerButtons.Action1 += delegate(object sender, EventArgs e) {
				   Console.WriteLine("clicked apply");
			   };
			//			cornerButtons.Image2 = Image.GetIcon("cancel", 22);
			cornerButtons.Action2 += delegate(object sender, EventArgs e) {
				   Console.WriteLine("clicked cancel");
			   };
			var cornerAnchor = new AnchorPane(cornerButtons, AnchorLocation.NE);
			RenderList.AddOverlay(cornerAnchor);
			
			
			// northwest buttons
			cornerButtons = new CornerButtons(Corner.NW);
			//			cornerButtons.Image1 = Image.GetIcon("zoom-in", 22);
			cornerButtons.Action1 += delegate(object sender, EventArgs e) {
				  Console.WriteLine("clicked zoom-in");
			  };
			//			cornerButtons.Image2 = Image.GetIcon("zoom-out", 22);
			cornerButtons.Action2 += delegate(object sender, EventArgs e) {
				  Console.WriteLine("clicked zoom-out");
			  };
			cornerAnchor = new AnchorPane(cornerButtons, AnchorLocation.NW);
			RenderList.AddOverlay(cornerAnchor);
			
			
			// east control stack
			var stack = new Stack();
			stack.Orientation = Orientation.Vertical;
			
			var image = new Image(ResourceHelper.GetStream("plugin.png"));
			//			var image = Image.GetIcon("apply", 48);
			var button = new Button("Apply", image) { ButtonStyle = ButtonStyle.ImageOverLabel };
			button.Clicked += delegate(object sender, EventArgs e) {
				 Console.WriteLine("clicked apply");
			 };
			stack.AddChild(button);
			
			//			image = Image.GetIcon("cancel", 48);
			button = new Button("Cancel", image) { ButtonStyle = ButtonStyle.ImageOverLabel };
			button.Clicked += delegate(object sender, EventArgs e) {
				 Console.WriteLine("clicked cancel");
			 };
			stack.AddChild(button);
			
			var menuBox = new MenuBox();
			menuBox.Parse("One,Two,Three,Four");
			stack.AddChild(menuBox);
			
			var toolAnchor = new AnchorPane(stack, AnchorLocation.E);
			RenderList.AddOverlay(toolAnchor);
			
			
			// the controls dialog
			var controlsDialog = mwx.Get<Dialog>("controls-dialog");
			
			// attach the slider to its value label
			var slider = mwx.Get<Slider>("slider");
			var sliderValue = mwx.Get<Label>("sliderValue");
			sliderValue.Body = slider.Value.ToString("##.##");
			slider.ValueChanged += delegate(object sender, DoubleChangedEvent evt)
			{
				sliderValue.Body = evt.NewValue.ToString("##.##");
			};
			
			// attach the ForceStep checkbox to the slider
			var forceStepCheck = mwx.Get<CheckBox>("forceStepCheck");
			forceStepCheck.CheckChanged += delegate(object sender, BoolChangedEvent evt)
			{
				slider.ForceStep = evt.NewValue;
			};
			
			// floating toolbar
			var toolbar = new ToolBar();
			toolbar.Orientation = Orientation.Vertical;
			toolbar.ButtonStyle = ButtonStyle.ImageNextToLabel;
			
			//			image = Image.GetIcon("controls-dialog", 48);
			button = new Button("General Controls", image);
			button.Clicked += delegate(object sender, EventArgs e) { 
				ShowModal(controlsDialog);
			};
			toolbar.AddChild(button);
			
			
			// the progress dialog
			var progressDialog = mwx.Get<Dialog>("progress-dialog");
			
			// attach the slider to the progress bars
			var progressSlider = mwx.Get<Slider>("progressSlider");
			var progressBarH = mwx.Get<ProgressBar>("progressBarH");
			var progressBarV = mwx.Get<ProgressBar>("progressBarV");
			var progressDial = mwx.Get<ProgressDial>("progressDial");
			progressBarH.Value = progressSlider.Value;
			progressBarV.Value = progressSlider.Value;
			progressDial.Value = progressSlider.Value;
			progressSlider.ValueChanged += delegate(object sender, DoubleChangedEvent evt)
			{
				progressBarH.Value = progressSlider.Value;
				progressBarV.Value = progressSlider.Value;
				progressDial.Value = progressSlider.Value;
			};
			
			image = new Image(ResourceHelper.GetStream("radial-progress.png"));
			button = new Button("Progress Indicators", image);
			button.Clicked += delegate(object sender, EventArgs e) { 
				ShowModal(progressDialog);
			};
			toolbar.AddChild(button);
			
			// the tree view dialog
			var treeDialog = mwx.Get<Dialog>("tree-dialog");
			
			image = new Image(ResourceHelper.GetStream("view-tree.png"));
			button = new Button("Tree View", image);
			button.Clicked += delegate(object sender, EventArgs e) { 
				ShowModal(treeDialog);
			};
			toolbar.AddChild(button);
			
			var toolActor = new ActorPane(toolbar);
			toolActor.Normal = new Vector(0, -1, 0);
			RenderList.AddActor(toolActor);
			toolActor.ComputeGeometry();
			
			
			// the ring bar
			var ringBar = mwx.Get<RingBar>("ring-bar");
			toolActor = new ActorPane(ringBar);
			toolActor.Normal = new Vector(1, 0, 0);
			toolActor.Origin.Y = -192;
			toolActor.XAxis = new Vector(0, 1, 0);
			RenderList.AddActor(toolActor);
			toolActor.ComputeGeometry();

			mwx.Get<RingButton>("forward-button").Clicked += delegate {
				Console.WriteLine("forward pushed");
			};
			mwx.Get<RingButton>("backward-button").Clicked += delegate {
				Console.WriteLine("backward pushed");
			};
			mwx.Get<RingButton>("play-button").Clicked += delegate {
				Console.WriteLine("play pushed");
			};
			mwx.Get<RingButton>("pause-button").Clicked += delegate {
				Console.WriteLine("pause pushed");
			};

			
			Camera.SetViewDirection(ViewDirection.Standard);
		}
	}
}

