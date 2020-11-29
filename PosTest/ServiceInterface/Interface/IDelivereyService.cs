using ServiceInterface.Model;
using System.Collections.Generic;

namespace ServiceInterface.Interface
{
    public interface IDelivereyService
    {
        IEnumerable<Delivereyman> GetAllDelivereymen(ref int statusCode);
        IEnumerable<Delivereyman> GetAllActiveDelivereymen(ref int statusCode);
        int SaveDelivereyman(Delivereyman delivereyman ,out long id, out IEnumerable<string> errors);
        int UpdateDelivereyman(Delivereyman delivereyman);
        int DeleteDelivereyman(long orderId);

        Delivereyman GetDelivereyman(int id, ref int statusCode);
    }

    public interface IWaiterService
    {
        IEnumerable<Waiter> GetAllWaiters(ref int statusCode);
        IEnumerable<Waiter> GetAllActiveWaiters(ref int statusCode);
        int SaveWaiter(Waiter waiter ,out long id ,out IEnumerable<string> errors);
        int UpdateWaiter(Waiter Waiter);
        int DeleteWaiter(long orderId);

        Waiter GetWaiters(int id, ref int statusCode);
    }

}
