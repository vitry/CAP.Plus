using System.Collections.Generic;

namespace System.Linq
{
    public static class EnumerableExtensions
    {
        public static IDictionary<TKey, TElement> And<TKey, TElement>(
            this IDictionary<TKey, TElement> first, IDictionary<TKey, TElement> second)
        {
            var dicts = new IDictionary<TKey, TElement>[] { first, second };
            return dicts.SelectMany(dict => dict)
                .ToLookup(dict => dict.Key, dict => dict.Value)
                .ToDictionary(p => p.Key, p => p.First());
        }
    }
}