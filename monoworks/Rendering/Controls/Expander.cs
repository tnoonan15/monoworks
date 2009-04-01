// Expander.cs - MonoWorks Project
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

using MonoWorks.Base;
using MonoWorks.Framework;

namespace MonoWorks.Rendering.Controls
{
	/// <summary>
	/// The Expander control is a container that has a button 
	/// allowing the user to show or hide the contents.
	/// </summary>
	public class Expander : Bin
	{

		/// <summary>
		/// Default constructor.
		/// </summary>
		public Expander()
			: base()
		{
			button = new Button(label, expandedIcon);
			button.Clicked += OnButtonClicked;
			button.ButtonStyle = ButtonStyle.ImageNextToLabel;
			button.StyleClassName = "tool";
			IsExpanded = true;
		}

		/// <summary>
		/// Whether or not the child is expanded.
		/// </summary>
		public bool IsExpanded { get; private set; }

		/// <summary>
		/// Expands the child.
		/// </summary>
		public void Expand()
		{
			IsExpanded = true;
			MakeDirty();
		}

		/// <summary>
		/// Contracts the child.
		/// </summary>
		public void Contract()
		{
			IsExpanded = false;
			MakeDirty();
		}

		public delegate void ExpanderChangedHandler(Expander sender, bool isExpanded);



#region The Button

		private Image expandedIcon = new Image(ResourceHelper.GetStream("expanded.png"));

		private Image contractedIcon = new Image(ResourceHelper.GetStream("contracted.png"));

		/// <summary>
		/// The text displayed on the expand button.
		/// </summary>
		public string ButtonText
		{
			get { return label.Text; }
			set { label.Text = value; }
		}

		private Label label = new Label("");

		/// <summary>
		/// The expander button.
		/// </summary>
		private Button button;

		/// <summary>
		/// Handles the button being clicked.
		/// </summary>
		void OnButtonClicked(object sender, EventArgs e)
		{
			if (IsExpanded)
				Contract();
			else
				Expand();
		}

#endregion


#region Rendering

		public override Coord MinSize
		{
			get
			{
				return child.Size + new Coord(0, button.Height);
			}
		}
		
		public override void ComputeGeometry()
		{
			base.ComputeGeometry();

			child.ComputeGeometry();
			button.UserSize = false;
			button.ComputeGeometry();
			button.UserSize = true;
			button.Width = Math.Max(button.Width, child.Width);

			button.StyleClassName = StyleClassName;

			size = child.Size + new Coord(0, button.Height);
			button.Position = new Coord(0, child.Height);

			if (IsExpanded)
			{
				button.Image = expandedIcon;
				child.IsVisible = true;
			}
			else // not expanded
			{
				button.Image = contractedIcon;
				child.IsVisible = false;
			}

		}

		protected override void Render(Viewport viewport)
		{
			base.Render(viewport);

			button.RenderOverlay(viewport);
		}

#endregion


#region Mouse Interaction

		public override void OnButtonPress(MonoWorks.Rendering.Events.MouseButtonEvent evt)
		{
			base.OnButtonPress(evt);
			button.OnButtonPress(evt);
		}

		public override void OnButtonRelease(MonoWorks.Rendering.Events.MouseButtonEvent evt)
		{
			base.OnButtonRelease(evt);
			button.OnButtonRelease(evt);
		}

		public override void OnMouseMotion(MonoWorks.Rendering.Events.MouseEvent evt)
		{
			base.OnMouseMotion(evt);
			button.OnMouseMotion(evt);
		}

#endregion

	}
}
