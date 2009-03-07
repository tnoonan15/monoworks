// DimensionManager.cs - MonoWorks Project
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
using System.Xml;
using System.IO;
using System.Reflection;

namespace MonoWorks.Base
{
	
	using UnitsHash = Dictionary<String, double>;
	
	/// <summary>
	/// The DimensionManager manages the document's display units for dimensional values.
	/// </summary>
	public class DimensionManager
	{
		
#region Singleton
		
		private static DimensionManager currentInstance = Default();
		/// <value>
		/// The current singleton instance.
		/// </value>
		public static DimensionManager Global
		{
			get {return currentInstance;}
			set {currentInstance = value;}
		}
		
#endregion
		
		
		
		/// <summary>
		/// Default constructor.
		/// </summary>
		public DimensionManager()
		{
			allUnits = new Dictionary<string,UnitsHash>();
			defaultUnits = new Dictionary<string,string>();
			displayUnits = new Dictionary<string,string>();
		}
		
		

#region File I/0
		
		/// <summary>
		/// Gets the default dimension manager.
		/// </summary>
		/// <returns> The default <see cref="DimensionManager"/>. </returns>
		protected static DimensionManager Default()
		{
			Assembly asm = Assembly.GetExecutingAssembly();
			foreach (string resName in asm.GetManifestResourceNames())
			{
				if (resName.Contains("DefaultUnits"))
					return FromStream(asm.GetManifestResourceStream(resName));
			}
			throw new Exception("DimensionManager couldn't find a DefaultUnits.xml resource.");
		}
		
		/// <summary>
		/// Reads a dimension manager from an XML file.
		/// </summary>
		/// <param name="fileName"> The name of the file to read from. </param>
		/// <returns> A new <see cref="DimensionManager"/>. </returns>
		public static DimensionManager FromFile(string fileName)
		{
			Stream stream = new FileStream(fileName, FileMode.Open);
			DimensionManager manager = FromStream(stream);
			stream.Close();
			return manager;
		}
		
		/// <summary>
		/// Reads a dimension manager from an XML strem.
		/// </summary>
		/// <param name="stream"> The stream to read from. </param>
		/// <returns> A new <see cref="DimensionManager"/>. </returns>
		public static DimensionManager FromStream(Stream stream)
		{
			DimensionManager manager = new DimensionManager();
			
			// temporary storage
			Dictionary<string, double> units = null;
			string name = null;
			string defaultUnits = null;
			string displayUnits = null;

			XmlTextReader reader = new XmlTextReader(stream);
			
			while (!reader.EOF)
			{
				reader.Read();
				
				switch (reader.NodeType)
				{
				case XmlNodeType.Element:
					
					switch (reader.Name)
					{
					case "Dimensional":
						name = reader.GetAttribute("name");
						units = new Dictionary<string,double>();
						defaultUnits = reader.GetAttribute("defaultUnits");
						displayUnits = reader.GetAttribute("displayUnits");
						break;
					case "Units":
						if (units==null)
							throw new Exception("All Units tags must be inside Dimensional tags.");
						string unitsName = reader.GetAttribute("name");
						reader.Read(); // read to the element value
						double val = Convert.ToDouble(reader.Value);
						units[unitsName] = val;
						break;
					}
					break;
					
				case XmlNodeType.EndElement:
					if (reader.Name == "Dimensional")
					{
						manager.allUnits[name] = units;
						manager.defaultUnits[name] = defaultUnits;
						manager.displayUnits[name] = displayUnits;
						units = null;
					}
					break;
				}
			}
			
			reader.Close();
			
			return manager;
		}
		
#endregion
		
		
#region Units
		
		
		/// <summary>
		/// Storage for all units.
		/// </summary>
		private Dictionary<string, UnitsHash> allUnits;
		
		/// <summary>
		/// Gets the units hash for the given type.
		/// </summary>
		/// <param name="type"> The unqualified type name. </param>
		/// <returns> The <see cref="UnitsHash"/> associated with the type. </returns>
		public UnitsHash GetUnits(string type)
		{
			if (allUnits.ContainsKey(type))
				return allUnits[type];
			else
				throw new Exception(type + " is not a registered Dimensional type name");
		}
		
		private Dictionary<string, string> defaultUnits;		
		
		/// <summary>
		/// Gets the default units name for the given type.
		/// </summary>
		/// <param name="type"> The unqualified type name. </param>
		/// <returns> The name of the default units of the type. </returns>
		public string GetDefaultUnits(string type)
		{
			if (defaultUnits.ContainsKey(type))
				return defaultUnits[type];
			else
				throw new Exception(type + " is not a registered Dimensional type name");
		}
		
		private Dictionary<string, string> displayUnits;
		
		/// <summary>
		/// Gets the display units name for the given type.
		/// </summary>
		/// <param name="type"> The unqualified type name. </param>
		/// <returns> The name of the display units of the type. </returns>
		public string GetDisplayUnits(string type)
		{
			if (displayUnits.ContainsKey(type))
				return displayUnits[type];
			else
				throw new Exception(type + " is not a registered Dimensional type name");
		}
		
#endregion



	}
}
