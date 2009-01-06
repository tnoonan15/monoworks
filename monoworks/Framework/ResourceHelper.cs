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

namespace MonoWorks.Framework
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
		/// <param name="name">Either fully qualified or unqualified resource
		/// name from the calling assembly.</param>
		/// <returns></returns>
		/// <remarks>Tries to handle naming differences between VS and 
		/// MonoDevelop embedded resources.</remarks>
		public static Stream GetStream(string name)
		{
			Assembly asm = Assembly.GetCallingAssembly();
			try
			{
				return asm.GetManifestResourceStream(name);
			}
			catch (Exception)
			{
				throw new InvalidResourceException(name, asm);
			}
		}

	}
}
