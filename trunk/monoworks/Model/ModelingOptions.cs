// ModelingOptions.cs - MonoWorks Project
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
using System.IO;
using System.Xml;
using System.Xml.Serialization;

using MonoWorks.Framework;
using MonoWorks.Rendering;

namespace MonoWorks.Model
{
	/// <summary>
	/// ModelingOptions contains the options for modeling.
	/// </summary>
	[XmlRoot()]
	public class ModelingOptions
	{

		public ModelingOptions()
		{
			colors.Add("sketchable", new ColorGroup("Sketchable", "Sketchable Hover", "Sketchable Selected"));
			colors.Add("ref-fill", new ColorGroup("Ref Fill", "Ref Fill Hover", "Ref Fill Selected"));
			colors.Add("ref-edge", new ColorGroup("Ref Edge", "Ref Edge Hover", "Ref Edge Selected"));
		}

		#region Singleton

		private static ModelingOptions instance = CreateInstance();

		/// <summary>
		/// The singleton modeling options instance.
		/// </summary>
		public static ModelingOptions Global
		{
			get { return instance; }
		}

		/// <summary>
		/// Create the singleton instance.
		/// </summary>
		protected static ModelingOptions CreateInstance()
		{
			// make sure the modeling colors are loaded
			ColorManager.Global.Load(ResourceHelper.GetStream("DefaultColors.xml"));

			XmlSerializer deserializer = new XmlSerializer(typeof(ModelingOptions));
			TextReader textReader = new StreamReader(ResourceHelper.GetStream("DefaultModelingOptions.xml"));
			ModelingOptions modelingOptions = (ModelingOptions)deserializer.Deserialize(textReader);
			textReader.Close();
			return modelingOptions;
		}

		#endregion


		/// <summary>
		/// Whether or not to draw the grid while sketching.
		/// </summary>
		[XmlElement()]
		public bool DrawGrids { get; set; }

		/// <summary>
		/// Whether or not to snap to the grid.
		/// </summary>
		[XmlElement()]
		public bool SnapToGrid { get; set; }


#region Colors

		[XmlArray]
		private Dictionary<string, ColorGroup> colors = new Dictionary<string, ColorGroup>();

		/// <summary>
		/// Gets the color entities of the given name and hit state.
		/// </summary>
		public Color GetColor(string name, HitState hitState)
		{
			ColorGroup group = null;
			if (colors.TryGetValue(name, out group))
				return group.GetColor(hitState);
			else
				throw new Exception("There is no color group called " + name);
		}

#endregion

	}


	/// <summary>
	/// A group of color names, one for each hit state.
	/// </summary>
	public class ColorGroup : Dictionary<HitState,string>
	{
		public ColorGroup()
		{
		}

		/// <summary>
		/// Initialize the class with the three color names.
		/// </summary>
		public ColorGroup(string none, string hovering, string selected) : this()
		{
			this[HitState.None] = none;
			this[HitState.Hovering] = hovering;
			this[HitState.Selected] = selected;
		}

		public Color GetColor(HitState hitState)
		{
			string name = null;
			if (TryGetValue(hitState, out name))
				return ColorManager.Global[name];
			if (hitState != HitState.None && TryGetValue(HitState.None, out name))
				return ColorManager.Global[name];
			else
				throw new Exception("Color group does not have an entry for this color with hit state " + hitState.ToString());
		}

	}


}
