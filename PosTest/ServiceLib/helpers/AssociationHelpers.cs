using System;
using System.Collections.Generic;
using System.Linq;
using FastPosFrontend.Helpers;
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
                },
                {
                    (typeof(Order), typeof(Customer)),
                    (o1, o2) =>
                    {

                        AssociateOrdersWithCustomers(o1 as ICollection<Order>, o2 as ICollection<Customer>);
                    }
                },
                {
                    (typeof(Role),typeof(User)),(o1, o2) =>
                    {
                        AssociateRolesWithUsers(o1 as ICollection<Role>,o2 as ICollection<User>);
                    }
                }
            };
        }

        private static void AssociateOrdersWithCustomers(ICollection<Order> orders, ICollection<Customer> customers)
        {
            foreach (Order order in orders)
            {
                order.Customer = customers.FirstOrDefault(c=>c.Id== order.CustomerId);
            }
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
            orders.Where(o => o.Type == OrderType.OnTable).ToList().ForEach(o =>
            {
                o.Table = tables.FirstOrDefault(t => t.Id == o.TableId);
            });
        }

        public static void AssociateOrdersWithWaiters(ICollection<Order> orders, ICollection<Waiter> waiters)
        {
            
            orders.Where(o=>o.Type == OrderType.OnTable).ToList().ForEach(o =>
            {
                o.Waiter = waiters.FirstOrDefault(w => w.Id == o.WaiterId);
            });
        }

        public static void AssociateOrdersWithDeliveryMen(ICollection<Order> orders, ICollection<Deliveryman> deliverymen)
        {
            orders.Where(o => o.Type == OrderType.Delivery).ToList().ForEach(o =>
            {
                o.Deliveryman = deliverymen.FirstOrDefault(d => d.Id == o.DeliverymanId);
            });
        }

        public static void AssociateOrdersWithProducts(ICollection<Order> orders, ICollection<Product> products)
        {
            foreach (Order order in orders)
            {
                order.MappingAfterReceiving(products);
            }
        }

        public static void AssociateRolesWithUsers(ICollection<Role> roles, ICollection<User> users)
        {
            foreach (var user in users)
            {
               
                user.RoleIds.ForEach(roleId =>
                {
                    var role = roles.FirstOrDefault(r => r.Id == roleId);
                    user.Roles.Add(role);
                });
            }
        }

        public static Action<ICollection<T1>,ICollection<T2>> GetAssociation<T1, T2>()
        {
            var key = (typeof(T1), typeof(T2));
            return _association[key];
        }
    }


    public interface IAssociationManager
    {
        
        
   

    }

    public interface IOneToMany
    {
        IManyToOne Associate<TOne>();
    }

    public interface IManyToOne
    {
        IAssociationAction With<TMany>();
    }

    public interface IAssociationAction
    {
        IAssociationBuild Using<TOne, TMany>(Action<IEnumerable<TOne>, IEnumerable<TMany>> map);
    }

}