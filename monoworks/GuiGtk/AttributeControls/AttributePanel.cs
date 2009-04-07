// AttributePanel.cs - MonoWorks Project
//
//  Copyright (C) 2009 Andy Selvig
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
using System.Collections.Generic;

using MonoWorks.Modeling;
using MonoWorks.Modeling.ViewportControls;
using MonoWorks.GuiGtk.Framework;

namespace MonoWorks.GuiGtk.AttributeControls
{
	
	/// <summary>
	/// Gtk implementation of IAttributePanel.
	/// </summary>
	public class AttributePanel : Gtk.VBox, IAttributePanel
	{
		
		public AttributePanel() : base()
		{
			int spacing = 4;
			
			var applyBox = new Gtk.HBox(false, spacing);
			applyBox.PackStart(new Gtk.Image("apply", Gtk.IconSize.Button), false, false, 0);
			applyBox.PackStart(new Gtk.Label("Apply"));
			var applyButton = new Gtk.Button() {
				Child = applyBox
			};
			applyButton.Clicked += OnApply;
			PackStart(applyButton, false, true, 0);
			
			var cancelBox = new Gtk.HBox(false, spacing);
			cancelBox.PackStart(new Gtk.Image("cancel", Gtk.IconSize.Button), false, false, 0);
			cancelBox.PackStart(new Gtk.Label("Cancel"));
			var cancelButton = new Gtk.Button() {
				Child = cancelBox
			};
			cancelButton.Clicked += OnCancel;
			PackStart(cancelButton, false, true, 0);
			
			
		}
		
		
		private Entity entity = null;
		
		private Dictionary<string,AttributeControl> controls = new Dictionary<string,AttributeControl>();
		
		
		/// <summary>
		/// Show the panel for the given entity.
		/// </summary>
		public void Show(DrawingController controller, Entity entity)
		{
			this.entity = entity;
			entity.AttributeUpdated += OnExternalUpdate;
			
			// clear the current controls
			foreach (var control in controls.Values)
				Remove(control);
			controls.Clear();
			
			// add the new controls
			foreach (var metaData in entity.MetaData.AttributeList)
			{
				if (metaData.Name != "locked")
				{
					var control = AttributeControl.GetControl(entity, metaData);
					PackStart(control, false, true, 6);
					controls[metaData.Name] = control;
					control.AttributeChanged += controller.OnAttributeChanged;
				}
			}
			
			WidthRequest = 150;
			
			ShowAll();
		}

		/// <summary>
		/// Raised when the panel is hidden.
		/// </summary>
		public new event AttributePanelHandler Hidden;
		
		public new void Hide()
		{
			if (entity != null)
				entity.AttributeUpdated -= OnExternalUpdate;
			
			WidthRequest = 0;
			HideAll();
			if (Hidden != null)
				Hidden(this);
		}

		/// <summary>
		/// Handles an attribute being updated externally.
		/// </summary>
		public void OnExternalUpdate(Entity entity, string name)
		{
			controls[name].Update();
		}
		
		

		/// <summary>
		/// Apply the current editing action.
		/// </summary>
		void OnApply(object sender, EventArgs e)
		{
			entity.Snapshot();
			EntityAction action = new EntityAction(entity);
			entity.TheDrawing.AddAction(action);
			Hide();
		}
		
		/// <summary>
		/// Cancel the current editing action.
		/// </summary>
		void OnCancel(object sender, EventArgs e)
		{
			entity.Revert();
			Hide();
		}
		
	}
}
