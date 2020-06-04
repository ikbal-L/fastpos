using ServiceInterface.Model;
using System.Collections.Generic;

namespace ServiceInterface.Interface
{
    public interface IOrderService 
    {
        IEnumerable<Order> GetAllOrders();
        IEnumerable<Order> GetManyOrders(IEnumerable<long> orderIds);
        int SaveOrder(Order order);
        int UpdateOrder(Order order);
        int DeleteOrder(int orderId);
        long GetIdmax();
    }
}
