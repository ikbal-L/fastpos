using System;

using System.Collections.Generic;
using System.Linq;
using Utilities.Extensions.Types;
using Utilities.Attributes;
using System.Reflection;
using Utilities.Extensions;

namespace FastPosFrontend.Helpers
{
    public class ObjectGraphMutationObserver<T>:IMutationObserver<T>
    {
        public T Source { get; private set ; }
        public ObjectMutationObserver<T> _rootMutationObserver;
        private Dictionary<string,IMutationObserver> _observerNodes;
        public ObjectGraphMutationObserver(T root , params PropertyInfo[] properties)
        {
            if (!CanCreateMutationObserverGraph(typeof(T)))
            {
                throw new ArgumentException($"{typeof(T).FullName} contains no properties decorated by {nameof(ObserveMutationsAttribute)}");
            }
            Source = root;
            _rootMutationObserver = new ObjectMutationObserver<T>(root);
            _observerNodes = new Dictionary<string, IMutationObserver>();
            if (!properties.Any())
            {
                properties = typeof(T).GetPropertiesDecoratedBy<ObserveMutationsAttribute>().ToArray();
            }
             
            if (properties.Any())
            {

                properties.Where(FilterByCollectionMutationFlag)
                .ToList().ForEach(p => {
                    var collectionObserver = CreateCollectionObserver(root, p);
                    _observerNodes.Add(p.Name, collectionObserver);

                });



                properties.Where(this.FilterByObjectMutationFlag).ToList().ForEach(p =>
                {
                    var mutationObserver= CreateMutationObserver(root, p);
                    _observerNodes.Add(p.Name, mutationObserver);

                });
               
            }

        }

        

        public bool FilterByCollectionMutationFlag(PropertyInfo property)
        {
            var attribute = property.GetDecoratingAttribute<ObserveMutationsAttribute>();
            return attribute.IsCollectionObserver|| attribute.IsCollectionObservingItemsObserver; 
        }

        public bool FilterByObjectMutationFlag(PropertyInfo property)
        {
            var attribute = property.GetDecoratingAttribute<ObserveMutationsAttribute>();
            return attribute.IsObjectObserver;
        }

        private IMutationObserver CreateCollectionObserver(object source,PropertyInfo property)
        {
            var isObservingItems = property.GetDecoratingAttribute<ObserveMutationsAttribute>().IsCollectionObservingItemsObserver;

            var observerType = typeof(CollectionMutationObserver<>);
            Type[] args = { property.PropertyType.GetGenericArguments()[0] };
            var propertyObserverType = observerType.MakeGenericType(args);
            var collectionObserver = Activator.CreateInstance(propertyObserverType, property.GetValue(source),isObservingItems);
            return (IMutationObserver)collectionObserver;
        }
        private IMutationObserver CreateObjectMutationObserver(T source, PropertyInfo property)
        {
            var observerType = typeof(ObjectMutationObserver<>);
            Type[] args = { property.PropertyType };
            var propertyObserverType = observerType.MakeGenericType(args);
            var instance = Activator.CreateInstance(propertyObserverType,property.GetValue(source));
            return (IMutationObserver)instance;
        }

        private IMutationObserver CreateObjectGraphMutationObserver(object source, PropertyInfo property)
        {
            var observerType = typeof(ObjectGraphMutationObserver<>);
            Type[] args = { property.PropertyType };
            var propertyObserverType = observerType.MakeGenericType(args);
            var instance = Activator.CreateInstance(propertyObserverType, property.GetValue(source));
            return (IMutationObserver)instance;
        }

        private IMutationObserver CreateMutationObserver(T root, PropertyInfo p)
        {
            if (ObjectGraphMutationObserver<object>.CanCreateMutationObserverGraph(p.PropertyType))
            {
                return CreateObjectGraphMutationObserver(root, p);
            }
            else
            {
                return CreateObjectMutationObserver(root, p);
            }
        }

        public void Commit()
        {
            _observerNodes.Values.ToList().ForEach(n => n.Commit());
            _rootMutationObserver.Commit();
        }

        public void Push()
        {
            _observerNodes.Values.ToList().ForEach(n => n.Push());
            _rootMutationObserver.Push();
        }

        public bool IsMutated()
        {
            var anyObserverNodesMutated = _observerNodes.Values.Any(n => n.IsMutated());
            var isRootMutated = _rootMutationObserver.IsMutated();
            return anyObserverNodesMutated || isRootMutated;
        }

        public static bool CanCreateMutationObserverGraph(Type type)
        {
            var properties = type.GetPropertiesDecoratedBy<ObserveMutationsAttribute>();
            return properties.Any(p=> IsPropertyObserverAnObjectMutationObserver(p));
        }

        private static bool IsPropertyObserverAnObjectMutationObserver(PropertyInfo property)
        {
            return property.GetDecoratingAttribute<ObserveMutationsAttribute>().IsObjectObserver;
        }

        public IMutationObserver this[string Property]
        {
            get => _observerNodes[Property];
            
        }
        public bool LateInit<TProperty>(string property, TProperty value = null) where TProperty:class
        {
            if (!_observerNodes.ContainsKey(property)) return false;

            value = _rootMutationObserver?.Source?.GetPropertyValue<TProperty>(property);
            if (value == null) return false;

            if (_observerNodes[property] is ObjectMutationObserver<TProperty> observer)
            {
                observer.LateInit(value);
                return true;
            }
            if (_observerNodes[property] is ObjectGraphMutationObserver<TProperty> graphObserver)
            {
                graphObserver.Source = value;
                return true;
            }

            return false;
        }
    }

    




}