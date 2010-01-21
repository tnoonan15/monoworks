// ToolBar.cs - MonoWorks Project
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

namespace MonoWorks.Controls
{
	
	/// <summary>
	/// A stack of buttons.
	/// </summary>
	public class ToolBar : GenericStack<Button>
	{
		
		public ToolBar()
		{
			ToolStyle = "tool";
		}


		public override void AddChild(Button child)
		{
			base.AddChild(child);
			
			child.ButtonStyle = buttonStyle;
		}

		
		private string _toolStyle = "tool";
		/// <value>
		/// The style class to use for the child controls.
		/// </value>
		public string ToolStyle
		{
			get {return _toolStyle;}
			set
			{
				_toolStyle = value;
			}
		}

		protected ButtonStyle buttonStyle = ButtonStyle.ImageOverLabel;
		/// <value>
		/// Set the button style of all buttons in the toolbar.
		/// </value>
		public ButtonStyle ButtonStyle
		{
			set
			{
				buttonStyle = value;
				foreach (Control2D child in Children)
				{
					if (child is Button)
						(child as Button).ButtonStyle = buttonStyle;
				}
			}
			get {return buttonStyle;}
		}
		
				
		/// <summary>
		/// Get a child button by it label.
		/// </summary>
		/// <returns> The button, or null if there isn't one present. </returns>
		public Button GetButton(string label)
		{
			foreach(var child in Children)
			{
				if (child is Button && (child as Button).LabelString == label)
					return (child as Button);
        	}
			return null;
		}


#region Mouse Interaction


		public override void OnMouseMotion(MonoWorks.Rendering.Events.MouseEvent evt)
		{
			base.OnMouseMotion(evt);

			// catch hover even if the buttons didn't
			if (HitTest(evt.Pos))
				evt.Handle();
		}


#endregion


	}
}
