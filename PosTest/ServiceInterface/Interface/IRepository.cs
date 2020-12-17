﻿using System.Collections.Generic;
using ServiceInterface.Model;

namespace ServiceInterface.Interface
{
    public interface IRepository<TState, in TIdentifier> where TState : IState<TIdentifier> where TIdentifier : struct
    {
        (int status, TState) Get(TIdentifier id);
        (int status, IEnumerable<TState>) Get();
        (int status, IEnumerable<TState>) Get(object param);
        (int status, IEnumerable<TState>) Get(IEnumerable<TIdentifier> ids);
        int Save(TState state, out IEnumerable<string> errors);
        int Delete(TIdentifier id);
        int Update(TState state);
        int Update(IEnumerable<TState> state);
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
}