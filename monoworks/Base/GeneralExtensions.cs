// GeneralExtensions.cs - MonoWorks Project
//
//  Copyright (C) 2009 Andy Selvig
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
using System.Collections;
using System.Collections.Generic;

namespace MonoWorks.Base
{
	
	/// <summary>
	/// General extensions to make life more pleasant.
	/// </summary>
	public static class GeneralExtensions
	{




		/// <summary>
		/// Parse a string with an Enum type.
		/// </summary>
		/// <typeparam name="T">An enum.</typeparam>
		/// <param name="value"></param>
		/// <returns></returns>
		public static T EnumParse<T>(this string value)
		{
			return GeneralExtensions.EnumParse<T>(value, false);

		}

		/// <summary>
		/// Parse a string with an Enum type.
		/// </summary>
		/// <typeparam name="T">An enum.</typeparam>
		/// <param name="value"></param>
		/// <param name="ignoreCase">Whether or not to ignore the case of the string.</param>
		/// <returns></returns>
		public static T EnumParse<T>(this string value, bool ignoreCase)
		{
			if (value == null)
				throw new ArgumentNullException("value");

			value = value.Trim();
			if (value.Length == 0)
				throw new ArgumentException("Must specify valid information for parsing in the string.", "value");

			Type t = typeof(T);
			if (!t.IsEnum)
				throw new ArgumentException("Type provided must be an Enum.", "T");
			T enumType = (T)Enum.Parse(t, value, ignoreCase);
			return enumType;
		}

	}
}
