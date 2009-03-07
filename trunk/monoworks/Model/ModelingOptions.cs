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

namespace MonoWorks.Model
{
	/// <summary>
	/// ModelingOptions contains the options for modeling.
	/// </summary>
	[XmlRoot()]
	public class ModelingOptions
	{

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
		public static ModelingOptions CreateInstance()
		{
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

	}
}
