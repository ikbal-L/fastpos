using System;



namespace Utilities.Attributes  
{
    [System.AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public class ObservePropertyMutationAttribute : Attribute
    {
       
        public ObservePropertyMutationAttribute()
        {
           
        }

    }

    [System.AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public class ObserveMutationsAttribute : Attribute
    {
        // See the attribute guidelines at 
        //  http://go.microsoft.com/fwlink/?LinkId=85236
        readonly string positionalString;

        // This is a positional argument
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