using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastPosFrontend.Extensions
{
    public static class ObjectEx
    {
        public static object GetPropertyValue(this object obj, string propertyName)
        {
            var property = obj.GetType().GetProperty(propertyName);
            if (property == null)
                throw new NullReferenceException(
                    $"Trying to access Property {propertyName} from object {obj}, which does not exist");
            return property.GetValue(obj);
        }
    }

    public static class CollectionEx
    {
        public static bool Add<T>(this ICollection<T> collection, T item, bool predicate)
        {
            if (!predicate) return false;
            collection.Add(item);
            return true;

        }

        public static bool Add<T>(this ICollection<T> collection, T item, Predicate<T> predicate)
        {
            if (!predicate.Invoke(item)) return false;
            collection.Add(item);
            return true;

        }
    }

    public static class KeyValuePairEx
    {
        public static (TKey key, TValue value) ToTuple<TKey, TValue>(this KeyValuePair<TKey, TValue> keyValuePair)
        {
            return (keyValuePair.Key, keyValuePair.Value);
        }
    }
}
