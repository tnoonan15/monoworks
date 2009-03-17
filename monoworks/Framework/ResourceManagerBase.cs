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
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace MonoWorks.Framework
{
	/// <summary>
	/// Base class for resource managers.
	/// </summary>
	public abstract class ResourceManagerBase
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		protected ResourceManagerBase()
		{
			singletonInstance = this;
			
		}

		/// <summary>
		/// Throws an exception if the resource manager is not initialized.
		/// </summary>
		protected static void EnsureInitialized()
		{
			if (singletonInstance == null)
				throw new Exception("ResourceManager is not initialized. " + 
			"You must initialize a GUI-dependent resource manager before trying to use the static methods.");
		}
		
		/// <summary>
		/// Singleton instance of the base resource manager. 
		/// </summary>
		/// <remarks>Objects in the rendering library can use this to 
		/// access some resources that aren't GUI specific.</remarks>
		private static ResourceManagerBase singletonInstance;

		
#region Directory Loading

		/// <summary>
		/// Loads resources from the given directory.
		/// </summary>
		protected void LoadDir(string dirName)
		{
			if (!DirIsLoaded(dirName))
			{
				DirectoryInfo info = new DirectoryInfo(dirName);
				LoadIconDirectory(info);
				loadedDirs.Add(dirName);
			}
		}

		/// <summary>
		/// Loads the icons from a root directory.
		/// </summary>
		protected virtual void LoadIconDirectory(DirectoryInfo info)
		{
			DirectoryInfo[] iconDirs = info.GetDirectories("icons*"); // get the icon directories
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
		protected abstract void LoadIcon(FileInfo fileInfo, int size);
		
		protected List<string> loadedDirs = new List<string>();

		/// <summary>
		/// Returns true if the given directory has been loaded.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public bool DirIsLoaded(string name)
		{
			return loadedDirs.Contains(name);
		}

#endregion
				
		
#region Assembly Loading

		/// <summary>
		/// Loads an assembly into the global resource manager.
		/// </summary>
		public static void LoadAssembly(string asmName)
		{
			EnsureInitialized();
			singletonInstance.LoadAsm(asmName);
		}

		/// <summary>
		/// Load an assembly.
		/// </summary>
		/// <param name="asmName"></param>
		protected void LoadAsm(string asmName)
		{
			if (!AsmIsLoaded(asmName))
			{
				Assembly asm = Assembly.Load(AssemblyName.GetAssemblyName(asmName + ".dll"));
				LoadIconAssembly(asm);
				loadedAsms.Add(asmName);
			}
		}


		/// <summary>
		/// Load icons from an assembly.
		/// </summary>
		protected virtual void LoadIconAssembly(Assembly asm)
		{
			string[] resNames = asm.GetManifestResourceNames();
			foreach (string resName in resNames)
			{
				// look for an icon declaration
				string[] comps = resName.Split('.');
				if (comps[comps.Length - 1] == "png" && comps.Length > 1)
				{
					Stream stream = asm.GetManifestResourceStream(resName);
					string name = comps[comps.Length - 2];
					LoadIconStream(stream, name);
				}
			}
		}

		/// <summary>
		/// Loads the icon from the given stream and stores it with the given name.
		/// </summary>
		/// <param name="stream"></param>
		/// <param name="name"></param>
		/// <remarks>The size is inferred.</remarks>
		protected abstract void LoadIconStream(Stream stream, string name);
		

		protected List<string> loadedAsms = new List<string>();

		/// <summary>
		/// Returns true if the given assembly has been loaded.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public bool AsmIsLoaded(string name)
		{
			return loadedAsms.Contains(name);
		}
		
#endregion


#region Icons

		/// <summary>
		/// Gets an icon stream from an already loaded icon.
		/// </summary>
		public abstract void WriteLoadedIconToFile(string name, string iconName, int size);

		/// <summary>
		/// Renders an already loaded icon back to a file.
		/// </summary>
		public static void RenderIconToFile(string fileName, string iconName, int size)
		{
			EnsureInitialized();
			singletonInstance.WriteLoadedIconToFile(fileName, iconName, size);
		}

#endregion


	}
}
