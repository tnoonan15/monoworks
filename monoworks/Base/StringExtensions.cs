// 
//  StringExtensions.cs - MonoWorks Project
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
using System.Text;
using System.Collections.Generic;

namespace MonoWorks.Base
{
	/// <summary>
	/// Extension methods for strings.
	/// </summary>
	public static class StringExtensions
	{
		/// <summary>
		/// Inserts spaces between the words in a camel-case string.
		/// </summary>
		public static string InsertCamelSpaces(this string s)
		{
			var builder = new StringBuilder();
			int lastWord = 0;
			for (int i = 1; i < s.Length; i++)
			{
				if (char.IsUpper(s[i])) // a new word
				{
					builder.Append(s.Substring(lastWord, i-lastWord));
					builder.Append(' ');
					lastWord = i;
				}
			}			
			builder.Append(s.Substring(lastWord));
			return builder.ToString();
		}
		
	}
}

