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

		/// <summary>
		/// Rounds val to the nearest value in array. 
		/// </summary>
		public static double Round(this double[] array, double val)
		{
			if (array.Length == 0)
				return val;
			if (val <= array[0])
				return array[0];
			for (int i = 1; i < array.Length; i++)
			{
				if (val < (array[i]+array[i-1])/2.0)
					return array[i-1];
			}
			return array.Last();
		}
		
		
		/// <summary>
		/// Casts list to an IList of the new type. 
		/// </summary>
		/// <remarks>T should be a base class of whatever the list stores.</remarks>
		public static IList<T> Cast<T>(this IList list)
		{
			var newList = new List<T>();
			foreach (var item in list)
			{
				newList.Add((T)item);
			}
			return newList;
		}

	}
}
