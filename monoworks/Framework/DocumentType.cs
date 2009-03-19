// DocumentType.cs - MonoWorks Project
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

using MonoWorks.Base;

namespace MonoWorks.Framework
{
	/// <summary>
	/// This class stores information about types that can be instantiated as documents.
	/// </summary>
	public class DocumentType
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		/// <param name="typeName"> The name of the type.</param>
		/// <param name="baseNamespace">The base of the namespace to which 
		/// the type belongs, or null if one isn't specified.</param>
		/// <remarks> All loaded assemblies will be searched for types of 
		/// this name and base namespace. This lookup does not require 
		/// fully-qualified names so that the same document type UI entry
		/// can be used in multiple GUI frameworks.</remarks>
		public DocumentType(string typeName, string baseNamespace)
		{
			this.typeName = typeName;

			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			foreach (Assembly assembly in assemblies) // search all loaded assemblies
			{
				Type[] types = assembly.GetExportedTypes();
				foreach (Type type in types) // search all exported types in this assembly
				{
					var comps = type.Name.Split('.');
					if (type.FullName.StartsWith(baseNamespace) &&
						comps[comps.Length-1] == typeName)
					{
						this.type = type;
					}
				}
			}

			if (this.type == null)
				throw new Exception(String.Format("Document type {0} is not present in any loaded assembly.", typeName));
		}


		protected Type type = null;
		/// <summary>
		/// The type of the document content.
		/// </summary>
		public Type Type
		{
			get { return type; }
		}

		protected string typeName;
		/// <summary>
		/// The name of the type (defined in the UI file).
		/// </summary>
		public string TypeName
		{
			get { return typeName; }
		}

		protected string displayName;
		/// <summary>
		/// The name used when displaying this type.
		/// </summary>
		public string DisplayName
		{
			get { return displayName; }
			set { displayName = value; }
		}

		private string iconName;
		/// <value>
		/// The name of the icon associated with this document type.
		/// </value>
		public string IconName
		{
			get {return iconName;}
			set{iconName = value;}
		}
	}


}
