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
                    else
                    {
                        item = null;
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

        public static void LoadPagesNotFilled<T>(IList<T> source, ICollection<T> target, int size,
            object parameter = null) where T : Ranked, new()
        {
            LoadPages<T>(source: source, target: target, size: size, parameter: parameter);
        }

        public static void InsertTElementInPositionOf<T>(ref T incomingArg,ref T targetArg ,ref IList<T> list) where T : Ranked, new()
        {
            if (incomingArg==null)
            {
                throw new NullReferenceException("Incoming Arg must not be null");
            }

            if (targetArg == null)
            {
                throw new NullReferenceException("Target Arg must not be null");
            }

            if (targetArg.GetType().GetProperty("Id")?.GetValue(targetArg) != null && incomingArg.Rank ==null)
            {
                throw new InvalidOperationException("Must Select an empty target");
            }

            if (targetArg.Rank == null)
            {
                throw new NullReferenceException("Target Rank must not be Null");
            }
            int indexOfTarget = list.IndexOf(targetArg);
            int indexOfIncomingArg = list.IndexOf(incomingArg);
            
            
            int targetArgRank = (int)targetArg.Rank;
            targetArg.Rank = incomingArg.Rank;
            if (incomingArg.Rank != null)
            {
                list[indexOfIncomingArg] = targetArg;
            }
            else
            {
                if (incomingArg is Product incomingProduct && targetArg is Product targetProduct)
                {
                    incomingProduct.Category = targetProduct.Category;
                }

                targetArg = null;
            }

            incomingArg.Rank = targetArgRank;
            list[indexOfTarget] = incomingArg;

        }
    }
}