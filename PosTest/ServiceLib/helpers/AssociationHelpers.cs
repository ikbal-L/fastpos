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
            _association = new Dictionary<(Type, Type), Action<object, object>>
            {
                {
                    (typeof(Product), typeof(Category)),
                    (object o1, object o2) =>
                    {

                        AssociateProductsWithCategories(o1 as ICollection<Product>, o2 as ICollection<Category>);
                    }
                },
                {
                    (typeof(Additive), typeof(Product)),
                    (object o1, object o2) =>
                    {

                        AssociateAdditivesWithProducts(o1 as ICollection<Additive>, o2 as ICollection<Product>);
                    }
                },
                {
                    (typeof(Order), typeof(Table)),
                    (object o1, object o2) =>
                    {

                        AssociateOrdersWithTables(o1 as ICollection<Order>, o2 as ICollection<Table>);
                    }
                },
                {
                    (typeof(Order), typeof(Waiter)),
                    (object o1, object o2) =>
                    {

                        AssociateOrdersWithWaiters(o1 as ICollection<Order>, o2 as ICollection<Waiter>);
                    }
                },
                {
                    (typeof(Order), typeof(Deliveryman)),
                    (object o1, object o2) =>
                    {

                        AssociateOrdersWithDeliveryMen(o1 as ICollection<Order>, o2 as ICollection<Deliveryman>);
                    }
                },
                {
                    (typeof(Order), typeof(Product)),
                    (object o1, object o2) =>
                    {

                        AssociateOrdersWithProducts(o1 as ICollection<Order>, o2 as ICollection<Product>);
                    }
                }
            };
        }
        public static void AssociateAdditivesWithProducts(ICollection<Additive> additives, ICollection<Product> products)
        {
            if(additives==null|| products == null) return ;
            foreach (Product product in products as ICollection<Product>)
            {
                if (product.IsPlatter)
                {
                    product.MappingAfterReceiving(category: null, (List<Additive>)additives);

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

        public static void AssociateOrdersWithTables(ICollection<Order> orders, ICollection<Table> tables)
        {
            foreach (Table table in tables)
            {
                foreach (Order order in orders)
                {
                    if (order.Type == OrderType.OnTable && order.TableId == table.Id)
                    {
                        order.Table = table;
                    }
                }
            }
        }

        public static void AssociateOrdersWithWaiters(ICollection<Order> orders, ICollection<Waiter> waiters)
        {
            foreach (Waiter waiter in waiters)
            {
                foreach (Order order in orders)
                {
                    if (order.Type == OrderType.OnTable && order.WaiterId == waiter.Id)
                    {
                        order.Waiter = waiter;
                    }
                }
            }
        }

        public static void AssociateOrdersWithDeliveryMen(ICollection<Order> orders, ICollection<Deliveryman> deliverymen)
        {
            foreach (Deliveryman deliveryman in deliverymen)
            {
                foreach (Order order in orders)
                {
                    if (order.Type == OrderType.Delivery && order.WaiterId == deliveryman.Id)
                    {
                        order.Deliveryman = deliveryman;
                    }
                }
            }
        }

        public static void AssociateOrdersWithProducts(ICollection<Order> orders, ICollection<Product> products)
        {
            foreach (Order order in orders)
            {
                order.MappingAfterReceiving(products);
            }
        }

        public static Action<ICollection<T1>,ICollection<T2>> GetAssociation<T1, T2>()
        {
            var key = (typeof(T1), typeof(T2));
            return _association[key];
        }
    }

}