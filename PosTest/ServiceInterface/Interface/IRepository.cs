using System;
using System.Collections.Generic;
using ServiceInterface.Model;

namespace ServiceInterface.Interface
{
    public interface IRepository<TState, in TIdentifier> where TState : IState<TIdentifier> where TIdentifier : struct
    {
        (int status, TState) Get(TIdentifier id);
        (int status, IEnumerable<TState>) Get();
        (int status, IEnumerable<TState>) Get(string subPath);
        (int status, IEnumerable<TState>) Get(IEnumerable<TIdentifier> ids);
        int Save(TState state, out IEnumerable<string> errors);
        int Save(IEnumerable<TState> state);
        int Delete(TIdentifier id);
        int Update(TState state, out IEnumerable<string> errors);
        (bool, TReturn) Update<TReturn>(TState state);
        int Update(IEnumerable<TState> state);
        (bool ,TReturn) Save<TReturn>(TState state);
        (bool, TReturn) Delete<TReturn>(TIdentifier id);


    }

    public interface IAdditiveRepository : IRepository<Additive, long>
    {

    }

    public interface IProductRepository : IRepository<Product, long>
    {

    }

    public interface ICategoryRepository : IRepository<Category, long>
    {

    }

    public interface IOrderRepository : IRepository<Order, long>
    {
        List<Order>  GetOrderByStates(string[] states, long deliverymanId);
        PageList<Order> getAllByDeliveryManAndStatePage( int pageNumber, int pageSize, long deliverymanId,string[] states);

    }

    public interface IOrderItemRepository : IRepository<OrderItem, long>
    {

    }

    public interface ITableRepository : IRepository<Table, long>
    {

    }

    public interface ICustomerRepository : IRepository<Customer, long>
    {

    }

    public interface IWaiterRepository : IRepository<Waiter, long>
    {

    }

    public interface IDeliverymanRepository : IRepository<Deliveryman, long>
    {

    }
    public interface IPaymentRepository: IRepository<Payment, long>
    {
        PageList<Payment> getAllByDeliveryManPage(int pageNumber, int pageSize, long deliverymanId);
        List<Payment> getByDeliveryManAndDate(long deliverymanId, DateTime date);
    }
}