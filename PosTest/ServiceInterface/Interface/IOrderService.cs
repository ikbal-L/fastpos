using ServiceInterface.Model;
using System.Collections.Generic;

namespace ServiceInterface.Interface
{
    public interface IOrderService 
    {
        IEnumerable<Order> GetAllOrders(out int statusCode,bool unprocessed = false);
        IEnumerable<Order> GetManyOrders(IEnumerable<long> orderIds, ref int statusCode);
        int SaveOrder(ref Order order,out IEnumerable<string> errors);
        int UpdateOrder(ref Order order, out IEnumerable<string> errors);
        int DeleteOrder(long orderId);
        Table GetTableByNumber(int tableNumber, ref int statusCode);
        Table GetTable(int id, ref int statusCode);
        int SaveTable(Table table ,out long id, out IEnumerable<string> errors);

        IEnumerable<Table> GeAlltTables(ref int statusCode);
    }

}
