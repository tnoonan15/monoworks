using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace MonoWorks.Base
{
	/// <summary>
	/// Extension methods for classes in the System.Collections.Generic.
	/// </summary>
	public static class CollectionExtensions
	{
		/// <summary>
		/// Writes a list to a string by concatenating the element string values.
		/// </summary>
		public static string ListString(this IList list)
		{
			var builder = new StringBuilder("[");
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
		
		/// <summary>
		/// Joins an array of strings together with a separator. 
		/// </summary>
		public static string Join(this string[] list, string sep)
		{
			var builder = new StringBuilder();
			for (int i = 0; i < list.Length; i++)
			{
				builder.Append(list[i]);
				if (i < list.Length - 1)
					builder.Append(sep);
			}
			return builder.ToString();
		}
		
		/// <summary>
		/// Joins an array of strings together with a separator. 
		/// </summary>
		public static string Join(this string[] list, char sep)
		{
			return list.Join(sep.ToString());
		}

		/// <summary>
		/// Returns the first element of a list.
		/// </summary>
		public static T First<T>(this IList<T> list)
		{
			return list[0];
		}

		/// <summary>
		/// Returns the last element of a list.
		/// </summary>
		public static T Last<T>(this IList<T> list)
		{
			return list[list.Count - 1];
		}


	}
}
