using System.Collections.Generic;
using System.Linq;
using Caliburn.Micro;

namespace FastPosFrontend.Navigation
{
    public static class NavigationIndexer
    {
        private static readonly Dictionary<string, int> Indexes = new Dictionary<string, int>();
        private static int LastAddedIndex = -1;

        public interface IIndexer
        {
            IIndexer Add<T>();
            IIndexer Add(string groupName);
        }

        class Indexer : IIndexer
        {
            public  IIndexer Add<T>()
            {
                var key = typeof(T).Name;
                if (!Indexes.ContainsKey(key))
                {
                    LastAddedIndex++;
                    Indexes.Add(key, LastAddedIndex);
                }

                return this;
            }

            public  IIndexer Add( string groupName)
            {

                if (!Indexes.ContainsKey(groupName))
                {
                    LastAddedIndex++;
                    Indexes.Add(groupName, LastAddedIndex);
                }

                return this;
            }
        }

        public static IIndexer ImplicitIndex()
        {
            return new Indexer();
        }

        public static List<NavigationLookupItem> AssignIndexes(this IEnumerable<NavigationLookupItem> items)
        {
            var appNavigationLookupItems = items.ToList();
            appNavigationLookupItems.ForEach(item =>
            {
                if (item.Target != null)
                {
                    item.Index = Indexes[item.Target.Name];
                }
                else
                {
                    if (item.IsGroupingItem)
                    {
                        item.Index = Indexes[item.Title];
                    }
                }

                if (item.SubItems != null)
                {
                    item.SubItems?.AssignIndexes();
                    var result = item.SubItems?.OrderBy(lookupItem => lookupItem.Index).ToList();
                    item.SubItems = new BindableCollection<NavigationLookupItem>(result);
                }
            });
            return appNavigationLookupItems.OrderBy(item => item.Index).ToList();
        }
    }
}