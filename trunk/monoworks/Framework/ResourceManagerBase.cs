// ResourceManagerBase.cs - MonoWorks Project
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
using System.IO;

namespace MonoWorks.Framework
{
	/// <summary>
	/// Base class for resource managers.
	/// </summary>
	public abstract class ResourceManagerBase
	{
		/// <summary>
		/// Constructor that takes the resource directory name.
		/// </summary>
		/// <param name="dir"> Absolute or relative path to the resource directory.</param>
		protected ResourceManagerBase(string dirName)
		{
			singletonInstance = this;
			
			resourceDir = new DirectoryInfo(dirName);
			IsInitialized = true;

			LoadIcons();
		}

		/// <summary>
		/// Whether the resources have been initialized.
		/// </summary>
		protected static bool IsInitialized = false;

		/// <summary>
		/// Throws an exception if the resource manager is not initialized.
		/// </summary>
		protected static void EnsureInitialized()
		{
			if (!IsInitialized)
				throw new Exception("Resource Manager is not initialized.");
		}
		
		/// <summary>
		/// Singleton instance of the base resource manager. 
		/// </summary>
		/// <remarks>Objects in the rendering library can use this to 
		/// access some resources that aren't GUI specific.</remarks>
		private static ResourceManagerBase singletonInstance;

		/// <summary>
		/// The resource directory.
		/// </summary>
		protected DirectoryInfo resourceDir;

		/// <summary>
		/// Loads the icons.
		/// </summary>
		protected virtual void LoadIcons()
		{
			DirectoryInfo[] iconDirs = resourceDir.GetDirectories("icons*"); // get the icon directories
			foreach (DirectoryInfo iconDir in iconDirs)
			{
				// determine the size associated with this directory
				int size = Convert.ToInt32(iconDir.Name.Substring(5));

				FileInfo[] iconFiles = iconDir.GetFiles();
				foreach (FileInfo iconFile in iconFiles)
				{
					if (iconFile.Extension == ".png") // only load supported file types
						LoadIcon(iconFile, size);
				}
			}
		}

		/// <summary>
		/// Loads a single icon from the given path.
		/// </summary>
		/// <param name="fileInfo"></param>
		/// <param name="size"></param>
		protected abstract void LoadIcon(FileInfo fileInfo, int size);

		/// <summary>
		/// Fills the buffer with the pixels of the icon with the given name.
		/// </summary>
		/// <remarks>The buffer will be automatically sized.</remarks>
		public abstract void FillIconBuffer(string name, int size, ref float[] buffer);

		/// <summary>
		/// Fills the buffer with the pixels of the icon with the given name.
		/// </summary>
		/// <remarks>The buffer will be automatically sized.</remarks>
		public static void GetIconPixels(string name, int size, ref float[] buffer)
		{
				singletonInstance.FillIconBuffer(name, size, ref buffer);
		}

	}
}
