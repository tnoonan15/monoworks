using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;

using MonoWorks.Framework;

namespace MonoWorks.GuiWpf.Framework
{
	/// <summary>
	/// The tool shelf belongs to a tool box and stores a list of action icons.
	/// </summary>
	public class ToolShelf : StackPanel
	{

		public ToolShelf(string name, ToolBox parent)
			: base()
		{
			// make the show button
			showButton = new Button();
			showButton.Content = name;
            showButton.Click += delegate(object sender, RoutedEventArgs args)
            {
                parent.ShowShelf(this);
            };
			Children.Add(showButton);
		
			// create the item panel
			actionPanel = new StackPanel();
			Children.Add(actionPanel);
		}

		/// <summary>
		/// The button that shows/hides the shelf.
		/// </summary>
		protected Button showButton;

        /// <summary>
        /// Shows the shelf.
        /// </summary>
        public void Show()
        {
            actionPanel.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Hides the shelf.
        /// </summary>
        public void Hide()
        {
            actionPanel.Visibility = Visibility.Collapsed;
        }

		/// <summary>
		/// The panal that contains the items.
		/// </summary>
		protected StackPanel actionPanel;

        /// <summary>
        /// The action buttons.
        /// </summary>
        protected Dictionary<ActionAttribute, Button> actionButtons = new Dictionary<ActionAttribute, Button>();

		/// <summary>
		/// Adds an action to the shelf.
		/// </summary>
		/// <param name="action"></param>
		public void AddAction(ActionAttribute action, Controller controller)
		{
			// get the action's image
			Image image = ResourceManager.RenderIcon(action.IconName, 48);
			image.Margin = new Thickness(8);

			// create the button
			Button button = new Button();
			if (action.Tooltip != null)
				button.ToolTip = action.Tooltip;
            StackPanel contentStack = new StackPanel();
            contentStack.Children.Add(image);
            Label label = new Label();
            label.Content = action.Name;
            contentStack.Children.Add(label);
			button.Content = contentStack;
            button.Background = new SolidColorBrush(Colors.White);
            button.Background.Opacity = 0.0;
            button.BorderBrush = null;

			// add the action
			button.Click += delegate(object sender, RoutedEventArgs args)
			{
				action.MethodInfo.Invoke(controller, null);
			};

			actionPanel.Children.Add(button);
            actionButtons[action] = button;
		}

		/// <summary>
		/// Removes an action from the shelf.
		/// </summary>
		/// <param name="action"></param>
		public void RemoveAction(ActionAttribute action)
		{

		}

	}
}
