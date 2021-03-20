using System;
using System.Collections.Generic;
using ServiceInterface.Model;

namespace FastPosFrontend.Helpers
{
    public class RankedItemsCollectionHelper
    {
        private static void LoadPages<T>(IList<T> source, ICollection<T> target, int size, bool filled = false,
            object parameter = null) where T : Ranked, new()
        {
            if (typeof(T) == typeof(Product) && parameter == null)
            {
                throw new NullReferenceException("Parameter must not be null if Type of T equals Type of  Product");
            }
            
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
                            product.CategoryId = category.Id;
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

        public static void InsertTElementInPositionOf<T>(ref T incomingArg,ref T targetArg ,IList<T> list) where T : Ranked, new()
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
                if (incomingArg is Product incomingProduct && targetArg is Product targetProduct)
                {
                    if (incomingProduct.Category!= targetProduct.Category)
                    {
                        Category incomingCategory = incomingProduct.Category;
                        Category targetCategory = targetProduct.Category;
                        
                        if (incomingProduct.Id != null)
                        {
                            incomingCategory.ProductIds.Remove(incomingProduct.Id.Value);
                            incomingCategory.Products.Remove(incomingProduct);
                            targetCategory.ProductIds.Add(incomingProduct.Id.Value);
                            targetCategory.Products.Add(incomingProduct);
                        }
                        if (targetProduct.Id != null)
                        {
                            targetCategory.ProductIds.Remove(targetProduct.Id.Value);
                            targetCategory.Products.Remove(targetProduct);
                            incomingCategory.ProductIds.Add(targetProduct.Id.Value);
                            incomingCategory.Products.Add(targetProduct);
                        }
                    }
                }
                if (indexOfIncomingArg>=0 && indexOfIncomingArg<list.Count)
                {
                    list[indexOfIncomingArg] = targetArg; 
                }

            }
            else
            {
                if (incomingArg is Product incomingProduct && targetArg is Product targetProduct)
                {
                    incomingProduct.Category = targetProduct.Category;
                    incomingProduct.Category.Products.Add(incomingProduct);
                    if (incomingProduct.Id != null) incomingProduct.Category.ProductIds.Add(incomingProduct.Id.Value);
                }

                targetArg = null;
            }

            incomingArg.Rank = targetArgRank;
            list[indexOfTarget] = incomingArg;

        }
    }
}