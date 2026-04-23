using System.Collections.Generic;

namespace CodeExtensions
{
    public static class CollectionsExtensions
    {
        public static bool IsNullOrEmpty<T>(this IReadOnlyList<T> array)
        {
            return array == null || array.Count == 0;
        }
    }
}