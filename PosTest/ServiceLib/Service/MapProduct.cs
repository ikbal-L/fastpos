using ServiceInterface.Model;
using System.Collections.Generic;
using System.Linq;

namespace ServiceLib.Service
{
    public class MapProduct
    {
        public static Product MapProductToSend(Product product)
        {
            if (product.CategorieId <= 0 && product.Category != null)
            {
                product.CategorieId = product.Category.Id;
            }

            if (product is Platter plat)
            {
                if (plat.Additives != null)
                    foreach (var a in plat.Additives)
                    {
                        if (plat.IdAdditives == null)
                        {
                            plat.IdAdditives = new List<long>();
                        }
                        plat.IdAdditives.Add(a.Id);
                    }

                if(plat.Ingredients != null)
                    foreach (var ing in plat.Ingredients)
                    {
                        if (ing.Product != null)
                        {
                            ing.ProductId = ing.Product.Id;
                        }
                    }
            }

            return product;

        }


        public static Product MapProductReceived(Product product, Category category, List<Additive> additives)
        {
            if (category != null && product.CategorieId == category.Id)
            {
                product.Category = category;
            }
            else
            {
                throw new ProductMappingException("Id category different of CategoryId of the related Product");
            }
            if(product is Platter plat && 
                plat.IdAdditives != null && additives != null && 
                additives.Count == plat.IdAdditives.Count)
            {
                plat.Additives = new List<Additive>();
                foreach (var a in additives)
                {
                    if (plat.IdAdditives.Any(id => id == a.Id))
                    {
                        plat.Additives.Add(a);
                    }
                    else
                    {
                        throw new ProductMappingException("Additive Id does not exist in the list of ids");
                    }
                }
            }
            return product;
        }
    }
}
