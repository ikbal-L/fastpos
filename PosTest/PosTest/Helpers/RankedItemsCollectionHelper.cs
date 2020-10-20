using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceInterface.Model;

namespace PosTest.Helpers
{
    public class RankedItemsCollectionHelper
    {
        private static void LoadPages<T>(IList<T> source, ICollection<T> target, int size, bool filled = false,
            object parameter = null) where T : Ranked, new()
        {
            int nbitem = 0;
            for (int i = 1; i <= size; i++)
            {
                T item = null;
                if (nbitem < source.Count)
                {
                    item = source[nbitem];
                }

                if (item != null && item.Rank == i)
                {
                    target.Add(item);
                    nbitem++;
                }
                else
                {
                    if (filled) //if filled is true target will add item with value otherwise target will add null
                    {
                        item = new T {Rank = i};
                        if (item is Product product && parameter is Category category)
                        {
                            product.Category = category;
                            product.CategorieId = category.Id;
                        }
                    }

                    target.Add(item);
                }
            }
        }

        public static void LoadPagesFilled<T>(IList<T> source, ICollection<T> target, int size, object parameter = null)
            where T : Ranked, new()
        {
            LoadPages<T>(source: source, target: target, size: size, filled: true, parameter: parameter);
        }

        public static void LoadPagesFilledNotFilled<T>(IList<T> source, ICollection<T> target, int size,
            object parameter = null) where T : Ranked, new()
        {
            LoadPages<T>(source: source, target: target, size: size, parameter: parameter);
        }
    }
}