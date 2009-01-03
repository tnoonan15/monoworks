// StyleClass.cs - MonoWorks Project
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

using System;
using System.Collections.Generic;
using System.Xml;

using MonoWorks.Base;

namespace MonoWorks.Rendering
{
	
	/// <summary>
	/// Represents the rendering style applied to a specific class of controls.
	/// </summary>
	public class StyleClass
	{
		
		public StyleClass()
		{
			backgrounds[HitState.None] = null;
			foregrounds[HitState.None] = null;
		}


		protected Dictionary<HitState,IFill> backgrounds = new Dictionary<HitState,IFill>();

		/// <summary>
		/// Get the background fill for the given hitstate.
		/// </summary>
		public IFill GetBackground(HitState hitState)
		{
			if (backgrounds.ContainsKey(hitState))
				return backgrounds[hitState];
			else
				return backgrounds[HitState.None];
		}

		/// <summary>
		/// Modifies the fill used for the background with the given hit state.
		/// </summary>
		/// <param name="hitState"></param>
		/// <param name="fill"></param>
		public void ModifyBackground(HitState hitState, IFill fill)
		{
			backgrounds[hitState] = fill;
		}


		protected Dictionary<HitState, Color> foregrounds = new Dictionary<HitState, Color>();

		/// <summary>
		/// Get the foreground color for the given hitstate.
		/// </summary>
		public Color GetForeground(HitState hitState)
		{
			if (foregrounds.ContainsKey(hitState))
				return foregrounds[hitState];
			else
				return foregrounds[HitState.None];
		}

		/// <summary>
		/// Modifies the color used for the foreground with the given hit state.
		/// </summary>
		/// <param name="hitState"></param>
		/// <param name="fill"></param>
		public void ModifyForeground(HitState hitState, Color color)
		{
			foregrounds[hitState] = color;
		}
		
		
#region XML I/O
		
		public static StyleClass FromXml(XmlReader reader)
		{
			StyleClass sc = new StyleClass();
			sc.LoadXml(reader);
			return sc;
		}
		
		public void LoadXml(XmlReader reader)
		{
			reader.ValidateElementName("StyleClass");
						
			while (!(reader.NodeType == XmlNodeType.EndElement && reader.Name=="StyleClass"))
			{
				reader.Read();
				if (reader.NodeType == XmlNodeType.Element)
				{
					// get the hit state
					string hitStateString = reader.GetRequiredString("hitState");
					HitState hitState = (HitState)Enum.Parse(typeof(HitState), hitStateString, true);
					
					switch (reader.Name)
					{
					case "Background":
						reader.Read();
						ModifyBackground(hitState, FillFromXml(reader));
						break;
						
					case "Foreground":
						reader.Read();
						reader.Read();
						ModifyForeground(hitState, Color.FromXml(reader));
						break;
						
					default:
						throw new Exception("Expecting a Foreground or Background element in StyleClass element, found " + reader.Name);
					}
				}
			}
		}
		
		/// <summary>
		/// Reads a fill from an XML reader.
		/// </summary>
		/// <param name="reader"> </param>
		/// <returns> </returns>
		protected IFill FillFromXml(XmlReader reader)
		{
			reader.Read();
			switch (reader.Name)
			{
			case "FillGradient":
				return FillGradient.FromXml(reader);
			case "Color":
				return Color.FromXml(reader);
			default:
				throw new Exception("Invalid fill element: " + reader.Name);
			}
		}
				
		
#endregion

	}
}
