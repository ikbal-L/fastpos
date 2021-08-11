using System;
using Utilities.Extensions;

namespace Utilities.Mutation
{
    public class PropertyMutation
    {
        public PropertyMutation(string propertyName, object mutatedObject)
        {
            PropertyName = propertyName;
            Initial = mutatedObject?.GetPropertyValue(propertyName);
        }

        public string PropertyName { get; }
        public object Initial { get; set; }

        public object Committed { get; set; }

        public bool IsCommitted { get; set; }
    }


    public interface IPropertyDiff
    {
        void Invoke(object obj, PropertyMutation mutation);
    }
    public class PropertyDiff<TProperty> : IPropertyDiff
    {
        private readonly Func<TProperty, TProperty, TProperty> _propertyMutationHanlder;

        public PropertyDiff(Func<TProperty, TProperty, TProperty> propertyMutationHanlder)
        {
            _propertyMutationHanlder = propertyMutationHanlder;
        }

        public void Invoke(object obj, PropertyMutation mutation)
        {
            if (mutation?.Initial is TProperty initial && mutation?.Committed is TProperty committed)
            {
                var value = _propertyMutationHanlder.Invoke(initial, committed);
                obj.GetType().GetProperty(mutation.PropertyName).SetValue(obj, value);
            }

        }
    }





}