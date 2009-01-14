using System;
using System.Collections.Generic;

using System.Windows;
using System.Windows.Controls;

using MonoWorks.Model;
using MonoWorks.Model.ViewportControls;

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
		/// Show the panel with the given entity.
		/// </summary>
		/// <param name="entity"></param>
		public void Show(Entity entity)
		{
			Visibility = Visibility.Visible;

			Children.Clear();

			Label label = new Label();
			label.Content = entity.Name;
			Children.Add(label);

		}

		/// <summary>
		/// Hide the panel.
		/// </summary>
		public void Hide()
		{
			Visibility = Visibility.Collapsed;
		}

	}
}
