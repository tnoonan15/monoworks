using System;
using System.Collections.Generic;
using System.Windows.Controls;


namespace MonoWorks.GuiWpf.Framework
{
	/// <summary>
	/// The toolbox is a tool control that stores shelves of action icons.
	/// </summary>
	public class ToolBox : ToolBar
	{

		public ToolBox(string name)
			: base()
		{
            // create the label
            label = new Label();
            label.Content = name;
            Items.Add(label);

			container = new ToolBoxContainer(this);
			Items.Add(container);
		}

        /// <summary>
        /// Container for shelves.
        /// </summary>
		ToolBoxContainer container;

        /// <summary>
        /// Tool box label.
        /// </summary>
        protected Label label;

        /// <summary>
        /// The name of the toolbox.
        /// </summary>
        public new string Name
        {
            get { return (string)label.Content; }
            set { label.Content = value; }
        }

		#region Shelves

		/// <summary>
		/// The shelves.
		/// </summary>
		protected Dictionary<string, ToolShelf> shelves = new Dictionary<string,ToolShelf>();

        /// <summary>
        /// Add a shelf to the tool box.
        /// </summary>
        /// <param name="name"> The name of the shelf.</param>
        /// <returns> The new shelf.</returns>
		public ToolShelf AddShelf(string name)
		{
			ToolShelf shelf = new ToolShelf(name, this);
			shelves[name] = shelf;
			container.Children.Add(shelf);
            ShowShelf(shelf);
			return shelf;
		}

        /// <summary>
        /// Shows the given shelf.
        /// </summary>
        public void ShowShelf(ToolShelf showShelf)
        {
            foreach (ToolShelf shelf in shelves.Values)
            {
                if (shelf == showShelf)
                    shelf.Show();
                else
                    shelf.Hide();
            }
        }

		#endregion



	}



	/// <summary>
	/// Container for tool shelves.
	/// </summary>
	internal class ToolBoxContainer : StackPanel
	{

		internal ToolBoxContainer(ToolBox parent)
			: base()
		{

		}

	}

}
