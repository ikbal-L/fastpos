using ServiceInterface.Model;
using System.Collections.Generic;

namespace ServiceInterface.Interface
{
    public interface IDelivereyService
    {
        (int,IEnumerable<Delivereyman>) GetAllDeliverymen();
        (int,IEnumerable<Delivereyman>) GetAllActiveDeliverymen();
        int SaveDeliveryman(Delivereyman deliveryman, out IEnumerable<string> errors);
        int UpdateDeliveryman(Delivereyman deliveryman);
        int DeleteDeliveryman(long orderId);

        (int,Delivereyman) GetDeliveryman(int id);
    }

    public interface IWaiterService
    {
        (int,IEnumerable<Waiter>) GetAllWaiters();
        (int,IEnumerable<Waiter>) GetAllActiveWaiters();
        int SaveWaiter(Waiter waiter ,out IEnumerable<string> errors);
        int UpdateWaiter(Waiter waiter);
        int DeleteWaiter(long orderId);

        (int,Waiter) GetWaiters(int id);
    }

}
