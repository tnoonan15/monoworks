using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace MonoWorks.Base
{
	public static class CollectionExtensions
	{


		/// <summary>
		/// Writes a list to a string by concatonating the element string values.
		/// </summary>
		/// <param name="list"></param>
		/// <returns></returns>
		public static string ListString(this IList list)
		{
			StringBuilder builder = new StringBuilder("[");
			foreach (object item in list)
			{
				builder.Append(item.ToString());
				builder.Append(',');
			}
			if (list.Count > 0) // remove the last comma
				builder.Remove(builder.Length - 1, 1);
			builder.Append(']');
			return builder.ToString();
		}
	}
}
