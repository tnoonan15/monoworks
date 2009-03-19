using System;
using System.Collections.Generic;


namespace MonoWorks.Framework
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
