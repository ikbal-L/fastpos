using System;

using System.Collections.Generic;
using System.Linq;
using Utilities.Extensions.Types;
using Utilities.Attributes;
using System.Reflection;
using Utilities.Extensions;

namespace Utilities.Mutation.Observers
{
    public class DeepMutationObserver<T>:IObjectMutationObserver<T>
    {
        public T Source { get; private set ; }

        public bool IsInitialized { get; private set; }

        public bool HasCommitedChanges { get; private set; }

        public ShallowMutationObserver<T> _rootMutationObserver;
        private Dictionary<string,IMutationObserver> _observerNodes;
        public DeepMutationObserver(T root , params PropertyInfo[] properties)
        {
            if (!CanCreateMutationObserverGraph(typeof(T)))
            {
                throw new ArgumentException($"{typeof(T).FullName} contains no properties decorated by {nameof(ObserveMutationsAttribute)}");
            }
            Source = root;
            IsInitialized = root != null;
            _rootMutationObserver = new ShallowMutationObserver<T>(root);
            _observerNodes = new Dictionary<string, IMutationObserver>();
            properties = GetPropertiesIfEmpty(properties);

            InitGraphNodes(root, properties);

        }

        private static PropertyInfo[] GetPropertiesIfEmpty(PropertyInfo[] properties)
        {
            if (!properties.Any())
            {
                properties = typeof(T).GetPropertiesDecoratedBy<ObserveMutationsAttribute>().ToArray();
            }

            return properties;
        }

        private void InitGraphNodes(T root, PropertyInfo[] properties)
        {
            if (properties.Any())
            {

                properties.Where(FilterByCollectionMutationFlag)
                .ToList().ForEach(p =>
                {
                    var collectionObserver = CreateCollectionObserver(root, p);
                    _observerNodes.Add(p.Name, collectionObserver);

                });

                properties.Where(this.FilterByObjectMutationFlag).ToList().ForEach(p =>
                {
                    var mutationObserver = CreateMutationObserver(root, p);
                    _observerNodes.Add(p.Name, mutationObserver);

                });

            }
        }

        private bool FilterByCollectionMutationFlag(PropertyInfo property)
        {
            var attribute = property.GetDecoratingAttribute<ObserveMutationsAttribute>();
            return attribute.IsCollectionObserver|| attribute.IsCollectionObservingItemsObserver; 
        }

        private bool FilterByObjectMutationFlag(PropertyInfo property)
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
            var collectionObserver = Activator.CreateInstance(propertyObserverType, property.GetValue(source),isObservingItems,true,this);
            return (IMutationObserver)collectionObserver;
        }
        private IMutationObserver CreateObjectMutationObserver(T source, PropertyInfo property)
        {
            var observerType = typeof(ShallowMutationObserver<>);
            Type[] args = { property.PropertyType };
            var propertyObserverType = observerType.MakeGenericType(args);
            var instance = Activator.CreateInstance(propertyObserverType,property.GetValue(source));
            return (IMutationObserver)instance;
        }

        private IMutationObserver CreateObjectGraphMutationObserver(object source, PropertyInfo property)
        {
            var observerType = typeof(DeepMutationObserver<>);
            Type[] args = { property.PropertyType };
            var propertyObserverType = observerType.MakeGenericType(args);
            var instance = Activator.CreateInstance(propertyObserverType, property.GetValue(source));
            return (IMutationObserver)instance;
        }

        private IMutationObserver CreateMutationObserver(T root, PropertyInfo p)
        {
            if (DeepMutationObserver<object>.CanCreateMutationObserverGraph(p.PropertyType))
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
            _observerNodes.Values.ToList().ForEach(n => {

                if (n.IsInitialized)
                {
                    n.Commit();
                }
            });
            _rootMutationObserver.Commit();
            HasCommitedChanges = true;
        }

        public void Push()
        {
            _observerNodes.Values.Where(n=>n.IsInitialized).ToList().ForEach(n => n.Push());
            _rootMutationObserver.Push();
            HasCommitedChanges = false;
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
            //return properties.Any(p=> IsPropertyObserverAnObjectMutationObserver(p));
            return properties.Any();
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

            if (_observerNodes[property] is ShallowMutationObserver<TProperty> observer)
            {
                observer.LateInit(value);
                return true;
            }
            if (_observerNodes[property] is DeepMutationObserver<TProperty> graphObserver)
            {
                graphObserver.Source = value;
                return true;
            }

            return false;
        }

        public TSource GetDifference<TSource>(Func<IMutationObserver, TSource> generator) where TSource : class
        {
            return generator.Invoke(this);
        }

        public IPropertyMutation GetPropertyMutation(string propertyName)
        {
            return ((IObjectMutationObserver)_rootMutationObserver).GetPropertyMutation(propertyName);
        }
    }

    




}