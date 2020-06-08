﻿using ServiceInterface.Model;
using System.Collections.Generic;

namespace ServiceInterface.Interface
{
    public interface IOrderService 
    {
        IEnumerable<Order> GetAllOrders(ref int statusCode);
        IEnumerable<Order> GetManyOrders(IEnumerable<long> orderIds, ref int statusCode);
        int SaveOrder(Order order);
        int UpdateOrder(Order order);
        int DeleteOrder(long orderId);
        long? GetIdmax(ref int statusCode);
        Table GetTableByNumber(int tableNumber, ref int statusCode);
        Table GetTable(int id, ref int statusCode);
    }
}
