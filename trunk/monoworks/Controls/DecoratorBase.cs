
// DecoratorBase.cs - MonoWorks Project
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

using System.Collections.Generic;
using MonoWorks.Rendering;


namespace MonoWorks.Controls
{
	/// <summary>
	/// The types of colors that the basic decorator stores color groups for.
	/// </summary>
	public enum ColorType {BackgroundStart, BackgroundStop, Stroke, Text, EditableStart, EditableStop};
	
	/// <summary>
	/// Abstract base class for decorators that decorate the controls by
	/// deciding how render their background and outline.
	/// </summary>
	public abstract class DecoratorBase
	{
		
		public DecoratorBase()
		{
			
			SetColorGroup(ColorType.BackgroundStart,
				new ColorGroup(
      			new Color(1f, 1f, 1f, 0.6f),
      			new Color(1f, 1f, 1f, 0.9f),
      			new Color(0.7f, 0.7f, 0.7f, 0.9f)
            ));
			
			SetColorGroup(ColorType.BackgroundStop,
				new ColorGroup(
      			new Color(1f, 1f, 1f, 0.9f),
      			new Color(1f, 1f, 1f, 0.6f),
      			new Color(0.9f, 1f, 1f, 0.9f)
            ));
			
			SetColorGroup(ColorType.Stroke,
				new ColorGroup(
				new Color(0.5f, 0.5f, 0.5f),
				new Color(0.5f, 0.5f, 0.5f),
				new Color(0.6f, 0.6f, 0.6f)
			));
			
			SetColorGroup(ColorType.EditableStart,
				new ColorGroup(
      			new Color(1f, 1f, 1f, 0.8f),
      			new Color(0.95f, 0.95f, 0.95f, 0.9f),
      			new Color(0.9f, 1f, 0.9f, 0.9f)
            ));
			
			SetColorGroup(ColorType.EditableStop,
				new ColorGroup(
      			new Color(1f, 1f, 1f, 1f),
      			new Color(1f, 1f, 1f, 1f),
      			new Color(1f, 1f, 1f, 1f)
            ));

			SetColorGroup(ColorType.Text,
				new ColorGroup(new Color(0, 0, 0))
			);
			
			SelectionColor = new Color(0.4f, 0.6f, 1f, 0.5f);
		}
		
		/// <value>
		/// The current context to render to.
		/// </value>
		public RenderContext Context {get; set;}
		
		/// <summary>
		/// Draws the decorations for the given control.
		/// </summary>
		public abstract void Decorate(Control2D control);
		
		
		#region Colors
		
		/// <summary>
		/// The color used to represent selections.
		/// </summary>
		public Color SelectionColor { get; set; }
		
		/// <summary>
		/// Associates color types with their groups.
		/// </summary>
		private Dictionary<ColorType,ColorGroup> _colorGroups = new Dictionary<ColorType, ColorGroup>();
		
		/// <summary>
		/// Gets the color group that stores colors for the given type.
		/// </summary>
		public ColorGroup GetColorGroup(ColorType colorType)
		{
			ColorGroup colorGroup = null;
			if (!_colorGroups.TryGetValue(colorType, out colorGroup))
			{
				colorGroup = new ColorGroup();
				_colorGroups[colorType] = colorGroup;
			}
			return colorGroup;
		}
		
		/// <summary>
		/// Assigns a new color group for the given type.
		/// </summary>
		public void SetColorGroup(ColorType colorType, ColorGroup colorGroup)
		{
			_colorGroups[colorType] = colorGroup;
		}
		
		/// <summary>
		/// Returns the color associated with the given type and hit state.
		/// </summary>
		public Color GetColor(ColorType colorType, HitState hitState)
		{
			return GetColorGroup(colorType).GetColor(hitState);
		}
		
		#endregion
	}
}
