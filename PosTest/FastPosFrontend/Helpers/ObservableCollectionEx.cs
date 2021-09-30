using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastPosFrontend.Helpers
{
    public static class ObservableCollectionEx
    {
        public static void AddRange<T>(this ObservableCollection<T> observableCollection,IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                observableCollection.Add(item);
            }
        }

        public static ObservableCollection<T> FromNullable<T>(IEnumerable<T> items)
        {
            if (items != null && items.Any())
            {
                return new ObservableCollection<T>(items);
            }
            return new ObservableCollection<T>();
        }
    }
}
