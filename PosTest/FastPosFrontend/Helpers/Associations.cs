using System.Collections.Generic;
using System.Linq;
using ServiceInterface.Model;
using ServiceLib.helpers;

namespace FastPosFrontend.Helpers
{
    public static class Associations
    {
        private static readonly AssociationManager AssociationManager;
            
        static Associations()
        {
            AssociationManager = AssociationManager.Instance;
           

            
        }

        public static void Setup()
        {
            AssociationManager.Register().Associate<Product>().With<Additive>().Using<Product, Additive>(AssociateProductsWithAdditives).Build();
            AssociationManager.Register().Associate<Category>().With<Product>().Using<Category, Product>(AssociateCategoriesWithProducts).Build();
            AssociationManager.Register().Associate<Order>().With<Table>().Using<Order, Table>(AssociateOrdersWithTables).Build();
            AssociationManager.Register().Associate<Order>().With<Waiter>().Using<Order, Waiter>(AssociateOrdersWithWaiters).Build();
            AssociationManager.Register().Associate<Order>().With<Deliveryman>().Using<Order, Deliveryman>(AssociateOrdersWithDeliveryMen).Build();
            AssociationManager.Register().Associate<Order>().With<Customer>().Using<Order, Customer>(AssociateOrdersWithCustomers).Build();
            AssociationManager.Register().Associate<Order>().With<Product>().Using<Order, Product>(AssociateOrdersWithProducts).Build();
            AssociationManager.Register().Associate<User>().With<Role>().Using<User, Role>(AssociateUsersWithRoles).Build();
            AssociationManager.Register().Associate<Role>().With<Permission>().Using<Role, Permission>(AssociateRolesWithPermissions).Build();
        }

        private static void AssociateRolesWithPermissions(IEnumerable<Role> roles, IEnumerable<Permission> permissions)
        {
            foreach (var role in roles)
            {
                var rolePermissions = permissions.Where(p => role.PermissionIds.Contains((long)p.Id));
                role.Permissions = rolePermissions.ToList();
            }
        }


        private static void AssociateOrdersWithCustomers(IEnumerable<Order> orders, IEnumerable<Customer> customers)
        {
            foreach (Order order in orders)
            {
                order.Customer = customers.FirstOrDefault(c => c.Id == order.CustomerId);
            }
        }

        public static void AssociateProductsWithAdditives(IEnumerable<Product> products,IEnumerable<Additive> additives)
        {
            if (additives == null || products == null) return;
            foreach (Product product in products as IEnumerable<Product>)
            {
                if (product.IsPlatter)
                {
                    product.MappingAfterReceiving(category: null, (List<Additive>)additives);

                }
            }
        }

        public static void AssociateCategoriesWithProducts(IEnumerable<Category> categories,IEnumerable<Product> products )
        {
            foreach (var category in categories)
            {
                category.MappingAfterReceiving(products);
            }
        }

        public static void AssociateOrdersWithTables(IEnumerable<Order> orders, IEnumerable<Table> tables)
        {
            orders.Where(o => o.Type == OrderType.OnTable).ToList().ForEach(o =>
            {
                o.Table = tables.FirstOrDefault(t => t.Id == o.TableId);
            });
        }

        public static void AssociateOrdersWithWaiters(IEnumerable<Order> orders, IEnumerable<Waiter> waiters)
        {

            orders.Where(o => o.Type == OrderType.OnTable).ToList().ForEach(o =>
            {
                o.Waiter = waiters.FirstOrDefault(w => w.Id == o.WaiterId);
            });
        }

        public static void AssociateOrdersWithDeliveryMen(IEnumerable<Order> orders, IEnumerable<Deliveryman> deliverymen)
        {
            orders.Where(o => o.Type == OrderType.Delivery).ToList().ForEach(o =>
            {
                o.Deliveryman = deliverymen.FirstOrDefault(d => d.Id == o.DeliverymanId);
            });
        }

        public static void AssociateOrdersWithProducts(IEnumerable<Order> orders, IEnumerable<Product> products)
        {
            foreach (Order order in orders)
            {
                order.MappingAfterReceiving(products);
            }
        }

        public static void AssociateUsersWithRoles(IEnumerable<User> users,IEnumerable<Role> roles)
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
    }
}