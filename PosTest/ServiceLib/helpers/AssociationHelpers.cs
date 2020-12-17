using System;
using System.Collections.Generic;
using ServiceInterface.Model;

namespace ServiceLib.helpers
{
    public static class AssociationHelpers
    {
        private static readonly IDictionary<(Type, Type), Action<ICollection<object>, ICollection<object>>> _association;

        static AssociationHelpers()
        {
            _association = new Dictionary<(Type, Type), Action<ICollection<object>, ICollection<object>>>();
            var key = (typeof(Product),typeof(Category));
            _association.Add(
                key,AssociateProductsWithCategories);
        }
        public static void AssociateAdditivesWithProducts(ICollection<object> additives, ICollection<object> products)
        {
            foreach (Product product in products as ICollection<Product>)
            {
                if (product is Platter platter)
                {
                    platter.MappingAfterReceiving(category: null, (List<Additive>)(additives as ICollection<Additive>)));

                }
            }
        }

        public static void AssociateProductsWithCategories(ICollection<object> products, ICollection<object> categories)
        {
            foreach (var category in (ICollection<Category>) categories)
            {
                category.MappingAfterReceiving( products as ICollection<Product> );
            }
        }

        public static Action<ICollection<object>,ICollection<object>> GetAssociation<T1, T2>()
        {

        }
    }
}