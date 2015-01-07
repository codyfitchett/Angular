using RevHR.Web.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RevHR.Web.Util
{
	public static class CollectionExtensions
	{
		public static List<T> Sort<T, K>(this List<T> list, Func<T, K> keySelector) where K : IComparable<K>
		{
			list.Sort((x, y) => keySelector(x).CompareTo(keySelector(y)));
			return list;
		}

		public static List<T> SortDescending<T, K>(this List<T> list, Func<T, K> keySelector) where K : IComparable<K>
		{
			list.Sort((x, y) => keySelector(y).CompareTo(keySelector(x)));
			return list;
		}

		public static IEnumerable<QuestField> OfType(this IEnumerable<QuestField> source, QuestFieldType type)
		{
			return source.Where(f => f.Type == type);
		}

		public static string[] Names(this IEnumerable<QuestField> source)
		{
			return source.Select(f => f.Name).ToArray();
		}

		public static void ForEachWithIndex<T>(this List<T> list, Action<T, int> action)
		{
			for (int i = 0; i < list.Count; ++i)
				action(list[i], i);
		}
	}
}