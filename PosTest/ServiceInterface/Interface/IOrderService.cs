using ServiceInterface.Model;
using System.Collections.Generic;

namespace ServiceInterface.Interface
{
    public interface IOrderService 
    {
        (int,IEnumerable<Order>) GetAllOrders(bool unprocessed = false);
        (int,IEnumerable<Order>) GetManyOrders(IEnumerable<long> orderIds);
        int SaveOrder(Order order,out IEnumerable<string> errors);
        int UpdateOrder(ref Order order, out IEnumerable<string> errors);
        int DeleteOrder(long orderId);
        (int status, Table table) GetTableByNumber(int tableNumber);
        (int,Table) GetTable(int id);
        int SaveTable(Table table ,out IEnumerable<string> errors);

        (int,IEnumerable<Table>) GeAllTables();

    }

}
