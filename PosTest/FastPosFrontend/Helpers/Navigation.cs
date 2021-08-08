using System.Collections.Generic;
using System.Linq;
using Caliburn.Micro;

namespace FastPosFrontend.Helpers
{
    public static class NavigationIndexer
    {
        private static readonly Dictionary<string, int> Indexes = new Dictionary<string, int>();
        private static int LastAddedIndex = -1;

        public interface IIndexer
        {
        }

        class Indexer : IIndexer
        {
        }

        public static IIndexer ImplicitIndex()
        {
            return new Indexer();
        }

        public static IIndexer Add<T>(this IIndexer indexer)
        {
            var key = typeof(T).Name;
            if (!Indexes.ContainsKey(key))
            {
                LastAddedIndex++;
                Indexes.Add(key, LastAddedIndex);
            }

            return indexer;
        }
        public static IIndexer Add(this IIndexer indexer,string groupName)
        {
            
            if (!Indexes.ContainsKey(groupName))
            {
                LastAddedIndex++;
                Indexes.Add(groupName, LastAddedIndex);
            }

            return indexer;
        }

        public static List<AppNavigationLookupItem> AssignIndexes(this IEnumerable<AppNavigationLookupItem> items)
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
                    item.SubItems = new BindableCollection<AppNavigationLookupItem>(result);
                }
            });
            return appNavigationLookupItems.OrderBy(item => item.Index).ToList();
        }
    }
}