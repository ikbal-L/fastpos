using ServiceInterface.Model;
using System.Collections.Generic;

namespace ServiceInterface.Interface
{
    public interface IDelivereyService
    {
        (int,IEnumerable<Deliveryman>) GetAllDeliverymen();
        (int,IEnumerable<Deliveryman>) GetAllActiveDeliverymen();
        int SaveDeliveryman(Deliveryman deliveryman, out IEnumerable<string> errors);
        int UpdateDeliveryman(Deliveryman deliveryman);
        int DeleteDeliveryman(long orderId);

        (int,Deliveryman) GetDeliveryman(int id);
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
