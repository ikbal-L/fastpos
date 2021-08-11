using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Utilities.Attributes;

namespace Utilities.Extensions
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
        public static T GetPropertyValue<T>(this object obj, string propertyName)
        {
            var property = obj.GetType().GetProperty(propertyName);
            if (property == null)
                throw new NullReferenceException(
                    $"Trying to access Property {propertyName} from object {obj}, which does not exist");
            if (property.GetValue(obj) is T tProperty)
            {
                return tProperty;
            }
            else
            {
                throw new InvalidCastException($"The type of the property you wanted to access is {property.PropertyType}, whereas the supplied Type argument is ${typeof(T)}");
            }
            
        }
    }

}
namespace Utilities.Extensions.Collections
{
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

namespace Utilities.Extensions.Types
{
    public static class TypeEx
    {
        public static ICollection<string> GetPropertyNamesDecoratedBy<TAttribute>(this Type type,bool inherit = false)where TAttribute:Attribute
        {
            var attributeType = typeof(TAttribute);
            return type.GetProperties().Where(p => p.IsDefined(attributeType, inherit)).Select(p => p.Name).ToList();
        }

        public static ICollection<PropertyInfo> GetPropertiesDecoratedBy<TAttribute>(this Type type, bool inherit = false) where TAttribute : Attribute
        {
            var attributeType = typeof(TAttribute);
            return type.GetProperties().Where(p => p.IsDefined(attributeType, inherit)).ToList();
        }

        public static TAttribute GetDecoratingAttribute<TAttribute>(this PropertyInfo property,bool inherit = false) where TAttribute : Attribute
        {
            var attributeType = typeof(TAttribute);
            var result = property.GetCustomAttributes(inherit).FirstOrDefault(a => a.GetType() == attributeType) ;
            if (result is TAttribute attribute)
            {
                return attribute;
            }
            else
            {
                return null;
            }
        }
    }
}
