using System;
using System.Collections.Generic;
using ServiceInterface.Model;

namespace ServiceLib.helpers
{
    public static class AssociationHelpers
    {
        private static readonly IDictionary<(Type, Type), Action<object,object>> _association;

        static AssociationHelpers()
        {
            _association = new Dictionary<(Type, Type), Action<object,object>>();
           
            _association.Add(
                (typeof(Product), typeof(Category)), (object o1, object o2)=> {

                    AssociateProductsWithCategories(o1 as ICollection<Product>, o2 as ICollection<Category>);
                });
            _association.Add(
              (typeof(Additive), typeof(Product)), (object o1, object o2) => {

                  AssociateAdditivesWithProducts(o1 as ICollection<Additive>, o2 as ICollection<Product>);
              });
        }
        public static void AssociateAdditivesWithProducts(ICollection<Additive> additives, ICollection<Product> products)
        {
            if(additives==null|| products == null) return ;
            foreach (Product product in products as ICollection<Product>)
            {
                if (product is Platter platter)
                {
                    platter.MappingAfterReceiving(category: null, (List<Additive>)additives);

                }
            }
        }

        public static void AssociateProductsWithCategories(ICollection<Product> products, ICollection<Category> categories)
        {
            foreach (var category in  categories)
            {
                category.MappingAfterReceiving( products );
            }
        }

        public static Action<ICollection<T1>,ICollection<T2>> GetAssociation<T1, T2>()
        {
            var key = (typeof(T1), typeof(T2));
            return _association[key];
        }
    }
}