using ServiceInterface.Model;
using System.Collections.Generic;

namespace ServiceInterface.Interface
{
    public interface IOrderService 
    {
        IEnumerable<Order> GetAllOrders(ref int statusCode);
        IEnumerable<Order> GetManyOrders(IEnumerable<long> orderIds, ref int statusCode);
        int SaveOrder(Order order,out IEnumerable<string> errors);
        int UpdateOrder(Order order);
        int DeleteOrder(long orderId);
        Table GetTableByNumber(int tableNumber, ref int statusCode);
        Table GetTable(int id, ref int statusCode);
        int SaveTable(Table table ,out long id);

        IEnumerable<Table> GeAlltTables(ref int statusCode);
    }

}
