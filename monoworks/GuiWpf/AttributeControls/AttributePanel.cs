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

using System.Windows;
using System.Windows.Controls;

using MonoWorks.Model;
using MonoWorks.Model.ViewportControls;
using fw=MonoWorks.GuiWpf.Framework;

namespace MonoWorks.GuiWpf.AttributeControls
{
	/// <summary>
	/// Panel containing attribute controls for an entity.
	/// </summary>
	public class AttributePanel : StackPanel, IAttributePanel
	{

		public AttributePanel() : base()
		{
		}

		/// <summary>
		/// The entity being edited.
		/// </summary>
		protected Entity entity = null;

		/// <summary>
		/// Adds the buttons to the top of the panel.
		/// </summary>
		protected void AddButtons()
		{
			Button applyButton = new Button();
			StackPanel panel = new StackPanel();
			panel.Orientation = Orientation.Horizontal;
			panel.Children.Add(fw.ResourceManager.RenderIcon("apply", 22));
			panel.AddLabel("Apply");
			applyButton.Content = panel;
			applyButton.Click += OnApply;
			Children.Add(applyButton);

			Button cancelButton = new Button();
			panel = new StackPanel();
			panel.Orientation = Orientation.Horizontal;
			panel.Children.Add(fw.ResourceManager.RenderIcon("cancel", 22));
			panel.AddLabel("Cancel");
			cancelButton.Content = panel;
			cancelButton.Click += OnCancel;
			Children.Add(cancelButton);
		}

		/// <summary>
		/// Handles the editing action being applied.
		/// </summary>
		void OnApply(object sender, RoutedEventArgs e)
		{
			entity.Snapshot();
			EntityAction action = new EntityAction(entity);
			entity.TheDrawing.AddAction(action);
			Hide();
			
		}

		/// <summary>
		/// Handles the editing action being cancelled.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void OnCancel(object sender, RoutedEventArgs e)
		{
			entity.Revert();
			Hide();
		}

		/// <summary>
		/// The attribute controls keyed by attribute name.
		/// </summary>
		private Dictionary<string, AttributeControl> controls = new Dictionary<string, AttributeControl>();

		/// <summary>
		/// Show the panel with the given entity.
		/// </summary>
		public void Show(DrawingController controller, Entity entity)
		{
			Children.Clear();
			controls.Clear();

			this.entity = entity;
			entity.AttributeUpdated += OnExternalUpdate;

			Width = 160;

			AddButtons();

			// create the attribute controls
			foreach (AttributeMetaData metaData in entity.MetaData.AttributeList)
			{
				if (metaData.Name == "locked")
					continue;

				AttributeControl control = AttributeControl.Generate(entity, metaData);
				control.Margin = new Thickness(6);
				Children.Add(control);
				control.AttributeChanged += controller.OnAttributeChanged;
				controls[metaData.Name] = control;
			}

			Visibility = Visibility.Visible;
		}

		/// <summary>
		/// Raised when the panel is hidden.
		/// </summary>
		public event AttributePanelHandler Hidden;

		/// <summary>
		/// Hide the panel.
		/// </summary>
		public void Hide()
		{
			if (entity != null)
				entity.AttributeUpdated -= OnExternalUpdate;
			Visibility = Visibility.Collapsed;
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

	}
}
