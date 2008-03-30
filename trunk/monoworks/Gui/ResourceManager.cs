// ResourceManager.cs - MonoWorks Project
//
// Copyright (C) 2008 Andy Selvig
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA

using System;
using System.IO;
using System.Collections.Generic;

using Qyoto;

namespace MonoWorks.Gui
{
	
	/// <summary>
	/// The ResourceManager manages icons and other resources for MonoWorks applications. 
	/// </summary>
	public static class ResourceManager
	{
		
		private static bool Initialized =false;
		
		/// <summary>
		/// Initializes all resources.
		/// The initializaiton only happens once, no matter how 
		/// many times this method is called. 
		/// </summary>
		public static void Initialize()
		{
			if (!Initialized) // only do this once
			{
				Initialized = true;
								
				// load the cursors
				InitializeCursors();
			}			
		}
		
		
		/// <value>
		/// The resource directory.
		/// </value>
		public static string ResourceDirectory
		{
			get {return Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/../../../Resources/";}
		}
		
	
		
#region Cursors

		private static Dictionary<string, QCursor> Cursors;
		
		/// <summary>
		/// Loads the custom cursors from files.
		/// </summary>		
		private static void InitializeCursors()
		{
			Cursors = new Dictionary<string,QCursor>();
			
			DirectoryInfo info = new DirectoryInfo(ResourceDirectory + "cursors/");
			foreach (FileInfo fileInfo in info.GetFiles("*.png"))
			{
				QPixmap pixmap = new QPixmap(fileInfo.FullName);
				QCursor cursor = new QCursor(pixmap);
				string name = fileInfo.Name.Split('.')[0];
				Cursors[name] = cursor;
			}			
		}
		
		/// <summary>
		/// Gets the cursor with the given name.
		/// </summary>
		/// <param name="name"> The name of the cursor to get. </param>
		/// <returns>The <see cref="QCursor"/>, or null if it didn't find it.
		/// </returns>
		public static QCursor GetCursor(string name)
		{
			QCursor cursor = null;
			if (Cursors.ContainsKey(name))
				cursor = Cursors[name];
			return cursor;
		}
		
#endregion
		
	}
}
