using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ServiceInterface.Model;

namespace ServiceInterface.Interface
{
    public  interface IRepository<T, in Id> :IRepositoryAsync<T,Id> where T : IState<Id> where Id : struct
    {
        public string BaseUrl { get; set; }
        (int status, T) GetById(Id id);
        (int status, IEnumerable<T> state) GetAll();
        (int status, IEnumerable<T>) Get(string subPath);
        (int status, IEnumerable<T>) GetByIds(IEnumerable<Id> ids);
        (int status, IEnumerable<string> errors) Save(T state);
        (int status, IEnumerable<T> state,IEnumerable<string> errors) SaveAll(IEnumerable<T> state);
        (int status,IEnumerable<string> errors) DeleteById(Id id);
        (int status, IEnumerable<string> errors) Update(T state);
        (bool, TReturn) Update<TReturn>(T state);
        (int status, IEnumerable<string> errors) Update(IEnumerable<T> state);
        
        (bool ,TReturn) Save<TReturn>(T state);
        int Delete(IEnumerable<Id> ids);

        Task<(int,IEnumerable<T>)> GetAllAsync();

        Task<(int status, IEnumerable<T>)> GetAllAsync(string endpoint, string? subpath = null);
        

        Page<T> GetByCriterias(object criterias);

        Task<List<T>> GetByCriteriasAsync(object criterias);

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

    public interface IUserRepository : IRepository<User, long>
    {

    }

    public interface IRoleRepository : IRepository<Role, long>
    {

    }

    public interface IPermissionRepository : IRepository<Privilege, long>
    {

    }

    public interface IPaymentRepository: IRepository<Payment, long>
    {
        PageList<Payment> getAllByDeliveryManPage(int pageNumber, int pageSize, long deliverymanId);
        List<Payment> getByDeliveryManAndDate(long deliverymanId, DateTime date);
    }

    public interface IDailyEarningsReportRepository : IRepository<DailyEarningsReport, long>
    {

    }

    public interface ICashRegisterExpenseRepository : IRepository<CashRegisterExpense, long>
    {
        List<string> GetEmployees();
    }

    public interface IExpenseDescriptionRepository: IRepository<ExpenseDescription, long>
    {

    }
}