// ResourceHelper.cs - MonoWorks Project
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
using System.Reflection;
using System.IO;

namespace MonoWorks.Base
{
	/// <summary>
	/// Exception for invalid resource names.
	/// </summary>
	public class InvalidResourceException : Exception
	{
		public InvalidResourceException(string resName, Assembly asm)
			: base(resName + " is an invalid resource name for assembly " + asm.FullName)
		{
		}
	}

	/// <summary>
	/// Helper class for managing Resources.
	/// </summary>
	public static class ResourceHelper
	{
		/// <summary>
		/// Gets a resource stream based on the provided name.
		/// </summary>
		/// <returns></returns>
		/// <remarks>Tries to handle naming differences between VS and 
		/// MonoDevelop embedded resources.</remarks>
		public static Stream GetStream(string name)
		{
			Assembly asm = Assembly.GetCallingAssembly();
			return GetStream(name, asm);
		}
		
		/// <summary>
		/// Gets a resource stream based on the provided name from the named assembly.
		/// </summary>
		/// <returns></returns>
		public static Stream GetStream(string name, string asmName)
		{
			Assembly asm = Assembly.Load(new AssemblyName(asmName));
			return GetStream(name, asm);
		}

		/// <summary>
		/// Gets a resource stream based on the provided name and assembly.
		/// </summary>
		/// <returns></returns>
		/// <remarks>Tries to handle naming differences between VS and 
		/// MonoDevelop embedded resources.</remarks>
		public static Stream GetStream(string name, Assembly asm)
		{
			string[] resNames = asm.GetManifestResourceNames();
			if (Array.IndexOf(resNames, name) > -1) // exact match
				return asm.GetManifestResourceStream(name);
			else
			{
				// search for incomplete matches
				foreach (string resName in resNames)
				{
					if (resName.EndsWith(name) && resName[resName.Length - name.Length - 1] == '.') // incomplete match
						return asm.GetManifestResourceStream(resName);
				}
			}
			throw new InvalidResourceException(name, asm);
		}

	}
}
