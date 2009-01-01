// ResourceManager.cs - Slate  Mono Application Framework
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
using System.IO;

using MonoWorks.Framework;

namespace MonoWorks.GuiGtk.Framework
{
	
	/// <summary>
	/// Resource manager for Gtk Slate applications.
	/// </summary>
	public class ResourceManager : ResourceManagerBase
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		/// <param name="resourceDir"> The resource directory. </param>
		protected ResourceManager(string resourceDir) : base(resourceDir)
		{
			
		}
		

#region Singleton

		/// <summary>
		/// The singleton instance.
		/// </summary>
		protected static ResourceManager Instance;

		/// <summary>
		/// Initialize the singleton resource manager.
		/// </summary>
		/// <param name="dirName"> The directory of the resources.</param>
		public static void Initialize(string dirName)
		{
			if (!IsInitialized)
			{
				Instance = new ResourceManager(dirName);
			}
		}


#endregion
		

#region Icons
		
		protected Dictionary<string, Gtk.IconSet> icons = new Dictionary<string,Gtk.IconSet>();
		
		protected Gtk.IconFactory iconFactory = new Gtk.IconFactory();
		/// <value>
		/// The icon factory.
		/// </value>
		public Gtk.IconFactory IconFactory
		{
			get {return iconFactory;}
		}
		
		/// <summary>
		/// Gets an icon.
		/// </summary>
		/// <param name="iconName"> The icon's name. </param>
		/// <returns> A <see cref="Gtk.IconSet"/>. </returns>
		public static Gtk.IconSet GetIcon(string iconName)
		{
			if (!Instance.icons.ContainsKey(iconName))
				throw new Exception(iconName + " is not a valid icon name.");
			return Instance.icons[iconName];
		}
		
		/// <summary>
		/// Loads the icons from files.
		/// </summary>
		protected override void LoadIcons()
		{
			base.LoadIcons();
			
			// add the icons to the factory
			foreach (KeyValuePair<string,Gtk.IconSet> icon in icons)
				iconFactory.Add(icon.Key, icon.Value);
			iconFactory.AddDefault();
		}
		
		protected override void LoadIcon(FileInfo fileInfo, int size)
		{
			string iconName = fileInfo.Name.Split('.')[0];
			if (icons.ContainsKey(iconName)) // this icon has already been made
			{
				Gtk.IconSource source = new Gtk.IconSource();
				source.Filename = fileInfo.FullName;
				icons[iconName].AddSource( source);
			}
			else // this icon hasn't been made
			{
				Gdk.Pixbuf pixbuf = new Gdk.Pixbuf(fileInfo.FullName);
				icons[iconName] = new Gtk.IconSet(pixbuf);
			}
		}

		
		public override void FillIconBuffer(string name, int size, ref float[] buffer)
		{
			if (!Instance.icons.ContainsKey(name))
				throw new Exception(name + " is not a valid icon name.");
			
//			Gtk.IconSet iconSet = icons[name];
//			Gdk.Pixbuf pixbuf = iconSet.RenderIcon(new Gtk.Style(), Gtk.TextDirection.Ltr, Gtk.StateType.Normal, Gtk.IconSize.Dnd, null, "");
//			pixbuf.
		}

		
#endregion
		
	}
}
