using System.Collections.Generic;
using System.Collections;

namespace com.AutopilotLlc.BinarySerializer
{
    internal static class EnumerableExtensions
    {
        internal static ICollection<T> ToCollection<T>(this IEnumerable<T> enumerable)
        {
            if (enumerable is ICollection<T>)
                return (ICollection<T>)enumerable;

            return new List<T>(enumerable);
        }

        internal static ICollection ToCollection(this IEnumerable enumerable)
        {
            if (enumerable is ICollection)
                return (ICollection)enumerable;

            var collection = new List<object>();
            foreach (var item in enumerable)
                collection.Add(item);

            return collection;
        }

        internal static IDictionary ToDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> enumerable)
        {
            if (enumerable is IDictionary)
                return (IDictionary)enumerable;

            var collection = new Dictionary<object, object>();
            foreach (var item in enumerable)
                collection.Add(item.Key, item.Value);

            return collection;
        }
    }
}
