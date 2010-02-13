// 
//  FileDialogDef.cs - MonoWorks Project
//  
//  Author:
//       Andy Selvig <ajselvig@gmail.com>
// 
//  Copyright (c) 2010 Andy Selvig
// 
//  This library is free software; you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as
//  published by the Free Software Foundation; either version 2.1 of the
//  License, or (at your option) any later version.
// 
//  This library is distributed in the hope that it will be useful, but
//  WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
//  Lesser General Public License for more details.
// 
//  You should have received a copy of the GNU Lesser General Public
//  License along with this library; if not, write to the Free Software
//  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA

using System;
using System.Collections.Generic;


namespace MonoWorks.Rendering
{
	/// <summary>
	/// File dialog types.
	/// </summary>
	public enum FileDialogType { Open, SaveAs };

	/// <summary>
	/// Contains the definition of a file dialog.
	/// </summary>
	public class FileDialogDef
	{

		public FileDialogDef()
		{
			Success = false;
			Extensions = new List<string>();
		}

		/// <summary>
		/// The type of dialog to create.
		/// </summary>
		public FileDialogType Type { get; set; }

		/// <summary>
		/// Whether or not the dialog closed in success.
		/// </summary>
		public bool Success {get; set;}

		/// <summary>
		/// The file name selected (this could be null often).
		/// </summary>
		public string FileName { get; set; }

		/// <summary>
		/// A list of extensions to allow the user to select.
		/// </summary>
		public List<string> Extensions { get; set; }


		#region Extension Descriptions

		/// <summary>
		/// Maps extensions to descriptions.
		/// </summary>
		protected static Dictionary<string, string> extensionDesc = new Dictionary<string, string>() {
			{"png", "Portable Network Graphics image file"}
		};

		/// <summary>
		/// Gets the description for an extension if it exists.
		/// </summary>
		public string GetDescription(string extension)
		{
			string desc = "";
			extensionDesc.TryGetValue(extension, out desc);
			return desc;
		}

		#endregion


	}
}
