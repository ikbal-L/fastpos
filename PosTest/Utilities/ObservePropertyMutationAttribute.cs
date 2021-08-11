using System;



namespace Utilities.Attributes  
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public class ObservePropertyMutationAttribute : Attribute
    {
       
        public ObservePropertyMutationAttribute()
        {
           
        }

    }

    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public class ObserveMutationsAttribute : Attribute
    {
      
        public ObserveMutationsAttribute(MutationObserverFlags flag)
        {
            switch (flag)
            {
                case MutationObserverFlags.Object:
                    IsObjectObserver = true;
                    break;
                case MutationObserverFlags.Collection:
                    IsCollectionObserver = true;
                    break;
                case MutationObserverFlags.CollectionObservingItems:
                    IsCollectionObservingItemsObserver = true;
                    break;
                
            }
        }

        public bool IsObjectObserver { get; }
        public bool IsCollectionObserver { get; }
        public bool IsCollectionObservingItemsObserver { get; }
    }
    
    public enum MutationObserverFlags
    {
        Object, Collection,CollectionObservingItems
    }




}