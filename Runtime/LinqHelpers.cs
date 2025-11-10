using System.Collections.Generic;

namespace Kott.Utilities
{
    public static class LinqHelpers
    {
        public static string JoinString<T>(this IEnumerable<T> items, char seperator)
        {
            return string.Join(seperator, items);
        }

        public static string JoinString<T>(this IEnumerable<T> items, string seperator)
        {
            return string.Join(seperator, items);
        }

        public static string JoinString(this IEnumerable<string> items, string seperator)
        {
            return string.Join(seperator, items);
        }
    }
}
