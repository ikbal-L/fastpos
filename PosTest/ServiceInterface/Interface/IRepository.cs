﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ServiceInterface.Model;

namespace ServiceInterface.Interface
{
    public interface IRepository<TState, in TIdentifier> where TState : IState<TIdentifier> where TIdentifier : struct
    {
        public string BaseUrl { get; set; }
        (int status, TState) GetById(TIdentifier id);
        (int status, IEnumerable<TState> state) GetAll();
        (int status, IEnumerable<TState>) Get(string subPath);
        (int status, IEnumerable<TState>) GetByIds(IEnumerable<TIdentifier> ids);
        int Save(TState state, out IEnumerable<string> errors);
        (int status, IEnumerable<TState> state,IEnumerable<string> errors) SaveAll(IEnumerable<TState> state);
        (int status,IEnumerable<string> errors) DeleteById(TIdentifier id);
        int Update(TState state, out IEnumerable<string> errors);
        (bool, TReturn) Update<TReturn>(TState state);
        (int status, IEnumerable<string> errors) Update(IEnumerable<TState> state);
        
        (bool ,TReturn) Save<TReturn>(TState state);
        (bool, TReturn) Delete<TReturn>(TIdentifier id);
        int Delete(IEnumerable<TIdentifier> ids);

        Task<(int,IEnumerable<TState>)> GetAsync();

        Task<(int status, IEnumerable<TState>)> GetAsync(string subPath);

        List<TState> GetByCriterias(object criterias);

        Task<List<TState>> GetByCriteriasAsync(object criterias);

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